using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders
{
    public interface IGenericSkippedMemberBuilder<out TBuilder, T> : IGenericMemberBuilder<TBuilder, T>
        where TBuilder : IGenericSkippedMemberBuilder<TBuilder, T>
    {
    }

    public interface ISkippedMemberBuilder<T> : IGenericSkippedMemberBuilder<ISkippedMemberBuilder<T>, T>
    {
        
    }
}
