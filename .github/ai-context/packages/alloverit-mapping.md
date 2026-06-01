# AllOverIt.Mapping

## Purpose
This package contains an object mapper that is mostly configuration free but also capable of property exclusion, aliasing, null replacement, value conversion, and deep copying.

## Package Metadata
- Project: Source/AllOverIt.Mapping/AllOverIt.Mapping.csproj
- Target frameworks: net10.0;net9.0;net8.0
- Tags: AllOverIt, Utilities, Mapping
- Depends on: AllOverIt

## Public API Signals (from source)
### Representative public types
- class ObjectMapper
- class ObjectMapperConfiguration
- class ObjectMapperException
- class ObjectMapperExtensions
- class PropertyMatcherOptions
- class TypedPropertyMatcherOptions
- interface IObjectMapper
- interface IObjectMapperOptions

### Active namespaces
- AllOverIt.Mapping
- AllOverIt.Mapping.Exceptions
- AllOverIt.Mapping.Extensions

## Demo Projects
- Demos/AllOverIt.Mapping/DtoMappingDemo/DtoMappingDemo.csproj

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
