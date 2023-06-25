using NonSucking.Framework.Serialization;

namespace BotMaster.Livestream.MessageContract
{
    [Nooson]

    public readonly partial struct StreamLiveInformation : IEquatable<StreamLiveInformation>
    {
        [NoosonInclude, NoosonOrder(0)]
        public const int TypeId = 2;

        public string UserId { get; }
        public string UserName { get; }
        public string SourcePlattform { get; }

        public bool Online { get; }

        public StreamLiveInformation(string userId, string userName, string sourcePlattform, bool online)
        {
            UserId = userId;
            UserName = userName;
            SourcePlattform = sourcePlattform;
            Online = online;
        }

        public StreamLiveInformation(string userId, string userName, string sourcePlattform, bool online, int typeId) : this(userId, userName, sourcePlattform, online)
        {
            UserId = userId;
            UserName = userName;
            SourcePlattform = sourcePlattform;
            Online = online;
        }

        public override bool Equals(object obj) => obj is StreamLiveInformation information && Equals(information);
        public bool Equals(StreamLiveInformation other) => UserId == other.UserId && UserName == other.UserName && SourcePlattform == other.SourcePlattform && Online == other.Online;
        public override int GetHashCode() => HashCode.Combine(UserId, UserName, SourcePlattform, Online);

        public static bool operator ==(StreamLiveInformation left, StreamLiveInformation right) => left.Equals(right);
        public static bool operator !=(StreamLiveInformation left, StreamLiveInformation right) => !(left == right);
    }
}
