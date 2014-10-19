using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders.Impl
{
    class SkippedMemberBuilder<T, TMember> : MemberBuilder<T>, ISkippedMemberBuilder<T, TMember>
    {
        private Expression<Func<IContext<T>, BinarySize>> _size;
        private Expression<Func<IContext<T>, Boolean>> _if;

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
            return Expression.Default(typeof(TMember));
        }

        protected override Expression BuildBodyExpression(ExpressionBuilderArgs args, ParameterExpression innerResultVar, ParameterExpression typeVar)
        {
            if (_size == null)
                throw new InvalidMappingException("Size of skipped member must be set.");
            return Expression.AddAssign(AdvancedExpression.Position(args.BrParameter), Expression.Convert(Invoke(_size, args), typeof(BinaryOffset)));
        }

        public ISkippedMemberBuilder<T, TMember> SizeOf(BinarySize sizeInBytes)
        {
            return SizeOf(context => sizeInBytes);
        }

        public ISkippedMemberBuilder<T, TMember> SizeOf(Expression<Func<IContext<T>, BinarySize>> expression)
        {
            _size = expression;
            return this;
        }

        public ISkippedMemberBuilder<T, TMember> If(Expression<Func<IContext<T>, bool>> expression)
        {
            _if = expression;
            return this;
        }

        public ISkippedMemberBuilder<T, TMember> SetOrder(int order)
        {
            Order = order;
            return this;
        }
    }
}
