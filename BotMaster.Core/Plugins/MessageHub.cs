using BotMaster.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Core.Plugins
{
    public class MessageHub : IDisposable
    {
        private readonly Subject<Message> internalSubject;

        public MessageHub()
        {
            internalSubject = new Subject<Message>();
        }

        public IDisposable SubscribeAsSender(IObservable<Message> messages)
            => messages.Subscribe(internalSubject);

        public IObservable<Message> SubscribeAsReceiver(string targetId)
            => internalSubject.Where(message => string.IsNullOrEmpty(message.TargetId) || message.TargetId == targetId);

        public void Dispose()
        {
            internalSubject.Dispose();
        }
    }
}
