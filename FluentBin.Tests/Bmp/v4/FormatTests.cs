using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinaryIO.Mapping;
using BinaryIO.Mapping.Builders;
using NUnit.Framework;

namespace BinaryIO.Tests.Bmp.v4
{
    [TestFixture]
    public class FormatTests
    {
        private IFileFormatBuilder formatBuilder;

        [SetUp]
        public void SetUp()
        {
            formatBuilder = Bin.Format(Endianness.BigEndian)
                .Includes<>()
        }

        [Test]
        public void CanRead()
        {
            
        }
    }
}
