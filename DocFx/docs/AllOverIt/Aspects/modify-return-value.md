# Aspects - Modify Return Value
---

Consider these methods on `ISecretService`:

```csharp
string GetSecret(string accessKey);
Task<string> GetSecretAsync(string accessKey);
```

Imagine a scenario where you want to ensure the returned result is always lower-cased. This can be achieved by overriding the `AfterInvoke()` method like so:

Below are some possible class-level and method-level interceptor implementations for `GetSecret(string accessKey)`.

```csharp
// An example class-level interceptor that implements InterceptorBase<ISecretService>.
protected override void AfterInvoke(
    MethodInfo targetMethod,
    object[] args,
    InterceptorState state,
    ref object result)
{
    var secret = (string) result;

    result = secret.ToLowerInvariant();
}
```

```csharp
// An example method-level interceptor that implements InterceptorHandlerBase<string>.
protected override string AfterInvoke(
    MethodInfo targetMethod,
    object[] args,
    InterceptorState state,
    string result)
{
    var secret = (string) result;

    result = secret.ToLowerInvariant();
}
```

Below are some possible class-level and method-level interceptor implementations for `GetSecretAsync(string accessKey)`.

```csharp
// An example class-level interceptor that implements InterceptorBase<ISecretService>.
protected override void AfterInvoke(
    MethodInfo targetMethod,
    object[] args,
    InterceptorState state,
    ref object result)
{
    var secretTask = (Task<string>) result;

    // This is safe. The AfterInvoke() method will not be called if the Task result
    // is in a faulted state.
    var secret = secretTask.Result.ToLowerInvariant();

    // GetSecretAsync() returns a Task<string> so it must be returned accordingly.
    result = Task.FromResult(secret);
}
```

```csharp
// An example method-level interceptor that implements InterceptorHandlerBase<string>.
protected override Task<string> AfterInvoke(
    MethodInfo targetMethod,
    object[] args,
    InterceptorState state,
    Task<string> result)
{
    var secretTask = (Task<string>) result;

    // This is safe. The AfterInvoke() method will not be called if the Task result
    // is in a faulted state.
    var secret = secretTask.Result.ToLowerInvariant();

    // GetSecretAsync() returns a Task<string> so it must be returned accordingly.
    return Task.FromResult(secret);
}
```
