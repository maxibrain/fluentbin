using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FluentBin.Mapping.Builders.Impl
{
    class StringMemberBuilder<T> : MemberBuilder<T, String>
    {
        public StringMemberBuilder(MemberExpression expression) : base(expression)
        {
        }

        public StringMemberBuilder(MemberInfo memberInfo) : base(memberInfo)
        {
        }

        public StringMemberBuilder(string memberName) : base(memberName)
        {
        }

        protected override Expression BuildCtorExpression(ExpressionBuilderArgs args)
        {
            return Expression.Constant(String.Empty);
        }

        protected override Expression BuildBodyExpression(ExpressionBuilderArgs args, ParameterExpression innerResultVar, ParameterExpression typeVar)
        {
            if (_encoding == null)
                throw new InvalidMappingException(string.Format("No encoding specified for {0}", innerResultVar.Name));
            return Expression.Assign(innerResultVar,
                                     Expression.Call(
                                         typeof (BitsReaderExtensions)
                                             .GetMethod("ReadString", new[]
                                                 {
                                                     typeof (BitsReader),
                                                     typeof (BinarySize),
                                                     typeof (Encoding)
                                                 }),
                                         args.BrParameter, 
                                         Invoke(Size, args), 
                                         Invoke(_encoding, args)));
        }
    }
}
