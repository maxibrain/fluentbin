using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace FluentBin
{
    public class BitsReader : IDisposable
    {
        private readonly BinaryReader _br;
        private sbyte _bitPosition;

        public BitsReader(Stream input)
        {
            _br = new BinaryReader(input);
        }

        public BitsReader(Stream input, Encoding encoding)
        {
            _br = new BinaryReader(input, encoding);
        }

        public byte[] ReadBits(BinarySize size, Endianness? endianness = null)
        {
            if (size.Bytes == 0 && size.Bits == 0)
                return new byte[0];
/*
            byte[] result;
            Debug.WriteLine("Reading {0} bits...", size.TotalBits);
            var position = Position;
            var hasBitsInCurrentByte = (byte) (Constants.BitsInByte - _bitPosition);
            Debug.Assert(hasBitsInCurrentByte <= 8);
            var bitsInCurrentByte = (byte) Math.Min(hasBitsInCurrentByte, size.TotalBits);
            Debug.Assert(bitsInCurrentByte <= 8 && bitsInCurrentByte <= hasBitsInCurrentByte);
            var bitsAfterCurrentByte = size.TotalBits - bitsInCurrentByte;
            Debug.Assert(bitsAfterCurrentByte == 0 ||
                         (bitsAfterCurrentByte > 0 && hasBitsInCurrentByte == bitsInCurrentByte));
            var bitsInLastByte = (byte) (bitsAfterCurrentByte%Constants.BitsInByte);
            Debug.Assert(bitsInLastByte < 8);
            int bytesCount = 1 + (int) (bitsAfterCurrentByte/Constants.BitsInByte);
            if (bitsInLastByte > 0)
                bytesCount++;
            var bytes = _br.ReadBytes(bytesCount);
            var bits = bytes.SelectMany(bs =>
                {
                    var bts = new[] {bs}.ToBits();
                    Array.Reverse(bts);
                    return bts;
                }).ToArray();
            bits = bits.Skip(_bitPosition).Take((int) size.TotalBits).ToArray();
            result = ConvertBitsToBytes(bits, endianness);
            Position = position.Add(size);
            return result;
*/
            Debug.WriteLine("Reading {0} bits...", size.TotalBits);
            var position = Position;
            var newPosition = position + size;
            var bytesCount = (newPosition - position).Bytes + (newPosition.Bits > 0 ? 1 : 0);
            Debug.WriteLine("Reading {0} bytes...", bytesCount);
            var bytes = _br.ReadBytes((int)bytesCount);
            Debug.WriteLine(string.Concat("Bytes: ", bytes.ToCsvString()));
            if (position.Bits > 0)
            {
                Debug.WriteLine("Truncating first byte {0:X}...", bytes[0]);
                bytes[0] = (Byte)(bytes[0] & (Byte)(Byte.MaxValue >> position.Bits));
                Debug.WriteLine("First byte truncated: {0:X}.", bytes[0]);
            }
            if (newPosition.Bits > 0)
            {
                Debug.WriteLine("Truncating last byte {0:X}...", bytes[bytes.Length - 1]);
                bytes[bytes.Length - 1] = (Byte)(bytes[bytes.Length - 1] & (Byte)(Byte.MaxValue << (Constants.BitsInByte - newPosition.Bits)));
                Debug.WriteLine("Last byte truncated: {0:X}.", bytes[bytes.Length - 1]);
            }
            int bitsInLastByte = newPosition.Bits;
            if (endianness == Endianness.BigEndian)
            {
                if (bytes.Length > 1 && newPosition.Bits > 0)
                {
                    Debug.WriteLine("Left shifting bytes by {0} bits...", newPosition.Bits);
                    bytes = LshBytes(bytes, newPosition.Bits);
                    Debug.WriteLine(string.Concat("Bytes: ", bytes.ToCsvString()));
                    bitsInLastByte = 0;
                }
            }
            else
            {
                if (bytes.Length > 1 && position.Bits > 0 && endianness == Endianness.LittleEndian)
                {
                    Debug.WriteLine("Right shifting bytes by {0} bits...", Constants.BitsInByte - position.Bits);
                    bytes = RshBytes(bytes, Constants.BitsInByte - position.Bits);
                    Debug.WriteLine(string.Concat("Bytes: ", bytes.ToCsvString()));
                    bitsInLastByte += Constants.BitsInByte - position.Bits;
                }
            }
            bitsInLastByte %= Constants.BitsInByte;
            if (bitsInLastByte > 0)
            {
                Debug.WriteLine("Shifting last byte {0:X}...", bytes[bytes.Length - 1]);
                bytes[bytes.Length - 1] = (byte)(bytes[bytes.Length - 1] >> (Constants.BitsInByte - bitsInLastByte));
                Debug.WriteLine("Last byte shifted: {0:X}.", bytes[bytes.Length - 1]);
            }
            Position = newPosition;
            return bytes;
        }

        private static Byte[] LshBytes(Byte[] b, Int32 n)
        {
            Byte tail = 0;
            Byte tailMask = (Byte)(Byte.MaxValue << (Constants.BitsInByte - n));

            var newLength = b.Length - 1;
            var iInc = 0;
            if ((b.First() & tailMask) > 0)
            {
                newLength++;
                iInc = 1;
            }
            var bytes = new Byte[newLength];

            for (var i = b.Length - 2; i >= 0; i--)
            {
                tail = (Byte)((b[i + 1] & tailMask) >> (Constants.BitsInByte - n));
                bytes[i + iInc] = (Byte)((b[i] << n) | tail);
            }
            tail = (Byte)((b[0] & tailMask) >> (Constants.BitsInByte - n));
            if (tail > 0)
            {
                bytes[0] = tail;
            }
            return bytes;
        }

        private static Byte[] RshBytes(Byte[] b, Int32 n)
        {
            Byte tail = 0;
            Byte tailMask = (Byte)((1 << n) - 1); // = 1 x n binary

            var newLength = b.Length - 1;
            if ((b.Last() & tailMask) > 0)
            {
                newLength++;
            }
            var bytes = new Byte[newLength];

            for (var i = 0; i < b.Length - 1; i++)
            {
                tail = (Byte)((b[i] & tailMask) << (Constants.BitsInByte - n));
                bytes[i] = (Byte)((b[i + 1] >> n) | tail);
            }
            tail = (Byte)((b.Last() & tailMask) << (Constants.BitsInByte - n));
            if (tail > 0)
            {
                bytes[newLength - 1] = tail;
            }
            return bytes;
        }


        private static readonly int[] PowOf2 = Enumerable.Range(0, Constants.BitsInByte).Select(i => 2 << i).ToArray();

        private byte[] ConvertBitsToBytes(bool[] bits, Endianness endianness)
        {
            var n = bits.Length;
            var m = n / Constants.BitsInByte + (n % Constants.BitsInByte > 0 ? 1 : 0);
            var b = new byte[m];
            Func<int, int> jFunc;
            Func<int, int, int> pow = (ji, j) => ji - j + Constants.BitsInByte - 1;
            switch (endianness)
            {
                case Endianness.LittleEndian:
                    jFunc = byteIndex => Constants.BitsInByte*byteIndex;
                    for (int i = 0; i < m - 1; i++)
                    {
                        var ji0 = jFunc(i);
                        var ji1 = jFunc(i + 1);
                        b[i] =
                            (byte) Enumerable
                                       .Range(ji0, ji1 - ji0)
                                       .Sum(j => bits[j] ? PowOf2[pow(ji0, j)] : 0);
                    }
                    var jm0 = jFunc(m - 1);
                    var jm1 = jFunc(m);
                    b[m - 1] = (byte) Enumerable
                                          .Range(jm0, n - jm0)
                                          .Sum(j => bits[j] ? PowOf2[pow(jm0, j) - (jm1 - n)] : 0);
                    break;
                case Endianness.BigEndian:
                    jFunc = byteIndex => Math.Max(0, n - 8*(m - byteIndex));
                    for (int i = 0; i < m; i++)
                    {
                        var ji0 = jFunc(i);
                        var ji1 = jFunc(i + 1);
                        b[i] =
                            (byte) Enumerable
                                       .Range(ji0, ji1 - ji0)
                                       .Sum(j => bits[j] ? PowOf2[pow(ji0, j)] : 0);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("endianness");
            }
            return b;
        }

        public BinaryOffset Position
        {
            get { return new BinaryOffset(_br.BaseStream.Position, _bitPosition); } 
            set
            {
                _br.BaseStream.Position = value.Bytes;
                _bitPosition = value.Bits;
            }
        }

        public BinarySize Length
        {
            get { return new BinarySize((UInt64)_br.BaseStream.Length); }
        }

        public void Dispose()
        {
            _br.Dispose();
        }
    }
}
