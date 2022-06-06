
using CommandManagementSystem;
using CommandManagementSystem.Attributes;

namespace BotMaster.Twitch
{
    [CommandManager("BotManager", "WarFabrik.Clone.Commands")]
    class BotCommandManager : CommandManager<string, BotCommandArgs, bool>
    {

    }
}
