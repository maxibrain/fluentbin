using System;
using System.Linq.Expressions;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders
{
    public interface IArrayMemberBuilder<T, TElement> : ICollectionMemberBuilder<T, TElement[], TElement>
    {
        IArrayMemberBuilder<T, TElement> Length(UInt64 length);
        IArrayMemberBuilder<T, TElement> Length(Expression<Func<IContext<T>, UInt64>> expression);
    }
}