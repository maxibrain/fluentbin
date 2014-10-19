using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentBin.Mapping.Contexts;
using FluentBin.Mapping.Contexts.Impl;

namespace FluentBin.Mapping.Builders.Impl
{
    abstract class CollectionMemberBuilder<T, TMember, TElement> : MemberBuilder<T, TMember>, ICollectionMemberBuilder<T, TMember, TElement>
        where TMember : ICollection<TElement>
    {
        private Expression<Func<ICollectionMemberContext<T, TMember>, TElement>> _elementFactory;
        private Expression<Func<ICollectionMemberContext<T, TMember>, ulong>> _elementPosition;
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


        #region Implementation of ICollectionMemberBuilder<T,TMember,TElement>

        public ICollectionMemberBuilder<T, TMember, TElement> Element(Action<IMemberBuilder<TMember, TElement>> elementBuilderConfiguration)
        {
            _elementBuilderConfiguration = elementBuilderConfiguration;
            return this;
        }

        public ICollectionMemberBuilder<T, TMember, TElement> UseElementFactory(Expression<Func<ICollectionMemberContext<T, TMember>, TElement>> expression)
        {
            _elementFactory = expression;
            return this;
        }

        public ICollectionMemberBuilder<T, TMember, TElement> ElementPosition(Expression<Func<ICollectionMemberContext<T, TMember>, ulong>> expression)
        {
            _elementPosition = expression;
            return this;
        }

        #endregion

        protected override Expression BuildBodyExpression(ExpressionBuilderArgs args, ParameterExpression innerResultVar, ParameterExpression typeVar)
        {
            var elementType = typeof(TElement);
            //IExpressionBuilder typeBuilder = FileFormatBuilder.Types[elementType];
            var iVar = Expression.Variable(typeof(int), "i");
            var elementVar = Expression.Variable(elementType, "iterator");
            var loopEndLabel = Expression.Label();
            var elementBuilder = MemberBuilder<TMember, TElement>.Create("[i]");
            if (_elementBuilderConfiguration != null)
            {
                _elementBuilderConfiguration(elementBuilder);
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
/*
                            Expression.Assign(elementVar, elementCtor),
                            typeBuilder.BuildExpression(brParameter, elementVar, typeReaders),
*/
                            InsertElement(args, innerResultVar, iVar, elementVar),
                            Expression.PostIncrementAssign(iVar)),
                        Expression.Break(loopEndLabel)),
                    loopEndLabel));
        }

        protected abstract Expression GetLoopCondition(ExpressionBuilderArgs args, ParameterExpression innerResultVar, ParameterExpression iVar, ParameterExpression iteratorVar);
        protected abstract Expression InsertElement(ExpressionBuilderArgs args, ParameterExpression innerResultVar, ParameterExpression iVar, ParameterExpression iteratorVar);
    }
}
