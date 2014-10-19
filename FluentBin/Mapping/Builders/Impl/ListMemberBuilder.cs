using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentBin.Mapping.Contexts;
using FluentBin.Mapping.Contexts.Impl;

namespace FluentBin.Mapping.Builders.Impl
{
    class ListMemberBuilder<T, TElement> : CollectionMemberBuilder<T, IList<TElement>, TElement>, IListMemberBuilder<T, TElement>
    {
        private Expression<Func<ICollectionMemberContext<T, IList<TElement>>, bool>> _lastElementWhen;

        public ListMemberBuilder(MemberExpression expression)
            : base(expression)
        {
            _lastElementWhen = c => c.StreamPosition >= c.StreamLength;
        }

        #region Implementation of IListMemberBuilder<T,TElement>

        public IListMemberBuilder<T, TElement> LastElementWhen(Expression<Func<ICollectionMemberContext<T, IList<TElement>>, bool>> expression)
        {
            _lastElementWhen = expression;
            return this;
        }

        #endregion

        protected override Expression GetLoopCondition(ExpressionBuilderArgs args, ParameterExpression innerResultVar, ParameterExpression iVar, ParameterExpression iteratorVar)
        {
            return Expression.Not(Invoke(_lastElementWhen, args, innerResultVar, iVar));
        }

        protected override Expression InsertElement(ExpressionBuilderArgs args, ParameterExpression innerResultVar, ParameterExpression iVar, ParameterExpression iteratorVar)
        {
            return Expression.Call(innerResultVar, "Add", null, iteratorVar);
        }
    }
}
