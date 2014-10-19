using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders
{
    public interface IListMemberBuilder<T, TElement> : ICollectionMemberBuilder<T, IList<TElement>, TElement>
    {
        IListMemberBuilder<T, TElement> LastElementWhen(
            Expression<Func<ICollectionMemberContext<T, IList<TElement>>, Boolean>> expression);
    }
}