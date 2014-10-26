using System;
using System.Linq.Expressions;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders
{
    public interface IGenericCollectionMemberBuilder<out TBuilder, T, TMember, TElement> : IGenericClassMemberBuilder<TBuilder, T, TMember>
        where TBuilder : IGenericCollectionMemberBuilder<TBuilder, T, TMember, TElement>
    {
        TBuilder Element(Action<IMemberBuilder<TMember, TElement>> elementBuilderConfiguration);
    }
}