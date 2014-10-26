using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders.Impl
{
    abstract class CollectionMemberBuilder<TBuilder, T, TMember, TElement> : GenericMemberBuilder<TBuilder, T, TMember>, IGenericCollectionMemberBuilder<TBuilder, T, TMember, TElement>
        where TMember : ICollection<TElement> 
        where TBuilder : CollectionMemberBuilder<TBuilder, T, TMember, TElement>
    {
        private Action<IMemberBuilder<TMember, TElement>> _elementBuilderConfiguration;

        protected CollectionMemberBuilder(MemberExpression expression)
            : base(expression)
        {
        }

        protected Expression Invoke<TResult>(Expression<Func<ICollectionMemberContext<T, TMember>, TResult>> expression,
            ExpressionBuilderArgs args, ParameterExpression innerResultVar, ParameterExpression iVar)
        {
            return Expression.Invoke(expression, AdvancedExpression.CollectionMemberContext<T, TMember>(args.BrParameter, args.InstanceVar, innerResultVar, iVar));
        }

        protected override Expression BuildBodyExpression(ExpressionBuilderArgs args, ParameterExpression innerResultVar, ParameterExpression typeVar)
        {
            var elementType = typeof(TElement);
            var iVar = Expression.Variable(typeof(int), "i");
            var elementVar = Expression.Variable(elementType, "iterator");
            var loopEndLabel = Expression.Label();
            var elementBuilder = MemberBuilderFactory.Create<TMember, TElement>("[i]");
            if (_elementBuilderConfiguration != null)
            {
                _elementBuilderConfiguration((IMemberBuilder<TMember, TElement>)elementBuilder);
            }
            var innerArgs = args.Clone();
            innerArgs.InstanceVar = innerResultVar;
            return Expression.Block(
                new[] {iVar, elementVar},
                Expression.Assign(iVar, Expression.Constant(0)),
                AdvancedExpression.Debug("Reading collection {0}...", Expression.Constant(MemberName)),
                Expression.Loop(
                    Expression.IfThenElse(
                        GetLoopCondition(args, innerResultVar, iVar, elementVar),
                        Expression.Block(
                            AdvancedExpression.Debug("Reading {0}[{1}]...", Expression.Constant(MemberName), iVar),
                            Expression.Assign(elementVar, elementBuilder.BuildExpression(innerArgs)),
                            InsertElement(args, innerResultVar, iVar, elementVar),
                            Expression.PostIncrementAssign(iVar)),
                        Expression.Break(loopEndLabel)),
                    loopEndLabel));
        }

        protected abstract Expression GetLoopCondition(ExpressionBuilderArgs args, ParameterExpression innerResultVar, ParameterExpression iVar, ParameterExpression iteratorVar);
        protected abstract Expression InsertElement(ExpressionBuilderArgs args, ParameterExpression innerResultVar, ParameterExpression iVar, ParameterExpression iteratorVar);
        
        public TBuilder Element(Action<IMemberBuilder<TMember, TElement>> elementBuilderConfiguration)
        {
            _elementBuilderConfiguration = elementBuilderConfiguration;
            return (TBuilder)this;
        }
    }
}
