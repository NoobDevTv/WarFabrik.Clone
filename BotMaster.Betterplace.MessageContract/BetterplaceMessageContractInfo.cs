using BotMaster.PluginSystem;
using System;

namespace BotMaster.Betterplace.MessageContract
{
    public readonly struct BetterplaceMessageContractInfo : IMessageContractInfo, IEquatable<BetterplaceMessageContractInfo>
    {
        public readonly Guid UID { get; }
        public readonly string Name { get; }

        public BetterplaceMessageContractInfo(Guid guid, string name)
        {
            UID = guid;
            Name = name;
        }

        public static BetterplaceMessageContractInfo Create()
            => new(new Guid("524FED8B-38C6-4241-B5A0-84752A6964AD"), nameof(BetterplaceMessageContractInfo));
        
        public override bool Equals(object obj)
            => obj is BetterplaceMessageContractInfo info 
            && Equals(info);

        public bool Equals(BetterplaceMessageContractInfo other) 
            => UID.Equals(other.UID) 
            && Name == other.Name;

        public override int GetHashCode() 
            => HashCode.Combine(UID, Name);

        public static bool operator ==(BetterplaceMessageContractInfo left, BetterplaceMessageContractInfo right) 
            => left.Equals(right);

        public static bool operator !=(BetterplaceMessageContractInfo left, BetterplaceMessageContractInfo right) 
            => !(left == right);
    }
}
