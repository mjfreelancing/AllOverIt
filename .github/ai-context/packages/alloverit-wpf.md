# AllOverIt.Wpf

## Purpose
This package contains general purpose WPF utilities.

## Package Metadata
- Project: Source/AllOverIt.Wpf/AllOverIt.Wpf.csproj
- Target frameworks: net10.0-windows;net9.0-windows10.0.19041;net9.0-windows10.0.17763;net8.0-windows10.0.19041;net8.0-windows10.0.17763
- Depends on: AllOverIt

## Public API Signals (from source)
### Representative public types
- class DispatcherExtensions
- class SynchronizationContextExtensions
- class UIThread
- class UIThreadAwaiter
- class WindowExtensions
- class WindowWrapper

### Representative extension methods
- BindTo
- GetAwaiter
- WrapWindow

### Active namespaces
- AllOverIt.Wpf.Threading
- AllOverIt.Wpf.Threading.Extensions
- AllOverIt.Wpf.Extensions
- AllOverIt.Wpf.Utils

## Demo Projects
- Demos/AllOverIt.Wpf/ThreadBindingDemo/ThreadBindingDemo.csproj
- Demos/AllOverIt.Wpf/WindowWrapperDemo/WindowWrapperDemo.csproj

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
