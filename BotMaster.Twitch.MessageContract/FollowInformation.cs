
using BotMaster.Core;

using NonSucking.Framework.Serialization;

namespace BotMaster.Twitch.MessageContract
{
    [Nooson]
    public readonly partial struct FollowInformation : IEquatable<FollowInformation>
    {
        [NoosonInclude, NoosonOrder(0)]
        public const int TypeId = 0;

        public string UserName { get; }

        public string UserId { get; }
        [NoosonCustom(SerializeImplementationType = typeof(DateTimeSerializer), SerializeMethodName = nameof(DateTimeSerializer.DateTimeToBinary)
            , DeserializeImplementationType = typeof(DateTimeSerializer), DeserializeMethodName = nameof(DateTimeSerializer.DateTimeFromBinary))]
        public DateTime Since { get; }

        public FollowInformation(string userName, string userId, DateTime since)
        {
            UserName = userName;
            UserId = userId;
            Since = since;
        }

        public FollowInformation(string userName, string userId, DateTime since, int typeId) : this(userName, userId, since)
        {
        }

        public override bool Equals(object obj) => obj is FollowInformation information && Equals(information);
        public bool Equals(FollowInformation other) => UserName == other.UserName && UserId == other.UserId && Since == other.Since;
        public override int GetHashCode() => HashCode.Combine(UserName, UserId, Since);

        public static bool operator ==(FollowInformation left, FollowInformation right) => left.Equals(right);
        public static bool operator !=(FollowInformation left, FollowInformation right) => !(left == right);
    }
}
