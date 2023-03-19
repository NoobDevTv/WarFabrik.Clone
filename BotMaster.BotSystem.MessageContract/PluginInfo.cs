using NonSucking.Framework.Serialization;

namespace BotMaster.BotSystem.MessageContract
{
    [Nooson]
    public partial record struct PluginInfo(
        string Id,
        string Name,
        string Description,
        string Author,
        string Version,
        bool Running);
}
