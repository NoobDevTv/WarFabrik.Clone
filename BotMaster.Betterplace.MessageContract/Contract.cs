using BotMaster.PluginSystem.Messages;
using System.Reactive.Linq;

namespace BotMaster.Betterplace.MessageContract
{
    public static class BetterplaceContract
    {
        public static readonly Guid UID = new("524FED8B-38C6-4241-B5A0-84752A6964AD");
        //public static int Id { get; private set; } = -1;

        public static bool CanConvert(Message message)
            => message.ContractUID == UID && message.Type == MessageType.Custom; 

        public static IObservable<BetterplaceMessage> ToDefineMessages(IObservable<Message> messages)
            => messages
                .Where(CanConvert)
                .Select(BetterplaceMessage.FromMessage);

        public static IObservable<Message> ToMessages(IObservable<BetterplaceMessage> messages)
            => messages
                .Select(m => m.ToMessage());
    }
}
