using System;
using System.Reflection.Emit;
using FluentBin.Mapping.Models;

namespace FluentBin.Mapping.Builders
{
    public interface IFileFormatBuilder
    {
        IFileFormatBuilder Includes<T>(Action<ITypeBuilder<T>> typeBuilderConfiguration = null);
        IFileFormat<T> Build<T>();
        void BuildToMethod<T>(MethodBuilder methodBuilder);
    }
}