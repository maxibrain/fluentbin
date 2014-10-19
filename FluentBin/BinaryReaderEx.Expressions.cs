using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BinaryIO
{
    partial class BinaryReaderEx
    {
        public Expression BuildReadObjectExpression<T>()
        {
            return BuildReadObjectExpression(Expression.Constant(typeof(T)));
        }

        public Expression BuildReadObjectExpression(Expression typeExpression)
        {
            var factoryExpression = Expression.Call(typeof (Activator), "CreateInstance",
                                                    new[] {typeof (Type)}, typeExpression);
            return BuildReadObjectExpression(factoryExpression);
        }

        public Expression BuildReadObjectExpression(MethodCallExpression factoryExpression)
        {
            var exceptionExpression = Expression.Parameter(typeof (Exception), "ex");
            return Expression.TryCatch(BuildReadObjectPropertiesExpression(factoryExpression),
                                Expression.Catch(exceptionExpression, Expression.Empty()));
/*
            var result = factoryMethod();
            try
            {
                ReadObjectProperties(result);
            }
            catch (Exception ex)
            {
                var exceptionWrapper = new BinaryReadingException(result, ex);
                var args = new BinaryReadingErrorArgs(exceptionWrapper);
                OnError(args);
                if (!args.Handled)
                {
                    throw exceptionWrapper;
                }
            }
            return result;
*/
        }

        private Expression BuildReadObjectPropertiesExpression(Expression instanceExpression)
        {
            throw new NotImplementedException();
        }
    }
}
