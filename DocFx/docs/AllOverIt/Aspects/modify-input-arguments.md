# Aspects - Modify Input Arguments
---

Consider this method on `ISecretService`:

```csharp
string GetSecret(string accessKey);
```

Imagine a scenario where the service retrieves the secret from an external store where the access keys must be lower-cased and you want to intercept this method to ensure the argument is modified before the external store is accessed. To achieve this, the argument can be modified as shown in the following examples.

An example for a class-level interceptor implementing `InterceptorBase<ISecretService>`:

```csharp
protected override InterceptorState BeforeInvoke(
    MethodInfo targetMethod,
    object[] args,
    ref object result)
{
    // The 'accessKey' passed to GetSecret() is at index 0 (it is the first argument).
    var accessKey = (string) args[0];

    // This will mutate the argument before sending it to the decorated service.
    // Note: The mutation is limited to the service call. The argument sent by the caller
    //       will remain unmodified.
    args[0] = accessKey.ToLowerInvariant();

    // Remainder of implementation here...
}
```

An example for a method-level interceptor implementing `InterceptorHandlerBase<string>`:

```csharp
protected override InterceptorState BeforeInvoke(
    MethodInfo targetMethod,
    object[] args,
    ref string result)
{
    // The 'accessKey' passed to GetSecret() is at index 0 (it is the first argument).
    var accessKey = (string) args[0];

    // This will mutate the argument before sending it to the decorated service.
    // Note: The mutation is limited to the service call. The argument sent by the caller
    //       will remain unmodified.
    args[0] = accessKey.ToLowerInvariant();

    // Remainder of implementation here...
}
```

In both cases, the `accessKey` argument provided to `GetSecret()` will now be lower-cased before being sent to the decorated service.
