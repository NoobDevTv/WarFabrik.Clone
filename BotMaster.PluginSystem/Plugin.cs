
using NonSucking.Framework.Extension.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BotMaster.PluginSystem
{
    public abstract class Plugin
    {
        public virtual void Register(ITypeContainer typeContainer)
        {

        }

        public virtual IEnumerable<IMessageContractInfo> ExposeContracts()
            => Enumerable.Empty<IMessageContractInfo>();

        public virtual IEnumerable<IMessageContractInfo> ConsumeContracts()
            => Enumerable.Empty<IMessageContractInfo>();

        //ToDo: Notifications instead packages
        public abstract IObservable<Package> Start(IObservable<Package> receivedPackages);
    }
}
