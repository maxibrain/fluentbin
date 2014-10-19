using System;

namespace FluentBin
{
    public struct BinarySize
    {
        private UInt64 _bytes;
        private Byte _bits;

        public BinarySize(UInt64 bytes, Byte bits)
        {
            _bytes = bytes;
            _bits = bits;
        }

        public BinarySize(UInt64 bytes)
            : this(bytes, 0)
        {
        }

        public UInt64 Bytes
        {
            get { return _bytes; }
            set { _bytes = value; }
        }

        public Byte Bits
        {
            get { return _bits; }
            set
            {
                if (value > 7)
                    throw new ArgumentOutOfRangeException("value", "Bits count must be in [0,7] range.");
                _bits = value;
            }
        }

        public UInt64 TotalBits
        {
            get { return Bytes * Constants.BitsInByte + Bits; }
        }

        public override string ToString()
        {
            return string.Format("({0}b {1}B)", Bytes, Bits);
        }

        public static implicit operator BinaryOffset(BinarySize size)
        {
            return new BinaryOffset((Int64)size.Bytes, (SByte)size.Bits);
        }

        public static implicit operator BinarySize(UInt64 size)
        {
            return new BinarySize(size / Constants.BitsInByte, (Byte)(size % Constants.BitsInByte));
        }
    }
}