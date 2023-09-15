## PropertyInfoExtensions

Properties can be declared as abstract, virtual, or static, as well as public, protected, private, or internal. Extension methods exist for each of these cases:

```csharp
public static bool IsAbstract(this PropertyInfo propertyInfo);
public static bool IsVirtual(this PropertyInfo propertyInfo);
public static bool IsStatic(this PropertyInfo propertyInfo);
public static bool IsPublic(this PropertyInfo propertyInfo);
public static bool IsProtected(this PropertyInfo propertyInfo);
public static bool IsPrivate(this PropertyInfo propertyInfo);
public static bool IsInternal(this PropertyInfo propertyInfo);
```

These extensions prove to be quite useful when used in conjunction with the `TypeInfo` extensions or the various methods within `ReflectionHelper` that return `PropertyInfo`. For example:

```csharp
public class Person 
{
  public string FirstName { get; set; }
  public string Surname { get; set; }
  public int Age { get; set; }
  public string FullName => $"{FirstName} {Surname}";
}
```

Assume `subject` in the following snippet is an instance of `Person`:

```csharp
var properties = new[] { "FirstName", "Surname", "Age", "FullName" };

// Using reflection, this will output the value of each property on `subject`
foreach (var property in properties)
{
  // GetPropertyInfo() is an extension method in TypeInfoExtensions
  var info = typeof(Person).GetPropertyInfo(property);
  var value = info.GetValue(subject);

  Console.WriteLine($"{property} = {value}");
}
```

The same `info` can be used to check the visibility of a property:

```csharp
var isPublic = info.IsPublic();        // true
var isProtected = info.IsProtected();  // false
var isPrivate = info.IsPrivate();      // false
var isInternal = info.IsInternal();    // false
```

Or accessibility:

```csharp
var isAbstract = info.IsAbstract();  // false
var isVirtual = info.IsVirtual();    // false
var isStatic = info.IsStatic();      // false
```