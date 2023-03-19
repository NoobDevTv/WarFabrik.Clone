
using BotMaster.Core;

using NonSucking.Framework.Serialization;

namespace BotMaster.BotSystem.MessageContract
{
    [Nooson]
    public partial record struct GetPlugins
    {
        [NoosonInclude, NoosonOrder(0)]
        public const int TypeId = 0;

        public GetPlugins()
        {
        }

        private GetPlugins(int typeId)
        {
        }

    }
}
