using NonSucking.Framework.Serialization;

namespace BotMaster.BotSystem.MessageContract
{
    [Nooson]
    public partial record struct GetPlugins
    {
        [NoosonInclude, NoosonOrder(0)]
        public const int TypeId = 0;

        public bool[] Empty { get; set; } = Array.Empty<bool>();

        public GetPlugins()
        {
        }

        private GetPlugins(int typeId)
        {
        }

    }
}
