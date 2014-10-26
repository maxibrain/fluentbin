using System;
using System.Linq.Expressions;
using FluentBin.Mapping.Builders.Impl;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders
{
    public interface IGenericClassMemberBuilder<out TBuilder, T, TMember> : IGenericMemberBuilder<TBuilder, T, TMember>
        where TBuilder : IGenericClassMemberBuilder<TBuilder, T, TMember>
    {
    }
}