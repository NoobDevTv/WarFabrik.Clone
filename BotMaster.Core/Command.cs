using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Core;

public enum Command
{
    Recreate,
    Start,
    Stop,
    Delete,
    GetState = 10
}
