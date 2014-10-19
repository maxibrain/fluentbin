namespace FluentBin.Mapping.Contexts.Impl
{
    class MemberContext<T, TMember> : Context<T>, IMemberContext<T, TMember>
    {
        public MemberContext(T o, TMember value, BinaryOffset position, BinarySize length) 
            : base(o, position, length)
        {
            Value = value;
        }

        public TMember Value { get; private set; }
    }
}