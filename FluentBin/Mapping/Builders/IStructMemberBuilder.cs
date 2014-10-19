using System;
using System.Linq.Expressions;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders
{
    public interface IStructMemberBuilder<T, TMember> : IMemberBuilder<T, TMember>
    {
    }
}