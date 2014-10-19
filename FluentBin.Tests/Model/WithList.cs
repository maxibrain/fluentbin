using System;
using System.Collections.Generic;
using System.IO;

namespace FluentBin.Tests.Model
{
    class WithList<T> : IEquatable<WithList<T>>, IWritable
        where T : IWritable
    {
        public List<T> Values { get; set; }

        public void WriteTo(BinaryWriter bw)
        {
            foreach (var value in Values)
            {
                value.WriteTo(bw);
            }
        }

        public bool Equals(WithList<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Values.ElementsEqual(other.Values);
        }

        public override int GetHashCode()
        {
            return (Values != null ? Values.GetHashCode() : 0);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WithList<T>)obj);
        }
    }
}