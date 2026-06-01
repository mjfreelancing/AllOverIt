# AllOverIt.Pipes

## Purpose
This package provides support for communication over anonymous pipes using StreamReader and StreamWriter, and strongly-typed message-based communication over named pipes using a binary serializer. The named pipes can use the binary serializer from AllOverIt.Serialization.Binary or you can create your own.

## Package Metadata
- Project: Source/AllOverIt.Pipes/AllOverIt.Pipes.csproj
- Target frameworks: net10.0-windows;net9.0-windows10.0.19041;net9.0-windows10.0.17763;net8.0-windows10.0.19041;net8.0-windows10.0.17763
- Tags: AllOverIt, Utilities, Async, Threading, IO, Pipes, NamedPipes
- Depends on: AllOverIt.Reactive, AllOverIt.Serialization.Binary

## Public API Signals (from source)
### Representative public types
- class AnonymousPipeBase
- class AnonymousPipeClient
- class AnonymousPipeServer
- class NamedPipeClient
- class NamedPipeClientFactory
- class NamedPipeConnectionEventArgs
- class NamedPipeConnectionExceptionEventArgs
- class NamedPipeConnectionMessageEventArgs
- class NamedPipeExceptionEventArgs
- class NamedPipeSerializer
- class NamedPipeServer
- class NamedPipeServerFactory

### Representative extension methods
- AddIdentityAccessRule

### Active namespaces
- AllOverIt.Pipes.Named.Client
- AllOverIt.Pipes.Named.Server
- AllOverIt.Pipes.Named.Connection
- AllOverIt.Pipes.Named.Events
- AllOverIt.Pipes.Anonymous
- AllOverIt.Pipes.Named.Serialization
- AllOverIt.Pipes.Exceptions
- AllOverIt.Pipes.Named.Extensions

## Demo Projects
- Demos/AllOverIt.Pipes/Anonymous/AnonymousPipeClientDemo/AnonymousPipeClientDemo.csproj
- Demos/AllOverIt.Pipes/Anonymous/AnonymousPipeServerDemo/AnonymousPipeServerDemo.csproj
- Demos/AllOverIt.Pipes/NamedPipes/NamedPipeClientDemo/NamedPipeClientDemo.csproj
- Demos/AllOverIt.Pipes/NamedPipes/NamedPipeServerDemo/NamedPipeServerDemo.csproj
- Demos/AllOverIt.Pipes/NamedPipes/NamedPipeTypes/NamedPipeTypes.csproj

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
