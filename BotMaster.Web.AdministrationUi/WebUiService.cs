using BotMaster.BotSystem.MessageContract;
using BotMaster.Commandos;
using BotMaster.Core;
using BotMaster.MessageContract;
using BotMaster.PluginSystem;
using BotMaster.PluginSystem.Messages;
using BotMaster.RightsManagement;
using BotMaster.Web.AdministrationUi;
using BotMaster.Web.AdministrationUi.Data;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using DefinedContract = BotMaster.MessageContract.Contract;


using Radzen;

using System.Reactive;
using System.Reactive.Linq;

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



    public IObservable<Message> Create(ILogger logger, SystemMessageHandler handler, IObservable<Message> receivedPackages)
    {
        receivedPackages = receivedPackages.Publish().RefCount();
        var systemMessages = SystemContract.ToDefineMessages(receivedPackages).Do(x=>logger.Info("Wir kommen an"));

        //TODO Implement correctly
        var incommingDefinedMessages = DefinedContract
           .ToDefineMessages(receivedPackages)
           .VisitMany(
                textMessage => Observable.Empty<DefinedMessage>(),
                commandMessage => Observable.Empty<DefinedMessage>(),
                chatMessage => chatMessage
                    .Do(message => logger.Debug($"[{message.Username}]: {message.Text}"))
                    .Select(x => (DefinedMessage)x)
            ).IgnoreElements().Subscribe();

        return SystemContract.ToMessages(handler.SetMessages(systemMessages));
    }

}

