namespace BotMaster.PluginSystem;
public class RunnerManifest
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Author { get; set; }
    public string Version { get; set; }
    public Dictionary<string, string> Filename { get; set; }
    public string Args { get; set; }
    public Dictionary<string, string> EnviromentVariable { get; set; }
}
