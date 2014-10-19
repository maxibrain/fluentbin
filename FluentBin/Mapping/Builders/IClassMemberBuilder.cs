using System;
using System.Linq.Expressions;
using FluentBin.Mapping.Builders.Impl;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders
{
    public interface IClassMemberBuilder<T, TMember> : IMemberBuilder<T, TMember>
    {
    }
}