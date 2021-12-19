using System;

namespace NonSucking.Framework.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Interface | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class NoosonCustomAttribute : Attribute
    {
        public Type SerializeImplementationType { get; set; }
        public Type DeserializeImplementationType { get; set; }
        public string SerializeMethodName { get; set; }
        public string DeserializeMethodName { get; set; }

        public const string deserializeName = "Deserialize";
        public const string serializeName = "Serialize";
    }
}