using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace FluentBin
{
    public partial class BinaryReaderEx : BinaryReader
    {
        #region ctor

        public BinaryReaderEx(Stream input) : base(input)
        {
        }

        public BinaryReaderEx(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        #endregion

        public event EventHandler<BinaryReadingErrorArgs> Error;

        protected virtual void OnError(BinaryReadingErrorArgs e)
        {
            EventHandler<BinaryReadingErrorArgs> handler = Error;
            if (handler != null) handler(this, e);
        }

        #region CoverageMap Support
        private readonly BinaryFileCoverageMap _coverageMap = new BinaryFileCoverageMap();
        public BinaryFileCoverageMap CoverageMap { get { return _coverageMap; } }

        public override byte[] ReadBytes(int count)
        {
            if (GenerateCoverageMap)
            {
                CoverageMap.AddReadRequest(Position, count);
            }
            return base.ReadBytes(count);
        }

        public bool GenerateCoverageMap { get; set; }

        #endregion

        public T ReadObject<T>()
        {
            return (T)ReadObject(typeof (T));
        }

        public object ReadObject(Type type)
        {
            return ReadObject(() => Activator.CreateInstance(type));
        }

        private object ReadObject(Func<object> factoryMethod)
        {
            var result = factoryMethod();
            try
            {
                ReadObjectProperties(result);
            }
            catch (Exception ex)
            {
                var exceptionWrapper = new BinaryReadingException(result, innerException:ex);
                var args = new BinaryReadingErrorArgs(exceptionWrapper);
                OnError(args);
                if (!args.Handled)
                {
                    throw exceptionWrapper;
                }
            }
            return result;
        }

        private static IEnumerable<MemberInfo> GetProperties(object obj)
        {
            SortedList<int, MemberInfo> properties = new SortedList<int, MemberInfo>();
            var order = 0;
            foreach (var memberInfo in obj.GetType().GetMembers().Where(ShouldReadMember))
            {
                var memberOrder = order;
                var memberAttr = memberInfo.GetCustomAttribute<BinaryMemberAttribute>();
                if (memberAttr != null)
                {
                    if (memberAttr.Order != 0)
                    {
                        memberOrder = memberAttr.Order;
                    }
                }
                properties.Add(memberOrder, memberInfo);
                order++;
            }
            return properties.Values;
        }

        private void ReadObjectProperties(object obj)
        {
            var properties = GetProperties(obj);
            foreach (var memberInfo in properties)
            {
                object value = ReadPropertyValue(memberInfo, obj);
                memberInfo.SetValue(obj, value);
            }
        }

        private object ReadClassPropertyValue(MemberInfo memberInfo, object declaringObject, Func<object> factoryMethod = null)
        {
            return factoryMethod != null ? ReadObject(factoryMethod) : ReadObject(memberInfo.GetMemberType());
        }

        private object ReadStructPropertyValue(MemberInfo memberInfo, object declaringObject, Func<object> factoryMethod = null)
        {
            var type = memberInfo.GetMemberType();
            
            object value;

            int size = 0;

            var memberAttr = memberInfo.GetCustomAttribute<BinaryMemberAttribute>();

            if (memberAttr != null)
            {
                if (!string.IsNullOrEmpty(memberAttr.SizeMember))
                {
                    var declaringType = declaringObject.GetType();
                    MemberInfo structSizeMember = declaringType.GetProperty(memberAttr.SizeMember);
                    if (structSizeMember == null)
                    {
                        structSizeMember = declaringType.GetField(memberAttr.SizeMember);
                    }
                    if (structSizeMember == null)
                    {
                        throw new NullReferenceException("Cannot find a member");
                    }
                    size = structSizeMember.GetValue<int>(declaringObject);
                }
                else
                {
                    size = memberAttr.Size;
                }
            }

            if (size > 0)
            {
                value = this.ReadStruct(type, size);
            }
            else
            {
                value = this.ReadStruct(type);
            }
            return value;
        }

        private object ReadPropertyValue(Type type, Func<object> factoryMethod)
        {
            object value;
            if (type.IsValueType)
            {
                value = this.ReadStruct(type);
            }
            else
            {
                if (factoryMethod != null)
                {
                    value = ReadObject(factoryMethod);
                }
                else
                {
                    value = ReadObject(type);
                }
            }
            return value;
        }

        private object ReadPropertyValue(MemberInfo memberInfo, object declaringObject)
        {
            var type = memberInfo.GetMemberType();
            var pos = Position;

            Func<object> factoryMethod = null;
            string converterMethod = null;
            Func<long, long, long> getPositionAfter = null;
            Action<object> afterRead = null;
            object assertValue = null;
            BinaryMemberAttribute memberAttr = memberInfo.GetCustomAttribute<BinaryMemberAttribute>();
            if (memberAttr != null)
            {
                if (!string.IsNullOrEmpty(memberAttr.ConditionMethod))
                {
                    var shouldRead = declaringObject.InvokeMethod<bool>(memberAttr.ConditionMethod);
                    if (!shouldRead)
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(memberAttr.GetPositionMethod))
                {
                    Position = declaringObject.InvokeMethod<long>(memberAttr.GetPositionMethod, Position);
                }
                
                if (!string.IsNullOrEmpty(memberAttr.FactoryMethod))
                {
                    factoryMethod = () => declaringObject.InvokeMethod<object>(memberAttr.FactoryMethod);
                }
                else if (!string.IsNullOrEmpty(memberAttr.TypeSelectorMethod))
                {
                    type = declaringObject.InvokeMethod<Type>(memberAttr.TypeSelectorMethod);
                    factoryMethod = () => Activator.CreateInstance(type);
                }

                converterMethod = memberAttr.ConverterMethod;
                if (!string.IsNullOrEmpty(memberAttr.GetPositionAfterMethod))
                {
                    getPositionAfter = (current, before) => declaringObject.InvokeMethod<long>(memberAttr.GetPositionAfterMethod, current, before);
                }

                if (!string.IsNullOrEmpty(memberAttr.AfterReadMethod))
                {
                    afterRead = o => declaringObject.InvokeMethod(memberAttr.AfterReadMethod, o);
                }
            }

            object value;
            if (type.IsValueType)
            {
                value = ReadStructPropertyValue(memberInfo, declaringObject, factoryMethod);
            }
            else if (type.IsArray && memberInfo != null && memberInfo.GetCustomAttribute<BinaryArrayAttribute>() != null)
            {
                value = ReadArrayPropertyValue(memberInfo, declaringObject, factoryMethod);
            }
            else if (memberInfo != null && memberInfo.GetCustomAttribute<BinaryListAttribute>() != null)
            {
                value = ReadListPropertyValue(memberInfo, declaringObject, factoryMethod);
            }
            else
            {
                value = ReadClassPropertyValue(memberInfo, declaringObject, factoryMethod);
            }

            if (!string.IsNullOrEmpty(converterMethod))
            {
                value = declaringObject.InvokeMethod<object>(converterMethod, value);
            }

            if (getPositionAfter != null)
            {
                Position = getPositionAfter(Position, pos);
            }

            if (afterRead != null)
            {
                afterRead(value);
            }

            return value;
        }

        private Array ReadArrayPropertyValue(MemberInfo memberInfo, object declaringObject, Func<object> factoryMethod = null)
        {
            Debug.Assert(memberInfo != null, "MemberInfo should not be null");
            if (memberInfo == null)
                throw new NotSupportedException();

            var type = memberInfo.GetMemberType();
            var elementType = type.GetElementType();

            var arrayAttr = memberInfo.GetCustomAttribute<BinaryArrayAttribute>();

            Array value;
            if (factoryMethod != null)
            {
                value = (Array)factoryMethod();
            }
            else
            {
                ulong length;
                if (!string.IsNullOrEmpty(arrayAttr.LengthMember))
                {
                    var declaringType = declaringObject.GetType();
                    MemberInfo arrayLengthMember = declaringType.GetProperty(arrayAttr.LengthMember);
                    if (arrayLengthMember == null)
                    {
                        arrayLengthMember = declaringType.GetField(arrayAttr.LengthMember);
                    }
                    if (arrayLengthMember == null)
                    {
                        throw new NullReferenceException("Cannot find a member");
                    }
                    length = arrayLengthMember.GetValue<ulong>(declaringObject);
                }
                else
                {
                    length = arrayAttr.Length;
                }
                value = Array.CreateInstance(elementType, (long)length);
            }

            if (value.Length > 0)
            {
                ReadCollection(value, memberInfo, declaringObject, elementType, 
                               (i, element) => value.SetValue(element, i),
                               (i, element, position) => (i >= value.Length));
                
            }
            return value;
        }

        private IList ReadListPropertyValue(MemberInfo memberInfo, object declaringObject, Func<object> factoryMethod = null)
        {
            var listAttr = memberInfo.GetCustomAttribute<BinaryListAttribute>();
            var type = memberInfo.GetMemberType();
            var result = (IList) (factoryMethod != null ? factoryMethod() : Activator.CreateInstance(type));
            
            ReadCollection(result, memberInfo, declaringObject,
                           type.GetGenericArguments()[0],
                           (i, element) => result.Insert(i, element),
                           (i, element, position) => declaringObject.InvokeMethod<bool>(listAttr.HasReadLastElementMethod, result, element, position));
            return result;
        }

        private void ReadCollection(object result, MemberInfo memberInfo, object declaringObject, Type elementType, Action<int, object> setValue, Func<int, object, long, bool> hasReadLastElement)
        {
            var collectionAttribute = memberInfo.GetCustomAttribute<BinaryCollectionAttribute>();
            Func<int, long, long> getElementPositionMethod = null;
            if (!string.IsNullOrEmpty(collectionAttribute.GetElementPositionMethod))
            {
                getElementPositionMethod =
                    (index, currentPosition) => declaringObject.InvokeMethod<long>(collectionAttribute.GetElementPositionMethod, result, index, currentPosition);
            }

            Func<int, object> elementFactoryMethod = null;
            if (!string.IsNullOrEmpty(collectionAttribute.ElementFactoryMethod))
            {
                elementFactoryMethod = i1 => declaringObject.InvokeMethod<object>(collectionAttribute.ElementFactoryMethod, result, i1);
            }

            Action<BinaryReadingErrorArgs> onElementErrorMethod = null;
            if (!string.IsNullOrEmpty(collectionAttribute.OnElementErrorMethod))
            {
                onElementErrorMethod = args => declaringObject.InvokeMethod(collectionAttribute.OnElementErrorMethod, args);
            }

            object lastElement = null;
            var i = 0;
            do
            {
                if (getElementPositionMethod != null)
                {
                    Position = getElementPositionMethod(i, Position);
                }
                object element = null;
                try
                {
                    Func<object> currentElementFactoryMethod = null;
                    if (elementFactoryMethod != null)
                    {
                        currentElementFactoryMethod = () => elementFactoryMethod(i);
                    }
                    element = ReadPropertyValue(elementType, currentElementFactoryMethod);
                    setValue(i, element);
                }
                catch (Exception ex)
                {
                    var args = new BinaryReadingErrorArgs(new BinaryReadingException(result, innerException:ex));
                    if (onElementErrorMethod != null)
                    {
                        onElementErrorMethod(args);
                    }
                    if (!args.Handled)
                    {
                        throw;
                    }
                }
                finally
                {
                    lastElement = element;
                    i++;
                }
            } while (!hasReadLastElement(i, lastElement, Position));

        }

        private static bool ShouldReadMember(MemberInfo m)
        {
            if (m.GetCustomAttribute<BinaryIgnoreAttribute>(true) != null)
                return false;
            return (m is PropertyInfo && (m as PropertyInfo).CanWrite)
                   || (m is FieldInfo && !(m as FieldInfo).IsInitOnly && !(m as FieldInfo).IsLiteral);
        }

        private long Position
        {
            get { return this.BaseStream.Position; }
            set { this.BaseStream.Position = value; }
        }
    }
}
