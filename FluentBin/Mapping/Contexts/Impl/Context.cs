using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentBin.Mapping.Contexts.Impl
{
    class Context<T> : IContext<T>
    {
        public Context(T o, BinaryOffset position, BinarySize length)
        {
            StreamPosition = position;
            Object = o;
            StreamLength = length;
        }

        public T Object { get; private set; }
        public BinaryOffset StreamPosition { get; private set; }
        public BinarySize StreamLength { get; private set; }
    }
}
