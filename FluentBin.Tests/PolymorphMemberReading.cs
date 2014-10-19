using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentBin.Mapping;
using FluentBin.Mapping.Contexts;
using FluentBin.Tests.Model;
using NUnit.Framework;

namespace FluentBin.Tests
{
    [TestFixture]
    public class PolymorphMemberReading
    {
        [Test]
        public void CanReadPolymorphMember()
        {
            using (var stream = new MemoryStream())
            {
                var expected = new WithPolymorph()
                {
                    Value = new WithArray() { FixedLegthArray = new[] { new WithStruct(161, 321, 641) }, VarLength = 0, VarLegthArray = new WithStruct[0] },
                    Values = new IWritable[]
                        {
                            new WithStruct(162, 322, 642), 
                            new WithClass() { Int16Value = 163, Value = new WithStruct(164, 324, 644) }
                        }
                };
                using (var bw = new BinaryWriter(stream, Encoding.Default, true))
                {
                    expected.WriteTo(bw);
                }
                stream.Position = 0;

                Func<IContext<IWritable[]>, IWritable> factory = context =>
                {
                    switch (context.Object.Count(w => w != null))
                    {
                        case 0:
                            return new WithStruct();
                        case 1:
                            return new WithClass();
                        default:
                            throw new ArgumentOutOfRangeException("context");
                    }
                };

                var formatBuilder = Bin.Format()
                    .Includes<WithPolymorph>(
                        cfg =>
                        cfg.Read(c => c.Values, mcfg => mcfg.Length(2).Element(e => e.UseFactory(c => factory(c))))
                            .Read(c => c.Value, mcfg => mcfg.UseFactory(context => new WithArray())))
                    .Includes<WithArray>(
                        cfg => cfg.Read(c => c.FixedLegthArray, acfg => acfg.Length(1))
                                   .Read(c => c.VarLegthArray, acfg => acfg.Length(c => c.Object.VarLength)))
                    .Includes<WithStruct>()
                    .Includes<WithClass>();

                /*
                                AssemblyName an = new AssemblyName();
                                an.Name = "ReadWithFluentBin";
                                AppDomain ad = AppDomain.CurrentDomain;
                                AssemblyBuilder ab = ad.DefineDynamicAssembly(an, AssemblyBuilderAccess.Save);
                                ModuleBuilder mb = ab.DefineDynamicModule(an.Name, "ReadWithFluentBin.dll");
                                foreach (var type in new [] { typeof(WithClass), typeof(WithStruct), typeof(WithStructInherited) })
                                {
                                    TypeBuilder.
                                    var tb = mb.DefineType(type.FullName, type.Attributes);
                                    tb.DefineConstructor()
                                }
                                TypeBuilder readerTypeBuilder = mb.DefineType("FluentBin.Reader", TypeAttributes.Public | TypeAttributes.Class);
                                MethodBuilder fb = readerTypeBuilder.DefineMethod("Read",
                                                                   MethodAttributes.Public |
                                                                   MethodAttributes.Static,
                                                                   typeof(WithClass), new Type[] { typeof(BinaryReader) });
                                formatBuilder.BuildToMethod<WithClass>(fb);
                                ab.Save("ReadWithFluentBin.dll");
                */

                var format = formatBuilder.Build<WithPolymorph>();
                var actual = format.Read(stream);

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
