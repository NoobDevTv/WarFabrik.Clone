using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.DockerRunner;

internal class DockerSettings
{
    public List<string> Networks { get; set; } = new();
    public List<string> DefaultNetworks { get; set; } = new();
    public List<string> AdditionalNetworks { get; set; } = new();
}
