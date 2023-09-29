# Aspects
---
Aspect-Oriented Programming (AOP) addresses the challenge of managing "cross-cutting concerns" of your application, like logging, security, or error handling. AOP introduces the concept of an "aspect" which is a modular unit specifically designed to handle one of these cross-cutting concerns.

The implementation provided by this library is limited to intercepting methods on classes that inherit from an interface as it uses `DispatchProxy` under the hood. The method interception is based on reflection so it's important to consider any performance concerns. This approach can be useful, however, when you need to intercept methods on a class that you don't have access to due to coming from a third-party library.

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
public sealed class SecretService : ISecretService
{
    public string GetSecret()
    {
        return Guid.NewGuid().ToString();
    }

    public Task<string> GetSecretAsync()
    {
        // Some async method implemented elsewhere
        return SecurityManager.GetSecretAsync();
    }
}
```

## Intercepting method calls

To intercept methods on this class, begin by creating a new class that inherits from `InterceptorBase<ISecretService>`:

```csharp
internal class TimedInterceptor : InterceptorBase<ISecretService>
{
}
```

`InterceptorBase<T>` is an abstract class with virtual methods that can be used to invoke custom code before or after each method on the interface, or if a method faults (throws an `Exception`).

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

The `BeforeInvoke()` method returns an implementation of `InterceptorState` that is provided to the `AfterInvoke()` method. If no state is required, use the static `InterceptorState.None` value.

Let's update this interceptor so it determines how long each method call takes to execute.

```csharp
internal class TimedInterceptor : InterceptorBase<ISecretService>
{
    private class TimedState : InterceptorState
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

To use the interceptor, create a proxy like so:

```csharp
var secretService = new SecretService();

var proxy = InterceptorFactory.CreateInterceptor<ISecretService, TimedInterceptor>(secretService);

var secret = proxy.GetSecret();
```

The `proxy.GetSecret()` call will result in the following sequence:

* `TimedInterceptor.BeforeInvoke()`
* `SecretService.GetSecret()`
* `TimedInterceptor.AfterInvoke()`

## How To

Refer to the [How To](./HowTo.md) section for additional usage examples.