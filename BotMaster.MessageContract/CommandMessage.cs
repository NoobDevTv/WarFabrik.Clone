using NonSucking.Framework.Serialization;
namespace BotMaster.MessageContract
{
    [Nooson]
    public partial struct CommandMessage : IEquatable<CommandMessage>
    {
        [NoosonInclude, NoosonOrder(0)]
        public const int TypeId = 1;

        public string Username { get; set; }
        public int UserId { get; set; }
        public string Command { get; set; }
        public string SourcePlattform { get; set; }
        public string PlattformUserId { get; set; }
        public bool Secure { get; set; }
        public IReadOnlyCollection<string> Parameter { get; }

        public CommandMessage(string command, string username, int userId, string sourcePlattform, string plattformUserId, bool secure, IReadOnlyCollection<string> parameter)
        {
            Command = command;
            Parameter = parameter;
            Username = username;
            UserId = userId;
            SourcePlattform = sourcePlattform;
            PlattformUserId = plattformUserId;
            Secure = secure;
        }

        private CommandMessage(string command, string username, int userId, string sourcePlattform, string plattformUserId, bool secure, int typeId, IReadOnlyCollection<string> parameter)
        {
            Command = command;
            Parameter = parameter;
            Username = username;
            UserId = userId;
            SourcePlattform = sourcePlattform;
            PlattformUserId = plattformUserId;
            Secure = secure;
        }

        public override bool Equals(object obj) => obj is CommandMessage message && Equals(message);
        public bool Equals(CommandMessage other) => Command == other.Command
            && EqualityComparer<IReadOnlyCollection<string>>.Default.Equals(Parameter, other.Parameter)
            && UserId == other.UserId
            && SourcePlattform == other.SourcePlattform
            && Secure == other.Secure
            && PlattformUserId == other.PlattformUserId;
        public override int GetHashCode() => HashCode.Combine(Command, Parameter);

        public static bool operator ==(CommandMessage left, CommandMessage right) => left.Equals(right);
        public static bool operator !=(CommandMessage left, CommandMessage right) => !(left == right);

        public override string ToString() => $"{Username}({SourcePlattform}|{UserId}): {Command}";
    }
}
