# AllOverIt.Serialization.Json.SystemText

## Purpose
This package contains a wrapper for System.Text.Json serialization based on AllOverIt.Serialization.Json.Abstractions.

## Package Metadata
- Project: Source/AllOverIt.Serialization.Json.SystemText/AllOverIt.Serialization.Json.SystemText.csproj
- Target frameworks: net10.0;net9.0;net8.0
- Tags: AllOverIt, Utilities, Serialization, Deserialization, JSON, SystemText
- Depends on: AllOverIt.Serialization.Json.Abstractions

## Public API Signals (from source)
### Representative public types
- class DateTimeAsUtcConverter
- class EnrichedEnumJsonConverter
- class EnrichedEnumJsonConverterFactory
- class InterfaceConverter
- class JsonHelper
- class NestedDictionaryConverterOptions
- class SystemTextJsonSerializer
- class SystemTextJsonSerializerExtensions

### Representative extension methods
- AddConverters

### Active namespaces
- AllOverIt.Serialization.Json.SystemText.Converters
- AllOverIt.Serialization.Json.SystemText
- AllOverIt.Serialization.Json.SystemText.Extensions

## Demo Projects
- Demos/AllOverIt.Serialization.Json.SystemText/InterfaceDeserializationDemo/InterfaceDeserializationDemo.csproj

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
