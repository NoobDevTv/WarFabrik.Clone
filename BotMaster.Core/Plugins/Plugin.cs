
using NonSucking.Framework.Extension.IoC;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotMaster.Core.Plugins
{
    public abstract class Plugin
    {
        public virtual void Register(ITypeContainer typeContainer)
        {

        }

        //ToDo: Notifications instead packages
        public abstract IObservable<Package> Start(IObservable<Package> receivedPackages);
    }
}
