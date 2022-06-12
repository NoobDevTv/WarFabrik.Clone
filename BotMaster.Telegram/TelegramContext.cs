using BotMaster.Commandos;
using BotMaster.MessageContract;

using System.Reactive.Disposables;

using Telegram.Bot;

namespace BotMaster.Telegram
{

    internal record TelegramContext(TelegramBotClient Client, CommandoCentral CommandoCentral) : IDisposable
    {
        private const string Plattform = "Telegram";
        private CompositeDisposable disposables = new();


        public void Dispose()
        {
            disposables.Dispose();
        }

        public void AddCommand(Func<CommandMessage, bool> guard, Action<CommandMessage> action)
        {
            disposables.Add(CommandoCentral.AddCommand(guard, action));
        }

        public void AddCommand(Func<CommandMessage, bool> guard, Action<CommandMessage> action, params string[] commandNames)
        {
            disposables.Add(CommandoCentral.AddCommand(guard, action, commandNames.Select(x => new PersistentCommand(x, "", Plattform)).ToArray()));
        }
        public void AddCommand(Action<CommandMessage> action, params string[] commandNames)
        {
            disposables.Add(CommandoCentral.AddCommand(action, commandNames.Select(x => new PersistentCommand(x, "", Plattform)).ToArray()));
        }
    }
}
