using BotMaster.MessageContract;
using BotMaster.PluginSystem;
using BotMaster.PluginSystem.Messages;

using System.Reactive.Linq;

using WarFabrik.Clone;


using DefinedMessageContract = BotMaster.MessageContract.Contract;

namespace BotMaster.Twitch
{
    public class TwitchService : Plugin
    {
        private readonly IMessageContractInfo[] messageContracts;

        public TwitchService()
        {
            messageContracts = new[]
            {
                (IMessageContractInfo)Betterplace.MessageContract.BetterplaceMessageContractInfo.Create()
            };
        }

        public override IObservable<Package> Start(IObservable<Package> receivedPackages)
            => Observable.Using(CreateBot,
                botInstance
                    => MessageConvert.ToPackage(Create(MessageConvert.ToMessage(receivedPackages), botInstance))
                );

        private static IObservable<Message> Create(IObservable<Message> notifications, BotInstance botInstance)
        {
            var messages = DefinedMessageContract
                   .ToDefineMessages(notifications)
                   .VisitMany(
                        textMessage => textMessage
                            .Do(message => botInstance.Bot.SendMessage(message.Text)).Select(x => (DefinedMessage)x),
                        commandMessage => commandMessage.Where(message => message.Command == "twitch")
                            .Do(x => botInstance.Bot.SendMessage(string.Join(' ', x.Parameter)))
                            .Select(x => (DefinedMessage)x)
                       );

            return Observable.Using(
                () => messages.Subscribe(),
                _ => Observable
                    .FromAsync(botInstance.Bot.Run)
                    .SelectMany(_ => GetMessage(botInstance.NewFollower, botInstance.Raids)));
        }

        public override IEnumerable<IMessageContractInfo> ConsumeContracts()
            => messageContracts;

        private static IObservable<Message> GetMessage(IObservable<FollowInformation> newFollower, IObservable<string> raids)
        {
            var rawMessages =
                Observable
                .Merge(
                    raids,
                    newFollower
                        .Select(follow => "Following person just followed: " + follow.UserName)
                )
                .Select(DefinedMessage.CreateTextMessage);

            return DefinedMessageContract.ToMessages(rawMessages);
        }

        private static BotInstance CreateBot()
        {
            var bot = new Bot();

            var raids =
                Observable.FromEventPattern<string>(
                    add => bot.OnRaid += add,
                    remove => bot.OnRaid -= remove)
                .Select(args => args.EventArgs);

            return new(bot, bot.FollowerSubject, raids);
        }

        private record BotInstance(Bot Bot, IObservable<FollowInformation> NewFollower, IObservable<string> Raids) : IDisposable
        {
            public void Dispose()
            {
                Bot.Disconnect();
            }
        }
    }
}
