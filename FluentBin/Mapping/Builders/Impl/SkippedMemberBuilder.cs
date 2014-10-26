using System;
using System.Linq.Expressions;
using System.Reflection;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders.Impl
{
    class SkippedMemberBuilder<TBuilder, T> : GenericMemberBuilder<TBuilder, T>, IGenericSkippedMemberBuilder<TBuilder, T>
        where TBuilder : SkippedMemberBuilder<TBuilder, T>
    {
        public SkippedMemberBuilder(Type memberType, string memberName) : base(memberType, memberName)
        {
        }

        public SkippedMemberBuilder(MemberInfo memberInfo) : base(memberInfo)
        {
        }

        public SkippedMemberBuilder(MemberExpression memberExpression) : base(memberExpression)
        {
        }

        protected override Expression BuildCtorExpression(ExpressionBuilderArgs args)
        {
            return Expression.Default(MemberType);
        }

        protected override Expression BuildBodyExpression(ExpressionBuilderArgs args, ParameterExpression innerResultVar, ParameterExpression typeVar)
        {
            if (Size == null)
                throw new InvalidMappingException("Size of skipped member must be set.");
            return Expression.AddAssign(AdvancedExpression.Position(args.BrParameter), Expression.Convert(Invoke(Size, args), typeof(BinaryOffset)));
        }
    }

    class SkippedMemberBuilder<T> : SkippedMemberBuilder<SkippedMemberBuilder<T>, T>, ISkippedMemberBuilder<T>
    {
        public SkippedMemberBuilder(Type memberType, string memberName) : base(memberType, memberName)
        {
        }

        public SkippedMemberBuilder(MemberInfo memberInfo) : base(memberInfo)
        {
        }

        public SkippedMemberBuilder(MemberExpression memberExpression) : base(memberExpression)
        {
        }

        public new ISkippedMemberBuilder<T> SizeOf(BinarySize size)
        {
            return base.SizeOf(size);
        }

        public new ISkippedMemberBuilder<T> SizeOf(Expression<Func<IContext<T>, BinarySize>> expression)
        {
            return base.SizeOf(expression);
        }

        public new ISkippedMemberBuilder<T> If(Expression<Func<IContext<T>, bool>> expression)
        {
            return base.If(expression);
        }

        public new ISkippedMemberBuilder<T> SetOrder(int order)
        {
            return base.SetOrder(order);
        }

        public new ISkippedMemberBuilder<T> Position(Expression<Func<IContext<T>, BinaryOffset>> expression)
        {
            return base.Position(expression);
        }

        public new ISkippedMemberBuilder<T> OverrideEndianess(Endianness endianness)
        {
            return base.OverrideEndianess(endianness);
        }
    }
}
