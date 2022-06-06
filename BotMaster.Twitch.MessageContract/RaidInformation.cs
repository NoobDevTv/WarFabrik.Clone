using NonSucking.Framework.Serialization;

namespace BotMaster.Twitch.MessageContract
{
    [Nooson]
    public readonly partial struct RaidInformation : IEquatable<RaidInformation>
    {
        [NoosonInclude, NoosonOrder(0)]
        public const int TypeId = 1;

        public string UserName { get; }
        public int Count { get; }
        public string PreDefinedMessage { get; }

        public RaidInformation(string userName, int count, string preDefinedMessage)
        {
            UserName = userName;
            Count = count;
            PreDefinedMessage = preDefinedMessage;
        }
        public RaidInformation(string userName, int count, string preDefinedMessage, int typeId) : this(userName, count, preDefinedMessage)
        {
            UserName = userName;
            Count = count;
            PreDefinedMessage = preDefinedMessage;
        }

        public override bool Equals(object obj) => obj is RaidInformation information && Equals(information);
        public bool Equals(RaidInformation other) => UserName == other.UserName && Count == other.Count && PreDefinedMessage == other.PreDefinedMessage;
        public override int GetHashCode() => HashCode.Combine(UserName, Count, PreDefinedMessage);

        public static bool operator ==(RaidInformation left, RaidInformation right) => left.Equals(right);
        public static bool operator !=(RaidInformation left, RaidInformation right) => !(left == right);
    }
}
