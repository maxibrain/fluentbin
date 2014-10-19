using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FluentBin.Mapping.Builders.Impl.ValueHandlers
{
    class FallbackValueHandler : IValueHandler
    {
        private readonly List<IValueHandler> _handlers;

        public FallbackValueHandler()
        {
            
        }

        public bool CanHandle()
        {
            throw new NotImplementedException();
        }

        public Expression BuildReadExpression()
        {
            throw new NotImplementedException();
        }
    }
}
