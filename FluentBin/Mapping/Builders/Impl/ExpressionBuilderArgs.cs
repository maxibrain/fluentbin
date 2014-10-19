using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FluentBin.Mapping.Builders.Impl
{
    class ExpressionBuilderArgs
    {
        public ParameterExpression BrParameter;
        public ParameterExpression InstanceVar;
        public ParameterExpression TypeReaders;
        public Expression Endianness;

        public ExpressionBuilderArgs Clone()
        {
            return (ExpressionBuilderArgs)MemberwiseClone();
        }
    }
}
