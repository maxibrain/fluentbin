using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FluentBin.Tests
{
    [TestFixture]
    public class BitsReaderTests
    {
        [Test]
        public void CanConvertBitArrayToStruct()
        {
            var endianness = Endianness.LittleEndian;
            var bits = new []
                {
                    true, true, true,
                    false, false, false,
                    true, false, true, false, true, false, true, false, true, false, true, false, true, false, true,
                    false, true, false, true, false,
                    true, true, true, false, false, false,
                };
            var values = new List<uint>();
            using (var ms = new MemoryStream(new byte[] { 0xE2, 0xAA, 0xAA, 0xB8 }))
            {
                using (var br = new BitsReader(ms))
                {
                    foreach (var size in new ulong[] { 3, 3, 15, 5, 6 })
                    {
                        values.Add(br.ReadStruct<uint>(size, endianness));
                    }
                }
            }
            Assert.AreEqual(0x7, values[0]);
            Assert.AreEqual(0x0, values[1]);
            if (endianness == Endianness.BigEndian)
            {
                Assert.AreEqual(0x5555, values[2]);
            }
            else
            {
                Assert.AreEqual(0x55AA, values[2]);
            }
            Assert.AreEqual(0xA, values[3]);
            Assert.AreEqual(0x38, values[4]);
        }

/*        [Test, Ignore]
        public void CanRead()
        {
            using (var stream = new MemoryStream())
            {
                var value = (UInt32)new bool[]
                    {
                        true, true, true,
                        false, false, false,
                        true, false, true, false, true, false, true, false, true, false, true, false, true, false, true, false, true, false, true, false, 
                        true, true, true, false, false, false, 
                    }.GetBitsRange(0, 32);
                using (var bw = new BinaryWriter(stream, Encoding.Default, true))
                {
                    bw.Write(value);
                }
                stream.Position = 0;
                using (var bitsReader = new BitsReader(stream))
                {
                    BinaryOffset position;
                    var firstThreeBits = bitsReader.ReadBits(3);
                    Assert.AreEqual(value >> 29, (int)firstThreeBits.ToUInt64());
                    position = bitsReader.Position;
                    Assert.IsTrue(position.Bytes == 0 && position.Bits == 3);
                    var secondThreeBits = bitsReader.ReadBits(3);
                    Assert.AreEqual(value << 3 >> 29, (int)secondThreeBits.ToUInt64());
                    position = bitsReader.Position;
                    Assert.IsTrue(position.Bytes == 0 && position.Bits == 6);
                    var twentyBits = bitsReader.ReadBits(20);
                    Assert.AreEqual(value << 6 >> 12, (int)twentyBits.ToUInt64());
                    position = bitsReader.Position;
                    Assert.IsTrue(position.Bytes == 3 && position.Bits == 2);
                    var sixBits = bitsReader.ReadBits(6);
                    Assert.AreEqual(value << 26 >> 26, (int)sixBits.ToUInt64());
                    position = bitsReader.Position;
                    Assert.IsTrue(position.Bytes == 4 && position.Bits == 0);
                }
            }
        }*/
    }
}
