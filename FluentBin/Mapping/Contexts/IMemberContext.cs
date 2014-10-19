namespace FluentBin.Mapping.Contexts
{
    public interface IMemberContext<out T, out TMember> : IContext<T>
    {
        TMember Value { get; }
    }
}