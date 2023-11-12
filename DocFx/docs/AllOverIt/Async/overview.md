# Async
---

Includes utilities such as......


## AsyncLazy<T>
an async version of `Lazy<T>`


## BackgroundTask
an awaitable background task that can be disposed to cancel the operation


## CompositeAsyncDisposable
a wrapper over multiple `IAsyncDisposable` instances that can be collectively disposed of (synchronously or asynchronously)


## RepeatingTask
a static factory that creates a task to repeatedly invoke an asynchronous action with delay options until cancelled


## TaskHelper
provides the ability to await for tasks that return different types and have their results available as a tuple


