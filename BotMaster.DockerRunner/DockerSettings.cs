namespace BotMaster.DockerRunner;

internal class DockerSettings
{
    public List<string> Networks { get; set; } = new();
    public List<string> DefaultNetworks { get; set; } = new();
    public List<string> AdditionalNetworks { get; set; } = new();
}
