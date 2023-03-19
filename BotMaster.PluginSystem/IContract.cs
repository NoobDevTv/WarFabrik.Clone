using BotMaster.PluginSystem.Messages;

namespace BotMaster.PluginSystem;

public interface IContract<T>
{
    public static abstract bool CanConvert(Message message);
    public static abstract IObservable<T> ToDefineMessages(IObservable<Message> messages);
    public static abstract IObservable<Message> ToMessages(IObservable<T> messages);
}