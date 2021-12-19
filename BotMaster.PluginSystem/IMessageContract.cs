using BotMaster.PluginSystem.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

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
