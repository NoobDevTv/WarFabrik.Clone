using BotMaster.Betterplace.MessageContract;
using BotMaster.MessageContract;
using BotMaster.PluginSystem;
using BotMaster.PluginSystem.Messages;
using System.Reactive.Linq;
using WarFabrik.Clone;
using static WarFabrik.Clone.FollowerServiceNew;
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
                (IMessageContractInfo)BetterplaceMessageContractInfo.Create()
            };
        }

        public override IObservable<Package> Start(IObservable<Package> receivedPackages)
            => Observable.Using(CreateBot,
                botInstance
                    => MessageConvert
                        .ToPackage(
                            Observable
                            .FromAsync(token => botInstance.Bot.Run(token))
                            .SelectMany(_ => GetMessage(botInstance.NewFollower, botInstance.Raids))
                        )
                );

        public override IEnumerable<IMessageContractInfo> ConsumeContracts()
            => messageContracts;

        private static IObservable<Message> GetMessage(IObservable<NewFollowerDetectedArgs> newFollower, IObservable<string> raids)
        {
            var rawMessages =
                Observable
                .Merge(
                    raids,
                    newFollower
                        .Select(follow => "Following people just followed: " + string.Join(", ", follow.NewFollowers.Select(x => x.UserName)))
                )
                .Select(s => DefinedMessage.CreateTextMessage(s));

            return DefinedMessageContract.ToMessages(rawMessages);
        }

        private static BotInstance CreateBot()
        {
            var bot = new Bot();
            var newFollower =
                Observable.FromEventPattern<NewFollowerDetectedArgs>(
                    add => bot.FollowerService.OnNewFollowersDetected += add,
                    remove => bot.FollowerService.OnNewFollowersDetected -= remove)
                .Select(args => args.EventArgs);

            var raids =
                Observable.FromEventPattern<string>(
                    add => bot.OnRaid += add,
                    remove => bot.OnRaid -= remove)
                .Select(args => args.EventArgs);

            return new(bot, newFollower, raids);
        }

        private record BotInstance(Bot Bot, IObservable<NewFollowerDetectedArgs> NewFollower, IObservable<string> Raids) : IDisposable
        {
            public void Dispose()
            {
                Bot.Disconnect();
            }
        }
    }
}
