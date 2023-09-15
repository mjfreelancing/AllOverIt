## DoubleExtensions

Comparing double values is problematic due to rounding issues. `AllOverIt` provides extension methods to help compare a `double` with another, or zero. The built-in tolerance is defined as `1E-7`. Overloads exist that allow you to provide your own tolerance.

```csharp
public static bool IsZero(this double value);
public static bool IsZero(this double value, double tolerance);
```
Compares a `double` value to zero, within a given tolerance.

```csharp
public static bool IsEqualTo(this double lhs, double rhs);
public static bool IsEqualTo(this double lhs, double rhs, double tolerance);
```
Compares to `double` values with each other, within a given tolerance.