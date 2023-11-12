# Aspects - Instantiating Interceptors
---

This section describes how to instantiate class and method level interceptors, with and without using dependency injection.

## Class Interceptors
Assume this `TimedInterceptor` is used to decorate an implementation of `ISecretService`.

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

This interceptor can be instantiated explicitly or resolved via dependency injection as described next.

### Without dependency injection
To decorate a service with a class interceptor called `TimedInterceptor`, the proxy is created like so:

```csharp
var secretService = new SecretService();

var proxy = InterceptorFactory.CreateInterceptor<ISecretService, TimedInterceptor>(
    // The service instabce being decorated.
    secretService,

    // Configure the interceptor.
    interceptor =>
    {
        interceptor.MinimimReportableMilliseconds = 100;
    });

var secret = proxy.GetSecret("secret_key");
```


### Using dependency injection
The example below shows registering an `ISecretService` with a `ServiceCollection` and then decorating it with the `TimedInterceptor` interceptor.

```csharp
// The IServiceCollection provided by your DI setup
var services = new ServiceCollection();

services
    // Typical registration of the required service - this could have been performed
    // by a third-party library. The key point is that ISecretService is registered
    // somewhere with the DI container.
    .AddScoped<ISecretService, SecretService>()

    // And now register an interceptor as a decorator of the ISecretService
    .DecorateWithInterceptor<ISecretService, TimedInterceptor>((provider, interceptor) =>
    {
        // If required, apply any configuration to your custom interceptor
        // using property or method injection.
        inteceptor.MinimimReportableMilliseconds = 1000;
    });

// At some point later, you'll have access to an IServiceProvider
var serviceProvider = services.BuildServiceProvider();

// When the ISecretService is resolved, either explicitly as shown here or implicitly
// when injected into another class instance, the proxied version (TimedInterceptor)
// will be returned.
var proxy = serviceProvider.GetRequiredService<ISecretService>();

// Make calls on the ISecretService as you would normally.
var secret = proxy.GetSecret();
```


> [!NOTE]
> Since custom interceptors cannot be sealed and must have a default constructor, if your interceptor requires additional dependencies they will need to be provided via property or method injection rather than constructor injection, as shown in the above example.


## Method Interceptor Handlers
Assume this method-level interceptor handler is used to intercept the `GetSecret()` method on `ISecretService`.

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


### Without dependency injection
To decorate a service with a method interceptor called `GetSecretHandler`, the proxy is created like so:

```csharp
var secretService = new SecretService();

var proxy = InterceptorFactory.CreateInterceptor<ISecretService, MethodInterceptor<ISecretService>>(
    // The service instabce being decorated.
    secretService,

    // Configure the interceptor.
    interceptor =>
    {
        // Instantiate the method handler and add it to the interceptor.
        interceptor.AddMethodHandler(new GetSecretHandler(1000, true));
    });

var secret = proxy.GetSecret("secret_key");
```


### Using dependency injection
Registration involves decorating the service with `MethodInterceptor<TService>` and adding the required handlers, as shown in the following examples:

```csharp
// Option 1: When you need to manually construct the handlers because you need to
//           pass arguments that cannot be resolved from the container.
var services = new ServiceCollection();

// Typical registration of the required service - this could have been performed
// by a third-party library. The key point is that ISecretService is registered
// somewhere with the DI container.
services.AddScoped<ISecretService, SecretService>();

services.DecorateWithInterceptor<ISecretService, MethodInterceptor<ISecretService>>(
    (provider, interceptor) =>
    {
        // Cache.Global represents a static ICache that hasn't been
        // registered with the service collection.
        interceptor
            .AddMethodHandler(new GetSecretHandler(Cache.Global))
            .AddMethodHandler(new GetSecretAsyncHandler(Cache.Global));
    });
```

```csharp
// Option 2: When you can resolve the handlers from the container.
var services = new ServiceCollection();

// Typical registration of the required service - this could have been performed
// by a third-party library. The key point is that ISecretService is registered
// somewhere with the DI container.
services.AddScoped<ISecretService, SecretService>();

services.AddSingleton<ICache>(Cache.Global);
services.AddSingleton<GetSecretHandler>();
services.AddSingleton<GetSecretAsyncHandler>();

services.DecorateWithInterceptor<ISecretService, MethodInterceptor<ISecretService>>(
    (provider, interceptor) =>
    {
        var secretHandler = provider.GetRequiredService<GetSecretHandler>();
        interceptor.AddMethodHandler(secretHandler);

        var secretAsyncHandler = provider.GetRequiredService<GetSecretAsyncHandler>();
        interceptor.AddMethodHandler(secretAsyncHandler);
    });
```


## Sequence Of Calls
In all examples, the call to `proxy.GetSecret()`  will result in the following sequence:

* `TimedInterceptor.BeforeInvoke()`
* `SecretService.GetSecret()`
* `TimedInterceptor.AfterInvoke()`