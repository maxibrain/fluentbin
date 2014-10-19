using System;
using System.IO;

namespace FluentBin.Tests.Model
{
    class WithStruct : IEquatable<WithStruct>, IWritable
    {
        public WithStruct()
        {

        }

        public WithStruct(Int16 int16, Int32 int32, Int64 int64)
        {
            Int16Value = int16;
            Int32Value = int32;
            Int64Value = int64;
        }

        public Int16 Int16Value { get; set; }
        public Int32 Int32Value { get; set; }
        public Int64 Int64Value { get; set; }

        public virtual void WriteTo(BinaryWriter bw)
        {
            bw.Write(Int16Value);
            bw.Write(Int32Value);
            bw.Write(Int64Value);
        }

        public bool Equals(WithStruct other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Int16Value == other.Int16Value && Int32Value == other.Int32Value && Int64Value == other.Int64Value;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Int16Value.GetHashCode();
                hashCode = (hashCode * 397) ^ Int32Value;
                hashCode = (hashCode * 397) ^ Int64Value.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WithStruct)obj);
        }

        public override string ToString()
        {
            return string.Join(",", Int16Value, Int32Value, Int64Value);
        }
    }
}