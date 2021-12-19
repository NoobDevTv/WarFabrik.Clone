using System;

namespace NonSucking.Framework.Serialization
{
    [AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
    public class NoosonPreferredCtorAttribute : Attribute
    {
    }
}