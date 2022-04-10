using NonSucking.Framework.Serialization;
namespace BotMaster.MessageContract
{
    [Nooson]
    public partial struct CommandMessage : IEquatable<CommandMessage>
    {
        [NoosonInclude, NoosonOrder(0)]
        public const int TypeId = 1;

        public string Username { get; set; }
        public string Command { get; set; }

        public IReadOnlyCollection<string> Parameter { get; }

        public CommandMessage(string command, string username, IReadOnlyCollection<string> parameter)
        {
            Command = command;
            Parameter = parameter;
            Username = username;
        }

        private CommandMessage(string command, string username, int typeId, IReadOnlyCollection<string> parameter)
        {
            Command = command;
            Parameter = parameter;
            Username = username;
        }

        public override bool Equals(object obj) => obj is CommandMessage message && Equals(message);
        public bool Equals(CommandMessage other) => Command == other.Command && EqualityComparer<IReadOnlyCollection<string>>.Default.Equals(Parameter, other.Parameter);
        public override int GetHashCode() => HashCode.Combine(Command, Parameter);

        public static bool operator ==(CommandMessage left, CommandMessage right) => left.Equals(right);
        public static bool operator !=(CommandMessage left, CommandMessage right) => !(left == right);

        public override string ToString() => $"{Username}: {Command}";
    }
}
