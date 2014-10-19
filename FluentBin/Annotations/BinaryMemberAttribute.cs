using System;

namespace FluentBin
{
    /// <summary>
    /// Basic attribute for describing reading process of a property or a field. Attribute is optional. All public properties and fields considered to be read unless BinaryIgnore attribute is specified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class BinaryMemberAttribute : Attribute
    {
        /// <summary>
        /// Size of value in bytes. Valid for value types only.
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// Name of a property that calculates size of value in bytes. Valid for value types only.
        /// </summary>
        /// <remarks>Should return ulong.</remarks>
        public string SizeMember { get; set; }
        /// <summary>
        /// Reading order. Member with Order equals to int.MinValue get read first, int.MaxValue - last. Default is 0.
        /// </summary>
        /// <remarks>If several members have the same order value they will be read in the order of appearance in declaring class.</remarks>
        public int Order { get; set; }
        /// <summary>
        /// Name of a method returning stream position where the current member should be read.
        /// </summary>
        /// <remarks>
        /// The method should have the following signature:
        /// <code>long GetPosition(long currentPosition)</code>
        /// </remarks>
        public string GetPositionMethod { get; set; }
        /// <summary>
        /// Name of a method returning stream position to set after the current member has been read.
        /// </summary>
        /// <remarks>
        /// The method should have the following signature:
        /// <code>long GetPositionAfter(long currentPosition, long beforePosition)</code>
        /// </remarks>
        public string GetPositionAfterMethod { get; set; }
        /// <summary>
        /// Name of a method constructing an instance for the current member.
        /// </summary>
        /// <remarks>
        /// The method should have the following signature:
        /// <code>T Create()</code>
        /// </remarks>
        public string FactoryMethod { get; set; }
        /// <summary>
        /// Name of a method selecting type of instance to be constructed for the current member.
        /// </summary>
        /// <remarks>
        /// The method should have the following signature:
        /// <code>Type SelectType()</code>
        /// </remarks>
        public string TypeSelectorMethod { get; set; }
        /// <summary>
        /// Name of a method converting the current member value after it has been read.
        /// </summary>
        /// <remarks>
        /// The method should have the following signature:
        /// <code>T Convert(T value)</code>
        /// </remarks>
        public string ConverterMethod { get; set; }
        /// <summary>
        /// Name of a method to be called after the current member has been read.
        /// </summary>
        /// <remarks>
        /// The method should have the following signature:
        /// <code>void AfterRead(T value)</code>
        /// </remarks>
        public string AfterReadMethod { get; set; }
        /// <summary>
        /// Name of a method determining if the current member should be read.
        /// </summary>
        /// <remarks>
        /// The method should have the following signature:
        /// <code>bool ShouldRead()</code>
        /// </remarks>
        public string ConditionMethod { get; set; }
    }
}