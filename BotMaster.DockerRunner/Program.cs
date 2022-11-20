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

namespace BotMaster.DockerRunner
{
    class Program
    {
        private static string ownNetwork;

        static async Task Main(string[] args)
        {
            //await DockerPlayground();
            var client = new DockerClientConfiguration()
                .CreateClient();
            var ownContainerId = Dns.GetHostName();

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
            ownNetwork = own.NetworkSettings.Networks.First().Key;

            if (args.Length == 0)
                args = new[] { "-l", "plugins/BotMaster.Twitch/plugin.manifest.json" };

            var config = ConfigManager.GetConfiguration("appsettings.json", args);

            using var logManager = Disposable.Create(LogManager.Shutdown);

            var info = new FileInfo(Path.Combine(".", "logs", $"pluginhost-{DateTime.Now:ddMMyyyy-HHmmss_fff}.log"));

            if (!info.Directory!.Exists)
                info.Directory.Create();

            var logger = LogManager
                .Setup()
                .LoadConfigurationFromSection(config)
                .GetCurrentClassLogger();

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

                        var success = await CreateContainer(client, imageName, containerName, bindings);
          

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

        private static async Task<bool> CreateContainer(DockerClient client, string imageName, string? containerName, List<string>? bindings)
        {
            var prog = new Progress<JSONMessage>();
            await client.Images.CreateImageAsync(new() { FromImage = imageName }, new AuthConfig(), prog);

            var cont = await client.Containers.CreateContainerAsync(new()
            {
                Image = imageName,
                NetworkingConfig = new()
                {
                    EndpointsConfig = new Dictionary<string, EndpointSettings>
                    {
                        { ownNetwork, new EndpointSettings()
                            {
                                NetworkID = ownNetwork
                            }
                        }
                    }
                },
                HostConfig = new HostConfig
                {
                    Binds = bindings?.ToArray(),//  new[] { "G:\\:/opt" },
                },
                Name = containerName
            });
            return await client.Containers.StartContainerAsync(cont.ID, new());
        }

        static async Task DockerPlayground()
        {
            DockerClient client = new DockerClientConfiguration()
                .CreateClient();


            var prog = new Progress<JSONMessage>();
            await client.Images.CreateImageAsync(new() { FromImage = "ghcr.io/noobdevtv/warfabrik.clone:develop" }, new AuthConfig(), prog);

            var cont = await client.Containers.CreateContainerAsync(new()
            {
                Image = "ghcr.io/noobdevtv/warfabrik.clone:develop",
                NetworkingConfig = new()
                {
                    EndpointsConfig = new Dictionary<string, EndpointSettings>
                    {
                        { ownNetwork, new EndpointSettings()
                            {
                                NetworkID = ownNetwork
                            }
                        }
                    }
                },
                HostConfig = new HostConfig
                {
                    Binds = new[] { "G:\\:/opt" },
                    PortBindings = new Dictionary<string, IList<PortBinding>> { { "988/tcp", new[] { new PortBinding { HostPort = "999" } } } },
                },
                ExposedPorts = new Dictionary<string, EmptyStruct>() { { "999/tcp", new() } },
                Name = "Test"
            });
            var success = await client.Containers.StartContainerAsync(cont.ID, new() { });

        }

        private static void Prog_ProgressChanged(object sender, JSONMessage e)
        {
            Console.WriteLine(e.ProgressMessage);
        }
        //Volumes = new Dictionary<string, EmptyStruct>(){{ "G:\\:/opt", new()}} 

    }
}
