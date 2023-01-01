using BotMaster.Core.Configuration;
using BotMaster.PluginHost;
using BotMaster.PluginSystem;
using BotMaster.PluginSystem.PluginCreator;

using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Extensions.Logging;

using System.Reactive.Disposables;
using System.Reactive.Linq;
using Docker.DotNet;
using Docker.DotNet.Models;

using Plugin = BotMaster.PluginSystem.Plugin;
using System.Text.Json;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace BotMaster.DockerRunner
{
    class Program
    {
        static async Task Main(string[] args)
        {

            if (args.Length == 0)
                args = new[] { "-l", "plugins/BotMaster.Twitch/plugin.manifest.json" };

            var config = ConfigManager.GetConfiguration("appsettings.json", args);

            using var logManager = Disposable.Create(LogManager.Shutdown);

            var logger = LogManager
               .Setup()
               .LoadConfigurationFromSection(config)
               .GetCurrentClassLogger();

            var dockerSettings = config.GetSection("Docker").Get<DockerSettings>();

            //await DockerPlayground();
            var client = new DockerClientConfiguration()
                .CreateClient();
            string[] networks = Array.Empty<string>();

            if (dockerSettings.Networks.Length == 0)
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

                if (own is not null)
                    networks = own.NetworkSettings.Networks.Select(x => x.Key).ToArray();
                else
                    networks = dockerSettings.DefaultNetworks;

                logger.Info($"Found own network: {own is not null}, Use Networks: {string.Join(',', networks)}");
            }
            else
            {
                networks = dockerSettings.Networks;
            }


            var info = new FileInfo(Path.Combine(".", "logs", $"pluginhost-{DateTime.Now:ddMMyyyy-HHmmss_fff}.log"));

            if (!info.Directory!.Exists)
                info.Directory.Create();


            var plugins = new List<Plugin>();
            logger.Debug("Gotten the following args: " + string.Join(" | ", args));

            try
            {
                for (int i = 0; i < args.Length; i++)
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
                            continue;
                        var imageName = imageNameJson.GetString();

                        var containsContainerName = dockerData.TryGetProperty("ContainerName", out var containerNameJson); //string
                        string? containerName = null;
                        if (containsContainerName)
                            containerName = containerNameJson.GetString();

                        var containsBindings = dockerData.TryGetProperty("Bindings", out var bindingsJson); //List<string>
                        List<string>? bindings = null;
                        if (containsBindings)
                            bindings = bindingsJson.EnumerateArray().Select(x => x.GetString()).ToList();

                        var success = await CreateContainer(client, imageName, containerName, bindings, networks, logger);


                        //TODO Later
                        //var containtsNetworks = dockerData.TryGetProperty("Networks", out var networks);
                        //var containtsPublishedPorts = dockerData.TryGetProperty("PublishedPorts", out var publishedPorts);

                    }
                }


            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Fatal Crash in application");
                throw;
            }
        }

        private static async Task<bool> CreateContainer(DockerClient client, string imageName, string? containerName, List<string>? bindings, string[] networks, ILogger logger)
        {
            logger.Debug("Start trying to create container");
            var prog = new Progress<JSONMessage>();
            var existings = (await client.Containers.ListContainersAsync(new ContainersListParameters() { All = true }));
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
                },
                Name = containerName
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
