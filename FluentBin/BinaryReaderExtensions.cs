using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FluentBin
{
    public static class BinaryReaderExtensions
    {
        public static T ReadStruct<T>(this BinaryReader binaryReader)
        {
            return (T)binaryReader.ReadStruct(typeof(T));
        }

        public static T ReadStruct<T>(this BinaryReader binaryReader, int size)
        {
            return (T)binaryReader.ReadStruct(typeof(T), size);
        }

        public static object ReadStruct(this BinaryReader binaryReader, Type structType)
        {
            int count = Marshal.SizeOf(structType);
            return binaryReader.ReadStruct(structType, count);
        }

        public static object ReadStruct(this BinaryReader binaryReader, Type structType, int size)
        {
            byte[] readBuffer = binaryReader.ReadBytes(size);
            GCHandle handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
            var structure = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), structType);
            handle.Free();
            return structure;
        }
    }
}
