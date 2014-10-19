using System;
using System.Linq.Expressions;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders
{
    public interface ICollectionMemberBuilder<T, TMember, TElement> : IClassMemberBuilder<T, TMember>
    {
        ICollectionMemberBuilder<T, TMember, TElement> Element(Action<IMemberBuilder<TMember, TElement>> elementBuilderConfiguration);

/*
        ICollectionMemberBuilder<T, TMember, TElement> UseElementFactory(
            Expression<Func<ICollectionMemberContext<T, TMember>, TElement>> expression);

        ICollectionMemberBuilder<T, TMember, TElement> ElementPosition(
            Expression<Func<ICollectionMemberContext<T, TMember>, UInt64>> expression);

        ICollectionMemberBuilder<T, TMember, TElement> ElementSizeOf(Binary i);
*/
    }
}