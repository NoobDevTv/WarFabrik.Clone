using CommandManagementSystem;
using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WarFabrik.Clone
{
    [CommandManager("BotManager", "WarFabrik.Clone.Commands")]
    class BotCommandManager : CommandManager<string, BotCommandArgs, bool>
    {
     
    }
}
