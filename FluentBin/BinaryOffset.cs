using System;

namespace FluentBin
{
    public struct BinaryOffset
    {
        private Int64 _bytes;
        private SByte _bits;

        public BinaryOffset(Int64 bytes, SByte bits)
        {
            _bytes = bytes;
            _bits = bits;
        }

        public Int64 Bytes
        {
            get { return _bytes; }
            set { _bytes = value; }
        }

        public SByte Bits
        {
            get { return _bits; }
            set
            {
                if (value < -7 || value > 7)
                    throw new ArgumentOutOfRangeException("value", "Bits count must be in [-7,7] range.");
                _bits = value;
            }
        }

        public BinaryOffset Add(BinaryOffset other)
        {
            var bitsSum = this.Bits + other.Bits;
            return new BinaryOffset(this.Bytes + other.Bytes + bitsSum / Constants.BitsInByte, (SByte)(bitsSum % Constants.BitsInByte));
        }

        public BinaryOffset Add(BinarySize other)
        {
            return Add((BinaryOffset) other);
        }

        public BinaryOffset Substract(BinaryOffset other)
        {
            return Add(new BinaryOffset(-other.Bytes, (SByte)(-other.Bits)));
        }

        public BinaryOffset Substract(BinarySize other)
        {
            return Substract((BinaryOffset)other);
        }

        public static BinaryOffset operator +(BinaryOffset lhs, BinaryOffset rhs)
        {
            return lhs.Add(rhs);
        }

        public static BinaryOffset operator -(BinaryOffset lhs, BinaryOffset rhs)
        {
            return lhs.Substract(rhs);
        }

        public static bool operator < (BinaryOffset lhs, BinaryOffset rhs)
        {
            return (lhs.Bytes < rhs.Bytes) || (lhs.Bytes == rhs.Bytes && lhs.Bits < rhs.Bits);
        }

        public static bool operator >(BinaryOffset lhs, BinaryOffset rhs)
        {
            return (lhs.Bytes > rhs.Bytes) || (lhs.Bytes == rhs.Bytes && lhs.Bits > rhs.Bits);
        }

        public static bool operator <=(BinaryOffset lhs, BinaryOffset rhs)
        {
            return (lhs.Bytes < rhs.Bytes) || (lhs.Bytes == rhs.Bytes && lhs.Bits <= rhs.Bits);
        }

        public static bool operator >=(BinaryOffset lhs, BinaryOffset rhs)
        {
            return (lhs.Bytes > rhs.Bytes) || (lhs.Bytes == rhs.Bytes && lhs.Bits >= rhs.Bits);
        }

        public override string ToString()
        {
            return string.Format("(0x{0:X}b {1}B)", Bytes, Bits);
        }
    }
}