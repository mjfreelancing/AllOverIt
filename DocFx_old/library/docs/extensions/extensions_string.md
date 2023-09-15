## StringExtensions

### As\<TType>() and AsNullable\<TType>()

```csharp
public static TType As<TType>(this string value, TType defaultValue = default,
  bool ignoreCase = true);

public static TType? AsNullable<TType>(this string value, bool ignoreCase = true)
  where TType : struct;
```

Similar to methods of the same name in the `objectextensions` namespace, these methods attempt to convert a string to another type, typically Boolean, Integral, or Enum types.

Supported conversions include byte, sbyte, decimal, double, float, int, uint, long, ulong, short, ushort, string, boolean and enum.

Char conversions must be performed using the `ObjectExtensions.As<TType>()` method.

No attempt is made to avoid overflow or argument exceptions.

Other conversions are possible if a suitable `TypeConverter` is available.

```csharp
// each will return a Boolean True (a variable would
// normally be in the place of the string)
var result = "true".As<bool>();
var result = "TRUE".As<bool>();
var result = "TrUe".As<bool>();
var result = "1".As<bool>();

// each will return NumberStyles.Integer (a variable would
// normally be in the place of the string)
var result = "7".As<NumberStyles>();
var result = "integer".As<NumberStyles>();
var result = "Integer".As<NumberStyles>();
var result = "INTEGER".As<NumberStyles>();
```
