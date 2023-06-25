using NonSucking.Framework.Serialization;

namespace BotMaster.BotSystem.MessageContract
{

    [Nooson]
    public partial record struct PluginList
    {
        [NoosonInclude, NoosonOrder(0)]
        public const int TypeId = 1;

        public List<PluginInfo> PluginInfos { get; set; }

        public PluginList()
        {
        }

        private PluginList(int typeId)
        {
        }

    }
}
