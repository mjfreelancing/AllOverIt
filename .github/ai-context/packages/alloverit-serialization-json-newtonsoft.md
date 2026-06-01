# AllOverIt.Serialization.Json.Newtonsoft

## Purpose
This package contains a wrapper for Newtonsoft.Json serialization based on AllOverIt.Serialization.Json.Abstractions.

## Package Metadata
- Project: Source/AllOverIt.Serialization.Json.Newtonsoft/AllOverIt.Serialization.Json.Newtonsoft.csproj
- Target frameworks: net10.0;net9.0;net8.0
- Tags: AllOverIt, Utilities, Serialization, Deserialization, JSON, Newtonsoft
- Depends on: AllOverIt.Serialization.Json.Abstractions

## Public API Signals (from source)
### Representative public types
- class DateTimeAsUtcConverter
- class EnrichedEnumJsonConverter
- class EnrichedEnumJsonConverterFactory
- class InterfaceConverter
- class JsonConverterFactory
- class JsonHelper
- class NewtonsoftJsonSerializer
- class NewtonsoftJsonSerializerExtensions

### Representative extension methods
- AddConverters

### Active namespaces
- AllOverIt.Serialization.Json.Newtonsoft.Converters
- AllOverIt.Serialization.Json.Newtonsoft
- AllOverIt.Serialization.Json.Newtonsoft.Extensions

## Demo Projects
- Demos/AllOverIt.Serialization.Json.Newtonsoft/InterfaceDeserializationDemo/InterfaceDeserializationDemo.csproj

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
