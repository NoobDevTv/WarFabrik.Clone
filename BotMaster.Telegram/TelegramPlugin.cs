using BotMaster.Core;
using BotMaster.PluginSystem;
using BotMaster.PluginSystem.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotMaster.Telegram
{
    public sealed class TelegramPlugin : Plugin
    {
        public override IObservable<Package> Start(IObservable<Package> receivedPackages)
        {            
            MessageContract.Contract.ToDefineMessages(MessageConvert.ToMessage(receivedPackages));
            throw new NotImplementedException();
        }
    }
}
