using System;
using System.Linq.Expressions;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders.Impl
{
    class ArrayMemberBuilder<TBuilder, T, TElement> : CollectionMemberBuilder<TBuilder, T, TElement[], TElement>, IGenericArrayMemberBuilder<TBuilder, T, TElement>
        where TBuilder : ArrayMemberBuilder<TBuilder, T, TElement>
    {
        private Expression<Func<IContext<T>, ulong>> _length;

        public ArrayMemberBuilder(MemberExpression expression)
            : base(expression)
        {
        }

        #region Implementation of IArrayMemberBuilder<T,TElement>

        public TBuilder Length(UInt64 length)
        {
            return Length(c => length);
        }

        public TBuilder Length(Expression<Func<IContext<T>, UInt64>> expression)
        {
            _length = expression;
            return (TBuilder)this;
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

    class ArrayMemberBuilder<T, TElement> : ArrayMemberBuilder<ArrayMemberBuilder<T, TElement>, T, TElement>, IArrayMemberBuilder<T, TElement>
    {
        public ArrayMemberBuilder(MemberExpression expression) : base(expression)
        {
        }

        public new IArrayMemberBuilder<T, TElement> SizeOf(BinarySize size)
        {
            return base.SizeOf(size);
        }

        public new IArrayMemberBuilder<T, TElement> SizeOf(Expression<Func<IContext<T>, BinarySize>> expression)
        {
            return base.SizeOf(expression);
        }

        public new IArrayMemberBuilder<T, TElement> If(Expression<Func<IContext<T>, bool>> expression)
        {
            return base.If(expression);
        }

        public new IArrayMemberBuilder<T, TElement> SetOrder(int order)
        {
            return base.SetOrder(order);
        }

        public new IArrayMemberBuilder<T, TElement> Position(Expression<Func<IContext<T>, BinaryOffset>> expression)
        {
            return base.Position(expression);
        }

        public new IArrayMemberBuilder<T, TElement> OverrideEndianess(Endianness endianness)
        {
            return base.OverrideEndianess(endianness);
        }

        public new IArrayMemberBuilder<T, TElement> UseFactory(Expression<Func<IContext<T>, TElement[]>> expression)
        {
            return base.UseFactory(expression);
        }

        public new IArrayMemberBuilder<T, TElement> ConvertValue(Expression<Func<IMemberContext<T, TElement[]>, TElement[]>> expression)
        {
            return base.ConvertValue(expression);
        }

        public new IArrayMemberBuilder<T, TElement> Assert(Expression<Func<IMemberContext<T, TElement[]>, bool>> expression)
        {
            return base.Assert(expression);
        }

        public new IArrayMemberBuilder<T, TElement> AfterRead(Expression<Action<IMemberContext<T, TElement[]>>> expression)
        {
            return base.AfterRead(expression);
        }

        public new IArrayMemberBuilder<T, TElement> PositionAfter(Expression<Func<IMemberContext<T, TElement[]>, BinaryOffset>> expression)
        {
            return base.PositionAfter(expression);
        }

        public new IArrayMemberBuilder<T, TElement> Element(Action<IMemberBuilder<TElement[], TElement>> elementBuilderConfiguration)
        {
            return base.Element(elementBuilderConfiguration);
        }

        public new IArrayMemberBuilder<T, TElement> Length(ulong length)
        {
            return base.Length(length);
        }

        public new IArrayMemberBuilder<T, TElement> Length(Expression<Func<IContext<T>, ulong>> expression)
        {
            return base.Length(expression);
        }
    }
}
