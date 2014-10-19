using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentBin.Mapping.Contexts
{
    public interface IContext<out T>
    {
        T Object { get; }
        BinaryOffset StreamPosition { get; }
        BinarySize StreamLength { get; }
    }
}
