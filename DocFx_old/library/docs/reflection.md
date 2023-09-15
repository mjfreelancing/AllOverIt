# Reflection
---
Reflection provides the ability to dynamically create instances of a type, get the type of an existing object and invoke its methods or access its properties. **AllOverIt** simplifies the access to an object's set of properties and methods by using a set of binding options.

## Binding Options
To simplify the explanation of using reflection binding options, consider the following class:

```csharp
public class SomeClass
{
  public int Prop1 { get; set; }
  private int Prop2 { get; set; }
  internal int Prop3 { get; set; }
  internal int Prop4 { get; private set; }
  public static bool Prop5 { get; set; }
}
```

This class contains a number of properaties that exhibit three categories of binding options:

| Option | Description |
| ------ | ----------- |
| Scope | Refers to whether a property or method is static or instance. |
| Accessor | Refers to whether a property or method is virtual, non-virtual, or abstract. |
| Visibility | Refers to whether a property or method is public, protected, private, or internal. |

When querying for properties and methods the binding must include at least one option from each of the three categories. Where a category option has not been provided then the following defaults will be applied:

| Option | Default Applied |
| ------ | --------------- |
| Scope | Static or instance |
| Accessor | Virtual, non-virtual, or abstract. |
| Visibility | Public or protected. |

There are also a number of options defined that provide often-used combinations:

| Option | Default Applied |
| ------ | --------------- |
| DefaultScope | Static or instance |
| DefaultAccessor | Virtual, non-virtual, or abstract. |
| DefaultVisibility | Public or protected. |
| AllScope | Static or instance |
| AllAccessor | Virtual, non-virtual, or abstract. |
| AllVisibility | Public, protected, private or internal. |
| All | Combines **AllScope**, **AllAccessor**, and **AllVisibility** |

## Obtain Property Information
`ProperytInfo` can be obtained from an object using any of the following:

1. The static `ReflectionHelper.GetPropertyInfo()` generic methods
2. The `GetPropertyInfo()` extension method of `Type`
3. The `GetPropertyInfo()` extension method of `TypeInfo`

The following example demonstrates how to obtain property metadata based on a class type.

```
public class Person
{
  private string FullName { get; set; }
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public int Age { get; set; }
}

// obtains all property information using the default binding options
var allInfo = ReflectionHelper.GetPropertyInfo<Person>();

// obtains property information for 'FirstName' (uses default binding options)
var firstNameInfo = ReflectionHelper.GetPropertyInfo<Person>("FirstName");

// obtains property information for the private property called 'FullName'
var fullNameInfo = ReflectionHelper
  .GetPropertyInfo<Person>(BindingOptions.Private)
  .Single(item => item.Name == "FullName");
```

The following example demonstrates how to achieve property information from a class instance using the *Type* extension method:

```
var person = new Person();

var ageInfo = person.GetType().GetPropertyInfo("Age");
```
## Obtain Method Information

`MethodInfo` can be obtained from an object using any of the following:

1. The static `ReflectionHelper.GetMethodInfo()` generic methods
2. The `GetMethodInfo()` extension methods of `Type`

The following example demonstrates how to obtain method metadata based on a class type.

```csharp
public class Person
{
  public string FirstName { get; set; }
  public string LastName { get; set; }

  public string GetFullName() => $"{FirstName} {LastName}";
}

// obtains method metadata for 'GetFullName' using default binding options
var fullNameInfo = ReflectionHelper
  .GetMethodInfo<Person>()
  .Single(item => item.Name == "GetFullName");

// when the method takes no arguments the same can be achieved using the following overload:
var fullNameInfo = ReflectionHelper.GetMethodInfo<Person>("GetFullName");
```

For cases where several method overloads exist on a class then the following alternative can be used to find the required `MethodInfo` by specifying the list of argument types:

```csharp
public static MethodInfo GetMethodInfo<TType>(string name, Type[] types);
```

As an example, `StringBuilder` has several `AppendFormat()` methods. To obtain the `MethodInfo` for this overload:

```csharp
public StringBuilder AppendFormat(string format, params object[] args);
```

Use the following code:

```csharp
var methodInfo = typeof(StringBuilder).GetMethodInfo(
  "AppendFormat",
  new[] {typeof(string), typeof(object[])});
```

## Convert to a `Dictionary<string, object>`
Property names and their values can be converted to a dictionary as shown in the following example:

```csharp
public class Person
{
  public string FirstName { get; set; }
  public string MiddleName { get; set; }
  public string LastName { get; set; }
}

var person = new Person
{
  FirstName = "Malcolm",
  LastName = "Smith"
};

// contains FirstName and LastName
// excludes null values and uses default binding options
var nonNullProperties = person.ToPropertyDictionary();

// contains FirstName, MiddleName and LastName
// includes null values and uses default binding options
var allProperties = person.ToPropertyDictionary(true);
```
