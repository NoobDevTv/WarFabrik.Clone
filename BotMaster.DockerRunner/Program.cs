using BotMaster.Core;
using BotMaster.Core.Configuration;
using BotMaster.PluginHost;
using BotMaster.PluginSystem;

using Docker.DotNet;
using Docker.DotNet.Models;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using NLog;
using NLog.Extensions.Logging;

using System.Net;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BotMaster.DockerRunner
{
    internal class DockerPluginClientMessage : IPluginControlMessage
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Command Command { get; set; }
    }

    internal enum DockerState
    {
        Created,
        Restarting,
        Running,
        Removing,
        Paused,
        Exited,
        Dead
    }

    //TODO Verwurschteln
    internal class DockerClientRunner : ClientRunner<DockerPluginClientMessage>
    {
        internal readonly DockerInfo dockerInfo;
        private readonly Logger logger;
        private readonly TcpClient client;
        private readonly RunnerInstance instance;

        internal DockerClientRunner(DockerInfo dockerInfo) : base("BotMaster", 44545, dockerInfo.InstanceId) //TODO Correct HostName and Port
        {
            this.dockerInfo = dockerInfo;
            this.logger = LogManager.GetCurrentClassLogger();
            OnNewMessage += DockerClientRunner_OnNewMessage;

        }

        private async Task DockerClientRunner_OnNewMessage(DockerPluginClientMessage obj)
        {
            logger.Info($"Got message command {obj.Command}");

            if (obj.Command == Command.GetState)
            {
                var client = dockerInfo.Client;
                var containerName = dockerInfo.ContainerName;
                var container = await Program.GetContainer(client, containerName);
                if (container == default)
                {
                    Execute("""{ "PluginStatus": false }""");
                    return;
                }
                logger.Info($"Trying to parse {container.State} to DockerState");
                if (Enum.TryParse<DockerState>(container.State, true, out var dockerState))
                {
                    if (dockerState == DockerState.Running || dockerState == DockerState.Restarting)
                        Execute("""{ "PluginStatus": true }""");
                    else
                        Execute("""{ "PluginStatus": false }""");
                }
                return;
            }

            var res = await Program.ExecuteContainerCommand(dockerInfo, obj.Command);
            if (res.Item1)
            {
                if (obj.Command == Command.Start)
                    Execute("""{ "PluginStatus": true }""");
                else if (obj.Command == Command.Stop)
                    Execute("""{ "PluginStatus": false }""");
            }

        }
    }

    internal class Program
    {
        internal static DockerClientRunner clientRunner;

        internal static async Task Main(string[] args)
        {

            if (args.Length == 0)
            {
                args = new[] { "-l", "plugins/BotMaster.Twitch/plugin.manifest.json" };
            }

            var config = ConfigManager.GetConfiguration("appsettings.json", args);

            using var logManager = Disposable.Create(LogManager.Shutdown);

            var logger = LogManager
               .Setup()
               .LoadConfigurationFromSection(config)
               .GetCurrentClassLogger();

            var dockerSettings = config.GetSection("Docker").Get<DockerSettings>();

            //await DockerPlayground();
            var client = new DockerClientConfiguration().CreateClient();

            var info = new FileInfo(Path.Combine(".", "logs", $"pluginhost-{DateTime.Now:ddMMyyyy-HHmmss_fff}.log"));

            if (!info.Directory!.Exists)
            {
                info.Directory.Create();
            }

            logger.Debug("Gotten the following args: " + string.Join(" | ", args));

            try
            {
                PluginManifest? pluginManifest = null;
                Command command = Command.Recreate;
                Guid? instanceId = null;
                for (var i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-l")
                    {
                        i++;
                        pluginManifest = JsonSerializer.Deserialize<PluginManifest>(File.ReadAllText(args[i]), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                    if (args[i] == "-s")
                    {
                        i++;
                        //TODO Start and Stop
                        command = Enum.Parse<Command>(args[i]);

                    }
                    if (args[i] == "--id")
                    {
                        i++;
                        instanceId = Guid.Parse(args[i]);
                    }
                }

                if (pluginManifest is null)
                    return;

                var dockerData = pluginManifest.ExtensionData["DockerData"];
                var containsImageName = dockerData.TryGetProperty("ImageName", out var imageNameJson); //string
                if (!containsImageName)
                {
                    return;
                }

                var imageName = imageNameJson.GetString();

                var containsContainerName = dockerData.TryGetProperty("ContainerName", out var containerNameJson); //string
                string? containerName = null;
                if (containsContainerName)
                {
                    containerName = containerNameJson.GetString();
                }

                var containsBindings = dockerData.TryGetProperty("Bindings", out var bindingsJson); //List<string>
                List<string>? bindings = null;
                if (containsBindings)
                {
                    bindings = bindingsJson.EnumerateArray().Select(x => x.GetString()).ToList();
                }

                var containsPorts = dockerData.TryGetProperty("PublishedPorts", out var publishedPorts); //List<string>

                List<string>? ports = null;
                if (containsPorts)
                {
                    ports = publishedPorts.EnumerateArray().Select(x => x.GetString()).ToList();
                }

                var networks = await GetNetworks(dockerSettings, logger, client, dockerData);


                RestartPolicy restartPolicy = new();
                if (dockerData.TryGetProperty("RestartPolicy", out var restartPolicyData))
                {
                    restartPolicy = restartPolicyData.Deserialize<RestartPolicy>() ?? new();
                }
                var dockerInfo = new DockerInfo(client, imageName, containerName, bindings, networks, ports, restartPolicy, instanceId, logger);
                var success = await ExecuteContainerCommand(dockerInfo, command);

                if (success.Item1)
                {
                    instanceId = success.Item2;
                    clientRunner = new DockerClientRunner(dockerInfo);
                    clientRunner.Start();
                }

                var hostBuilder = new HostBuilder();
                var host = hostBuilder.Build();
                host.Run();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Fatal Crash in application");
                throw;
            }
        }

        internal static async Task<string[]> GetNetworks(DockerSettings dockerSettings, ILogger logger, DockerClient client, JsonElement dockerData)
        {
            List<string> networks = new();

            var containsNetworks = dockerData.TryGetProperty("Networks", out var manifestNetworks); //List<string>

            if (containsNetworks)
            {
                networks = manifestNetworks.EnumerateArray().Select(x => x.GetString()).ToList();
            }

            if (!containsNetworks && dockerData.TryGetProperty("FallbackNetworks", out var fallbackNetworks))
            {
                networks = fallbackNetworks.EnumerateArray().Select(x => x.GetString()).ToList();
            }

            if (dockerSettings.Networks.Count == 0 && networks.Count == 0)
            {
                var ownContainerId = Dns.GetHostName();
                logger.Info($"Container Id is: {ownContainerId}");

                var all = await client.Containers.ListContainersAsync(new ContainersListParameters()
                {
                    Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    { "id", new Dictionary<string, bool>
                        {
                            { ownContainerId, true }
                        }
                    }
                }
                });
                var own = all.FirstOrDefault();

                networks = own is not null ? own.NetworkSettings.Networks.Select(x => x.Key).ToList() : dockerSettings.DefaultNetworks;

                logger.Info($"Found own network: {own is not null}, Use Networks: {string.Join(',', networks)}");
            }
            else if (networks.Count == 0)
            {
                networks = dockerSettings.Networks;
            }

            networks.AddRange(dockerSettings.AdditionalNetworks);

            if (dockerData.TryGetProperty("AdditionalNetworks", out var additionalNetworks))
            {
                networks.AddRange(additionalNetworks.EnumerateArray().Select(x => x.GetString()));
            }

            return networks.Distinct().ToArray();
        }


        internal static async Task<ContainerListResponse?> GetContainer(DockerClient client, string? containerName)
        {
            var existings = await client.Containers.ListContainersAsync(new ContainersListParameters() { All = true });
            return existings.FirstOrDefault(x => x.Names.Contains($"/{containerName}"));
        }

        internal static async Task<(bool, Guid)> ExecuteContainerCommand(DockerInfo dockerInfo, Command command)
        {
            var imageName = dockerInfo.ImageName;
            var containerName = dockerInfo.ContainerName;
            var bindings = dockerInfo.Bindings;
            var networks = dockerInfo.Networks;
            var ports = dockerInfo.Ports;
            var restartPolicy = dockerInfo.RestartPolicy;
            var instanceId = dockerInfo.InstanceId;
            var client = dockerInfo.Client;
            var logger = dockerInfo.Logger;

            logger.Debug("Start trying to create container");
            var prog = new Progress<JSONMessage>();
            var existing = await GetContainer(client, containerName);

            if (command == Command.Stop)
            {
                if (existing is null)
                {
                    logger.Debug($"No Container found to stop with name{containerName}");
                    return (false, Guid.Empty);
                }
                logger.Debug($"Stopping existing container {existing.Names.First()}");
                await client.Containers.StopContainerAsync(existing.ID, new() { });
                return (true, instanceId);
            }
            if (command == Command.Start)
            {
                if (existing is null)
                {
                    logger.Debug($"No Container found to start with name{containerName}");
                    return (false, Guid.Empty);
                }

                logger.Debug($"Starting existing container {existing.Names.First()}");
                await client.Containers.StartContainerAsync(existing.ID, new() { });
                return (true, instanceId);
            }

            if (existing is not null)
            {
                logger.Debug($"Deleting existing container {existing.Names.First()}");

                await client.Containers.RemoveContainerAsync(existing.ID, new() { Force = true });
            }

            if (command == Command.Delete)
                return (true, Guid.Empty);

            if (command == Command.Recreate)
            {


                logger.Debug($"Creating image {imageName}");
                await client.Images.CreateImageAsync(new() { FromImage = imageName }, new AuthConfig(), prog);

                logger.Debug($"Creating container {imageName}");

                var endpointConfigs = new Dictionary<string, EndpointSettings>();
                var firstNetwork = networks.First();

                endpointConfigs[firstNetwork] = new EndpointSettings()
                {
                    NetworkID = firstNetwork
                };
                var container = await client.Containers.CreateContainerAsync(new()
                {
                    Image = imageName,
                    NetworkingConfig = new()
                    {
                        EndpointsConfig = endpointConfigs
                    },
                    HostConfig = new HostConfig
                    {
                        Binds = bindings?.ToArray(),
                        RestartPolicy = restartPolicy,
                        PublishAllPorts = true
                    },
                    Name = containerName,
                    Env = new List<string> { $"DockerPluginInstanceId={instanceId}" }

                    //ExposedPorts = ports?.ToDictionary(x => x, x => new EmptyStruct()),

                })
                ;

                foreach (var item in networks.Skip(1))
                {
                    logger.Debug($"Connect container {container.ID} to network {item}");
                    await client.Networks.ConnectNetworkAsync(item, new() { EndpointConfig = new() { NetworkID = item }, Container = container.ID });
                }

                logger.Debug($"Start container {container.ID}");

                return (await client.Containers.StartContainerAsync(container.ID, new()), instanceId);
            }

            return (false, Guid.Empty);
        }

        internal static void Prog_ProgressChanged(object sender, JSONMessage e)
        {
            Console.WriteLine(e.ProgressMessage);
        }
        //Volumes = new Dictionary<string, EmptyStruct>(){{ "G:\\:/opt", new()}} 

    }
}
