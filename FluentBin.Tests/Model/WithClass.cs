using System;
using System.IO;

namespace FluentBin.Tests.Model
{
    class WithClass : IEquatable<WithClass>, IWritable
    {
        public Int16 Int16Value { get; set; }
        public WithStruct Value { get; set; }

        public void WriteTo(BinaryWriter bw)
        {
            bw.Write(Int16Value);
            Value.WriteTo(bw);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WithClass)obj);
        }

        public bool Equals(WithClass other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Int16Value == other.Int16Value && Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Int16Value.GetHashCode() * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Join(",", Int16Value, string.Format("({0})", Value));
        }
    }
}