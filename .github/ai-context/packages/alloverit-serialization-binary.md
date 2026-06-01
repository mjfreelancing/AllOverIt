# AllOverIt.Serialization.Binary

## Purpose
This package provides enhanced support for binary serialization. Complex object graphs, lists, dictionaries and nullable values can all be dynamically serialized using reflection-based readers and writers or, for greater performance, a custom serializer per object within the graph.

## Package Metadata
- Project: Source/AllOverIt.Serialization.Binary/AllOverIt.Serialization.Binary.csproj
- Target frameworks: net10.0;net9.0;net8.0
- Tags: AllOverIt, Utilities, Binary, Serialization, Deserialization
- Depends on: AllOverIt

## Public API Signals (from source)
### Representative public types
- class EnrichedBinaryValueReader
- class EnrichedBinaryValueWriter
- class BinaryReaderException
- class BinaryWriterException
- class DynamicBinaryValueReader
- class DynamicBinaryValueWriter
- class EnrichedBinaryReader
- class EnrichedBinaryReaderExtensions
- class EnrichedBinaryValueReaderExtensions
- class EnrichedBinaryValueWriterExtensions
- class EnrichedBinaryWriter
- class EnrichedBinaryWriterExtensions

### Representative extension methods
- WriteBytes
- WriteChars
- WriteDictionary
- WriteEnumerable
- ReadArray
- ReadDateOnly
- ReadDateTime
- ReadDictionary
- ReadEnum
- ReadEnumerable
- ReadGuid
- ReadSafeString

### Active namespaces
- AllOverIt.Serialization.Binary.Readers
- AllOverIt.Serialization.Binary.Writers
- AllOverIt.Serialization.Binary.Exceptions
- AllOverIt.Serialization.Binary.Readers.Extensions
- AllOverIt.Serialization.Binary.Writers.Extensions
- AllOverIt.Serialization.Binary

## Demo Projects
- Demos/AllOverIt.Serialization.Binary/BinarySerializationDemo/BinarySerializationDemo.csproj

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
