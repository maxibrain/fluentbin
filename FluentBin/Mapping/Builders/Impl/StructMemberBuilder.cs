using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders.Impl
{
    class StructMemberBuilder<TBuilder, T, TMember> : GenericMemberBuilder<TBuilder, T, TMember>, IGenericStructMemberBuilder<TBuilder, T, TMember>
        where TBuilder : StructMemberBuilder<TBuilder, T, TMember>
    {
        public StructMemberBuilder(MemberExpression expression)
            : base(expression)
        {
        }

        public StructMemberBuilder(MemberInfo memberInfo)
            : base(memberInfo)
        {
        }

        public StructMemberBuilder(string memberName)
            : base(memberName)
        {
        }

        protected override Expression BuildBodyExpression(ExpressionBuilderArgs args, ParameterExpression innerResultVar, ParameterExpression typeVar)
        {
            return Expression.Assign(innerResultVar, BuildStructExpression(args));
        }

        private Expression BuildStructExpression(ExpressionBuilderArgs args)
        {
            Expression readExpression;
            if (Size != null)
            {
                readExpression = Expression.Call(
                    typeof(BitsReaderExtensions)
                        .GetMethod("ReadStruct", new[]
                            {
                                typeof (BitsReader),
                                typeof (BinarySize),
                                typeof (Endianness)
                            })
                        .MakeGenericMethod(MemberType),
                    args.BrParameter, Invoke(Size, args), args.Endianness);
            }
            else
            {
                readExpression = Expression.Call(
                    typeof(BitsReaderExtensions)
                        .GetMethod("ReadStruct", new[]
                            {
                                typeof (BitsReader),
                                typeof (Endianness)
                            })
                        .MakeGenericMethod(MemberType),
                    args.BrParameter, args.Endianness);
            }
            return readExpression;
        }
    }

    class StructMemberBuilder<T, TMember> : StructMemberBuilder<StructMemberBuilder<T, TMember>, T, TMember>, IMemberBuilder<T, TMember>
    {
        public StructMemberBuilder(MemberExpression expression) : base(expression)
        {
        }

        public StructMemberBuilder(MemberInfo memberInfo) : base(memberInfo)
        {
        }

        public StructMemberBuilder(string memberName) : base(memberName)
        {
        }

        public new IMemberBuilder<T, TMember> SizeOf(BinarySize size)
        {
            return base.SizeOf(size);
        }

        public new IMemberBuilder<T, TMember> SizeOf(Expression<Func<IContext<T>, BinarySize>> expression)
        {
            return base.SizeOf(expression);
        }

        public new IMemberBuilder<T, TMember> If(Expression<Func<IContext<T>, bool>> expression)
        {
            return base.If(expression);
        }

        public new IMemberBuilder<T, TMember> SetOrder(int order)
        {
            return base.SetOrder(order);
        }

        public new IMemberBuilder<T, TMember> Position(Expression<Func<IContext<T>, BinaryOffset>> expression)
        {
            return base.Position(expression);
        }

        public new IMemberBuilder<T, TMember> OverrideEndianess(Endianness endianness)
        {
            return base.OverrideEndianess(endianness);
        }

        public new IMemberBuilder<T, TMember> UseFactory(Expression<Func<IContext<T>, TMember>> expression)
        {
            return base.UseFactory(expression);
        }

        public new IMemberBuilder<T, TMember> ConvertValue(Expression<Func<IMemberContext<T, TMember>, TMember>> expression)
        {
            return base.ConvertValue(expression);
        }

        public new IMemberBuilder<T, TMember> Assert(Expression<Func<IMemberContext<T, TMember>, bool>> expression)
        {
            return base.Assert(expression);
        }

        public new IMemberBuilder<T, TMember> AfterRead(Expression<Action<IMemberContext<T, TMember>>> expression)
        {
            return base.AfterRead(expression);
        }

        public new IMemberBuilder<T, TMember> PositionAfter(Expression<Func<IMemberContext<T, TMember>, BinaryOffset>> expression)
        {
            return base.PositionAfter(expression);
        }
    }
}
