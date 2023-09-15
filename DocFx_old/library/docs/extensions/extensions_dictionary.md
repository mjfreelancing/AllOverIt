## DictionaryExtensions

### GetValueOrDefault()

```csharp
public static TValue GetValueOrDefault<TKey, TValue>(
  this IDictionary<TKey, TValue> dictionary, TKey key,
  TValue defaultValue = default)
```

This extension method returns the value associated with a key if found, otherwise it returns a (optional) default value.

```csharp
// assuming managers is of type Dictionary<int, Person>

// id is an integer identifier to look up
var person = managers.GetValueOrDefault(id);
```

If found, the associated `Person` instance will be returned, otherwise _null_.

```csharp
// id is an integer identifier to look up
// manager is an instance of a Person object
var person = managers.GetValueOrDefault(id, manager);
```

If found, the associated `Person` instance will be returned, otherwise _manager_ will be. The _managers_ dictionary is not mutated if the key is not found.

### GetOrSet()

```csharp
public static TValue GetOrSet<TKey, TValue>(
  this IDictionary<TKey, TValue> dictionary, TKey key,
  Func<TValue> valueCreator)
```

This extension method returns the value associated with a key if found, otherwise it invokes _valueCreator_ to get the value that should be added to the dictionary, and then returned to the caller.

```csharp
// id is an integer identifier to look up
// manager is an instance of a Person object
var person = managers.GetValueOrDefault(id, () => manager);
```

If found, the associated `Person` instance will be returned, otherwise _manager_ will be returned after adding it to the dictionary. The _managers_ dictionary is mutated if the key is not found.