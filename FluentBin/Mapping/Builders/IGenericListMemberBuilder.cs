using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders
{
    public interface IGenericListMemberBuilder<out TBuilder, T, TElement> : IGenericCollectionMemberBuilder<TBuilder, T, IList<TElement>, TElement>
        where TBuilder : IGenericListMemberBuilder<TBuilder, T, TElement>
    {
        TBuilder LastElementWhen(Expression<Func<ICollectionMemberContext<T, IList<TElement>>, bool>> expression);
    }
}