using BotMaster.PluginSystem.Messages;
using System.Runtime.Versioning;

namespace BotMaster.PluginSystem
{
    [RequiresPreviewFeatures]
    public interface IMessageContract<TMessageType>
    {
        abstract static bool CanConvert(Message message);

        abstract static IObservable<TMessageType> ToDefineMessages(IObservable<Message> messages);

        abstract static IObservable<Message> ToMessages(IObservable<TMessageType> messages);
    }
}
