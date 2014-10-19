using System;
using System.IO;

namespace FluentBin.Mapping.Models
{
    public interface IFileFormat
    {
        object Read(Stream stream);
    }

    public interface IFileFormat<out T>
    {
        T Read(Stream stream);
    }
}