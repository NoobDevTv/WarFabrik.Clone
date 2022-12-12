using BotMaster.Core.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.PluginSystem;
public class BotmasterConfig : ISetting
{
    public string ConfigName => "Botmaster";

    public string PluginCreator { get; set; }
    public string RunnersPath { get; set; }
    public string PluginsPath { get; set; }
    public ushort PortForPluginCreation { get; set; } = 6789;
}
