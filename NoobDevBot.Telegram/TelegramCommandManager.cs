using CommandManagementSystem;
using CommandManagementSystem.Attributes;

namespace NoobDevBot.Telegram
{
    [CommandManager("BotManager", "NoobDevBot.Telegram.Commands")]
    internal class TelegramCommandManager : CommandManager<string, TelegramCommandArgs, bool>
    {
    }
}