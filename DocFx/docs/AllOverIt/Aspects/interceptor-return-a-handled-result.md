# Aspects - Return A Handled Result
---

A common use case for interceptors is to provide a level of caching in order to improve performance. Class and method-level interceptors support this via the `ref object result` argument on the `BeforeInvoke()` method.

```csharp
InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref object result)
```

When this method is called, `result` will be `null` since the decorated service hasn't yet been called. If the concrete interceptor sets this to an alternate value then that will be considered as a 'handled result', meaning this result will be immediately provided to the `AfterInvoke()` method and ultimately returned to the caller. The method will not be called on the decorated service.

> [!NOTE]
> If the intercepted method can be expected to return `null` in certain circumtances and you need to differentiate between unhandled and an actual `null` result then one approach would be to use a value object (wrapper) that represents the actual value. This way a `null` value can be interpreted as unhandled while a non-null object containing a `null` result can be interpreted as a `null` value that is returned to the caller.
>
> Refer to [ValueObject](../Patterns/ValueObject/overview.md) for a possible approach to creating a suitable value object.

Consider this method on `ISecretService`:

```csharp
string GetSecret(string accessKey);
```

The `BeforeInvoke()` method can be used to determine if a cached result is available, like so:

```csharp
protected override InterceptorState BeforeInvoke(
    MethodInfo targetMethod,
    object[] args,
    ref object result)
{
    // The 'accessKey' passed to GetSecret() is at index 0 (it is the first argument).
    var accessKey = (string) args[0];

    if (_cache.TryGetValue(out var cacheResult))
    {
        // Setting 'result' to a non-null value will be interpreted as a 'handled result' so the
        // decorated service will not be called. The AfterInvoke() method will still be called.
        result = cacheResult;
    }
    
    return InterceptorState.None;
}
```
