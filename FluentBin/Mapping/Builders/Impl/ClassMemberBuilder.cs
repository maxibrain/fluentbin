using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentBin.Mapping.Contexts;
using FluentBin.Mapping.Contexts.Impl;

namespace FluentBin.Mapping.Builders.Impl
{
    class ClassMemberBuilder<T, TMember> : MemberBuilder<T, TMember>, IClassMemberBuilder<T, TMember>
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
}
