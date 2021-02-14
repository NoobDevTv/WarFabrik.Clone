using BotMaster.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Core.Messages
{
    public static class MessageConvert
    {
        public static IObservable<Message> ToMessage(IObservable<Package> packages)
            => packages.Select(p => Message.FromPackage(p));

        public static IObservable<Package> ToPackage(IObservable<Message> messages)
            => messages.Select(m => m.ToPackage());
    }
}
