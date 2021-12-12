using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.PluginSystem
{
    public interface IMessageContractInfo
    {
        public Guid UID { get;  }
        public string Name { get;  }
    }
}
