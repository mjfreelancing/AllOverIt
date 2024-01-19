# Async - Overview
---

The `Async` namespace includes the following utilities:

## [AsyncLazy](asynclazy.md)
Similar to how  `Lazy<TType>` provides lazy initialization of a type, `TType`, via a factory, the `AsyncLazy<TType>` provides an asynchronous version.


## [BackgroundTask](backgroundtask.md)
Provides support for executing an action asynchronously in an awaitable background task with automatic cancellation when disposed.


## [CompositeAsyncDisposable](compositeasyncdisposable.md)
A wrapper over multiple `IAsyncDisposable` instances that can be collectively disposed of (synchronously or asynchronously).


## [RepeatingTask](repeatingtask.md)
Provides static factory methods to create a cancellable task that repeatedly invokes an asynchronous action with initial and interval delay options.


## [TaskHelper](taskhelper.md)
Provides the ability to await for tasks that return different types and have their results available as a tuple.
