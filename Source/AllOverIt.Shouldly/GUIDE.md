# AllOverIt.Shouldly — User Guide

A practical, example-driven guide for every feature provided by `AllOverIt.Shouldly`.

This library extends [Shouldly](https://github.com/shouldly/shouldly) with deep structural comparison capabilities via `ShouldBeEquivalentTo` and `ShouldNotBeEquivalentTo`. All examples assume the following `using` directives are present:

```csharp
using AllOverIt.Shouldly;
using Shouldly;
```

---

## Contents

1. [Quick start](#1-quick-start)
2. [Comparing objects of the same type](#2-comparing-objects-of-the-same-type)
3. [Comparing objects of different types](#3-comparing-objects-of-different-types)
4. [Using an anonymous type as the expected shape](#4-using-an-anonymous-type-as-the-expected-shape)
5. [Sequence comparison — strict order](#5-sequence-comparison--strict-order)
6. [Sequence comparison — any order](#6-sequence-comparison--any-order)
7. [Negative assertion — ShouldNotBeEquivalentTo](#7-negative-assertion--shouldnotbeequivalentto)
8. [Excluding members from comparison](#8-excluding-members-from-comparison)
9. [Including non-public members](#9-including-non-public-members)
10. [Tolerating missing members](#10-tolerating-missing-members)
11. [Custom type comparers](#11-custom-type-comparers)
12. [Custom path comparers](#12-custom-path-comparers)
13. [Numeric tolerance in deep comparisons](#13-numeric-tolerance-in-deep-comparisons)
14. [Custom leaf types](#14-custom-leaf-types)
15. [Dictionary comparison](#15-dictionary-comparison)
16. [Re-using options across multiple assertions](#16-re-using-options-across-multiple-assertions)
17. [Fluent options configuration](#17-fluent-options-configuration)
18. [Combining multiple options](#18-combining-multiple-options)
19. [Common pitfalls](#19-common-pitfalls)
20. [Custom assertion messages](#20-custom-assertion-messages)
21. [Negative sequence assertion — ShouldNotBeEquivalentTo for sequences](#21-negative-sequence-assertion--shouldnotbeequivalentto-for-sequences)

---

## 1. Quick start

Install the package and add one `using` directive. Then call `ShouldBeEquivalentTo` anywhere you would normally call Shouldly's `ShouldBe` for object comparison:

```csharp
// Before (Shouldly — only works when both sides are the same type)
actual.ShouldBe(expected);

// After (AllOverIt.Shouldly — works across types, with options)
actual.ShouldBeEquivalentTo(expected);
```

`ShouldBeEquivalentTo` fails with a human-readable Shouldly-style message that identifies the first mismatching member path, along with the actual and expected values.

---

## 2. Comparing objects of the same type

`ShouldBeEquivalentTo` works perfectly well when both sides share the same type. It recursively compares every public member by name.

```csharp
public sealed class Address
{
    public string Street { get; init; }
    public string City { get; init; }
    public string PostCode { get; init; }
}

public sealed class Person
{
    public int Id { get; init; }
    public string Name { get; init; }
    public Address Address { get; init; }
}

// Arrange
var expected = new Person
{
    Id = 1,
    Name = "Alice",
    Address = new Address { Street = "1 Main St", City = "London", PostCode = "EC1A 1BB" }
};

var actual = new Person
{
    Id = 1,
    Name = "Alice",
    Address = new Address { Street = "1 Main St", City = "London", PostCode = "EC1A 1BB" }
};

// Assert
actual.ShouldBeEquivalentTo(expected); // passes
```

If any value differs, the assertion fails and identifies the full path — for example:

```
actual.Address.City
    should be
"Paris"
    but was
"London"
```

---

## 3. Comparing objects of different types

The most common use case: compare a domain object with a DTO, view model, or API response type that has different ancestry but equivalent data members.

```csharp
// Domain type
public sealed class ProductEntity
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }
    public DateTime CreatedAt { get; init; }
}

// DTO returned from a factory / mapper under test
public sealed class ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }
}

// Arrange
var entity = new ProductEntity
{
    Id = Guid.Parse("a1b2c3d4-0000-0000-0000-000000000000"),
    Name = "Widget",
    Price = 9.99m,
    CreatedAt = DateTime.UtcNow // we don't care about this in the DTO
};

var expectedDto = new ProductDto
{
    Id = entity.Id,
    Name = "Widget",
    Price = 9.99m
};

// Act
ProductDto actualDto = MapToDto(entity);

// Assert — only Id, Name, and Price are compared (they are in expected)
// CreatedAt exists only on ProductEntity and is not checked
actualDto.ShouldBeEquivalentTo(expectedDto);
```

---

## 4. Using an anonymous type as the expected shape

For quick assertions on a subset of members, an anonymous type is the most concise option.

```csharp
// Only Id and Status are under test — other members are irrelevant
actual.ShouldBeEquivalentTo(new { Id = 42, Status = "Active" });
```

Only the two members on the anonymous type are compared. All other members of `actual` are ignored.

**Checking a nested member:**

```csharp
actual.ShouldBeEquivalentTo(new { Address = new { City = "London" } });
```

Only `Address.City` is verified. All other members of `Address` and all other top-level members are unaffected.

---

## 5. Sequence comparison — strict order

When `actual` and `expected` are both `IEnumerable<T>`, `ShouldBeEquivalentTo` performs a structural comparison element-by-element. The sequences must have the same length, and corresponding elements must match structurally.

```csharp
IReadOnlyList<PersonDto> actual = repository.GetAll();

var expected = new[]
{
    new PersonDto { Id = 1, Name = "Alice" },
    new PersonDto { Id = 2, Name = "Bob" }
};

actual.ShouldBeEquivalentTo(expected); // Element 0 vs 0, element 1 vs 1
```

This is more powerful than Shouldly's `ShouldBe(IEnumerable<T>)` which compares elements with `Equals()` and therefore only works when elements override `Equals` or are value types.

---

## 6. Sequence comparison — any order

When the order of results is not guaranteed, use `SequenceOrdering.AnyOrder`. Every expected element must be matched by at least one actual element; no actual element can satisfy more than one expected element.

```csharp
actual.ShouldBeEquivalentTo(expected, opts => opts.SequenceOrdering = SequenceOrdering.AnyOrder);
```

**Example — database query with non-deterministic ordering:**

```csharp
var expected = new[]
{
    new TagDto { Name = "csharp" },
    new TagDto { Name = "dotnet" },
    new TagDto { Name = "testing" }
};

// The repository may return tags in any order
IReadOnlyList<TagDto> actual = repository.GetTagsForPost(postId: 1);

actual.ShouldBeEquivalentTo(expected, opts => opts.SequenceOrdering = SequenceOrdering.AnyOrder);
```

---

## 7. Negative assertion — ShouldNotBeEquivalentTo

`ShouldNotBeEquivalentTo` passes when the two objects do **not** structurally match. It accepts the same option overloads as `ShouldBeEquivalentTo`.

```csharp
// The object must differ from the "before" snapshot in some way
var before = Snapshot(subject);
Mutate(subject);
subject.ShouldNotBeEquivalentTo(before);
```

**With options:**

```csharp
// Differ in anything other than the audit fields
subject.ShouldNotBeEquivalentTo(before, opts =>
{
    opts.ExcludeMember("CreatedAt");
    opts.ExcludeMember("UpdatedAt");
});
```

---

## 8. Excluding members from comparison

### By simple name — excludes everywhere in the graph

```csharp
// Any member named "CreatedAt" at any depth is skipped
actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMember("CreatedAt"));
```

### By dotted path — targeted exclusion

```csharp
// Only skips the City member inside Address; top-level City (if any) is still compared
actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMember("Address.City"));
```

### By full traversal path

```csharp
// The traversal path always begins with '$.'
actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMember("$.Address.City"));
```

### By type-safe lambda expression

```csharp
// Extracts the simple member name "PostCode" from the expression
actual.ShouldBeEquivalentTo(expected, opts => opts.Excluding<Address, string>(x => x.PostCode));
```

> **Important:** The lambda overload supports only a single member access (e.g. `x => x.PostCode`). It is equivalent to calling `opts.ExcludeMember("PostCode")`. To exclude a nested member, use the string overload with a dotted path.

### Excluding multiple members

Chain multiple calls on the options instance:

```csharp
actual.ShouldBeEquivalentTo(expected, opts =>
{
    opts.ExcludeMember("CreatedAt");
    opts.ExcludeMember("UpdatedAt");
    opts.Excluding<Order, Guid>(x => x.CorrelationId);
});
```

---

## 9. Including non-public members

By default, only public instance members are compared. Enabling `IncludeNonPublicMembers` extends the comparison to **all** visibility levels: private, protected, internal, protected internal, and private protected.

```csharp
// Comparing internal state of two objects
actual.ShouldBeEquivalentTo(expected, opts => opts.IncludeNonPublicMembers = true);
// or fluently:
actual.ShouldBeEquivalentTo(expected, opts => opts.IncludeNonPublicMembers());
```

**Example — verifying that a factory correctly sets a private backing field:**

```csharp
// Assumes tests project has InternalsVisibleTo access to the source assembly
var expected = CreateExpectedInstance();
var actual = factory.Create(...);
actual.ShouldBeEquivalentTo(expected, opts => opts.IncludeNonPublicMembers());
```

> **Warning:** Do not use `IncludeNonPublicMembers = true` when comparing objects that contain mock or proxy objects (such as FakeItEasy or Moq stubs). The proxy's internal fields return new object instances on every access, defeating cycle detection and causing a stack overflow. See [Common pitfalls](#19-common-pitfalls).

---

## 10. Tolerating missing members

If the `expected` type declares a member that does not exist on `actual`, the assertion normally fails immediately. Setting `ExcludeMissingMembers = true` silently skips those members.

```csharp
var expected = new FullModel { Id = 1, Name = "Alice", Score = 99.5 };
var actual = new LightweightModel { Id = 1, Name = "Alice" }; // no Score property

// Without the option this fails because Score is missing from LightweightModel
actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMissingMembers = true);
```

---

## 11. Custom type comparers

Register a `Func<T, T, bool>` to override how a particular type is compared anywhere in the graph.

**Case-insensitive string comparison:**

```csharp
actual.ShouldBeEquivalentTo(expected, opts =>
    opts.UseComparer<string>((a, e) => string.Equals(a, e, StringComparison.OrdinalIgnoreCase)));
```

**Comparing a third-party value type with a tolerance not covered by the built-in tolerance options:**

```csharp
actual.ShouldBeEquivalentTo(expected, opts =>
    opts.UseComparer<CurrencyAmount>((a, e) => Math.Abs(a.Value - e.Value) < 0.001m));
```

The comparer receives the actual value as the first argument and the expected value as the second.

---

## 12. Custom path comparers

Register a `Func<object?, object?, bool>` for a specific traversal path. Path comparers take priority over type comparers. The path is the full internal key, always starting with `$.`.

```csharp
// Compare a tags array at a known path without caring about order
actual.ShouldBeEquivalentTo(expected, opts =>
    opts.UseComparer("$.Article.Tags", (a, e) =>
    {
        var actualTags = (string[])a!;
        var expectedTags = (string[])e!;
        return actualTags.OrderBy(t => t).SequenceEqual(expectedTags.OrderBy(t => t));
    }));
```

**Example paths:**

- `$.Name` — top-level `Name` member
- `$.Address.City` — `City` inside nested `Address`
- `$.Orders` — the `Orders` collection as a whole

---

## 13. Numeric tolerance in deep comparisons

`ShouldBeEquivalentTo` supports tolerance for `float`, `double`, and `decimal` values **at any depth** in the object graph, including inside collections and nested objects.

```csharp
// All float members across the entire graph must be within 0.001
actual.ShouldBeEquivalentTo(expected, opts => opts.FloatTolerance = 0.001f);

// All double members across the entire graph must be within 1e-9
actual.ShouldBeEquivalentTo(expected, opts => opts.DoubleTolerance = 1e-9);

// All decimal members across the entire graph must be within 0.01
actual.ShouldBeEquivalentTo(expected, opts => opts.DecimalTolerance = 0.01m);
```

**Combined example:**

```csharp
public sealed class Measurement
{
    public float Width { get; init; }
    public double Height { get; init; }
    public decimal Weight { get; init; }
}

var expected = new Measurement { Width = 10.0f, Height = 5.0, Weight = 1.000m };
var actual   = new Measurement { Width = 10.0005f, Height = 5.0000000001, Weight = 1.005m };

actual.ShouldBeEquivalentTo(expected, opts =>
{
    opts.FloatTolerance   = 0.001f;
    opts.DoubleTolerance  = 1e-8;
    opts.DecimalTolerance = 0.01m;
});
// passes — all differences are within tolerance
```

---

## 14. Custom leaf types

By default, complex types (classes, structs) are recursed into member-by-member. Use `TreatAsLeaf<T>()` to mark a type as atomic, so it is compared using its `Equals()` implementation instead.

```csharp
// Money implements value equality via Equals()
actual.ShouldBeEquivalentTo(expected, opts => opts.TreatAsLeaf<Money>());
```

**Using a runtime `Type` reference:**

```csharp
Type leafType = typeof(NodaTime.LocalDate);
actual.ShouldBeEquivalentTo(expected, opts => opts.TreatAsLeaf(leafType));
```

Nullable variants are handled automatically. Registering `Money` also covers `Money?`.

---

## 15. Dictionary comparison

When both `actual` and `expected` implement `IDictionary`, `ShouldBeEquivalentTo` verifies that for every key in `expected`, `actual` contains that key and the corresponding value matches structurally. Extra keys in `actual` are permitted.

```csharp
var expected = new Dictionary<string, Address>
{
    ["home"]   = new Address { City = "London" },
    ["work"]   = new Address { City = "Manchester" }
};

Dictionary<string, Address> actual = BuildAddressBook();

// Each address is compared member-by-member, not by reference
actual.ShouldBeEquivalentTo(expected);
```

**Mixed-value dictionary:**

```csharp
var expected = new Dictionary<string, object>
{
    ["count"] = 3,
    ["label"] = "results"
};

// values are recursively compared
actual.ShouldBeEquivalentTo(expected);
```

---

## 16. Re-using options across multiple assertions

Build an `EquivalenceOptions` instance once and share it across tests, or use it in a helper method.

```csharp
private static readonly EquivalenceOptions AuditFreeOptions = new EquivalenceOptions()
    .ExcludeMember("CreatedAt")
    .ExcludeMember("UpdatedAt")
    .ExcludeMember("CreatedBy");

// In a test:
actual.ShouldBeEquivalentTo(expected, AuditFreeOptions);
```

> **Note:** `EquivalenceOptions` is mutable. If sharing across tests in a concurrent environment, prefer a factory method instead of a single shared instance.

---

## 17. Fluent options configuration

All options methods return the options instance, enabling a fluent chain:

```csharp
actual.ShouldBeEquivalentTo(expected, new EquivalenceOptions()
    .ExcludeMissingMembers()
    .IncludeNonPublicMembers()
    .ExcludeMember("RowVersion"));
```

This is equivalent to using the `Action<EquivalenceOptions>` overload:

```csharp
actual.ShouldBeEquivalentTo(expected, opts =>
{
    opts.ExcludeMissingMembers = true;
    opts.IncludeNonPublicMembers = true;
    opts.ExcludeMember("RowVersion");
});
```

---

## 18. Combining multiple options

Options can be freely combined. The following example covers a realistic integration test scenario:

```csharp
actual.ShouldBeEquivalentTo(expected, opts =>
{
    // Ignore audit fields
    opts.ExcludeMember("CreatedAt");
    opts.ExcludeMember("UpdatedAt");

    // Results come back in database order — any order is fine
    opts.SequenceOrdering = SequenceOrdering.AnyOrder;

    // Prices from the payment gateway have floating-point noise
    opts.DoubleTolerance = 1e-6;

    // Status codes are compared case-insensitively
    opts.UseComparer<string>((a, e) => string.Equals(a, e, StringComparison.OrdinalIgnoreCase));
});
```

---

## 19. Common pitfalls

### Using `IncludeNonPublicMembers` with mock/proxy objects

**Problem:** FakeItEasy, Moq, and similar frameworks build dynamic proxy subclasses. These proxies have many internal fields that represent interceptor state. When `IncludeNonPublicMembers = true`, `ShouldBeEquivalentTo` traverses those fields. Each access to a proxy field can return a **new instance**, so the visited-set cycle detection never fires — the traversal recurses indefinitely and causes a stack overflow.

**Solution:** Never use `IncludeNonPublicMembers = true` on an object that is, or contains, a proxy. Instead, assert specific members directly:

```csharp
// BAD — causes stack overflow when _expression is a FakeItEasy stub
sut.ShouldBeEquivalentTo(new { _expression = fakeExpression },
    opts => opts.IncludeNonPublicMembers());

// GOOD — assert the internal field directly
sut._expression.ShouldBeSameAs(fakeExpression);
```

### Lambda exclusion does not capture paths

`Excluding<TModel, TMember>(x => x.Member)` extracts only the simple member name. It does **not** support chained access:

```csharp
// BAD — chained expression; only "City" is extracted, not "Address.City"
opts.Excluding<Person, string>(x => x.Address.City);

// GOOD — use the string overload for nested paths
opts.ExcludeMember("Address.City");
// or with the full traversal path:
opts.ExcludeMember("$.Address.City");
```

### `ExcludeMember("Name")` excludes the name everywhere

A simple name (no dots) is matched as the **last segment** of any path. This means `ExcludeMember("Name")` will skip `Name`, `Address.Name`, `Order.Customer.Name`, etc. If you want to exclude only one, use the full dotted path:

```csharp
opts.ExcludeMember("$.Address.Name"); // only Address.Name is skipped
```

### Member names must match exactly

`ShouldBeEquivalentTo` resolves members by name. There is no mapping layer. If the actual type has `FirstName` and the expected type has `GivenName`, they will not be compared.

```csharp
// actual has FirstName; expected has GivenName — ShouldBeEquivalentTo finds no match and skips GivenName
// unless ExcludeMissingMembers = false (default), in which case it fails
```

Use `ExcludeMissingMembers = true` with caution — it can silently mask missing assertions.

### Type comparers match exact runtime type only

A custom comparer registered for `Animal` is **not** applied when the actual member is of type `Dog : Animal`. Register for the concrete type, or use a path comparer for precision:

```csharp
// Works only when runtime type is exactly Animal, not Dog
opts.UseComparer<Animal>((a, e) => a.Name == e.Name);

// More reliable: register for Dog explicitly
opts.UseComparer<Dog>((a, e) => a.Name == e.Name);
```

### Sequence length mismatches fail immediately

`ShouldBeEquivalentTo` for sequences requires both sequences to have the same number of elements. If a test is only interested in the first N results, slice before asserting:

```csharp
// Compare only the first 3 results
actual.Take(3).ShouldBeEquivalentTo(expected.Take(3));
```

---

## 20. Custom assertion messages

Every `ShouldBeEquivalentTo` and `ShouldNotBeEquivalentTo` overload accepts an optional `string? customMessage` trailing parameter. When provided and non-`null`, the message is prepended to the standard assertion failure detail, separated by a blank line.

This makes it easy to identify which assertion failed when a test contains multiple assertions:

```csharp
actual.ShouldBeEquivalentTo(expected, "Step 3: order confirmation should match the placed order");
```

The same parameter is available on every overload:

```csharp
// With inline options
actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMember("UpdatedAt"), "Step 4: body should match");

// With pre-built options
actual.ShouldBeEquivalentTo(expected, sharedOptions, "Step 4: body should match");

// Sequences
actualItems.ShouldBeEquivalentTo(expectedItems, "Step 5: returned items should match");

// Negative
actual.ShouldNotBeEquivalentTo(staleSnapshot, "Post-update: object must differ from snapshot");
```

Example failure output when a custom message is provided:

```
Step 3: order confirmation should match the placed order

ShouldBeEquivalentTo failed at '$.TotalAmount': value mismatch, expected '99.99' (System.Decimal) but found '0.00' (System.Decimal).
```

Passing `null` (or omitting the parameter entirely) produces exactly the same output as before — there is no behavioural change for existing call sites.

---

## 21. Negative sequence assertion — ShouldNotBeEquivalentTo for sequences

`ShouldNotBeEquivalentTo` now accepts `IEnumerable<TActual>` as the receiver, mirroring all three `ShouldBeEquivalentTo` sequence overloads. The assertion passes when at least one element difference is detected and fails when the sequences structurally match completely.

**Basic usage:**

```csharp
// Passes — second sequence has different values
actualItems.ShouldNotBeEquivalentTo(differentItems);
```

**Order sensitivity:**

```csharp
var actual   = new[] { new Item { Id = 1 }, new Item { Id = 2 } };
var reversed = new[] { new Item { Id = 2 }, new Item { Id = 1 } };

// Passes — strict ordering (default) treats reversed order as a mismatch
actual.ShouldNotBeEquivalentTo(reversed);

// Fails — AnyOrder finds a complete match regardless of position
actual.ShouldNotBeEquivalentTo(reversed, opts => opts.SequenceOrdering = SequenceOrdering.AnyOrder);
```

**With pre-built options:**

```csharp
actual.ShouldNotBeEquivalentTo(previousSnapshot, new EquivalenceOptions
{
    SequenceOrdering = SequenceOrdering.AnyOrder
});
```

**With a custom message:**

```csharp
actual.ShouldNotBeEquivalentTo(staleItems, "Post-refresh: item list must differ from cached version");
```
