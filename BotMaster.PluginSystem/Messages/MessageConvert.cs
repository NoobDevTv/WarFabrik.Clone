using System.Reactive.Linq;

namespace BotMaster.PluginSystem.Messages
{
    public static class MessageConvert
    {
        public static IObservable<Message> ToMessage(IObservable<Package> packages)
            => packages.Select(Message.FromPackage);

        public static IObservable<Package> ToPackage(IObservable<Message> messages)
            => messages.Select(m => m.ToPackage());
    }
}
