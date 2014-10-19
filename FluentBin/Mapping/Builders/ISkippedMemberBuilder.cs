using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders
{
    public interface ISkippedMemberBuilder<T, TMember> : IMemberBuilder<T>
    {
        ISkippedMemberBuilder<T, TMember> SizeOf(BinarySize sizeInBytes);
        ISkippedMemberBuilder<T, TMember> SizeOf(Expression<Func<IContext<T>, BinarySize>> expression);
        ISkippedMemberBuilder<T, TMember> If(Expression<Func<IContext<T>, Boolean>> expression);
        ISkippedMemberBuilder<T, TMember> SetOrder(Int32 order);
    }
}
