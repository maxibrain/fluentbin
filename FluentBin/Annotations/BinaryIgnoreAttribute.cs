using System;

namespace FluentBin.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class BinaryIgnoreAttribute : Attribute
    {
    }
}
