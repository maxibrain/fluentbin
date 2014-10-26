using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentBin.Mapping.Contexts;
using FluentBin.Mapping.Contexts.Impl;

namespace FluentBin.Mapping.Builders.Impl
{
    class ListMemberBuilder<TBuilder, T, TElement> : CollectionMemberBuilder<TBuilder, T, IList<TElement>, TElement>, IGenericListMemberBuilder<TBuilder, T, TElement>
        where TBuilder : ListMemberBuilder<TBuilder, T, TElement>
    {
        private Expression<Func<ICollectionMemberContext<T, IList<TElement>>, bool>> _lastElementWhen;

        public ListMemberBuilder(MemberExpression expression)
            : base(expression)
        {
            _lastElementWhen = c => c.StreamPosition >= c.StreamLength;
        }

        #region Implementation of IListMemberBuilder<T,TElement>

        public TBuilder LastElementWhen(Expression<Func<ICollectionMemberContext<T, IList<TElement>>, bool>> expression)
        {
            _lastElementWhen = expression;
            return (TBuilder)this;
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

    class ListMemberBuilder<T, TElement> : ListMemberBuilder<ListMemberBuilder<T, TElement>, T, TElement>, IListMemberBuilder<T, TElement>
    {
        public ListMemberBuilder(MemberExpression expression) : base(expression)
        {
        }

        public new IListMemberBuilder<T, TElement> SizeOf(BinarySize size)
        {
            return base.SizeOf(size);
        }

        public new IListMemberBuilder<T, TElement> SizeOf(Expression<Func<IContext<T>, BinarySize>> expression)
        {
            return base.SizeOf(expression);
        }

        public new IListMemberBuilder<T, TElement> If(Expression<Func<IContext<T>, bool>> expression)
        {
            return base.If(expression);
        }

        public new IListMemberBuilder<T, TElement> SetOrder(int order)
        {
            return base.SetOrder(order);
        }

        public new IListMemberBuilder<T, TElement> Position(Expression<Func<IContext<T>, BinaryOffset>> expression)
        {
            return base.Position(expression);
        }

        public new IListMemberBuilder<T, TElement> OverrideEndianess(Endianness endianness)
        {
            return base.OverrideEndianess(endianness);
        }

        public new IListMemberBuilder<T, TElement> UseFactory(Expression<Func<IContext<T>, IList<TElement>>> expression)
        {
            return base.UseFactory(expression);
        }

        public new IListMemberBuilder<T, TElement> ConvertValue(Expression<Func<IMemberContext<T, IList<TElement>>, IList<TElement>>> expression)
        {
            return base.ConvertValue(expression);
        }

        public new IListMemberBuilder<T, TElement> Assert(Expression<Func<IMemberContext<T, IList<TElement>>, bool>> expression)
        {
            return base.Assert(expression);
        }

        public new IListMemberBuilder<T, TElement> AfterRead(Expression<Action<IMemberContext<T, IList<TElement>>>> expression)
        {
            return base.AfterRead(expression);
        }

        public new IListMemberBuilder<T, TElement> PositionAfter(Expression<Func<IMemberContext<T, IList<TElement>>, BinaryOffset>> expression)
        {
            return base.PositionAfter(expression);
        }

        public new IListMemberBuilder<T, TElement> Element(Action<IMemberBuilder<IList<TElement>, TElement>> elementBuilderConfiguration)
        {
            return base.Element(elementBuilderConfiguration);
        }

        public IListMemberBuilder<T, TElement> LastElementWhen(Expression<Func<ICollectionMemberContext<T, IList<TElement>>, bool>> expression)
        {
            return base.LastElementWhen(expression);
        }
    }
}
