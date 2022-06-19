using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotMaster.Betterplace.Model
{
    public record struct Alert (int Id, string Name, string Message, int Amount, DateTime Created);
}
