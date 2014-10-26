using System;
using System.Linq.Expressions;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders
{
    public interface IGenericStructMemberBuilder<out TBuilder, T, TMember> : IGenericMemberBuilder<TBuilder, T, TMember>
        where TBuilder : IGenericStructMemberBuilder<TBuilder, T, TMember>
    {
    }
}