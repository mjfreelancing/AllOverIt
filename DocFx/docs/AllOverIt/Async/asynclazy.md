# AsyncLazy
---

`AsyncLazy<TType>` is similar to `Lazy<TType>` in that it invokes a factory method, but asynchronously, to return a `TType` when its' `Value` property is awaited. All subsequent accesses to `Value` will then return the cached result.

Assume you have this code to create a `HttpClient`:

```csharp
private static HttpClient GetHttpClient()
{
    return new()
    {
        BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
    };
}
```

And you use this with a  `Lazy<HttpClient>` to create a single instance on first access:

```csharp
var httpClientFactory = new Lazy<HttpClient>(GetHttpClient);
```

You can then use an `AsyncLazy<string>` to call this endpoint exactly once, like so:

```csharp
var todoResult = new AsyncLazy<string>(async () =>
{
    // Invoke the 'httpClientFactory' to get a new (or existing) HttpClient instance.
    var httpClient = httpClientFactory.Value;

    using (var response = await httpClient.GetAsync("todos/1"))
    {
        return await response.Content.ReadAsStringAsync();
    }
});

var result = await todoResult;
```

> [!NOTE]
> * `todoResult.Value` returns a `Task<string>`
> * `await todoResult.Value` will await and return the `string` value
> * `await todoResult` behaves identically to `await todoResult.Value`