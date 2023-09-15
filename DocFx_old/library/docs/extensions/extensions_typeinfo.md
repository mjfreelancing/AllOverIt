## TypeInfoExtensions

```csharp
public static IEnumerable<PropertyInfo> GetPropertyInfo(this TypeInfo typeInfo,
  bool declaredOnly = false);

public static PropertyInfo GetPropertyInfo(this TypeInfo typeInfo, string propertyName);
```


These methods are identical to those of the same name in the `TypeExtensions` namespace, except they extend `TypeInfo` instead of `Type`. Refer to [TypeExtensions](#typeextensions) for more information.