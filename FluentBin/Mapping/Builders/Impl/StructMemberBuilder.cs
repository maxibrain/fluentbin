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
    class StructMemberBuilder<T, TMember> : MemberBuilder<T, TMember>, IStructMemberBuilder<T, TMember>
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
}
