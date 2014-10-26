using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FluentBin.Mapping.Builders.Impl
{
    internal static class MemberBuilderFactory
    {
        public static IMemberBuilderBase Create<T>(MemberInfo memberInfo) 
        {
            return Create<T>(memberInfo.GetMemberType(), memberInfo.Name);
        }

        public static IMemberBuilderBase Create<T>(Type memberType, string memberName) 
        {
            Type builderType;
            Type t2 = memberType;
            int argsCount = 2;

            if (memberType == typeof(String))
            {
                builderType = typeof(StringMemberBuilder<>);
                argsCount = 1;
            }
            else if (memberType.IsStruct())
            {
                builderType = typeof(StructMemberBuilder<,>);
            }
            else if (memberType.IsClass())
            {
                builderType = typeof(ClassMemberBuilder<,>);
            }
            else if (memberType.IsList())
            {
                builderType = typeof(ListMemberBuilder<,>);
                t2 = memberType.GetGenericArguments()[0];
            }
            else
            {
                throw new ArgumentException("Not supported type.", "memberInfo");
            }

            var typeArgs = new[] { typeof(T), t2 }.Take(argsCount).ToArray();

            return (IMemberBuilderBase)Activator.CreateInstance(builderType.MakeGenericType(typeArgs), memberName);
        }

        public static IMemberBuilderBase Create<T, TMember>(string memberName) 
        {
            return Create<T>(typeof(TMember), memberName);
        }
    }
}
