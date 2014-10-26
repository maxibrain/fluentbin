using System;
using System.Linq.Expressions;
using System.Text;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders
{
    public interface IGenericMemberBuilder<out TBuilder, T, TMember> : IGenericMemberBuilder<TBuilder, T>
        where TBuilder : IGenericMemberBuilder<TBuilder, T, TMember>
    {
        TBuilder UseFactory(Expression<Func<IContext<T>, TMember>> expression);

        TBuilder ConvertValue(Expression<Func<IMemberContext<T, TMember>, TMember>> expression);
        TBuilder Assert(Expression<Func<IMemberContext<T, TMember>, Boolean>> expression);
        TBuilder AfterRead(Expression<Action<IMemberContext<T, TMember>>> expression);

        TBuilder PositionAfter(Expression<Func<IMemberContext<T, TMember>, BinaryOffset>> expression);
    }

    public interface IGenericMemberBuilder<out TBuilder, T>
        where TBuilder : IGenericMemberBuilder<TBuilder, T>
    {
        TBuilder SizeOf(BinarySize size);
        TBuilder SizeOf(Expression<Func<IContext<T>, BinarySize>> expression);
        TBuilder If(Expression<Func<IContext<T>, Boolean>> expression);
        TBuilder SetOrder(Int32 order);
        TBuilder Position(Expression<Func<IContext<T>, BinaryOffset>> expression);
        TBuilder OverrideEndianess(Endianness endianness);
    }
}