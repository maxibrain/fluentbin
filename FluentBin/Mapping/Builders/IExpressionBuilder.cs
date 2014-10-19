using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FluentBin.Mapping.Builders.Impl;

namespace FluentBin.Mapping.Builders
{
    interface IExpressionBuilder
    {
        Expression BuildExpression(ExpressionBuilderArgs args);
    }

    interface IExpressionBuilder<TDelegate>
    {
        Expression<TDelegate> BuildExpression();
    }
}
