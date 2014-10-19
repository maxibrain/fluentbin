using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders.Impl
{
    abstract class MemberBuilder<T, TMember> : MemberBuilder<T>, IMemberBuilder<T, TMember>
    {
        protected MemberBuilder(MemberExpression expression)
            : base(expression)
        {
        }

        protected MemberBuilder(MemberInfo memberInfo)
            : base(memberInfo)
        {
            
        }

        protected MemberBuilder(string memberName)
            : base(typeof(TMember), memberName)
        {
        }

        protected Expression<Func<IContext<T>, Boolean>> _if;
        protected Expression<Func<IContext<T>, BinaryOffset>> _position;
        protected Expression<Func<IContext<T>, TMember>> _factory;
        protected Expression<Func<IMemberContext<T, TMember>, TMember>> _convertValue;
        protected Expression<Func<IMemberContext<T, TMember>, Boolean>> _assertValue;
        protected Expression<Action<IMemberContext<T, TMember>>> _afterRead;
        protected Expression<Func<IMemberContext<T, TMember>, BinaryOffset>> _positionAfter;
        protected Expression<Func<IContext<T>, Encoding>> _encoding;

        protected Expression Invoke<TResult>(Expression<Func<IMemberContext<T, TMember>, TResult>> expression, ExpressionBuilderArgs args, ParameterExpression innerResultVar)
        {
            return Expression.Invoke(expression, AdvancedExpression.MemberContext<T, TMember>(args.BrParameter, args.InstanceVar, innerResultVar));
        }

        #region Implementation of IMemberBuilder<T,TMember>

        public IMemberBuilder<T, TMember> UseFactory(Expression<Func<IContext<T>, TMember>> expression)
        {
            _factory = expression;
            return this;
        }

        public IMemberBuilder<T, TMember> If(Expression<Func<IContext<T>, Boolean>> expression)
        {
            _if = expression;
            return this;
        }

        public IMemberBuilder<T, TMember> ConvertValue(Expression<Func<IMemberContext<T, TMember>, TMember>> expression)
        {
            _convertValue = expression;
            return this;
        }

        public IMemberBuilder<T, TMember> Assert(Expression<Func<IMemberContext<T, TMember>, Boolean>> expression)
        {
            _assertValue = expression;
            return this;
        }

        public IMemberBuilder<T, TMember> AfterRead(Expression<Action<IMemberContext<T, TMember>>> expression)
        {
            _afterRead = expression;
            return this;
        }

        public IMemberBuilder<T, TMember> SizeOf(BinarySize size)
        {
            return SizeOf(c => size);
        }

        public IMemberBuilder<T, TMember> SizeOf(Expression<Func<IContext<T>, BinarySize>> expression)
        {
            Size = expression;
            return this;
        }

        public IMemberBuilder<T, TMember> SetOrder(int order)
        {
            Order = order;
            return this;
        }

        public IMemberBuilder<T, TMember> Position(Expression<Func<IContext<T>, BinaryOffset>> expression)
        {
            _position = expression;
            return this;
        }

        public IMemberBuilder<T, TMember> PositionAfter(Expression<Func<IMemberContext<T, TMember>, BinaryOffset>> expression)
        {
            _positionAfter = expression;
            return this;
        }

        public IMemberBuilder<T, TMember> OverrideEndianess(Endianness endianness)
        {
            EndiannessOverride = endianness;
            return this;
        }

        public IMemberBuilder<T, TMember> StringEncoding(Encoding encoding)
        {
            return StringEncoding(c => encoding);
        }

        public IMemberBuilder<T, TMember> StringEncoding(Expression<Func<IContext<T>, Encoding>> encodingExpression)
        {
            _encoding = encodingExpression;
            return this;
        }

        protected Endianness? EndiannessOverride { get; set; }

        #endregion

        protected override Expression BuildBeforeExpression(ExpressionBuilderArgs args)
        {
            var expressions = new List<Expression>();
            if (_position != null)
                expressions.Add(Expression.Assign(AdvancedExpression.Position(args.BrParameter), Invoke(_position, args)));
            expressions.Add(base.BuildBeforeExpression(args));
            return Expression.Block(expressions);
        }

        protected override Expression BuildCtorExpression(ExpressionBuilderArgs args)
        {
            if (_factory != null)
            {
                return Expression.Convert(Expression.Invoke(_factory, AdvancedExpression.Context<T>(args.BrParameter, args.InstanceVar)), MemberType);
            }
            else
            {
                return Expression.New(MemberType);
            }
        }

        protected override Expression BuildAfterExpression(ExpressionBuilderArgs args, ParameterExpression innerResultVar)
        {
            var expressions = new List<Expression>();
            expressions.Add(base.BuildAfterExpression(args, innerResultVar));
            if (_convertValue != null)
                expressions.Add(Expression.Assign(innerResultVar, Invoke(_convertValue, args, innerResultVar)));
            if (_assertValue != null)
                expressions.Add(Expression.IfThen(
                    Expression.Not(Invoke(_assertValue, args, innerResultVar)),
                    Expression.Throw(Expression.New(
                    typeof(BinaryReadingAssertException).GetConstructor(new[] { typeof(Object), typeof(Object), typeof(String) }),
                    Expression.TypeAs(args.InstanceVar, typeof(Object)),
                    Expression.TypeAs(innerResultVar, typeof(Object)),
                    Expression.Constant(MemberName)))));
            if (_positionAfter != null)
                expressions.Add(Expression.Assign(AdvancedExpression.Position(args.BrParameter), Invoke(_positionAfter, args, innerResultVar)));
            return Expression.Block(expressions);
        }

        public static MemberBuilder<T, TMember> Create(string memberName)
        {
            return Create(typeof (TMember), memberName) as MemberBuilder<T, TMember>;
        }
    }

    internal abstract class MemberBuilder<T> : IMemberBuilder<T>, IExpressionBuilder
    {
        public Type MemberType { get; private set; }
        public string MemberName { get; private set; }

        protected MemberBuilder(Type memberType, string memberName)
        {
            MemberType = memberType;
            MemberName = memberName;
        }

        protected MemberBuilder(MemberInfo memberInfo)
            : this(memberInfo.GetMemberType(), memberInfo.Name)
        {
        }

        protected MemberBuilder(MemberExpression memberExpression)
            : this(memberExpression.Member)
        {
        }

        public int Order { get; protected set; }

        protected Expression<Func<IContext<T>, BinarySize>> Size;

        protected Expression Invoke<TResult>(Expression<Func<IContext<T>, TResult>> expression, ExpressionBuilderArgs args)
        {
            return Expression.Invoke(expression, AdvancedExpression.Context<T>(args.BrParameter, args.InstanceVar));
        }

        #region Implementation of IExpressionBuilder

        public Expression BuildExpression(ExpressionBuilderArgs args)
        {
            var innerResultVar = Expression.Variable(MemberType, MemberName);
            var typeVar = Expression.Variable(typeof(Type), string.Format("{0}Type", MemberName));
            var positionVar = Expression.Variable(typeof (BinaryOffset), "positionBefore");
            var expressions = new List<Expression>()
                {
                    AdvancedExpression.Debug(string.Format("Reading {0}...", MemberName)),
                    BuildBeforeExpression(args),
                    Expression.Assign(positionVar, AdvancedExpression.Position(args.BrParameter)),
                    Expression.Assign(innerResultVar, BuildCtorExpression(args)),
                    AdvancedExpression.Debug(string.Format("{0} constructed.", MemberName)),
                    Expression.IfThenElse(
                        Expression.Equal(innerResultVar, Expression.Default(MemberType)),
                        Expression.Assign(typeVar, Expression.Constant(MemberType)),
                        Expression.Assign(typeVar, AdvancedExpression.GetType(Expression.TypeAs(innerResultVar, typeof (Object))))
                        ),
                    AdvancedExpression.Debug(string.Format("{0} type is {{0}}.", MemberName), typeVar),
                    BuildBodyExpression(args, innerResultVar, typeVar),
                    AdvancedExpression.Debug(string.Format("{0} = {{0}}.", MemberName), innerResultVar),
                };
            if (Size != null)
            {
                expressions.Add(Expression.Assign(
                    AdvancedExpression.Position(args.BrParameter),
                    Expression.Add(positionVar, Expression.Convert(Invoke(Size, args), typeof (BinaryOffset)))));
            }
            expressions.AddRange(new []
                {
                    BuildAfterExpression(args, innerResultVar),
                                    AdvancedExpression.Debug("Position = {0}", AdvancedExpression.Position(args.BrParameter)),
                                    innerResultVar
                });

            return Expression.Block(new[] {innerResultVar, typeVar, positionVar}, expressions);
        }

        protected virtual Expression BuildBeforeExpression(ExpressionBuilderArgs args)
        {
            return Expression.Empty();
        }

        protected abstract Expression BuildCtorExpression(ExpressionBuilderArgs args);

        protected abstract Expression BuildBodyExpression(ExpressionBuilderArgs args, ParameterExpression innerResultVar, ParameterExpression typeVar);

        protected virtual Expression BuildAfterExpression(ExpressionBuilderArgs args, ParameterExpression innerResultVar)
        {
            return Expression.Empty();
        }


        #endregion

        public static MemberBuilder<T> Create(MemberInfo memberInfo)
        {
            return Create(memberInfo.GetMemberType(), memberInfo.Name);
        }

        public static MemberBuilder<T> Create(Type memberType, string memberName)
        {
            Type builderType;
            Type t2 = memberType;
            
            if (memberType == typeof(String))
            {
                return new StringMemberBuilder<T>(memberName);
            }
            
            if (memberType.IsStruct())
            {
                builderType = typeof (StructMemberBuilder<,>);
            }
            else if (memberType.IsClass())
            {
                builderType = typeof (ClassMemberBuilder<,>);
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
            return (MemberBuilder<T>)Activator.CreateInstance(builderType.MakeGenericType(typeof(T), t2), memberName);
        }
    }
}