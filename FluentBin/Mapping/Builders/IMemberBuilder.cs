using System;
using System.Linq.Expressions;
using System.Text;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders
{
    public interface IMemberBuilder<T, TMember> : IMemberBuilder<T>
    {
        IMemberBuilder<T, TMember> UseFactory(Expression<Func<IContext<T>, TMember>> expression);

        IMemberBuilder<T, TMember> SizeOf(BinarySize sizeInBytes);
        IMemberBuilder<T, TMember> SizeOf(Expression<Func<IContext<T>, BinarySize>> expression);
        
        IMemberBuilder<T, TMember> If(Expression<Func<IContext<T>, Boolean>> expression);

        IMemberBuilder<T, TMember> ConvertValue(Expression<Func<IMemberContext<T, TMember>, TMember>> expression);
        IMemberBuilder<T, TMember> Assert(Expression<Func<IMemberContext<T, TMember>, Boolean>> expression);
        IMemberBuilder<T, TMember> AfterRead(Expression<Action<IMemberContext<T, TMember>>> expression);

        IMemberBuilder<T, TMember> SetOrder(Int32 order);
        IMemberBuilder<T, TMember> Position(Expression<Func<IContext<T>, BinaryOffset>> expression);
        IMemberBuilder<T, TMember> PositionAfter(Expression<Func<IMemberContext<T, TMember>, BinaryOffset>> expression);

        IMemberBuilder<T, TMember> OverrideEndianess(Endianness endianness);

        IMemberBuilder<T, TMember> StringEncoding(Encoding encoding);
        IMemberBuilder<T, TMember> StringEncoding(Expression<Func<IContext<T>, Encoding>> encodingExpression);
    }

    public interface IMemberBuilder<T>
    {
    }
}