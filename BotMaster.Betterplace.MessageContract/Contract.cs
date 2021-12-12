using BotMaster.PluginSystem.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Betterplace.MessageContract
{
    public static class Contract
    {
        public static readonly Guid UID = new("524FED8B-38C6-4241-B5A0-84752A6964AD");
        public static int Id { get; private set; } = -1;

        public static bool CanConvert(Message message)
            => message.Type == MessageType.Custom; //TODO: Contract id

        public static IObservable<BetterplaceMessage> ToDefineMessages(IObservable<Message> messages)
            => messages
                .Where(CanConvert)
                .Select(BetterplaceMessage.FromMessage);

        public static IObservable<Message> ToMessages(IObservable<BetterplaceMessage> messages)
            => messages
                .Select(m => m.ToMessage());
    }
}
