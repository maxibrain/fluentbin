using System;
using System.Linq.Expressions;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders
{
    public interface IGenericArrayMemberBuilder<out TBuilder, T, TElement> : IGenericCollectionMemberBuilder<TBuilder, T, TElement[], TElement>
        where TBuilder : IGenericArrayMemberBuilder<TBuilder, T, TElement>
    {
        TBuilder Length(UInt64 length);
        TBuilder Length(Expression<Func<IContext<T>, ulong>> expression);
    }
}