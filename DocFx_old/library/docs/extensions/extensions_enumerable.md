## EnumerableExtensions

### AsList(), AsReadOnlyList(), AsReadOnlyCollection()

```csharp
public static IList<TType> AsList<TType>(
  this IEnumerable<TType> items);

public static IReadOnlyList<TType> AsReadOnlyList<TType>(
  this IEnumerable<TType> items);

public static IReadOnlyCollection<TType> AsReadOnlyCollection<TType>(
  this IEnumerable<TType> items);
```

When working with an Enumerable there is a real concern associated with potentially enumerating it more than once. To overcome this, a commonly used approach is to convert the `IEnumerable<TType>` to a `List<TType>` by using the `ToList()` method.

If the `items` being enumerated is already a list, then calling `ToList()` will create a second list. This can quickly become a memory and performance issue. The `AsList()`, `AsReadOnlyList()`, and `AsReadOnlyCollection()` methods help solve this by determining if a new collection actually needs to be created.

In all cases, if the `IEnumerable<TType>` is already a list or read-only collection then the same reference is returned, otherwise a new collection is created.

### IsNullOrEmpty()

```csharp
public static bool IsNullOrEmpty(this IEnumerable items);
```

Applicable to collections and strings, this method returns _true_ if the instance is _null_ or empty, otherwise false.

### Batch
The `Batch()` method provides the ability to partition data into a group of smaller collections.

```csharp
// partitions items into batches of 100 and processes each batch in parallel
foreach (var batch in items.Batch(100).AsParallel())
{
  ProcessBatch(batch);
}

private void ProcessBatch(IEnumerable<string> items)
{
  foreach (var item in items)
  {
    // process an individual item
  }
}
```
