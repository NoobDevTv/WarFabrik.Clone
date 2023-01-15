
using BotMaster.PluginSystem.Messages;

using NLog;
using NonSucking.Framework.Extension.IoC;

using System.Reactive.Linq;

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
        public abstract IObservable<Package> Start(ILogger logger, IObservable<Package> receivedPackages);


        protected static IObservable<Message> GetEmptyFrom<T>(IObservable<T> observe) => observe.IgnoreElements().Select(_ => Message.Empty);
    }
}
