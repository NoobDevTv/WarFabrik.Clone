using System;

namespace NonSucking.Framework.Serialization
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class NoosonIncludeAttribute : Attribute
    {
    }
}