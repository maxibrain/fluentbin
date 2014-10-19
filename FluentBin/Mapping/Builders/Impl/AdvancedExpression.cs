using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FluentBin.Mapping.Contexts.Impl;

namespace FluentBin.Mapping.Builders.Impl
{
    static class AdvancedExpression
    {
        public static Expression Debug(string format, params Expression[] args)
        {
            return Expression.Call(
                typeof (Debug).GetMethod("WriteLine", new[] {typeof (String), typeof (object[])}),
                Expression.Constant(format),
                Expression.NewArrayInit(typeof(Object), args.Select(arg => Expression.TypeAs(arg, typeof(Object)))));
        }

        public static Expression Position(ParameterExpression brParameter)
        {
            return Expression.Property(brParameter, "Position");
        }

        public static Expression Length(ParameterExpression brParameter)
        {
            return Expression.Property(brParameter, "Length");
        }

        public static Expression Context<T>(ParameterExpression brParameter, ParameterExpression instanceVar)
        {
            return Expression.New(
                typeof (Context<T>).GetConstructor(new[] {typeof (T), typeof (BinaryOffset), typeof(BinarySize)}),
                instanceVar,
                Position(brParameter),
                Length(brParameter));
        }

        public static Expression MemberContext<T, TMember>(ParameterExpression brParameter, ParameterExpression instanceVar, ParameterExpression innerResultVar)
        {
            return Expression.New(
                typeof(MemberContext<T, TMember>)
                    .GetConstructor(new[]
                        {
                            typeof (T),
                            typeof (TMember),
                            typeof (BinaryOffset),
                            typeof (BinarySize)
                        }),
                instanceVar,
                innerResultVar,
                Position(brParameter),
                Length(brParameter));
        }

        public static Expression CollectionMemberContext<T, TMember>(ParameterExpression brParameter, ParameterExpression instanceVar, ParameterExpression innerResultVar, ParameterExpression iVar)
        {
            return Expression.New(
                typeof (CollectionMemberContext<T, TMember>)
                    .GetConstructor(new[]
                        {
                            typeof (T),
                            typeof (TMember),
                            typeof (Int32),
                            typeof (BinaryOffset),
                            typeof (BinarySize)
                        }),
                instanceVar,
                innerResultVar,
                iVar,
                Position(brParameter),
                Length(brParameter));
        }

        public static Expression GetType(Expression instanceVar)
        {
            return Expression.Call(instanceVar, "GetType", null);
        }

        public static Expression GetTypeBuilder(Expression typeReaders, Expression type)
        {
            var builderVar = Expression.Variable(typeof (Expression<ReadFunc>), "builder");
            return
                Expression.Block(new[] {builderVar},
                                 Expression.IfThen(
                                     Expression.IsFalse(
                                         Expression.Call(typeReaders, "TryGetValue", null, type, builderVar)),
                                     Expression.Throw(
                                         Expression.New(
                                             typeof (ArgumentException).GetConstructor(new[] {typeof (String)}),
                                             Expression.Constant(string.Format("Not included type: {0}", type))))),
                                 builderVar);
        }
    }
}
