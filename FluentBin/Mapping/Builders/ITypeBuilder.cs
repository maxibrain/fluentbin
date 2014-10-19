using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FluentBin.Mapping.Builders
{
    public interface ITypeBuilder<T>
    {
        ITypeBuilder<T> OverrideEndianness(Endianness? endianness);
        ITypeBuilder<T> Read<TMember>(Expression<Func<T, TMember>> expression, Action<IMemberBuilder<T, TMember>> memberConfiguration = null);
        ITypeBuilder<T> Read<TElement>(Expression<Func<T, TElement[]>> expression, Action<IArrayMemberBuilder<T, TElement>> memberConfiguration);
        ITypeBuilder<T> Read<TElement>(Expression<Func<T, IList<TElement>>> expression, Action<IListMemberBuilder<T, TElement>> memberConfiguration = null);
        ITypeBuilder<T> Skip<TMember>(Expression<Func<T, TMember>> expression, Action<ISkippedMemberBuilder<T, TMember>> memberConfiguration);
        ITypeBuilder<T> Ignore<TMember>(Expression<Func<T, TMember>> expression);
/*
        ITypeBuilder<T> Read(Expression<Func<T, String>> expression, Action<IStringMemberBuilder<T>> memberConfiguration = null);
*/
    }
}