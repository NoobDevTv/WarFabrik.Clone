using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.DockerRunner;

internal class DockerSettings
{
    public string[] Networks { get; set; } = Array.Empty<string>();
    public string[] DefaultNetworks { get; set; } = Array.Empty<string>();
}
