using System.Text.Json;
using System.Text.Json.Serialization;

namespace BotMaster.PluginSystem
{
    public sealed class PluginManifest : IEquatable<PluginManifest>
    {
        [JsonIgnore]
        public FileInfo CurrentFileInfo { get; set; }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string File { get; set; }
        public string ProcessRunner { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement> ExtensionData { get; set; }

        public override bool Equals(object obj) 
            => Equals(obj as PluginManifest);

        public bool Equals(PluginManifest other) 
            => other != null 
            && Id == other.Id 
            && Name == other.Name 
            && Description == other.Description 
            && Author == other.Author 
            && Version == other.Version 
            && ProcessRunner == other.ProcessRunner
            && File == other.File;

        public override int GetHashCode() 
            => HashCode.Combine(Id, Name, Description, Author, Version, File, ProcessRunner);

        public static bool operator ==(PluginManifest left, PluginManifest right) 
            => EqualityComparer<PluginManifest>.Default.Equals(left, right);
        public static bool operator !=(PluginManifest left, PluginManifest right) 
            => !(left == right);
    }
}
