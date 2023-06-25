using BotMaster.BotSystem.MessageContract;
using BotMaster.Livestream.MessageContract;
using BotMaster.MessageContract;
using BotMaster.PluginSystem.Messages;

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using DefinedContract = BotMaster.MessageContract.Contract;

namespace BotMaster.Web.AdministrationUi;

public class SystemMessageHandler
{
    public IObservable<IReadOnlyCollection<PluginInfo>> PluginListInformation { get; private set; }
    public IObservable<ChatMessage> ChatMessages { get; private set; }
    private Subject<SystemMessage> Messages { get; } = new();

    public SystemMessageHandler()
    {

    }

    public void RequestPluginList()
    {
        Messages.OnNext(new GetPlugins());
    }

    public void ExecuteCommand(Command command, Guid instanceId)
    {
        Messages.OnNext(new PluginCommand() { Command = command, InstanceId = instanceId });
    }

    public IObservable<SystemMessage> SetMessages(IObservable<Message> receivedMessages)
    {

        receivedMessages = receivedMessages.Publish().RefCount(); 

        var systemMessages = SystemContract.ToDefineMessages(receivedMessages);
        var internalMessages = systemMessages.Publish();

        var infoList = internalMessages.Match((PluginList pluginList) => pluginList).Select(x => x.PluginInfos).Publish().RefCount();
        var infos = internalMessages.Match((PluginInfo pluginInfo) => pluginInfo);

        var pluginList = infos.CombineLatest(infoList).Select(data =>
        {
            var info = data.First;
            var existing = data.Second.FirstOrDefault(x => x.Id == info.Id);
            if (existing != default)
                data.Second.Remove(existing);
            data.Second.Add(info);
            return (IReadOnlyCollection<PluginInfo>)data.Second;
        });

        PluginListInformation = infoList.Merge(pluginList);

        var incommingDefinedMessages = DefinedContract
           .ToDefineMessages(receivedMessages);

        var chatMessages = incommingDefinedMessages.Publish();
        ChatMessages = chatMessages.Match((ChatMessage chatMessage) => chatMessage);

#pragma warning disable DF0001,DF0100
        return Observable.Using(() => StableCompositeDisposable.Create(internalMessages.Connect(), chatMessages.Connect()), _ => Messages);
#pragma warning restore DF0001,DF0100
    }
}
