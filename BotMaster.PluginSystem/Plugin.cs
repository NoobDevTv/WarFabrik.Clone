
using NonSucking.Framework.Extension.IoC;

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
