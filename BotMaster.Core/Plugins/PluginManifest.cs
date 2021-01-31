using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BotMaster.Core.Plugins
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

        public override bool Equals(object obj) 
            => Equals(obj as PluginManifest);

        public bool Equals(PluginManifest other) 
            => other != null 
            && Id == other.Id 
            && Name == other.Name 
            && Description == other.Description 
            && Author == other.Author 
            && Version == other.Version 
            && File == other.File;

        public override int GetHashCode() 
            => HashCode.Combine(Id, Name, Description, Author, Version, File);

        public static bool operator ==(PluginManifest left, PluginManifest right) 
            => EqualityComparer<PluginManifest>.Default.Equals(left, right);
        public static bool operator !=(PluginManifest left, PluginManifest right) 
            => !(left == right);
    }
}
