# Expressions
---

## PredicateBuilder
The following is an example of a lambda expression in the form of a delegate:

```csharp
Func<Product, bool> isPromoted = product => product.Promoted;
```

And the following is the same lambda expression in the form of an expression tree:

```csharp
Expression<Func<Product, bool>> isPromoted = product => product.Promoted;
```

An expression tree can be compiled into the delegate form:

```csharp
Expression<Func<Product, bool>> isPromoted = product => product.Promoted;

Func<Product, bool> compiledExpression = isPromoted.Compile();
```

Expression trees are useful for building composable expressions that can be later compiled into a delegate and used within LINQ queries, amongst other things.

Consider the following model with two static methods used for building predicates as an expression tree:

```csharp
private class Product
{
  public int Id { get; }
  public string Category { get; }
  public bool Promoted { get; }

  public Product(int id, string category, bool promoted)
  {
    Id = id;
    Category = category;
    Promoted = promoted;
  }

  public static Expression<Func<Product, bool>> IsPromoted()
  {
    return product => product.Promoted;
  }

  public static Expression<Func<Product, bool>> IsInCategory(params string[] categories)
  {
    var predicate = PredicateBuilder.False<Product>();

    foreach (var category in categories)
    {
      var temp = category;    // must capture for the closure
      predicate = predicate.Or(p => p.Category.Contains(temp));
    }

    return predicate;
  }
}
```

The `IsPromoted()` and `IsInCategory()` methods return an expression that can participate in any `IQueryable` based query.

`PredicateBuilder`, as shown in `IsInCategory()`, can be used to dynamically build logical `And` and `Or` queries. Let's look at a simple example using the above methods:

```csharp
var products = new List<Product>
{
  new Product(1, "Fruit", true),
  new Product(2, "Fruit", false),
  new Product(3, "Vegetable", true),
  new Product(4, "Vegetable", false),
  new Product(5, "Vehicle", true),
  new Product(6, "Vehicle", false)
};

var filtered = _products
  .AsQueryable()
  .Where(Product.IsPromoted());
```

This will return items 1, 3 and 5.

Let's now compose two queries into one:

```csharp
var isFoodOrVegetable = Product.IsInCategory("Fruit", "Vegetable");
var isPromoted = Product.IsPromoted();
var isPromotedFruitOrVegetable = isFoodOrVegetable.And(isPromoted);

var filtered = _products
  .AsQueryable()
  .Where(isPromotedFruitOrVegetable);
```

The combined query will filter items that are a "Fruit" or "Vegetable", and is promoted. The combined query will return items 1 and 3.

`IQueryable` based queries are often used for translating LINQ queries into SQL (or similar) based queries by traversing, and translating, the expression tree. For regular, in-memory queries, it is more efficient to create the composite queries and cache them for future use by compiling them:

```csharp
// store in a variable for re-use as required
var compiledPredicate = isPromotedFruitOrVegetable.Compile();

var filtered = _products.Where(compiledPredicate);
```

## Extensions

### GetValue()
The `GetValue()` extension method can evaluate `ConstantExpression`, `MemberExpression` (property and field members), and `MethodCallExpression` expressions.

Consider the following class and helper method:

```csharp
private class ChildClass
{
  public int Property { get; set; }
  public int Field;

  public int GetPropertyValue()
  {
    return Property;
  }
}

private static int GetValue(Expression<Func<int>> expression)
{
  var value = expression.GetValue();

  return value.As<int>();
}
```

Each of the following are possible:

```csharp
// returns 4 (ConstantExpression)
var value = GetValue(() => 4);
```

```csharp
var item = new ChildClass { Property = 10};

// returns 10 (MemberExpression, PropertyInfo)
var value = GetValue(() => item.Property);
```

```csharp
var item = new ChildClass { Field = 20};

// returns 20 (MemberExpression, FieldInfo)
var value = GetValue(() => item.Field);
```

```csharp
var item = new ChildClass { Property = 30};

// returns 30 (MethodCallExpression)
var value = GetValue(() => item.GetProperty());
```

### GetMemberExpressions()
A `MemberExpression` may be part of a `LambdaExpression`, or a `UnaryExpression` (which may be within a `LambdaExpression`). `GetMemberExpressions()` exists to retrieve all linked `MemberExpression`s without having to be concerned with unwrapping everything.

As an example, consider the following classes:

```csharp
private class ChildClass
{
  public int Property { get; set; }
}

private class ParentClass
{
  public ChildClass Child { get; set; }
}
```

And the following expression:

```csharp
// 'parent' is an instance of 'ParentClass'
Expression<Func<int>> expression = () => parent.Child.Property;
```

You can use `UnwrapMemberExpression()` to get the root `MemberExpression` from the `LambdaExpression` and then `GetMemberExpressions()` to obtain all linked `MemberExpression`s.

```csharp
// unwrap the LambdaExpression
var memberExpression = expression.UnwrapMemberExpression();

var members = memberExpression.GetMemberExpressions()
  .Select(exp => exp.GetValue())
  .AsReadOnlyList();
```

The calls to `GetValue()` will result in each expression being evaluated:

```csharp
// members[0] = parent
// members[1] = parent.Child
// members[2] = parent.Child.Property
```

### GetFieldOrProperty()
Fields and Properties of an expression are both referenced as `MemberExpression`s. The `GetFieldOrProperty()` method returns the appropriate `MemberInfo` details. The following example shows how to get `MemberInfo` of a property, but the same applies to fields:

```csharp
Expression<Func<int>> expression = () => parent.Child.Property;

var memberInfo = expression.GetFieldOrProperty();

var name = memberInfo.Name;
var value = ReflectionHelper.GetMemberValue(memberInfo, parent.Child);
```

`name` will have the name of the property ("Property" in this example) and `value` will have the current value of the property.

