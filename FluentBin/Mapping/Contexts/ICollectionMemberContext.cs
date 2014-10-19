namespace FluentBin.Mapping.Contexts
{
    public interface ICollectionMemberContext<out T, out TMember> : IMemberContext<T, TMember>
    {
        int Index { get; }
    }
}