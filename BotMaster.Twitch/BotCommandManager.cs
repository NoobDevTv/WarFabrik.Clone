
using CommandManagementSystem;
using CommandManagementSystem.Attributes;

using System;
using System.Collections.Generic;
using System.Text;

namespace BotMaster.Twitch
{
    [CommandManager("BotManager", "WarFabrik.Clone.Commands")]
    class BotCommandManager : CommandManager<string, BotCommandArgs, bool>
    {

    }
}
