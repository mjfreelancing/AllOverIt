# Tasks
---

## AsyncLazy\<TType>

```csharp
public class AsyncLazy<TType> : Lazy<Task<TType>>
{
  public AsyncLazy(Func<TType> factory);
  public AsyncLazy(Func<Task<TType>> factory);

  public TaskAwaiter<TType> GetAwaiter();
}
```

Provides support for lazy initialization using a factory that returns a `Task<TType>`.

`AsyncLazy` can be constructed using either a `Func<TType>` or a `Func<Task<TType>>`. The `GetAwaiter()` method allows you to get the result by awaiting the `AsyncLazy<TType>` instance without the need to reference the `Value` property as shown in the following example:

```csharp
var lazyPreferences = new AsyncLazy<Preferences>(() => GetPreferences());

// ...

var preferences = await lazyPreferences;
```

## RepeatingTask

```csharp
public static Task Start(Func<Task> action, CancellationToken cancellationToken,
  int repeatDelay, int initialDelay = 0);

public static Task Start(Action action, CancellationToken cancellationToken,
  int repeatDelay, int initialDelay = 0);
```
The `RepeatingTask.Start()` methods provide the ability to execute a repeating, long running, action or awaitable task in the background.

Each overload provides the ability to wait for an initial period before the first invocation, followed by a repeating delay between subsequent invocations.

## TaskHelper

### WhenAll

```csharp
public static async Task<(T1, T2)> WhenAll<T1, T2>(Task<T1> task1, Task<T2> task2,
  bool continueOnCapturedContext = false);
```
Overloads of `WhenAll` are available to cater for up to seven tasks. They all await for all tasks to complete and return the results as a Tuple, as shown in the following example:

```csharp
var (preferences, catalog) = await TaskHelper.WhenAll(
  GetPreferencesAsync(),
  GetCatalogAsync()
);
```
