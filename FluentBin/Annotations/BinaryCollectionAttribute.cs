namespace FluentBin
{
    /// <summary>
    /// Base attribute for reading collection typed values like arrays and lists.
    /// </summary>
    public class BinaryCollectionAttribute : BinaryMemberAttribute
    {
        /// <summary>
        /// Name of a method returning stream position where collection item should be read.
        /// </summary>
        /// <remarks>
        /// The method should have the following signature:
        /// <code>long GetElementPosition(TCollection collection, int index, long currentPosition)</code>
        /// </remarks>
        public string GetElementPositionMethod { get; set; }
        /// <summary>
        /// Name of a method constructing an instance for collection item.
        /// </summary>
        /// <remarks>
        /// The method should have the following signature:
        /// <code>TElement CreateElement(TCollection collection, int index)</code>
        /// </remarks>
        public string ElementFactoryMethod { get; set; }
        /// <summary>
        /// Name of a method to be called when an error occurs during collection item reading.
        /// </summary>
        /// <remarks>
        /// The method should have the following signature:
        /// <code>void OnElementError(BinaryReadingErrorArgs args)</code>
        /// </remarks>
        public string OnElementErrorMethod { get; set; }
    }
}