using NonSucking.Framework.Serialization;

namespace BotMaster.Livestream.MessageContract
{
    [Nooson]
    public partial struct FollowInformation : IEquatable<FollowInformation>
    {
        [NoosonInclude, NoosonOrder(0)]
        public const int TypeId = 0;

        public string UserName { get; }
        public string UserId { get; }
        public DateTime Since { get; }
        public string SourcePlattform { get; }

        public FollowInformation(string userName, string userId, DateTime since, string sourcePlattform)
        {
            UserName = userName;
            UserId = userId;
            Since = since;
            SourcePlattform = sourcePlattform;
        }

        public FollowInformation(string userName, string userId, DateTime since, string sourcePlattform, int typeId) : this(userName, userId, since, sourcePlattform)
        {
        }

        public override bool Equals(object obj) => obj is FollowInformation information && Equals(information);
        public bool Equals(FollowInformation other) => UserName == other.UserName && UserId == other.UserId && SourcePlattform == other.SourcePlattform;
        public override int GetHashCode() => HashCode.Combine(UserName, UserId);

        public static bool operator ==(FollowInformation left, FollowInformation right) => left.Equals(right);
        public static bool operator !=(FollowInformation left, FollowInformation right) => !(left == right);
    }
}
