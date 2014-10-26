using System;
using System.Linq.Expressions;
using System.Reflection;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders.Impl
{
    class ClassMemberBuilder<TBuilder, T, TMember> : GenericMemberBuilder<TBuilder, T, TMember>, IGenericClassMemberBuilder<TBuilder, T, TMember>
        where TBuilder : ClassMemberBuilder<TBuilder, T, TMember>
    {
        public ClassMemberBuilder(MemberExpression expression) 
            : base(expression)
        {
        }

        public ClassMemberBuilder(MemberInfo memberInfo)
            : base(memberInfo)
        {
        }

        public ClassMemberBuilder(string memberName)
            : base(memberName)
        {
        }

        protected override Expression BuildBodyExpression(ExpressionBuilderArgs args, ParameterExpression innerResultVar, ParameterExpression typeVar)
        {
            return Expression.Block(AdvancedExpression.Debug("Type: {0}", typeVar),
                                    Expression.Invoke(AdvancedExpression.GetTypeBuilder(args.TypeReaders, typeVar),
                                                      args.BrParameter, innerResultVar, args.TypeReaders));
        }
    }

    class ClassMemberBuilder<T, TMember> : ClassMemberBuilder<ClassMemberBuilder<T, TMember>, T, TMember>, IMemberBuilder<T, TMember>
    {
        public ClassMemberBuilder(MemberExpression expression) : base(expression)
        {
        }

        public ClassMemberBuilder(MemberInfo memberInfo) : base(memberInfo)
        {
        }

        public ClassMemberBuilder(string memberName) : base(memberName)
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
