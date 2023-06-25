using BotMaster.BotSystem.MessageContract;
using BotMaster.PluginSystem;
using BotMaster.PluginSystem.Messages;
using BotMaster.Web.AdministrationUi;

using ILogger = NLog.ILogger;

namespace BotMaster.Betterplace;

public sealed class WebUiService : Plugin
{

    public override IObservable<Package> Start(ILogger logger, IObservable<Package> receivedPackages)
    {
        var handler = Program.SystemMessageHandler;

        return MessageConvert
                .ToPackage(
                    Create(logger,
                           handler,
                           MessageConvert.ToMessage(receivedPackages)));
    }



    public IObservable<Message> Create(ILogger logger, SystemMessageHandler handler, IObservable<Message> receivedMessages)
    {

        return SystemContract.ToMessages(handler.SetMessages(receivedMessages));
    }

}

