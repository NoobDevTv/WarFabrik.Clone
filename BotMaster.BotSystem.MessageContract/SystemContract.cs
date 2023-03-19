using BotMaster.PluginSystem;
using BotMaster.PluginSystem.Messages;
using System.Reactive.Linq;

namespace BotMaster.BotSystem.MessageContract
{
    public class SystemContract : IContract<SystemMessage>
    {
        internal static readonly Guid UID = new("0D3A9E09-4307-4EF0-8B31-9E2486AE44E0");

        public static  bool CanConvert(Message message)
            => message.ContractUID == UID && message.Type == MessageType.Custom;

        public static  IObservable<SystemMessage> ToDefineMessages(IObservable<Message> messages)
            => messages
                .Where(CanConvert)
                .Select(SystemMessage.FromMessage);

        public static  IObservable<Message> ToMessages(IObservable<SystemMessage> messages)
            => messages
                .Select(m => m.ToMessage());
     
    }
}
