
using BotMaster.Commandos;
using BotMaster.MessageContract;

using NLog;
using System.Reactive.Disposables;

using TwitchLib.Api;
using TwitchLib.Client;

namespace BotMaster.Twitch
{
    internal record TwitchContext(TwitchAPI Api, TwitchClient Client, string UserId, string Channel, CommandoCentral CommandoCentral, CompositeDisposable Disposables) : IDisposable
    {
        public Logger Logger { get; } = LogManager.GetLogger($"{nameof(Bot)}_{UserId}");
        internal const string Plattform = "Twitch";

        public void Dispose()
        {
            Disposables.Dispose();
        }

        public void AddCommand(Func<CommandMessage, bool> guard, Action<CommandMessage> action)
        {
            Disposables.Add(CommandoCentral.AddCommand(guard, action));
        }

        public void AddCommand(Func<CommandMessage, bool> guard, Action<CommandMessage> action, params string[] commandNames)
        {
            Disposables.Add(CommandoCentral.AddCommand(guard, action, commandNames.Select(x=>new PersistentCommand(x, "", Plattform)).ToArray()));
        }
        public void AddCommand(Action<CommandMessage> action, params string[] commandNames)
        {
            Disposables.Add(CommandoCentral.AddCommand(action, commandNames.Select(x => new PersistentCommand(x, "", Plattform)).ToArray()));
        }
    }
}
