# AllOverIt.Shouldly

Structural, member-by-member assertion extensions for [Shouldly](https://github.com/shouldly/shouldly).

Shouldly is an excellent assertion library but it has a specific gap: there is no built-in way to perform a deep, structural comparison of two objects that share member names but are not the same type. `AllOverIt.Shouldly` fills that gap with `ShouldBeEquivalentTo` and its companion `ShouldNotBeEquivalentTo`, plus a rich set of options for controlling exactly how the comparison is performed.

---

## Contents

- [Core concept](#core-concept)
- [Gaps filled](#gaps-filled)
  - [Cross-type structural matching](#1-cross-type-structural-matching)
  - [Asymmetric comparison](#2-asymmetric-comparison)
  - [Sequence structural matching](#3-sequence-structural-matching)
  - [Order-insensitive sequence matching](#4-order-insensitive-sequence-matching)
  - [Negative structural assertion](#5-negative-structural-assertion)
  - [Non-public member access](#6-non-public-member-access)
  - [Missing member tolerance](#7-missing-member-tolerance)
  - [Member exclusion by name or path](#8-member-exclusion-by-name-or-path)
  - [Custom type comparers](#9-custom-type-comparers)
  - [Custom path comparers](#10-custom-path-comparers)
  - [Numeric tolerance in deep comparisons](#11-numeric-tolerance-in-deep-comparisons)
  - [Custom leaf types](#12-custom-leaf-types)
  - [Dictionary comparison](#13-dictionary-comparison)
  - [Cycle detection](#14-cycle-detection)
  - [Static and const member support](#15-static-and-const-member-support)
  - [Custom assertion messages](#16-custom-assertion-messages)
  - [Negative sequence assertion](#17-negative-sequence-assertion)
- [Not supported / known limitations](#not-supported--known-limitations)
- [API reference](#api-reference)

---

## Core concept

`ShouldBeEquivalentTo` compares two objects **member-by-member by name**. It walks every readable member declared on `expected` and looks for a member with the same name on `actual`, then compares their values recursively. The comparison is:

- **Type-independent** — `actual` and `expected` may be completely different types.
- **Asymmetric** — only members present on `expected` are checked; extra members on `actual` are silently permitted.
- **Recursive** — nested objects, collections, and dictionaries are all walked automatically.
- **Configurable** — a rich `EquivalenceOptions` object controls every aspect of the comparison.

All behaviour is opt-in. The default call `actual.ShouldBeEquivalentTo(expected)` uses public members only, strict sequence ordering, no tolerance, and no exclusions.

---

## Gaps filled

### 1. Cross-type structural matching

Shouldly's `ShouldBe` requires both sides to be the same type. `ShouldBeEquivalentTo` imposes no such constraint — a DTO returned from a factory can be compared against an anonymous type, a view model, or any other shape that has overlapping member names.

```csharp
actual.ShouldBeEquivalentTo(expected);
```

Members are resolved by **name** on both sides. If `expected` has a property `Name` and `actual` has a property `Name` (regardless of either type's other members), those two values are compared.

---

### 2. Asymmetric comparison

Only members found on `expected` are checked. Members present on `actual` but absent from `expected` are **ignored**. This allows an expected anonymous type or partial view model to describe only the subset of state that matters for a given test.

```csharp
// Only Id and Name are checked; actual may have many more properties.
actual.ShouldBeEquivalentTo(new { Id = 42, Name = "Alice" });
```

---

### 3. Sequence structural matching

Shouldly's `ShouldBe(IEnumerable<T>)` compares elements using `Equals()` (reference or value equality). `ShouldBeEquivalentTo` for sequences performs a **structural, member-by-member comparison of each element pair** — useful when the element type is a class that does not override `Equals`.

```csharp
IEnumerable<ActualDto> actual = ...;
IEnumerable<ExpectedDto> expected = ...;

actual.ShouldBeEquivalentTo(expected);
```

Both sequence length and per-element structure must match.

---

### 4. Order-insensitive sequence matching

By default sequences are compared in strict order (index 0 to index N). Setting `SequenceOrdering.AnyOrder` performs a greedy pairing: every expected element must find at least one structural match somewhere in the actual sequence, regardless of position.

```csharp
actual.ShouldBeEquivalentTo(expected, opts => opts.SequenceOrdering = SequenceOrdering.AnyOrder);
```

---

### 5. Negative structural assertion

`ShouldNotBeEquivalentTo` is the inverse — it passes only when the two objects do **not** match. All three option-passing overloads are available.

```csharp
actual.ShouldNotBeEquivalentTo(notExpected);
actual.ShouldNotBeEquivalentTo(notExpected, opts => opts.ExcludeMissingMembers = true);
actual.ShouldNotBeEquivalentTo(notExpected, options);
```

---

### 6. Non-public member access

Shouldly provides no mechanism for inspecting private, protected, or internal members. When `IncludeNonPublicMembers` is `true`, `ShouldBeEquivalentTo` uses `BindingOptions.AllVisibility` which covers **all** visibility levels: public, private, protected, and internal (including protected internal and private protected).

```csharp
actual.ShouldBeEquivalentTo(expected, opts => opts.IncludeNonPublicMembers = true);
// or fluently:
actual.ShouldBeEquivalentTo(expected, opts => opts.IncludeNonPublicMembers());
```

---

### 7. Missing member tolerance

By default, if `expected` names a member that does not exist on `actual`, the assertion fails. Setting `ExcludeMissingMembers` silently skips those members instead. This is useful when comparing against a richer expected shape where some properties may not exist on every implementation.

```csharp
actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMissingMembers = true);
// or fluently:
actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMissingMembers());
```

---

### 8. Member exclusion by name or path

Individual members can be excluded from comparison by simple name, by dotted member path, or by a type-safe lambda expression. Exclusion uses **last-segment matching**, so excluding `"CreatedAt"` will skip every member named `CreatedAt` anywhere in the object graph. More precise targeting is possible with a full dotted path.

**By member name (global):**

```csharp
actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMember("CreatedAt"));
```

**By dotted path (targeted):**

```csharp
// Excludes only Address.CreatedAt, not top-level CreatedAt
actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMember("Address.CreatedAt"));
```

**By full traversal path:**

```csharp
// The traversal path always starts with '$.'
actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMember("$.Address.CreatedAt"));
```

**By lambda expression (type-safe, extracts member name):**

```csharp
actual.ShouldBeEquivalentTo(expected, opts => opts.Excluding<MyModel, string>(x => x.CreatedAt));
```

> **Note:** The lambda form extracts only the **simple member name** — it does not capture a path. Chained expressions (e.g. `x => x.Address.City`) are not supported; use `ExcludeMember("Address.City")` for nested targeting.

Multiple members can be excluded by chaining calls:

```csharp
actual.ShouldBeEquivalentTo(expected, opts =>
{
    opts.ExcludeMember("CreatedAt");
    opts.ExcludeMember("UpdatedAt");
});
```

---

### 9. Custom type comparers

A `Func<T, T, bool>` can be registered for any type. Whenever a member of that type is encountered during traversal, the custom function is called instead of the built-in structural comparison.

```csharp
actual.ShouldBeEquivalentTo(expected, opts =>
    opts.UseComparer<string>((a, e) => string.Equals(a, e, StringComparison.OrdinalIgnoreCase)));
```

The comparer receives the actual value as the first argument and the expected value as the second. Returning `false` fails the assertion at the current path.

---

### 10. Custom path comparers

A raw `Func<object?, object?, bool>` can be registered for a **specific traversal path**. Path comparers take priority over type comparers. The path must be the full traversal path used internally, which always starts with `$.` and uses `.` to separate nested members.

```csharp
actual.ShouldBeEquivalentTo(expected, opts =>
    opts.UseComparer("$.Metadata.Tags", (a, e) =>
        ((string[])a!).SequenceEqual((string[])e!)));
```

---

### 11. Numeric tolerance in deep comparisons

Shouldly supports `ShouldBe(expected, tolerance)` only for top-level scalar comparisons. `ShouldBeEquivalentTo` supports configurable tolerance for `float`, `double`, and `decimal` values **anywhere in the object graph** — including deeply nested members and collection elements.

```csharp
actual.ShouldBeEquivalentTo(expected, opts =>
{
    opts.FloatTolerance = 0.001f;
    opts.DoubleTolerance = 1e-9;
    opts.DecimalTolerance = 0.01m;
});
```

When a tolerance is set, any member of the corresponding type is compared with `Math.Abs(actual - expected) <= tolerance`. If no tolerance is set for a numeric type, exact equality is used.

---

### 12. Custom leaf types

Built-in leaf types (primitives, enums, `string`, `DateTime`, `Guid`, `Uri`, etc.) are compared by value using `Equals()`. To force any additional complex type to be treated the same way — comparing the whole value with `Equals()` rather than recursing into its members — register it with `TreatAsLeaf`.

```csharp
actual.ShouldBeEquivalentTo(expected, opts => opts.TreatAsLeaf<Money>());
// or by Type instance:
actual.ShouldBeEquivalentTo(expected, opts => opts.TreatAsLeaf(typeof(Money)));
```

Nullable variants are handled automatically — registering `Money` also covers `Money?`.

**Built-in leaf types** (always treated as atomic):
`bool`, `byte`, `sbyte`, `char`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `float`, `double`, `Half`, all `enum` types, `string`, `decimal`, `DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly`, `TimeSpan`, `Guid`, `Uri`, `Version`, `Type` (and subtypes), `MemberInfo` (and subtypes).

---

### 13. Dictionary comparison

When both `actual` and `expected` implement `IDictionary`, `ShouldBeEquivalentTo` iterates over every key in `expected` and checks that `actual` contains the same key with a structurally matching value. Extra keys in `actual` are permitted (asymmetric, consistent with object matching). Values are compared recursively.

```csharp
var expected = new Dictionary<string, Address> { ["home"] = new Address { City = "London" } };
actual.ShouldBeEquivalentTo(expected);
```

---

### 14. Cycle detection

`ShouldBeEquivalentTo` tracks every `(actual, expected)` object pair that has been visited during recursion. If the same pair is encountered again, it is silently skipped. This prevents stack overflows when comparing objects with circular references (e.g. a parent that holds a reference back to itself, or bidirectional navigation properties).

Cycle detection uses reference equality for the pair, so two structurally identical but distinct object instances are treated as different pairs and are both fully compared.

---

### 15. Static and const member support

When using an anonymous type as the `expected` shape, you can reference `const` or `static readonly` fields from the `actual` type by name. `ShouldBeEquivalentTo` first looks for an instance member; if none is found it falls back to a public static field (which covers `const` and `static readonly`).

```csharp
// actual type has: public const string DefaultName = "Default";
actual.ShouldBeEquivalentTo(new { DefaultName = MyClass.DefaultName, Id = 1 });
```

---

### 16. Custom assertion messages

Every `ShouldBeEquivalentTo` and `ShouldNotBeEquivalentTo` overload accepts an optional `string? customMessage` parameter. When provided and non-`null`, the message is prepended to the assertion failure detail, separated by a blank line, making it easy to identify which assertion failed in a multi-step test.

```csharp
actual.ShouldBeEquivalentTo(expected, "Step 3: order confirmation should match the placed order");

actualSequence.ShouldBeEquivalentTo(expectedSequence, "Step 5: returned items should equal the expected list");

actual.ShouldNotBeEquivalentTo(staleSnapshot, "Post-update: object should differ from the pre-update snapshot");
```

Passing `null` (or omitting the parameter entirely) produces exactly the same output as before — there is no behavioural change for existing call sites.

---

### 17. Negative sequence assertion

`ShouldNotBeEquivalentTo` now accepts `IEnumerable<TActual>` as the receiver, mirroring all three `ShouldBeEquivalentTo` sequence overloads. The assertion passes when at least one element difference is detected and fails when the sequences match entirely.

```csharp
// Passes — sequences differ
actualItems.ShouldNotBeEquivalentTo(differentItems);

// Passes — strict ordering means reversed order is a mismatch
actualItems.ShouldNotBeEquivalentTo(reversedItems);

// Fails — sequences match under AnyOrder
actualItems.ShouldNotBeEquivalentTo(reorderedItems, opts => opts.SequenceOrdering = SequenceOrdering.AnyOrder);
```

---

## Not supported / known limitations

| Limitation                                            | Details                                                                                                                                                                                                                                                                                                                            |
| ----------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Wildcards in exclusion paths**                      | Exclusion patterns like `Items[*].Name` are not supported. Exclusion matches either the full path, a simplified dotted path, or the simple member name as the last path segment.                                                                                                                                                   |
| **Index-based collection exclusions**                 | It is not possible to exclude a specific element index, e.g. `$.Items[0].Price`.                                                                                                                                                                                                                                                   |
| **Member name mapping**                               | `actual.FirstName` does not match `expected.Name`. Member names must be identical on both sides.                                                                                                                                                                                                                                   |
| **`IEquatable<T>` is not respected**                  | `ShouldBeEquivalentTo` always uses its own reflection-based comparison. If the type implements `IEquatable<T>`, that implementation is not called unless the type is also a built-in leaf type.                                                                                                                                    |
| **No recursion depth limit**                          | There is no configurable maximum depth. The cycle-detection visited set is the only safeguard against infinite recursion. Objects with complex, non-circular graphs can produce deeply recursive call stacks.                                                                                                                      |
| **Proxy/mock objects with `IncludeNonPublicMembers`** | Dynamic proxy objects (e.g. FakeItEasy or Moq stubs of abstract types) have many internal fields added by the proxy framework. These fields typically return new instances on every access, which defeats cycle detection and can cause a stack overflow. Never use `IncludeNonPublicMembers = true` when comparing proxy objects. |
| **Type comparer inheritance**                         | A type comparer registered for `Animal` will not be applied when a member is of type `Dog : Animal`. The match is by exact runtime type.                                                                                                                                                                                           |
| **Path comparer key syntax**                          | Path comparers must use the exact internal traversal path (e.g. `$.Address.City`), including the `$` root prefix.                                                                                                                                                                                                                  |
| **Lambda exclusion is name-only**                     | `Excluding<T, TMember>(x => x.Address.City)` is **not** supported for chained access. Only a single-level member access (`x => x.City`) is valid; the extracted path is the simple member name.                                                                                                                                    |

---

## API reference

### Extension methods

| Method                                                                                                 | Description                                              |
| ------------------------------------------------------------------------------------------------------ | -------------------------------------------------------- |
| `actual.ShouldBeEquivalentTo(expected)`                                                                | Structural match with default options.                   |
| `actual.ShouldBeEquivalentTo(expected, customMessage?)`                                                | Structural match with optional failure message.          |
| `actual.ShouldBeEquivalentTo(expected, Action<EquivalenceOptions>)`                                    | Structural match with inline option configuration.       |
| `actual.ShouldBeEquivalentTo(expected, Action<EquivalenceOptions>, customMessage?)`                    | Inline configuration with optional failure message.      |
| `actual.ShouldBeEquivalentTo(expected, EquivalenceOptions)`                                            | Structural match with a pre-built options instance.      |
| `actual.ShouldBeEquivalentTo(expected, EquivalenceOptions, customMessage?)`                            | Pre-built options with optional failure message.         |
| `actualSequence.ShouldBeEquivalentTo(expectedSequence)`                                                | Element-wise structural match of two sequences.          |
| `actualSequence.ShouldBeEquivalentTo(expectedSequence, customMessage?)`                                | Element-wise match with optional failure message.        |
| `actualSequence.ShouldBeEquivalentTo(expectedSequence, Action<EquivalenceOptions>)`                    | Element-wise match with inline configuration.            |
| `actualSequence.ShouldBeEquivalentTo(expectedSequence, Action<EquivalenceOptions>, customMessage?)`    | Element-wise inline configuration with message.          |
| `actualSequence.ShouldBeEquivalentTo(expectedSequence, EquivalenceOptions)`                            | Element-wise match with pre-built options.               |
| `actualSequence.ShouldBeEquivalentTo(expectedSequence, EquivalenceOptions, customMessage?)`            | Pre-built options with optional failure message.         |
| `actual.ShouldNotBeEquivalentTo(expected)`                                                             | Passes when the objects do **not** structurally match.   |
| `actual.ShouldNotBeEquivalentTo(expected, customMessage?)`                                             | Negative match with optional failure message.            |
| `actual.ShouldNotBeEquivalentTo(expected, Action<EquivalenceOptions>)`                                 | Negative match with inline configuration.                |
| `actual.ShouldNotBeEquivalentTo(expected, Action<EquivalenceOptions>, customMessage?)`                 | Negative inline configuration with message.              |
| `actual.ShouldNotBeEquivalentTo(expected, EquivalenceOptions)`                                         | Negative match with pre-built options.                   |
| `actual.ShouldNotBeEquivalentTo(expected, EquivalenceOptions, customMessage?)`                         | Negative pre-built options with message.                 |
| `actualSequence.ShouldNotBeEquivalentTo(expectedSequence)`                                             | Passes when the sequences do **not** structurally match. |
| `actualSequence.ShouldNotBeEquivalentTo(expectedSequence, customMessage?)`                             | Negative sequence match with optional failure message.   |
| `actualSequence.ShouldNotBeEquivalentTo(expectedSequence, Action<EquivalenceOptions>)`                 | Negative sequence match with inline configuration.       |
| `actualSequence.ShouldNotBeEquivalentTo(expectedSequence, Action<EquivalenceOptions>, customMessage?)` | Negative sequence inline configuration with message.     |
| `actualSequence.ShouldNotBeEquivalentTo(expectedSequence, EquivalenceOptions)`                         | Negative sequence match with pre-built options.          |
| `actualSequence.ShouldNotBeEquivalentTo(expectedSequence, EquivalenceOptions, customMessage?)`         | Negative sequence pre-built options with message.        |

### `EquivalenceOptions` properties

| Property                  | Type                                                | Default  | Description                                                                  |
| ------------------------- | --------------------------------------------------- | -------- | ---------------------------------------------------------------------------- |
| `IncludeNonPublicMembers` | `bool`                                              | `false`  | When `true`, includes all non-public members (private, protected, internal). |
| `ExcludeMissingMembers`   | `bool`                                              | `false`  | When `true`, silently skips expected members that do not exist on actual.    |
| `SequenceOrdering`        | `SequenceOrdering`                                  | `Strict` | Controls whether sequence elements must appear in the same order.            |
| `ExcludedMembers`         | `ISet<string>`                                      | empty    | Member names or paths to skip during comparison.                             |
| `TypeComparers`           | `IDictionary<Type, Func<object?, object?, bool>>`   | empty    | Custom comparers keyed by exact runtime type.                                |
| `PathComparers`           | `IDictionary<string, Func<object?, object?, bool>>` | empty    | Custom comparers keyed by full traversal path (e.g. `$.Name`).               |
| `LeafTypes`               | `ISet<Type>`                                        | empty    | Additional types to treat as atomic values compared via `Equals()`.          |
| `FloatTolerance`          | `float?`                                            | `null`   | Tolerance for `float` comparisons. `null` means exact equality.              |
| `DoubleTolerance`         | `double?`                                           | `null`   | Tolerance for `double` comparisons. `null` means exact equality.             |
| `DecimalTolerance`        | `decimal?`                                          | `null`   | Tolerance for `decimal` comparisons. `null` means exact equality.            |

### `EquivalenceOptions` methods

| Method                                              | Description                                                       |
| --------------------------------------------------- | ----------------------------------------------------------------- |
| `ExcludeMember(string)`                             | Adds a member name or path to the exclusion set. Returns `this`.  |
| `UseComparer<TType>(Func<TType?, TType?, bool>)`    | Registers a custom comparer for a type. Returns `this`.           |
| `UseComparer(string, Func<object?, object?, bool>)` | Registers a custom comparer for a traversal path. Returns `this`. |
| `TreatAsLeaf<TType>()`                              | Adds `TType` to the set of leaf types. Returns `this`.            |
| `TreatAsLeaf(Type)`                                 | Adds the given type to the set of leaf types. Returns `this`.     |

### `EquivalenceOptionsExtensions` fluent methods

| Method                                                               | Description                                                                                 |
| -------------------------------------------------------------------- | ------------------------------------------------------------------------------------------- |
| `opts.IncludeNonPublicMembers()`                                     | Sets `IncludeNonPublicMembers = true`. Returns options.                                     |
| `opts.ExcludeMissingMembers()`                                       | Sets `ExcludeMissingMembers = true`. Returns options.                                       |
| `opts.Excluding<TModel, TMember>(Expression<Func<TModel, TMember>>)` | Adds the named member to the exclusion set (single-level expression only). Returns options. |

### `SequenceOrdering` enum

| Value      | Description                                                                               |
| ---------- | ----------------------------------------------------------------------------------------- |
| `Strict`   | Elements must appear in the same order in both sequences (default).                       |
| `AnyOrder` | Each expected element is matched to any unmatched element in actual, regardless of index. |
