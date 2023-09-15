## TypeExtensions

Property and method metadata can be obtained for a `Type` using various reflection methods. It can be difficult, however, to easily extract that information when you need to filter down to specific requirements. `AllOverIt` includes several methods to simplify the process.

The following class is used in the examples below:

```csharp
public class PersonBase
{
  protected int Id { get; set; }
}

public class Person : PersonBase
{
  public string FirstName { get; set; }
  public string Surname { get; set; }
}
```

### GetPropertyInfo()

```csharp
public static PropertyInfo GetPropertyInfo(this Type type, string propertyName);

public static IEnumerable<PropertyInfo> GetPropertyInfo(this Type type,
  BindingOptions binding = BindingOptions.Default, bool declaredOnly = false);
```

Getting `PropertyInfo` for public or protected properties, by name, is as simple as:

```csharp
var firstNameInfo = typeof(Person).GetPropertyInfo("FirstName");
var surnameInfo = typeof(Person).GetPropertyInfo("Surname");

// assuming 'subject' is of type 'Person'
var firstName = firstNameInfo.GetValue(subject);
var surname = surnameInfo.GetValue(subject);
```

To get the `PropertyInfo` of select properties, use any of the following:

```csharp
// all public and protected properties
var allInfo = typeof(Person).GetPropertyInfo();

// all properties, including internal and private
var allInfo = typeof(Person).GetPropertyInfo(BindingOptions.All);

// all properties, including internal and private, of the declared class only
// (will not return Id)
var allInfo = typeof(Person).GetPropertyInfo(BindingOptions.All, true);
```

### GetMethodInfo()

```csharp
public static MethodInfo GetMethodInfo(this Type type, string name);

public static MethodInfo GetMethodInfo(this Type type, string name, Type[] types);

public static IEnumerable<MethodInfo> GetMethodInfo(this Type type,
  BindingOptions binding = BindingOptions.Default,
  bool declaredOnly = false);
```

Getting `MethodInfo` for any static, instance, public, or non-public method can be obtained by name and, where required, by a list of argument types.

```csharp
// get MethodInfo for StringBuilder.Clear()
var methodInfo = typeof(StringBuilder).GetMethodInfo("Clear");

// get MethodInfo for the StringBuilder.AppendFormat() overload:
// public StringBuilder AppendFormat(string format, params object[] args);
var methodInfo = typeof(StringBuilder).GetMethodInfo(
  "AppendFormat",
  new[] {typeof(string), typeof(object[])});
```

The same functionality is also available in the `ReflectionHelper` namespace.  Refer to [Obtain Method Information](../reflection.md#obtain-method-information) for more information.