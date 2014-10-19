using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentBin.Mapping.Builders;
using FluentBin.Mapping.Builders.Impl;

namespace FluentBin.Mapping
{
    public static class Bin
    {
        public static IFileFormatBuilder Format(Endianness endianness = Endianness.LittleEndian)
        {
            return new FileFormatBuilder(endianness);
        }
    }
}
