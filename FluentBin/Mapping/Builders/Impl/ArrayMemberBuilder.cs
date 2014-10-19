using System;
using System.Diagnostics;
using System.Linq.Expressions;
using FluentBin.Mapping.Contexts;
using FluentBin.Mapping.Contexts.Impl;

namespace FluentBin.Mapping.Builders.Impl
{
    class ArrayMemberBuilder<T, TElement> : CollectionMemberBuilder<T, TElement[], TElement>, IArrayMemberBuilder<T, TElement>
    {
        private Expression<Func<IContext<T>, ulong>> _length;

        public ArrayMemberBuilder(MemberExpression expression)
            : base(expression)
        {
        }

        #region Implementation of IArrayMemberBuilder<T,TElement>

        public IArrayMemberBuilder<T, TElement> Length(UInt64 length)
        {
            return Length(c => length);
        }

        public IArrayMemberBuilder<T, TElement> Length(Expression<Func<IContext<T>, UInt64>> expression)
        {
            _length = expression;
            return this;
        }

        #endregion

        protected override Expression BuildCtorExpression(ExpressionBuilderArgs args)
        {
            var elementType = MemberType.GetElementType();
            var lengthVar = Expression.Variable(typeof (UInt64), "length");
            var ctorExp = Expression.NewArrayBounds(
                elementType,
                lengthVar);
            return Expression.Block( new [] { lengthVar },
                Expression.Assign(lengthVar, Invoke(_length, args)),
                AdvancedExpression.Debug(string.Format("Array {0} initialized. ({{0}} elements).", MemberName), lengthVar),
                ctorExp);
        }

        protected override Expression BuildBodyExpression(ExpressionBuilderArgs args, ParameterExpression innerResultVar, ParameterExpression typeVar)
        {
            var elementType = MemberType.GetElementType();
            if (elementType == typeof(Byte))
            {
                return Expression.Call(
                    args.BrParameter,
                    "ReadBits",
                    null,
                    Expression.New(
                        typeof (BinarySize).GetConstructor(new[] {typeof (UInt64), typeof (Byte)}),
                        Invoke(_length, args),
                        Expression.Constant((Byte)0, typeof(Byte))),
                    Expression.Constant(null, typeof(Endianness?)));
            }
            else
            {
                return base.BuildBodyExpression(args, innerResultVar, typeVar);
            }
        }

        protected override Expression GetLoopCondition(ExpressionBuilderArgs args, ParameterExpression innerResultVar, ParameterExpression iVar, ParameterExpression iteratorVar)
        {
            return Expression.LessThan(iVar, Expression.ArrayLength(innerResultVar));
        }

        protected override Expression InsertElement(ExpressionBuilderArgs args, ParameterExpression innerResultVar, ParameterExpression iVar, ParameterExpression iteratorVar)
        {
            return Expression.Assign(Expression.ArrayAccess(innerResultVar, iVar), iteratorVar);
        }
    }
}
