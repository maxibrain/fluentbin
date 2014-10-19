using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using FluentBin.Mapping;
using FluentBin.Mapping.Builders;
using FluentBin.Mapping.Contexts;
using FluentBin.Mapping.Models;
using FluentBin.Tests.Model;
using NUnit.Framework;

namespace FluentBin.Tests
{
    [TestFixture]
    public class MemberReading
    {
        [Test]
        public void CanReadStructMember()
        {
            using (var stream = new MemoryStream())
            {
                var expected = new WithStruct(16, 32, 64);
                using (var bw = new BinaryWriter(stream, Encoding.Default, true))
                {
                    expected.WriteTo(bw);
                }
                stream.Position = 0;

                var formatBuilder = Bin.Format()
                    .Includes<WithStruct>();

                var format = formatBuilder.Build<WithStruct>();
                var actual = format.Read(stream);
                
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void CanReadByCondition()
        {
            using (var stream = new MemoryStream())
            {
                var expected = new WithStruct(16, 32, 64);
                using (var bw = new BinaryWriter(stream, Encoding.Default, true))
                {
                    expected.WriteTo(bw);
                }
                stream.Position = 0;

                var formatBuilder = Bin.Format()
                    .Includes<WithStruct>(cfg => cfg.Read(c => c.Int64Value, builder => builder.If(c => false)));

                var format = formatBuilder.Build<WithStruct>();
                var actual = format.Read(stream);

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void CanReadClassMember()
        {
            using (var stream = new MemoryStream())
            {
                var expected = new WithClass()
                    {
                        Int16Value = 160,
                        Value = new WithStruct(16, 32, 64)
                    };
                using (var bw = new BinaryWriter(stream, Encoding.Default, true))
                {
                    expected.WriteTo(bw);
                }
                stream.Position = 0;

                var formatBuilder = Bin.Format()
                    .Includes<WithClass>()
                    .Includes<WithStruct>();

                var format = formatBuilder.Build<WithClass>();
                var actual = format.Read(stream);
                
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void CanBuildFormatForRecursion()
        {
            var formatBuilder = Bin.Format()
                .Includes<WithRecursion>();

            var format = formatBuilder.Build<WithRecursion>();

            Assert.NotNull(format);
        }
    }
}
