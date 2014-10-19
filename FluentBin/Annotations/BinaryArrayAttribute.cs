namespace FluentBin
{
    /// <summary>
    /// Attribute for reading array typed property.
    /// </summary>
    public class BinaryArrayAttribute : BinaryCollectionAttribute
    {
        /// <summary>
        /// Length of array.
        /// </summary>
        /// <remarks>LengthMember has higher priority than Length</remarks>
        public ulong Length { get; set; }
        /// <summary>
        /// Name of a property that calculates length of array. Must return ulong.
        /// </summary>
        /// <remarks>The specified property should return ulong. LengthMember has higher priority than Length</remarks>
        public string LengthMember { get; set; }
    }
}