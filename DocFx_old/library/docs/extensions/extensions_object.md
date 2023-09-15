## ObjectExtensions

### GetPropertyValue()

```csharp
public static TValue GetPropertyValue<TValue>(this object instance, string propertyName,
  BindingFlags bindingFlags );

public static TValue GetPropertyValue<TValue>(this object instance, string propertyName,
  BindingOptions bindingOptions = BindingOptions.Default);
```

Both versions of `GetPropertyValue()` use reflection to get the value of an object's property by name. The first version uses .NET's `BindingFlags` and the second version uses `BindingOptions` found in `AllOverIt`.

```csharp
public class Request
{
  public string Token { get; set; }
}

// using BindingFlags
var token = request.GetPropertyValue<string>(nameof(Request.Token),
  BindingFlags.Instance | BindingFlags.Public);

// using BindingOptions (the default is BindingOptions.Default)
var token = request.GetPropertyValue<string>(nameof(Request.Token));
```

### SetPropertyValue()

```csharp
public static void SetPropertyValue<TValue>(this object instance, string propertyName,
  TValue value, BindingFlags bindingFlags);

public static void SetPropertyValue<TValue>(this object instance, string propertyName,
  TValue value, BindingOptions bindingOptions = BindingOptions.Default);
```

Both versions of `SetPropertyValue()` use reflection to set the value of an object's property by name. The first version uses .NET's `BindingFlags` and the second version uses `BindingOptions` found in `AllOverIt`.

```csharp
public class Request
{
  public string Token { get; set; }
}

// using BindingFlags
request.SetPropertyValue<string>(nameof(Request.Token), "token_value",
  BindingFlags.Instance | BindingFlags.Public);

// using BindingOptions (the default is BindingOptions.Default)
request.GetPropertyValue<string>(nameof(Request.Token), "token_value");
```

### ToPropertyDictionary()

```csharp
public static IDictionary<string, object> ToPropertyDictionary(this object instance,
  bool includeNulls = false, BindingOptions bindingOptions = BindingOptions.Default);
```

Use this method to convert any object to a dictionary of property key names mapped to their value. The default options exclude _null_ properties and includes properties that are:

* Static or instance scope
* Abstract, virtual, or non-virtual access
* Public or protected visibility

### As\<TType>()

```csharp
public static TType As<TType>(this object instance, TType defaultValue = default);
```

This method will attempt to convert an object to another type. The following conversions are attempted (in the order shown):

* If _null_, returns `defaultValue`

* If `TType` is
  - the same type as `instance`, or
  - `TType` is `object`, or
  - `TType` is a class type, but not a `string`, then

  The original reference is returned, casted to a `TType`.

* If `TType` is a `Boolean` and `instance` is an integral type (`byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`) then a zero value will be returned as False, and a value of one will be returned as True. All other values will throw an `ArgumentOutOfRangeException` exception.

* If `instance` is an integral value and `TType` is an `Enum` then a conversion will be attempted. If the conversion cannot be performed then an `ArgumentOutOfRangeException` exception will be thrown.

* If any of the following are true:
  - `TType` is a `Boolean`
  - `instance` is a `Boolean`
  - `TType` is a `char`
  - `instance` is a `char`

  Then an attempt to convert `instance` to `TType` will be performed using `Convert.ChangeType()`.

* In all other cases, `instance` will be converted to a `string` (using string interpolation) and passed to `StringExtensions.As<TType>()` for conversion.

### AsNullable\<TType>

```csharp
public static TType? AsNullable<TType>(this object instance, TType? defaultValue = null)
  where TType : struct;
```

This method behaves identical to `As<TType>()`.

### CalculateHashCode()
There are two overloads, each of which are described below.

```csharp
public static int CalculateHashCode<TType>(this TType instance,
  IEnumerable<string> includeProperties = null,
  IEnumerable<string> excludeProperties = null);
```

This overload uses reflection to find all instance properties and calculate a hash code. The main advantage of using reflection is that it will automatically include properties of all access (abstract, virtual, and non-virtual) and visibility (public, protected, and private). Furthermore, this overload satisfies OCP (Open-Closed Principle) because the hash code will automatically adjust the calculations if properties are added or removed.

The `includeProperties` and `excludeProperties` options exist to provide finer control over which properties are considered for calculating the hash code.

* When `includeProperties` is _null_ then all properties are included, otherwise only those specified are considered.

* When `excludeProperties` is _null_ then no properties are excluded, otherwise the specified properties are excluded.

```csharp
public static int CalculateHashCode<TType>(this TType instance,
  params Func<TType, object>[] propertyResolvers);
```

This overload calculates the hash code based on explicitly specified properties, fields, or the return result from a method call. If properties are added to the class then you must remember to later include those for them to be considered during the hash code calculation.