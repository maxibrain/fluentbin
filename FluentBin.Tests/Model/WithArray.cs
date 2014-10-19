using System;
using System.IO;

namespace FluentBin.Tests.Model
{
    class WithArray : IEquatable<WithArray>, IWritable
    {
        public WithStruct[] FixedLegthArray { get; set; }
        public UInt64 VarLength { get; set; }
        public WithStruct[] VarLegthArray { get; set; }

        public void WriteTo(BinaryWriter bw)
        {
            foreach (var withStructMembers in FixedLegthArray)
            {
                withStructMembers.WriteTo(bw);
            }
            bw.Write((UInt64)VarLegthArray.Length);
            foreach (var withStructMembers in VarLegthArray)
            {
                withStructMembers.WriteTo(bw);
            }
        }

        public bool Equals(WithArray other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return FixedLegthArray.ElementsEqual(other.FixedLegthArray) && VarLength == other.VarLength && VarLegthArray.ElementsEqual(other.VarLegthArray);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (FixedLegthArray != null ? FixedLegthArray.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ VarLength.GetHashCode();
                hashCode = (hashCode * 397) ^ (VarLegthArray != null ? VarLegthArray.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WithArray)obj);
        }
    }
}