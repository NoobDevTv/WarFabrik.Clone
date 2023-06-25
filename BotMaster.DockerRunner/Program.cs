using BotMaster.Core.Configuration;
using BotMaster.PluginSystem;

using Docker.DotNet;
using Docker.DotNet.Models;

using Microsoft.Extensions.Configuration;

using NLog;
using NLog.Extensions.Logging;

using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;

namespace BotMaster.DockerRunner
{
    internal class Program
    {
        private static async Task Main(string[] args)
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
                for (var i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-l")
                    {
                        i++;
                        var pluginManifest = JsonSerializer.Deserialize<PluginManifest>(File.ReadAllText(args[i]), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        var dockerData = pluginManifest.ExtensionData["DockerData"];
                        var containsImageName = dockerData.TryGetProperty("ImageName", out var imageNameJson); //string
                        if (!containsImageName)
                        {
                            continue;
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

                        var success = await CreateContainer(client, imageName, containerName, bindings, networks, ports, restartPolicy, logger);
                    }
                    if (args[i] == "-s")
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Fatal Crash in application");
                throw;
            }
        }

        private static async Task<string[]> GetNetworks(DockerSettings dockerSettings, ILogger logger, DockerClient client, JsonElement dockerData)
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

        private static async Task<bool> CreateContainer(DockerClient client, string imageName, string? containerName, List<string>? bindings, string[] networks, List<string>? ports, RestartPolicy policy, ILogger logger)
        {
            logger.Debug("Start trying to create container");
            var prog = new Progress<JSONMessage>();
            var existings = await client.Containers.ListContainersAsync(new ContainersListParameters() { All = true });
            var existing = existings.FirstOrDefault(x => x.Names.Contains($"/{containerName}"));

            if (existing is not null)
            {
                logger.Debug($"Deleting existing container {existing.Names.First()}");

                await client.Containers.RemoveContainerAsync(existing.ID, new() { Force = true });
            }

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
                    RestartPolicy = policy,
                    PublishAllPorts = true
                },
                Name = containerName,
                Env = new List<string> { $"DockerPluginInstanceId:{Guid.NewGuid()}" }

                //ExposedPorts = ports?.ToDictionary(x => x, x => new EmptyStruct()),

            });

            foreach (var item in networks.Skip(1))
            {
                logger.Debug($"Connect container {container.ID} to network {item}");
                await client.Networks.ConnectNetworkAsync(item, new() { EndpointConfig = new() { NetworkID = item }, Container = container.ID });
            }

            logger.Debug($"Start container {container.ID}");

            return await client.Containers.StartContainerAsync(container.ID, new());
        }

        private static void Prog_ProgressChanged(object sender, JSONMessage e)
        {
            Console.WriteLine(e.ProgressMessage);
        }
        //Volumes = new Dictionary<string, EmptyStruct>(){{ "G:\\:/opt", new()}} 

    }
}
