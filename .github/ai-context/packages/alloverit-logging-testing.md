# AllOverIt.Logging.Testing

## Purpose
This package contains helper methods that assist with asserting logger calls.

## Package Metadata
- Project: Source/AllOverIt.Logging.Testing/AllOverIt.Logging.Testing.csproj
- Target frameworks: net10.0;net9.0;net8.0
- Tags: AllOverIt, Utilities, Testing, Logging
- Depends on: AllOverIt.Logging

## Public API Signals (from source)
### Representative public types
- class LogCallExpectation
- class LoggerExtensions
- class MethodCallContext
- class MethodCallsWithArgumentsExtensions
- record Item

### Representative extension methods
- AssertMessageWithArgumentsEntry
- AssertExceptionLogEntry
- AssertExceptionWithArgumentsLogEntry
- AssertMessageEntry
- AssertStaticLogCall
- AssertStaticLogCallAsync
- AssertStaticLogCallEntry
- AssertStaticLogCallWithArguments
- AssertStaticLogCallWithArgumentsAsync
- AssertStaticLogCallWithArgumentsEntry
- CaptureLogCalls
- CaptureLogCallsAsync

### Active namespaces
- AllOverIt.Logging.Testing
- AllOverIt.Logging.Testing.Extensions

## Demo Projects
- No package-specific demo folder found under Demos/AllOverIt.Logging.Testing

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
