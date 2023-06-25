using NonSucking.Framework.Serialization;

namespace BotMaster.BotSystem.MessageContract
{
    [Nooson]
    public partial record struct PluginInfo(
        Guid Id,
        string Name,
        string Description,
        string Author,
        string Version,
        bool Running)
    {
        [NoosonInclude, NoosonOrder(0)]
        public const int TypeId = 3;

    }
}
