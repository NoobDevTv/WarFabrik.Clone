using BotMaster.Configuraiton;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster;
internal class BotmasterConfig : ISetting
{
    public string ConfigName => "Botmaster";

    public bool RunPluginsInOwnProcess { get; set; }
    public string RunnersPath { get; set; }
    public string PluginsPath { get; set; }
}
