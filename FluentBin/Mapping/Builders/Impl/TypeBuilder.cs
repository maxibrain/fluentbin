using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FluentBin.Mapping.Builders.Impl
{
    class TypeBuilder<T> : ITypeBuilder<T>, IExpressionBuilder 
    {
        private readonly Dictionary<MemberInfo, IMemberBuilderBase> _memberBuilders = new Dictionary<MemberInfo, IMemberBuilderBase>();
        private Endianness? _endianness;

        private TBuilder GetOrCreateBuilder<TBuilder>(MemberInfo memberInfo, Func<TBuilder> factoryMethod)
            where TBuilder : class, IMemberBuilderBase
        {
            Debug.Assert(memberInfo != null);
            Debug.Assert(factoryMethod != null);
            IMemberBuilderBase resultObj;
            if (!_memberBuilders.TryGetValue(memberInfo, out resultObj) || !(resultObj is TBuilder))
            {
                resultObj = factoryMethod();
            }
            _memberBuilders[memberInfo] = resultObj;
            return resultObj as TBuilder;
        }

        #region Implementation of ITypeBuilder<T>

        public ITypeBuilder<T> OverrideEndianness(Endianness? endianness)
        {
            _endianness = endianness;
            return this;
        }

        public ITypeBuilder<T> Read<TMember>(Expression<Func<T, TMember>> expression, Action<IMemberBuilder<T, TMember>> memberConfiguration = null)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Expression should point a member", "expression");
            Func<IMemberBuilderBase> factoryMethod;
            if (typeof(TMember) == typeof(String))
            {
                factoryMethod = () => new StringMemberBuilder<T>(memberExpression);
            }
            else if (memberExpression.Member.IsClass())
            {
                factoryMethod = () => new ClassMemberBuilder<T, TMember>(memberExpression);
            }
            else if (memberExpression.Member.IsStruct())
            {
                factoryMethod = () => new StructMemberBuilder<T, TMember>(memberExpression);
            }
            else
            {
                throw new ArgumentException("Expression should point a single value, not a collection", "expression");
            }
            var memberBuilder = GetOrCreateBuilder(memberExpression.Member, factoryMethod);
            if (memberConfiguration != null)
                memberConfiguration((IMemberBuilder<T, TMember>)memberBuilder);
            return this;
        }

/*
        public ITypeBuilder<T> Read(Expression<Func<T, String>> expression, Action<IStringMemberBuilder<T>> memberConfiguration = null)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Expression should point a member", "expression");
            var memberBuilder = GetOrCreateBuilder(memberExpression.Member, () => new StringMemberBuilder<T>(memberExpression));
            if (memberConfiguration != null)
                memberConfiguration(memberBuilder);
            return this;
        }

*/
        public ITypeBuilder<T> Read<TElement>(Expression<Func<T, TElement[]>> expression, Action<IArrayMemberBuilder<T, TElement>> memberConfiguration)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Expression should point a member", "expression");
            var memberBuilder = GetOrCreateBuilder(memberExpression.Member, () => new ArrayMemberBuilder<T, TElement>(memberExpression));
            if (memberConfiguration != null)
                memberConfiguration(memberBuilder);
            return this;
        }

        public ITypeBuilder<T> Read<TElement>(Expression<Func<T, IList<TElement>>> expression, Action<IListMemberBuilder<T, TElement>> memberConfiguration = null)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Expression should point a member", "expression");
            var memberBuilder = GetOrCreateBuilder(memberExpression.Member, () => new ListMemberBuilder<T, TElement>(memberExpression));
            if (memberConfiguration != null)
                memberConfiguration(memberBuilder);
            return this;
        }

        public ITypeBuilder<T> Ignore<TMember>(Expression<Func<T, TMember>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Expression should point a member", "expression");
            _memberBuilders[memberExpression.Member] = null;
            return this;
        }

        public ITypeBuilder<T> Skip<TMember>(Expression<Func<T, TMember>> expression, Action<ISkippedMemberBuilder<T>> memberConfiguration)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Expression should point a member", "expression");
            var memberBuilder = new SkippedMemberBuilder<T>(memberExpression);
            _memberBuilders[memberExpression.Member] = memberBuilder;
            if (memberConfiguration != null)
                memberConfiguration(memberBuilder);
            return this;
        }

        #endregion

        private SortedList<Int32, IMemberBuilderBase> GetMembers()
        {
            var result = new SortedList<Int32, IMemberBuilderBase>();
            var order = 0;
            foreach (var memberInfo in typeof(T).GetMembers().Where(ShouldReadMember))
            {
                IMemberBuilderBase builder;
                if (!_memberBuilders.ContainsKey(memberInfo))
                {
                    if (memberInfo.IsArray())
                    {
                        throw new InvalidMappingException(string.Format("Array member '{0}' can't be configured automatically.", memberInfo));
                    }
                    // TODO: add check for default ctor existance
                    builder = MemberBuilderFactory.Create<T>(memberInfo);
                }
                else
                {
                    builder = _memberBuilders[memberInfo];
                }
                if (builder != null)
                {
                    result.Add(builder.Order != 0 ? builder.Order : order, builder);
                    order++;
                }
            }
            return result;
        }

        private static bool ShouldReadMember(MemberInfo m)
        {
            return (m is PropertyInfo && (m as PropertyInfo).CanWrite)
                   || (m is FieldInfo && !(m as FieldInfo).IsInitOnly && !(m as FieldInfo).IsLiteral);
        }

        #region Implementation of IExpressionBuilder

        public Expression BuildExpression(ExpressionBuilderArgs args)
        {
            var members = GetMembers();
            var resultObjectVar = args.InstanceVar;
            var innerArgs = args.Clone();
            if (_endianness.HasValue)
            {
                innerArgs.Endianness = Expression.Constant(_endianness.Value, typeof(Endianness));
            }
            var instanceVar = Expression.Parameter(typeof(T), "result");
            var expressions = new List<Expression>();
            expressions.Add(Expression.Call(typeof(Debug), "Indent", null));
            expressions.Add(Expression.Assign(instanceVar, Expression.Convert(resultObjectVar, typeof(T))));
            if (typeof(T).IsClass())
            {
                expressions.Add(Expression.IfThen(Expression.Equal(instanceVar, Expression.Constant(null)),
                                            Expression.Throw(Expression.New(typeof(ArgumentNullException).GetConstructor(new[] { typeof(String) }),
                                            Expression.Constant(instanceVar.Name, typeof(String))))));
            }
            innerArgs.InstanceVar = instanceVar;
            expressions.AddRange(members
                // filter ignored members
                .Where(pair => pair.Value != null)
                .Select(pair =>
                    {
                        var expression = pair.Value.BuildExpression(innerArgs);
                        return (Expression) Expression.Assign(
                            Expression.PropertyOrField(instanceVar, pair.Value.MemberName),
                            expression);
                    }));
            expressions.Add(Expression.Call(typeof(Debug), "Unindent", null));
            return Expression.Block(new[] { instanceVar }, expressions);
        }

        #endregion
    }
}
