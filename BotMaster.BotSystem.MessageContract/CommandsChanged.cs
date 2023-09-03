using BotMaster.Core;

using NonSucking.Framework.Serialization;

namespace BotMaster.BotSystem.MessageContract;

[Nooson]
public partial record CommandsChanged
{
    [NoosonInclude, NoosonOrder(0)]
    public const int TypeId = 4;

}
