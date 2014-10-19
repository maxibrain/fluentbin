using System;

namespace FluentBin.Mapping.Contexts.Impl
{
    class CollectionMemberContext<T, TMember> : MemberContext<T, TMember>, ICollectionMemberContext<T, TMember>
    {
        public CollectionMemberContext(T collection, TMember value, Int32 index, BinaryOffset position, BinarySize length)
            : base(collection, value, position, length)
        {
            Index = index;
        }

        public Int32 Index { get; private set; }
    }
}