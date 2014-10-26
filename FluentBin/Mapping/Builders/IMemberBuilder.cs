namespace FluentBin.Mapping.Builders
{
    public interface IMemberBuilder<T> : IGenericMemberBuilder<IMemberBuilder<T>, T>
    {
        
    }

    public interface IMemberBuilder<T, TMember> : IGenericMemberBuilder<IMemberBuilder<T, TMember>, T, TMember>
    {

    }
}