using System.Text.Json;
using System.Text.Json.Serialization;

namespace BotMaster.PluginSystem
{
    public enum ManifestInfo
    {
        None = 0,
        File = 1,
        Remote = 2,
    }

    public sealed class ConnectionInfo : IEquatable<ConnectionInfo>
    {
        public string Type { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement> ExtensionData { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ConnectionInfo);
        }

        public bool Equals(ConnectionInfo other)
        {
            return other is not null &&
                   Type == other.Type &&
                   EqualityComparer<Dictionary<string, JsonElement>>.Default.Equals(ExtensionData, other.ExtensionData);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, ExtensionData);
        }

        public static bool operator ==(ConnectionInfo left, ConnectionInfo right)
        {
            return EqualityComparer<ConnectionInfo>.Default.Equals(left, right);
        }

        public static bool operator !=(ConnectionInfo left, ConnectionInfo right)
        {
            return !(left == right);
        }
    }

    public sealed class PluginManifest : IEquatable<PluginManifest>
    {
        [JsonIgnore]
        public FileInfo CurrentFileInfo { get; set; }
        [JsonIgnore]
        public ManifestInfo ManifestInfo { get; set; }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string File { get; set; }
        public string ProcessRunner { get; set; }
        public ConnectionInfo Connection { get; set; }

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
            && File == other.File
            && Connection == other.Connection;

        public override int GetHashCode()
            => HashCode.Combine(Id, Name, Description, Author, Version, File, ProcessRunner, Connection);

        public static bool operator ==(PluginManifest left, PluginManifest right)
            => EqualityComparer<PluginManifest>.Default.Equals(left, right);
        public static bool operator !=(PluginManifest left, PluginManifest right)
            => !(left == right);
    }
}
