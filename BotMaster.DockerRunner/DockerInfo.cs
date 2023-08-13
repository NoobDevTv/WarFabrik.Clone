using Docker.DotNet;
using Docker.DotNet.Models;

using NLog;

namespace BotMaster.DockerRunner;
internal class DockerInfo
{
    public DockerClient Client { get; set; }
    internal string? ImageName { get; set; }
    internal string? ContainerName { get; set; }
    internal List<string>? Bindings { get; set; }
    internal string[] Networks { get; set; }
    internal List<string>? Ports { get; set; }
    internal RestartPolicy RestartPolicy { get; set; }
    internal Guid InstanceId { get; set; }
    public Logger Logger { get; set; }

    public DockerInfo(Docker.DotNet.DockerClient client, string? imageName, string? containerName, List<string>? bindings, string[] networks, List<string>? ports, RestartPolicy restartPolicy, Guid? instanceId, NLog.Logger logger)
    {
        Client = client;
        ImageName = imageName;
        ContainerName = containerName;
        Bindings = bindings;
        Networks = networks;
        Ports = ports;
        RestartPolicy = restartPolicy;
        InstanceId = instanceId ?? Guid.NewGuid();
        Logger = logger;
    }
}