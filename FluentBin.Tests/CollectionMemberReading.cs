using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentBin.Mapping;
using FluentBin.Tests.Model;
using NUnit.Framework;

namespace FluentBin.Tests
{
    [TestFixture]
    public class CollectionMemberReading
    {
        [Test]
        public void CanReadArrayMember()
        {
            using (var stream = new MemoryStream())
            {
                WithStruct value1 = new WithStruct(161, 321, 641);
                WithStruct value2 = new WithStruct(162, 322, 642);
                WithStruct value3 = new WithStruct(163, 323, 643);
                WithArray expected = new WithArray()
                {
                    FixedLegthArray = new[] { value1, value2 },
                    VarLength = 1,
                    VarLegthArray = new[] { value3 }
                };
                using (var bw = new BinaryWriter(stream, Encoding.Default, true))
                {
                    expected.WriteTo(bw);
                }
                stream.Position = 0;

                var formatBuilder = Bin.Format()
                    .Includes<WithArray>(cfg => cfg
                                                           .Read(t => t.FixedLegthArray, acfg => acfg.Length(2))
                                                           .Read(t => t.VarLegthArray, acfg => acfg.Length(ctx => ctx.Object.VarLength)))
                    .Includes<WithStruct>();

                var format = formatBuilder.Build<WithArray>();
                var actual = format.Read(stream);

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void CanReadListMember()
        {
            using (var stream = new MemoryStream())
            {
                WithStruct value1 = new WithStruct(161, 321, 641);
                WithStruct value2 = new WithStruct(162, 322, 642);
                WithStruct value3 = new WithStruct(163, 323, 643);
                WithList<WithStruct> expected = new WithList<WithStruct>()
                {
                    Values = new List<WithStruct>() { value1, value2, value3 }
                };
                using (var bw = new BinaryWriter(stream, Encoding.Default, true))
                {
                    expected.WriteTo(bw);
                }
                stream.Position = 0;

                var formatBuilder = Bin.Format()
                    .Includes<WithList<WithStruct>>(cfg => cfg.Read(t => t.Values, lcfg => lcfg.LastElementWhen(c => c.Index > 2)))
                    .Includes<WithStruct>();

                var format = formatBuilder.Build<WithList<WithStruct>>();
                var actual = format.Read(stream);

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
