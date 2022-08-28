
using BotMaster.Core.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.RightsManagement;
public class RightConfiguration : ISetting
{
    public string ConfigName => nameof(RightConfiguration);
    public string ConnectionString { get; set; }
    public string DatabasePluginName { get; set; }
    public RightConfiguration()
    {
        ConnectionString = Path.Combine("additionalfiles", "Rights.db");
    }
}
