namespace FluentBin.Annotations
{
    /// <summary>
    /// Attribute for reading List&lt;T&gt; typed property.
    /// </summary>
    public class BinaryListAttribute : BinaryCollectionAttribute
    {
        /// <summary>
        /// Name of a method determining if the last element in the list has been read already.
        /// </summary>
        /// <remarks>Method must have the following signature: 
        /// <code>bool HasReadLastElement(List&lt;T&gt; result, T element, long currentPosition)</code>
        /// </remarks>
        public string HasReadLastElementMethod { get; set; }

        public BinaryListAttribute(string hasReadLastElementMethod)
        {
            HasReadLastElementMethod = hasReadLastElementMethod;
        }
    }
}