using System.IO;

namespace FluentBin.Tests.Model
{
    interface IWritable
    {
        void WriteTo(BinaryWriter bw);
    }
}