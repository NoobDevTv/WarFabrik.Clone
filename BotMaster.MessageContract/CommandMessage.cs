using NonSucking.Framework.Serialization;

namespace BotMaster.MessageContract
{
    [Nooson]
    public partial class CommandMessage
    {
        [NoosonInclude, NoosonOrder(0)]
        public const int TypeId = 1;

        public string Command { get; set; }

        public IReadOnlyCollection<string> Parameter { get; }

        public CommandMessage(string command, IReadOnlyCollection<string> parameter)
        {
            Command = command;
            Parameter = parameter;
        }

        private CommandMessage(string command, int typeId, IReadOnlyCollection<string> parameter)
        {
            Command = command;
            Parameter = parameter;

        }
    }
}
