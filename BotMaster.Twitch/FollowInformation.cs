using System;
using System.Collections.Generic;
using System.Text;

namespace WarFabrik.Clone
{
    public readonly struct FollowInformation : IEquatable<FollowInformation>
    {
        public readonly string UserName { get; }
        public readonly string UserId { get; }
        public readonly DateTime Since { get; }

        public FollowInformation(string userName, string userId, DateTime since)
        {
            UserName = userName;
            UserId = userId;
            Since = since;
        }

        public override bool Equals(object obj)
            => obj is FollowInformation information && Equals(information);
        public bool Equals(FollowInformation other)
            => UserId == other.UserId && Since == other.Since;

        public override int GetHashCode()
            => HashCode.Combine(UserId, Since);

        public static bool operator ==(FollowInformation left, FollowInformation right)
            => left.Equals(right);

        public static bool operator !=(FollowInformation left, FollowInformation right)
            => !(left == right);
    }
}
