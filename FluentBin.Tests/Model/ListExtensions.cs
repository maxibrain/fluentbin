using System;
using System.Collections;

namespace FluentBin.Tests.Model
{
    static class ListExtensions
    {
        public static bool ElementsEqual(this IList collection, IList other)
        {
            if (collection == null || other == null)
                return false;
            if (collection.Count != other.Count)
                return false;
            for (int i = 0; i < collection.Count; i++)
            {
                if (!Object.Equals(collection[i], other[i]))
                    return false;
            }
            return true;
        }
    }
}