using BotMaster.Commandos;
using BotMaster.MessageContract;

using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using NLog;

using System.Reactive.Disposables;


namespace BotMaster.YouTube
{
    internal record YoutubeContext(YouTubeService Api, YoutubeClient Client, YoutubeMetaData MetaData, Channel Channel, CommandoCentral CommandoCentral, CompositeDisposable Disposables) : IDisposable
    {
        public Logger Logger { get; } = LogManager.GetLogger($"{nameof(Bot)}_{MetaData.User_Id}");
        internal const string Plattform = "Youtube";

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
            Disposables.Add(CommandoCentral.AddCommand(guard, action, commandNames.Select(x => new PersistentCommand(x, "", Plattform)).ToArray()));
        }
        public void AddCommand(Action<CommandMessage> action, params string[] commandNames)
        {
            Disposables.Add(CommandoCentral.AddCommand(action, commandNames.Select(x => new PersistentCommand(x, "", Plattform)).ToArray()));
        }
    }
}
