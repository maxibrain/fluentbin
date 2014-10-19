using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BinaryIO.Mapping.Builders.Impl.ValueHandlers
{
    class StructValueHandler : IValueHandler
    {
/*
        private Expression BuildStructExpression(ParameterExpression brParameter, ParameterExpression instanceVar, ParameterExpression innerResultVar)
        {
            var type = MemberInfo.GetMemberType();

            Expression readExpression;
            if (Size != null)
            {
                readExpression = Expression.Call(
                    typeof(BinaryReaderExtensions)
                        .GetMethod("ReadStruct", new[]
                            {
                                typeof (BinaryReader),
                                typeof (Int32)
                            })
                        .MakeGenericMethod(type),
                    brParameter, Invoke(Size, brParameter, instanceVar));
            }
            else
            {
                readExpression = Expression.Call(
                    typeof(BinaryReaderExtensions)
                        .GetMethod("ReadStruct", new[]
                            {
                                typeof (BinaryReader)
                            })
                        .MakeGenericMethod(type),
                    brParameter);
            }
            return readExpression;
        }
*/


        public Expression CanHandle(Expression typeVar)
        {
            return Expression.Property(typeVar, "IsValueType");
        }

        public Expression BuildReadExpression(Expression brParameter, Expression typeVar)
        {
            Expression readExpression;
            if (Size != null)
            {
                readExpression = Expression.Call(
                    typeof(BinaryReaderExtensions)
                        .GetMethod("ReadStruct", new[]
                            {
                                typeof (BinaryReader),
                                typeof (Type),
                                typeof (Int32)
                            }),
                    brParameter, typeVar, Invoke(Size, brParameter, instanceVar));
            }
            else
            {
                readExpression = Expression.Call(
                    typeof(BinaryReaderExtensions)
                        .GetMethod("ReadStruct", new[]
                            {
                                typeof (BinaryReader),
                                typeof (Type)
                            }),
                    brParameter, typeVar);
            }
            return readExpression;

        }
    }
}
