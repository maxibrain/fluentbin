namespace FluentBin.Mapping.Builders
{
    public interface ICollectionMemberBuilder<T, TMember, TElement> : IGenericCollectionMemberBuilder<ICollectionMemberBuilder<T, TMember, TElement>, T, TMember, TElement>
    {
        
    }
}