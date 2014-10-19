using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;

namespace FluentBin
{
    public static class BinaryExtensions
    {
        public static bool[] ToBits(this byte[] bytes)
        {
            var bitArray = new BitArray(bytes);
            var bits = Enumerable.Range(0, bytes.Length * Constants.BitsInByte).Select(i => bitArray[i]).ToArray();

            return bits;
        }

        public static ulong GetBitsRange(this bool[] bits, int start, int count)
        {
            ulong result = 0;
            int i, p;
            for (i = start + count - 1, p = count - 1; i >= start; i--, p--)
            {
                if (bits[i])
                {
                    result += (ulong)Math.Pow(2, p);
                }
            }
            return result;
        }

        public static ulong GetBitsRange(this byte[] bytes, int start, int count)
        {
            return bytes.ToBits().GetBitsRange(start, count);
        }

        public static ulong ToUInt64(this byte[] bytes)
        {
            return GetBitsRange(bytes, 0, bytes.Length*8);
        }

        public static string ToCsvString(this byte[] bytes)
        {
            if (bytes.Length <= 8)
            {
                return string.Join(",", bytes.Select(b => b.ToString("X")));
            }
            else
            {
                return string.Join(",...,", 
                    string.Join(",", bytes.Take(4).ToArray().ToCsvString()),
                    string.Join(",", bytes.Skip(bytes.Length - 4).Take(4).ToArray().ToCsvString()));
            }
        }
    }
}
