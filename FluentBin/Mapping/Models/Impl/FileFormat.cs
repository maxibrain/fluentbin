using System;
using System.IO;
using System.Linq.Expressions;
using FluentBin.Mapping.Builders;
using FluentBin.Mapping.Builders.Impl;

namespace FluentBin.Mapping.Models.Impl
{
    class FileFormat<T> : IFileFormat<T>, IFileFormat
    {
        private readonly ReadDelegate<T> _readFunc;

        public FileFormat(ReadDelegate<T> readFunc)
        {
            _readFunc = readFunc;
        }

        T IFileFormat<T>.Read(Stream stream)
        {
            using (var br = new BitsReader(stream))
            {
                return _readFunc(br);
            }
        }

        object IFileFormat.Read(Stream stream)
        {
            return ((IFileFormat<T>) this).Read(stream);
        }
    }
}