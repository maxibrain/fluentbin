using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FluentBin.Mapping.Builders.Impl
{
    interface IValueHandler
    {
        bool CanHandle();
        Expression BuildReadExpression();
    }
}
