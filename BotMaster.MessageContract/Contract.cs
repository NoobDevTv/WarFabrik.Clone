using BotMaster.PluginSystem.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.MessageContract
{
    public static class Contract
    {

        public static bool CanConvert(Message message)
            => message.Type == MessageType.Defined;

        public static IObservable<DefinedMessage> ToDefineMessages(IObservable<Message> messages)
            => messages
                .Where(CanConvert)
                .Select(DefinedMessage.FromMessage);

        public static IObservable<Message> ToMessages(IObservable<DefinedMessage> messages)
            => messages
                .Select(m => m.ToMessage());
    }
}
