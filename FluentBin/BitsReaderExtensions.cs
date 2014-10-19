using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FluentBin
{
    public static class BitsReaderExtensions
    {
        public static T ReadStruct<T>(this BitsReader bitsReader, Endianness endianness)
        {
            return (T)bitsReader.ReadStruct(typeof(T), endianness);
        }

        public static T ReadStruct<T>(this BitsReader bitsReader, BinarySize size, Endianness endianness)
        {
            return (T)bitsReader.ReadStruct(typeof(T), size, endianness);
        }

        public static object ReadStruct(this BitsReader bitsReader, Type structType, Endianness endianness)
        {
            if (structType == null)
                throw new ArgumentNullException("structType");
            ulong count = (ulong)Marshal.SizeOf(structType);
            return bitsReader.ReadStruct(structType, new BinarySize(count), endianness);
        }

        public static object ReadStruct(this BitsReader bitsReader, Type structType, BinarySize size, Endianness endianness)
        {
            if (structType == null)
                throw new ArgumentNullException("structType");
            byte[] readBuffer = bitsReader.ReadBits(size, endianness);
            if (readBuffer.Length > 1 && !endianness.MatchesMachineEndianness())
            {
                Debug.WriteLine("Endianness mismatch. Reversing bytes...");
                Array.Reverse(readBuffer);
                Debug.WriteLine(string.Concat("Bytes: ", readBuffer.ToCsvString()));
            }
            GCHandle handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
            var structure = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), structType);
            handle.Free();
            return structure;
        }

        private static bool MatchesMachineEndianness(this Endianness endianness)
        {
            return (endianness == Endianness.BigEndian && !BitConverter.IsLittleEndian)
                || (endianness == Endianness.LittleEndian && BitConverter.IsLittleEndian);
        }

        public static string ReadString(this BitsReader bitsReader, BinarySize size, Encoding encoding)
        {
            Debug.WriteLine("Reading String...");
            if (encoding == null)
                throw new ArgumentNullException("encoding");
            var buffer = bitsReader.ReadBits(size);
            var result = encoding.GetString(buffer);
            Debug.WriteLine("String = {0}.", result);
            return result;
        }
    }
}