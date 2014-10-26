using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FluentBin.Mapping.Contexts;

namespace FluentBin.Mapping.Builders
{
    public interface IStringMemberBuilder<out TBuilder, T> : IGenericMemberBuilder<TBuilder, T, String>
        where TBuilder : IStringMemberBuilder<TBuilder, T>
    {
        TBuilder StringEncoding(Encoding encoding);
        TBuilder StringEncoding(Expression<Func<IContext<T>, Encoding>> expression);
    }

    public interface IStringMemberBuilder<T> : IStringMemberBuilder<IStringMemberBuilder<T>, T>
    {
        
    }
}
