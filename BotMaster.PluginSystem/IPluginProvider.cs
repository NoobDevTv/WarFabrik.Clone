using NLog;

using NonSucking.Framework.Extension.IoC;

namespace BotMaster.PluginSystem;
public interface IPluginProvider
{
    IObservable<PluginInstance> GetPluginInstances(ILogger logger, ITypeContainer typeContainer);
}