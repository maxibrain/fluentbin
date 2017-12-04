# fluentbin
FluentBin is a library for reading binary files. 

## PURPOSE

Develop an universal library for reading binary formatted files (streams). Avoid reflection at the run-time. Support bit-by-bit reading. Have a nice API for declaring file format specification.

## WHAT DOES IT SUPPORT AT THE MOMENT

1) Reading object tree that consists of:
  - Value types (Int32, Byte, Double, Boolean, etc.)
  - Complex types (other objects)
  - Strings
  - Fixed and variable sized arrays
  - Lists
2) Big-endian and little-endian byte order
3) Bit-by-bit reading
4) Polymorphic members
5) Various configuration for object members
 

## HOW DOES IT WORK

The main interfaces that an API client deals with are:

- `IFileFormatBuilder` – a file format configuration builder.
- `IFileFormat<T>` – represents compiled expression tree for reading the file format configured with IFileFormatBuilder.

Basic usage involving those types:

```c#
IFileFormatBuilder formatBuilder = Bin.Format()…; // file format configuration follows
IFileFormat<T> format = formatBuilder.Build<T>(); // builds expression tree for reading T
T instance = format.Read(inputStream);
```
 

## FILE FORMAT CONFIGURATION

Any file format may be mapped to an object tree. All objects types must be included into file format configuration with the `IFileFormatBuilder.Includes` method:

```c#
IFileFormatBuilder.Includes<T>(Action<ITypeBuilder<T>> typeBuilderConfiguration = null)
```

`ITypeBuilder<T>` interface is used to configure included type members. Not all members must be mentioned in the configuration. Some may be configured automatically. ITypeBuilder<T> interface has several methods:

- `ITypeBuilder<T>.Read(member, memberConfiguration)` – overloaded. Use it to configure a T member format.
- `ITypeBuilder<T>.Skip(member, memberConfiguration)` – a T member can be read from file but it is not needed so this member is to be skipped.
- `ITypeBuilder<T>.Ignore(member)` – a T member that should be ignored while reading.
- `ITypeBuilder<T>.OverrideEndianess(endianness)` – makes entire object tree starting from T to use passed endianness.

`member` argument in listed above methods is actually a `Expression<Func<T, TMember>>` that must be a `MemberExpression`;

`memberConfiguration` is an `Action<memberBuilder>`, where `memberBuilder` is an interface inherited from `IMemberBuilder<T>`:

`IMemberBuilder<T, TMember>`, has the following methods:
- `UseFactory(Expression<Func<IContext<T>, TMember>> expression)` – factory method;
- `SizeOf(BinarySize sizeInBytes)` – static size of member;
- `SizeOf(Expression<Func<IContext<T>, BinarySize>> expression)` – dynamic size of member;
- `If(Expression<Func<IContext<T>, Boolean>> expression)` – conditional reading;
- `ConvertValue(Expression<Func<IMemberContext<T, TMember>, TMember>> expression)` – value conversion after read;
- `Assert(Expression<Func<IMemberContext<T, TMember>, Boolean>> expression)` – value assertion, throws BinaryReadingAssertException if fails;
- `AfterRead(Expression<Action<IMemberContext<T, TMember>>> expression)` – an action to perform after read;
- `SetOrder(Int32 order)` – order of the member in T;
- `Position(Expression<Func<IContext<T>, BinaryOffset>> expression)` – stream position where to start reading;
- `PositionAfter(Expression<Func<IMemberContext<T, TMember>, BinaryOffset>> expression)` – stream position where to set pointer after reading;
- `OverrideEndianess(Endianness endianness)` – read member with different endianness;
- `StringEncoding(Encoding encoding)` – static string encoding (may be used only for string member);
- `StringEncoding(Expression<Func<IContext<T>, Encoding>> encodingExpression)` – dynamic string encoding (may be used only for string member);

`ICollectionMemberBuilder<T, TMember, TElement>`, inherits from `IMemberBuilder<T, TMember>`, has additionally a method for configuring its elements:
- `Element(Action<IMemberBuilder<TMember, TElement>> elementBuilderConfiguration)`

`IArrayMemberBuilder<T, TElement>`, inherits from `ICollectionMemberBuilder<T, TMember, TElement>`, has additionally a couple of methods for configuration:
- `Length(UInt64 length)` – fixed sized array;
- `Length(Expression<Func<IContext<T>, UInt64>> expression)` – variable sized array;

`IListMemberBuilder<T, TElement>`, inherits from `ICollectionMemberBuilder<T, TMember, TElement>`, has additionally the following method:
- `LastElementWhen(Expression<Func<ICollectionMemberContext<T, IList<TElement>>, Boolean>> expression)` – declares a function for determining the last element in the list.

## PROS & CONS

Pros:
+ bit size level data
+ polymorphic member support
+ fluent api
+ expression trees. no reflection

Cons:
- no native support for stream files (however there are several workarounds)
