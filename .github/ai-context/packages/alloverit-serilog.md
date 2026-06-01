# AllOverIt.Serilog

## Purpose
This package contains general-purpose Serilog sinks and enrichers.

## Package Metadata
- Project: Source/AllOverIt.Serilog/AllOverIt.Serilog.csproj
- Target frameworks: net10.0;net9.0;net8.0
- Tags: AllOverIt, Utilities, Serilog, Logging, Sink, Enricher
- Depends on: AllOverIt

## Public API Signals (from source)
### Representative public types
- class LoggerSinkConfigurationExtensions
- class ServiceCollectionExtensions
- class CircularBufferSink
- class CircularBufferSinkMessage
- class CircularBufferSinkMessages
- class ObservableSink
- class ServiceProviderExtensions
- class ThreadIdEnricher
- interface ICircularBufferSinkMessages
- interface IObservableSink

### Representative extension methods
- GetCircularBufferSinkMessages
- GetObservableSink

### Active namespaces
- AllOverIt.Serilog.Extensions
- AllOverIt.Serilog.Sinks.CircularBuffer
- AllOverIt.Serilog.Sinks.Observable
- AllOverIt.Serilog.Enrichers.ThreadId

## Demo Projects
- Demos/AllOverIt.Serilog/CircularBufferSinkDemo/CircularBufferSinkDemo.csproj
- Demos/AllOverIt.Serilog/ObservableSinkDemo/ObservableSinkDemo.csproj

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
