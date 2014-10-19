using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FluentBin.Mapping.Builders.Impl
{
    internal delegate T ReadDelegate<out T>(BitsReader reader);

    internal delegate void ReadFunc(BitsReader br, object value, Dictionary<Type, Expression<ReadFunc>> typeReaders);
}
