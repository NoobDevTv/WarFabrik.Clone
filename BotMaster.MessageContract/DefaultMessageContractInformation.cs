using BotMaster.PluginSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.MessageContract
{
    public readonly struct DefaultMessageContractInformation : IMessageContractInfo, IEquatable<DefaultMessageContractInformation>
    {
        public readonly Guid UID { get; } 
        public readonly string Name { get;  }

        public DefaultMessageContractInformation(Guid guid, string name)
        {
            UID = guid;
            Name = name;
        }

        public static DefaultMessageContractInformation Create()
            => new(new Guid("A4B5EF4B-248B-4831-A6F4-6EB1ED6088D2"), nameof(DefaultMessageContractInformation));
        public override bool Equals(object obj) 
            => obj is DefaultMessageContractInformation information 
            && Equals(information);

        public bool Equals(DefaultMessageContractInformation other) 
            => UID.Equals(other.UID) 
            && Name == other.Name;
        public override int GetHashCode() 
            => HashCode.Combine(UID, Name);

        public static bool operator ==(DefaultMessageContractInformation left, DefaultMessageContractInformation right) 
            => left.Equals(right);
        public static bool operator !=(DefaultMessageContractInformation left, DefaultMessageContractInformation right) 
            => !(left == right);
    }
}
