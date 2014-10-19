using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace FluentBin
{
    public static class ReflectionExtensions
    {
        public static T GetCustomAttribute<T>(this MemberInfo memberInfo)
            where T : Attribute
        {
            return memberInfo.GetCustomAttribute<T>(true);
        }

        public static T GetCustomAttribute<T>(this MemberInfo memberInfo, bool inherit)
            where T : Attribute
        {
            return memberInfo.GetCustomAttributes(typeof(T), inherit).Cast<T>().FirstOrDefault();
        }

        public static void InvokeMethod(this object declaringObject, string methodName, params object[] args)
        {
            declaringObject.GetType().GetMethod(methodName).Invoke(declaringObject, args);
        }

        public static T InvokeMethod<T>(this object declaringObject, string methodName, params object[] args)
        {
            return (T)declaringObject.GetType().GetMethod(methodName).Invoke(declaringObject, args);
        }

        public static void SetValue(this MemberInfo m, object obj, object value)
        {
            if (m is PropertyInfo)
            {
                (m as PropertyInfo).SetValue(obj, value, null);
            }
            else if (m is FieldInfo)
            {
                (m as FieldInfo).SetValue(obj, value);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public static T GetValue<T>(this MemberInfo m, object obj)
        {
            if (m is PropertyInfo)
            {
                return (T)(m as PropertyInfo).GetValue(obj, null);
            }
            if (m is FieldInfo)
            {
                return (T)(m as FieldInfo).GetValue(obj);
            }
            throw new ArgumentOutOfRangeException();
        }

        public static Type GetMemberType(this MemberInfo m)
        {
            if (m is PropertyInfo)
                return (m as PropertyInfo).PropertyType;
            if (m is FieldInfo)
                return (m as FieldInfo).FieldType;
            throw new ArgumentOutOfRangeException();
        }

        public static bool IsClass(this Type type)
        {
            return type.IsClass || type.IsInterface;
        }

        public static bool IsClass(this MemberInfo m)
        {
            return m.GetMemberType().IsClass();
        }

        public static bool IsStruct(this Type type)
        {
            return type.IsValueType;
        }

        public static bool IsStruct(this MemberInfo m)
        {
            return m.GetMemberType().IsStruct();
        }

        public static bool IsArray(this MemberInfo m)
        {
            return m.GetMemberType().IsArray;
        }

        public static bool IsList(this MemberInfo m)
        {
            return m.GetMemberType().IsList();
        }

        public static bool IsList(this Type type)
        {
            return type.IsSubclassOf(typeof (IList));
        }
    }
}
