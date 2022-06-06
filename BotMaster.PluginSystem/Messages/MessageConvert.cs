using System.Reactive.Linq;

namespace BotMaster.PluginSystem.Messages
{
    public static class MessageConvert
    {
        public static IObservable<Message> ToMessage(IObservable<Package> packages)
            => packages.Select(p => Message.FromPackage(p));

        public static IObservable<Package> ToPackage(IObservable<Message> messages)
            => messages.Select(m => m.ToPackage());
    }
}
