using BotMaster.PluginSystem;
using BotMaster.PluginSystem.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.MessageContract
{
    public sealed class Contract : IMessageContract<DefinedMessage>
    {
        public static readonly Guid UID = new("A4B5EF4B-248B-4831-A6F4-6EB1ED6088D2");
        public static int Id { get; private set; } = -1;

        private Contract() { }

        public static void SetId(int id)
        {
            Id = id;
        }

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
