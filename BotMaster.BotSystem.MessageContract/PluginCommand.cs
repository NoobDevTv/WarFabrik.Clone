using NonSucking.Framework.Serialization;

namespace BotMaster.BotSystem.MessageContract;

[Nooson]
public partial record struct PluginCommand
{

    [NoosonInclude, NoosonOrder(0)]
    public const int TypeId = 2;

    public Guid InstanceId { get; set; }
    public Command Command { get; set; }
}

public enum Command
{
    Start,
    Stop
}
