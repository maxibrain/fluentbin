using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders.Impl
{
    class StringMemberBuilder<TBuilder, T> : GenericMemberBuilder<TBuilder, T, String>
        where TBuilder : StringMemberBuilder<TBuilder, T>
    {
        private Expression<Func<IContext<T>, Encoding>> _encoding;

        public StringMemberBuilder(MemberExpression expression) : base(expression)
        {
        }

        public StringMemberBuilder(MemberInfo memberInfo) : base(memberInfo)
        {
        }

        public StringMemberBuilder(string memberName) : base(memberName)
        {
        }

        public TBuilder StringEncoding(Encoding encoding)
        {
            return StringEncoding(c => encoding);
        }

        public TBuilder StringEncoding(Expression<Func<IContext<T>, Encoding>> encodingExpression)
        {
            _encoding = encodingExpression;
            return (TBuilder)this;
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

    class StringMemberBuilder<T> : StringMemberBuilder<StringMemberBuilder<T>, T>, IStringMemberBuilder<T>
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

        public new IStringMemberBuilder<T> SizeOf(BinarySize size)
        {
            return base.SizeOf(size);
        }

        public new IStringMemberBuilder<T> SizeOf(Expression<Func<IContext<T>, BinarySize>> expression)
        {
            return base.SizeOf(expression);
        }

        public new IStringMemberBuilder<T> If(Expression<Func<IContext<T>, bool>> expression)
        {
            return base.If(expression);
        }

        public new IStringMemberBuilder<T> SetOrder(int order)
        {
            return base.SetOrder(order);
        }

        public new IStringMemberBuilder<T> Position(Expression<Func<IContext<T>, BinaryOffset>> expression)
        {
            return base.Position(expression);
        }

        public new IStringMemberBuilder<T> OverrideEndianess(Endianness endianness)
        {
            return base.OverrideEndianess(endianness);
        }

        public new IStringMemberBuilder<T> UseFactory(Expression<Func<IContext<T>, string>> expression)
        {
            return base.UseFactory(expression);
        }

        public new IStringMemberBuilder<T> ConvertValue(Expression<Func<IMemberContext<T, string>, string>> expression)
        {
            return base.ConvertValue(expression);
        }

        public new IStringMemberBuilder<T> Assert(Expression<Func<IMemberContext<T, string>, bool>> expression)
        {
            return base.Assert(expression);
        }

        public new IStringMemberBuilder<T> AfterRead(Expression<Action<IMemberContext<T, string>>> expression)
        {
            return base.AfterRead(expression);
        }

        public new IStringMemberBuilder<T> PositionAfter(Expression<Func<IMemberContext<T, string>, BinaryOffset>> expression)
        {
            return base.PositionAfter(expression);
        }

        public new IStringMemberBuilder<T> StringEncoding(Encoding encoding)
        {
            return base.StringEncoding(encoding);
        }

        public new IStringMemberBuilder<T> StringEncoding(Expression<Func<IContext<T>, Encoding>> expression)
        {
            return base.StringEncoding(expression);
        }
    }
}
