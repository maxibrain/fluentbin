namespace FluentBin.Mapping.Builders
{
    public interface IArrayMemberBuilder<T, TElement> : IGenericArrayMemberBuilder<IArrayMemberBuilder<T, TElement>, T, TElement>
    {
        
    }
}