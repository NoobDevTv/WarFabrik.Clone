using BotMaster.BotSystem.MessageContract;
using BotMaster.Livestream.MessageContract;
using BotMaster.MessageContract;

using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BotMaster.Web.AdministrationUi;

public class SystemMessageHandler
{
    public IObservable<PluginList> PluginListInformation { get; private set; }
    private Subject<SystemMessage> Messages { get; } = new();
    private DateTime createdAt = DateTime.Now;

    public SystemMessageHandler()
    {
            
    }

    public void RequestPluginList()
    {
        Messages.OnNext(new GetPlugins());
    }

    public void ExecuteCommand(Command command, string pluginId)
    {
        Messages.OnNext(new PluginCommand() { Command = command, PluginId = pluginId });
    }

    public IObservable<SystemMessage> SetMessages(IObservable<SystemMessage> messages)
    {
        var internalMessages = messages.Publish();

        var pluginList = internalMessages.Match((PluginList pluginList) => pluginList);
     
        PluginListInformation = pluginList;

        return Observable.Using(internalMessages.Connect, _ => Messages);
    }
}
