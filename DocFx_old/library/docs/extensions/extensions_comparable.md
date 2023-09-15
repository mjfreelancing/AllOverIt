## ComparableExtensions
The `IComparable` interface allows for one object to be custom compared to another. `AllOverIt` contains the following extension methods:

```csharp
LessThan<TType>()
LessThanOrEqual<TType>()
GreaterThan<TType>()
GreaterThanOrEqual<TType>()
EqualTo<TType>()
NotEqualTo<TType>()
```

The following example shows how to compare two `Person` objects based on their _Surname_ and _FirstName_.

```csharp
public class Person : IComparable<Person>
{
  public string FirstName { get; }
  public string Surname { get; }

  public Person(string firstName, string surname)
  {
    FirstName = firstName;
    Surname = surname;
  }

  public int CompareTo(Person other)
  {
    // compare by surname, then first name, case insensitively
    var surnameComparison = CompareName(Surname, other.Surname);

    return surnameComparison != 0
      ? surnameComparison
      : CompareName(FirstName, other.FirstName);
  }

  private static int CompareName(string lhs, string rhs)
  {
    return string.Compare(lhs, rhs, StringComparison.InvariantCultureIgnoreCase);
  }
}
```

And this is how it can be used within a LINQ query using the extension methods available in `AllOverIt`.

```csharp
var persons = new[]
{
  new Person("Adam", "Baker"),
  new Person("Adam", "Murphy"),
  new Person("Paul", "Nielson"),
  new Person("Jon", "Nielson")
};

// custom comparer will consider them in the following order:
//   Baker, Adam
//   Murphy, Adam
//   Nielson, Jon
//   Nielson, Paul

var comparisonPerson = new Person("Amy", "Myer");

var actual = persons.Where(
  item => item.LessThan(comparisonPerson)
).AsReadOnlyList();

actual.Should().BeEquivalentTo(
  new Person("Adam", "Baker"),
  new Person("Adam", "Murphy")
);
```