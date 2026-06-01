# AllOverIt

## Purpose
This package contains a diverse range of classes and utility functions, intended to tackle a wide array of commonly encountered issues. Key areas of concern include threading, tasks, generic caching, event (message) aggregation, expression and reflection utilities, plugin contexts, enriched enums, pipelines, value objects, specifications, and dozens of extensions.

## Package Metadata
- Project: Source/AllOverIt/AllOverIt.csproj
- Target frameworks: net10.0;net9.0;net8.0
- Tags: AllOverIt, Utilities, Async, Threading, Diagnostics, Patterns, IO, Reflection, Process, Collections, Caching, Conversions
- Depends on: AllOverIt.Assertion

## Public API Signals (from source)
### Representative public types
- record GenericCacheKey
- class EnumerableExtensions
- class ReflectionCache
- interface IAsyncCommand
- interface ICommand
- class AsyncCommandPipeline
- class BackgroundTask
- class CommandPipeline
- class EndianExtensions
- class EnrichedError
- class EnrichedResult
- class FieldHelper

### Representative extension methods
- Add
- CreateType
- GetFieldInfo
- GetPropertyInfo
- GetValue
- IsEqualTo
- IsZero
- WithArguments
- WithEnvironmentVariables
- WithTimeout
- AddCallSite
- AddExtendedCallSite

### Active namespaces
- AllOverIt.Extensions
- AllOverIt.Patterns.Specification
- AllOverIt.Collections
- AllOverIt.Diagnostics.Breadcrumbs
- AllOverIt.Formatters.Objects
- AllOverIt.Reflection
- AllOverIt.Events
- AllOverIt.Async
- AllOverIt.Patterns.Pipeline
- AllOverIt.Process

## Demo Projects
- Demos/AllOverIt/Aspects/InterceptorDemo/InterceptorDemo.csproj
- Demos/AllOverIt/Async/BackgroundTaskDemo/BackgroundTaskDemo.csproj
- Demos/AllOverIt/Async/RepeatingTaskDemo/RepeatingTaskDemo.csproj
- Demos/AllOverIt/Caching/GenericCacheDemo/GenericCacheDemo.csproj
- Demos/AllOverIt/Diagnostics/DiagnosticsDemo/DiagnosticsDemo.csproj
- Demos/AllOverIt/Events/EventAggregatorDemo/EventAggregatorDemo.csproj
- Demos/AllOverIt/Formatters/SerializeObjectPropertiesDemo/SerializeObjectPropertiesDemo.csproj
- Demos/AllOverIt/Helpers/PropertyNavigationDemo/PropertyNavigationDemo.csproj
- Demos/AllOverIt/IO/DirectorySearchDemo/DirectorySearchDemo.csproj
- Demos/AllOverIt/IO/FileSearchDemo/FileSearchDemo.csproj
- Demos/AllOverIt/Parallel/SelectAsParallelDemo/SelectAsParallelDemo.csproj
- Demos/AllOverIt/Patterns/ChainOfResponsibilityAsyncDemo/ChainOfResponsibilityAsyncDemo.csproj
- Demos/AllOverIt/Patterns/ChainOfResponsibilityDemo/ChainOfResponsibilityDemo.csproj
- Demos/AllOverIt/Patterns/CommandDemo/CommandDemo.csproj
- Demos/AllOverIt/Patterns/EnumerationDemo/EnumerationDemo.csproj
- Demos/AllOverIt/Patterns/LinqSpecificationDemo/LinqSpecificationDemo.csproj
- Demos/AllOverIt/Patterns/PipelineAsyncDemo/PipelineAsyncDemo.csproj
- Demos/AllOverIt/Patterns/PipelineDemo/PipelineDemo.csproj
- Demos/AllOverIt/Patterns/PipelineParallelDemo/PipelineParallelDemo.csproj
- Demos/AllOverIt/Patterns/ResultOrErrorDemo/ResultOrErrorDemo.csproj
- Demos/AllOverIt/Patterns/SpecificationDemo/SpecificationDemo.csproj
- Demos/AllOverIt/Patterns/ValueObjectDemo/ValueObjectDemo.csproj
- Demos/AllOverIt/Plugin/StartupHook/GCCollectorStartupHook/GCCollectorStartupHook.csproj
- Demos/AllOverIt/Plugin/StartupHook/GCCollectorStartupHookProxy/GCCollectorStartupHookProxy.csproj
- Demos/AllOverIt/Plugin/StartupHook/StartupHookDemo/StartupHookDemo.csproj
- Demos/AllOverIt/Process/ExecuteProcessAndWaitDemo/ExecuteProcessAndWaitDemo.csproj

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
