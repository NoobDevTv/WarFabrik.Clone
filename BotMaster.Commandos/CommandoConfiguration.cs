using BotMaster.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Commandos;
public class CommandoConfiguration : ISetting
{
    public string ConfigName => nameof(CommandoConfiguration);
    public string ConnectionString { get; set; }
    public string DatabasePluginName { get; set; }

    public CommandoConfiguration()
    {
        ConnectionString = Path.Combine("additionalfiles", "Rights.db");
    }
}
