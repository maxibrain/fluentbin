using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentBin.Mapping.Builders
{
    interface IMemberBuilderBase : IExpressionBuilder
    {
        int Order { get; }
        string MemberName { get; }
    }
}
