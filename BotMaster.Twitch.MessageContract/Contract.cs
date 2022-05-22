using BotMaster.PluginSystem.Messages;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Twitch.MessageContract
{
    public static class Contract
    {
        public static readonly Guid UID = new("02128092-1EAF-4CC0-A544-AC951BC83C49");
        //public static int Id { get; private set; } = -2;

        public static bool CanConvert(Message message)
            => message.ContractUID == UID && message.Type == MessageType.Custom; //TODO: Contract id

        public static IObservable<TwitchMessage> ToDefineMessages(IObservable<Message> messages)
            => messages
                .Where(CanConvert)
                .Select(TwitchMessage.FromMessage);

        public static IObservable<Message> ToMessages(IObservable<TwitchMessage> messages)
            => messages
                .Select(m => m.ToMessage());
    }
}
