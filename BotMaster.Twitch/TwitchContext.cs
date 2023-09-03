
using BotMaster.Commandos;
using BotMaster.MessageContract;
using BotMaster.Twitch.Commands;

using NLog;
using System.Reactive.Disposables;

using TwitchLib.Api;
using TwitchLib.Client;

namespace BotMaster.Twitch
{
    internal record TwitchContext(
        TwitchAPI Api,
        TwitchClient Client,
        TwitchLib.PubSub.TwitchPubSub pubSub,
        string UserId,
        string Channel,
        CommandoCentral CommandoCentral,
        CompositeDisposable Disposables,
        SerialDisposable TextCommandDisposables
        ) : IDisposable
    {
        public Logger Logger { get; } = LogManager.GetLogger($"{nameof(Bot)}_{UserId}");
        internal const string Plattform = "Twitch";
        private CompositeDisposable textCommandDisposables;

        public void Dispose()
        {
            Disposables.Dispose();
            TextCommandDisposables.Dispose();
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
        
        public void ReinitializeDbCommands()
        {
            textCommandDisposables = new();
            TextCommandDisposables.Disposable = textCommandDisposables;
        }

        public void AddDbCommand(PersistentCommand item)
        {
            textCommandDisposables.Add(CommandoCentral.AddCommand(x=> SimpleCommands.SendTextCommand(x, item, this), new PersistentCommand(item.Command, "", Plattform)));
        }
    }
}
