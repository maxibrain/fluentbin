namespace FluentBin.Mapping.Builders
{
    public interface IListMemberBuilder<T, TElement> : IGenericListMemberBuilder<IListMemberBuilder<T, TElement>, T, TElement>
    {
        
    }
}