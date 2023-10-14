# Aspects - How To

This section offers general information on how to use the method interceptors for a variety of scenarios.

The following `ISecretService` interface represents a service returning a 'secret value' and will be used for each of the provided examples:

```csharp
public interface ISecretService
{
	string GetSecret(string accessKey);
	Task<string> GetSecretAsync(string accessKey);
}
```

And here is an example implementation:

```csharp
public sealed class SecretService : ISecretService
{
    public string GetSecret(string accessKey)
    {
        return $"{accessKey}-{Guid.NewGuid()}";
    }

    public Task<string> GetSecretAsync(string accessKey)
    {
        // Some async method implemented elsewhere
        return SecurityManager.GetSecretAsync(string accessKey);
    }
}
```

And a modified version of this interceptor will be used with several examples.

```csharp
internal class TimedInterceptor : InterceptorBase<ISecretService>
{
    public long? MinimimReportableMilliseconds { get; set; }

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


## Using Dependency Injection

As described on the [Aspects](Aspects.md) page, you can manually create a proxy for the real service that needs intercepting, like so:

```csharp
var secretService = new SecretService();

var proxy = InterceptorFactory.CreateInterceptor<ISecretService, TimedInterceptor>(secretService);

var secret = proxy.GetSecret();
```

The `CreateInterceptor()` call is essentially creating a decorated version of the `ISecretService` instance. When using dependency injection, this decoration is performed like so:

```csharp
// The IServiceCollection provided by your DI setup
var services = new ServiceCollection();

services
    // Typical registration of the required service - this could have been performed
    // by a third-party library. The key point is that ISecretService is registered
    // somewhere with the DI container.
    .AddScoped<ISecretService, SecretService>()

    // And now register an interceptor as a decorator of the ISecretService
    .DecorateWithInterceptor<ISecretService, TimedInterceptor>(inteceptor =>
    {
        // If required, apply any configuration to your custom interceptor
        // using property or method injection.
        inteceptor.MinimimReportableMilliseconds = 1000;
    });

// At some point later, you'll have access to an IServiceProvider
var serviceProvider = services.BuildServiceProvider();

// When the ISecretService is resolved, the proxied version (TimedInterceptor) will be returned
var proxy = serviceProvider.GetRequiredService<ISecretService>();

// Make calls on the ISecretService as you would normally
var secret = proxy.GetSecret();
```

The `proxy.GetSecret()` call will result in the following sequence:

* `TimedInterceptor.BeforeInvoke()`
* `SecretService.GetSecret()`
* `TimedInterceptor.AfterInvoke()`

> [!NOTE]
> The custom interceptor cannot be sealed and it must have a default constructor. If your interceptor requires additional dependencies they will need to be provided via property or method injection rather than constructor injection, as shown in the above example.


## Modifying input arguments
Consider this method on `ISecretService`:

```csharp
string GetSecret(string accessKey);
```

Imagine a scenario where the service retrieves the secret from an external store where the access keys must be lower-cased and you want to intercept this method to ensure the argument is modified before the external store is accessed. To achieve this, the argument can be modified like so:

```csharp
protected override InterceptorState BeforeInvoke(
    MethodInfo targetMethod,
    object[] args,
    ref object result)
{
    // The 'accessKey' passed to GetSecret() is at index 0.
    var accessKey = (string) args[0];

    // This will mutate the argument before sending it to the decorated service.
    // Note: The mutation is limited to the service call. The argument sent by the caller
    //       will remain unmodified.
    args[0] = accessKey.ToLowerInvariant();

    // Remainder of implementation here...
}
```

The `accessKey` argument provided to `GetSecret()` will now be lower-cased before being sent to the decorated service.


## Modifying return values
Consider this method on `ISecretService`:

```csharp
string GetSecret(string accessKey);
```

Imagine a scenario where you want to ensure the returned result is always lower-cased. This can be achieved by overriding the `AfterInvoke()` method like so:

```csharp
// Implementation for intercepting a call to GetSecret():
protected override void AfterInvoke(
    MethodInfo targetMethod,
    object[] args,
    InterceptorState state,
    ref object result)
{
    var secret = (string) result;

    result = secret.ToLowerInvariant();

    // Remainder of implementation here...
}

// Implementation for intercepting a call to GetSecretAsync():
protected override void AfterInvoke(
    MethodInfo targetMethod,
    object[] args,
    InterceptorState state,
    ref object result)
{
    var secretTask = (Task<string>) result;

    // This is safe. The AfterInvoke() method will not be called if the Task result is in a faulted state.
    var secret = secretTask.Result.ToLowerInvariant();

    result = Task.FromResult(secret);

    // Remainder of implementation here...
}
```




## Returning a handled result from the interceptor
- BeforeInvoke() - without calling the decorated service
- AfterInvoke() - after calling the decorated service






## Modifying return values of async methods






