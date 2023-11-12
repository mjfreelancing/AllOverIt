# Aspects - Overview
---

Aspect-Oriented Programming (AOP) addresses the challenge of managing "cross-cutting concerns" of your application like logging, security, or error handling. AOP introduces the concept of an "aspect" which is a modular unit specifically designed to handle one of these cross-cutting concerns.

The implementation provided by this library is limited to intercepting methods on classes that inherit from an interface as it uses [DispatchProxy](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.dispatchproxy/) under the hood. Interception is based on reflection so it's important to consider any performance concerns. This approach can be useful, however, when you need to intercept methods on a class that you don't have access to due to coming from a third-party library.

Consider the following `interface` representing a service that returns a 'secret value':

```csharp
public interface ISecretService
{
	string GetSecret();
	Task<string> GetSecretAsync();
}
```

And here is an example implementation:

```csharp
internal sealed class SecretService : ISecretService
{
    // Assuming SecurityManager is a static provider defined elsewhere.
    public string GetSecret()
    {
        return SecurityManager.GetSecret();
    }

    public Task<string> GetSecretAsync()
    {
        return SecurityManager.GetSecretAsync();
    }
}
```

There are two approaches to intercepting method calls:
- Intercept all methods by implementing an interceptor that inherits `InterceptorBase<TService>`.
- Filter the methods to be intercepted by implementing one or more method intercept handlers that inherit `MethodInterceptor<TService>`.


## Class Interceptors
For scenarios that require appying the same logic to all methods on a class, create an interceptor that inherits from `InterceptorBase<TService>`, such as:

```csharp
internal class TimedInterceptor : InterceptorBase<ISecretService>
{
}
```

`InterceptorBase<TService>` is an abstract class with virtual methods that can be overriden to invoke custom code before or after each method on the interface, or if a method faults (throws an `Exception`). An example implementation is shown below.

```csharp
internal class TimedInterceptor : InterceptorBase<ISecretService>
{
    protected override InterceptorState BeforeInvoke(
        MethodInfo targetMethod,
        ref object[] args,
        ref object result)
    {
        // Custom code here

        return new InterceptorState.None;
    }

    protected override void AfterInvoke(
        MethodInfo targetMethod,
        object[] args,
        InterceptorState state,
        ref object result)
    {
        // Custom code here
    }

    protected override void Faulted(
        MethodInfo targetMethod,
        object[] args,
        InterceptorState state,
        Exception exception)
    {
        // Custom code here
    }
}
```

The `BeforeInvoke()` method returns an implementation of `InterceptorState` that is later provided to the `AfterInvoke()` method. If no state is required, use the static `InterceptorState.None` value.

Let's update this interceptor so it determines how long each method call takes to execute.

```csharp
internal class TimedInterceptor : InterceptorBase<ISecretService>
{
    private sealed class TimedState : InterceptorState
    {
        public Stopwatch Stopwatch { get; } = Stopwatch.StartNew();
    }

    protected override InterceptorState BeforeInvoke(
        MethodInfo targetMethod,
        ref object[] args,
        ref object result)
    {
        return new TimedState();
    }

    protected override void AfterInvoke(
        MethodInfo targetMethod,
        object[] args,
        InterceptorState state,
        ref object result)
    {
        var timedState = state as TimedState;
        var elapsed = timedState.Stopwatch.ElapsedMilliseconds;
        
        // Do something here...
    }
}
```

In this example, a new `TimedState` instance is created before every method is called and it is provided to `AfterInvoke()` when the decorated method returns.

> [!NOTE]
> The custom interceptor cannot be sealed and it must have a default constructor.


## Method Interceptor Handlers
It isn't always desirable to use a single interceptor for a given class.
- You may only want to intercept a subset of the methods available on the class.
- Trying to apply a strategy within `BeforeInvoke()` and `AfterInvoke()` can become quite difficult to manage over time.
- Using a different interceptor for each method is preferred as it takes on the "Single Responsibility Principle". Your code will not only be cleaner but it will also be easier to test.

Class interceptors implement `InterceptorBase<TService>` but these will have their  `BeforeInvoke()` and `AfterInvoke()` methods called for every method that is invoked on the class being decorated. Method filtering is achieved by instead using `MethodInterceptor<TService>` and registering one or more method handlers.

Consider the scenario where you want to register a handler per method on this `ISecretService` interface:

```csharp
public interface ISecretService
{
    string GetSecret(string accessKey);
    Task<string> GetSecretAsync(string accessKey);
}
```

A handler can be created for each of these methods. Synchronous (non-async) method handlers must inherit `InterceptorHandlerBase<TResult>` and asynchronous method handlers must inherit `InterceptorHandlerAsyncBase<TResult>`.

Each handler then overrides the `TargetMethods` property to indicate which method, or methods, the handler should be invoked for.

The handler for `GetSecret()` may look something like this:

```csharp
internal sealed class GetSecretHandler : InterceptorHandlerBase<string>
{
    private readonly ICache _cache;

    // This handler will be invoked when ISecretService.GetSecret() is called.
    public override MethodInfo[] TargetMethods { get; }
        = [typeof(ISecretService).GetMethod(nameof(ISecretService.GetSecret))];

    public GetSecretHandler(ICache cache)
    {
        _cache = cache;
    }

    protected override InterceptorState BeforeInvoke(
        MethodInfo targetMethod,
        ref object[] args,
        ref string result)
    {
        var accessKey = (string) args[0];

        // Use _cache to see if there is a suitable value to assign to 'result',
        // otherwise just return.

        return InterceptorState.None;
    }

    protected override string AfterInvoke(
        MethodInfo targetMethod,
        object[] args,
        InterceptorState state,
        string result)
    {
        var accessKey = (string) args[0];

        // Do something with the input argument(s) or final result.

        return result;
    }
}
```

Note the `BeforeInvoke()` and `AfterInvoke()` methods are strongly typed to the `string` return type.

The handler for `GetSecretAsync()` may look something like this:

```csharp
internal sealed class GetSecretAsyncHandler : InterceptorHandlerAsyncBase<string>
{
    private readonly ICache _cache;

    // This handler will be invoked when ISecretService.GetSecretAsync() is called.
    public override MethodInfo[] TargetMethods { get; }
        = [typeof(ISecretService).GetMethod(nameof(ISecretService.GetSecretAsync))];

    public GetSecretAsyncHandler(ICache cache)
    {
        _cache = cache;
    }

    protected override InterceptorState BeforeInvoke(
        MethodInfo targetMethod,
        ref object[] args,
        ref Task<string> result)
    {
        var accessKey = (string) args[0];

        // Use _cache to see if there is a suitable value to assign to 'result',
        // otherwise just return.

        return InterceptorState.None;
    }

    protected override Task<string> AfterInvoke(
        MethodInfo targetMethod,
        object[] args,
        InterceptorState state,
        Task<string> result)
    {
        var accessKey = (string) args[0];

        // Using Task.Result is safe because AfterInvoke() is only called if the
        // Task completed successfully.
        var finalResult = result.Result;

        // Do something with the input argument(s) or final result.

        // The result must be returned as a Task<TResult>.
        return Task.FromResult(finalResult);
    }
}
```

Note the `BeforeInvoke()` and `AfterInvoke()` methods are strongly typed to the `Task<string>` return type.
