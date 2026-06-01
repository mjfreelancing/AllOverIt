# AllOverIt.AspNetCore

## Purpose
This package provides useful ASP.NET Core utilities such as custom model binding for AllOverIt based enriched enum types and arrays of values in a query string.

## Package Metadata
- Project: Source/AllOverIt.AspNetCore/AllOverIt.AspNetCore.csproj
- Target frameworks: net10.0;net9.0;net8.0
- Tags: AllOverIt, Utilities, AspNetCore
- Depends on: AllOverIt

## Public API Signals (from source)
### Representative public types
- class EnrichedEnumModelBinder
- class EnrichedEnumModelBinderProvider
- class GuidArray
- class IntegerArray
- class MvcOptionsExtensions
- class StringArray
- class ValueArray
- class ValueArrayModelBinder
- class ValueArrayModelBinderBase

### Representative extension methods
- AddEnrichedEnumModelBinder

### Active namespaces
- AllOverIt.AspNetCore.ValueArray
- AllOverIt.AspNetCore.ModelBinders
- AllOverIt.AspNetCore.Extensions

## Demo Projects
- Demos/AllOverIt.AspNetCore/EnrichedEnumModelBindingDemo/EnrichedEnumModelBindingDemo.csproj

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
