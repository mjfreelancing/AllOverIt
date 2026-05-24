# Test Migration Rules

## Purpose

This repository is migrating tests from FluentAssertions-style chains to Shouldly-based assertions. Keep the migration local, mechanical where possible, and aligned to the helper surfaces already present in `AllOverIt.Shouldly` and `AllOverIt.Fixture`.

## Hard Constraints

- Do not modify `Tests/Directory.Build.props`.
- Do not make broad regex-based rewrites across many projects.
- Preserve the original intent of each test; prefer the smallest possible conversion.
- Keep one blank line at the end of every edited file.
- If a migration shape is unclear or unsupported by the helper APIs, stop and review it rather than guessing.
- Do not rely on project-level or directory-level implicit `<Using>` items. Every file must carry its own explicit `using` directives.

## Preferred Assertion Mappings

### Execution and exception assertions

- `Invoking(() => { ... }).Should().NotThrow();` -> `Should.NotThrow(() => { ... });`
- `Invoking(() => { ... }).Should().Throw<TException>();` -> `Should.Throw<TException>(() => { ... });`
- `await Invoking(async () => { ... }).Should().ThrowAsync<TException>();` -> `await Should.ThrowAsync<TException>(async () => { ... });`
- `Should().NotThrow(...)` and `Should().Throw...` chains should be replaced directly, not wrapped in new fluent helpers unless the test already uses a shared fixture helper.

### Equality and structure

- Use `ShouldMatch(...)` for deep structural comparisons of objects and sequences. `ShouldMatch` resolves `const` and `static readonly` members on the actual type by name, so they can be included directly in an anonymous `expected` type (e.g. `DefaultTheme = Theme.Neutral`) without a separate assertion.
- Use `ShouldNotMatch(...)` to assert two objects do **not** match member-by-member (negates `ShouldMatch`; same overloads with optional `Action<EquivalenceOptions>` or `EquivalenceOptions` parameter). Requires `using AllOverIt.Shouldly;`.
- Use `ShouldMatch(..., opts => opts.ExcludeMissingMembers())` when validating partial DTO / validation-result shapes.
- Use `ShouldMatch(..., opts => opts.SequenceOrdering = SequenceOrdering.AnyOrder)` for order-insensitive collection comparison.
- Use `ShouldMatch(..., opts => opts.IncludeInternalMembers = true)` only when the test intentionally compares internal members.
- Use `ShouldBeTrue()`, `ShouldBeFalse()`, `ShouldBeNull()`, `ShouldBeSameAs(...)`, `ShouldBeOfType(...)`, and `ShouldBeGreaterThan(...)` for direct value checks.
- Prefer direct count checks such as `sequence.Count().ShouldBe(n)` or `array.Length.ShouldBe(n)`.
- Use `ShouldNotBeNullOrEmpty()` for both `string` and `IEnumerable<T>` values. For strings, Shouldly provides this built-in (requires `using Shouldly;`). For `IEnumerable<T>` (including `byte[]`), use the extension from `AllOverIt.Fixture.Extensions` (requires `using AllOverIt.Fixture.Extensions;`). Never split into two separate null + empty assertions.

### Exception message helpers

Use the helpers from `AllOverIt.Fixture` where they already exist:

- `exception.WithMessageWhenNull(...)`
- `exception.WithMessageWhenEmpty(...)`
- `exception.WithNamedMessageWhenNull(name)`
- `exception.WithNamedMessageWhenEmpty(name)`
- `exception.WithInnerException<TInner>()`
- `exception.WithInnerException<TInner, TException>()`

## Required `using` Directives

Every migrated file must declare its own using directives. Never rely on project-level or directory-level implicit usings.

| Namespace                             | Required when                                                                                                                                                                                                                                                                                      |
| ------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `using Shouldly;`                     | Any Shouldly built-in: `ShouldBe`, `ShouldNotBe`, `ShouldBeNull`, `ShouldNotBeNull`, `ShouldBeTrue`, `ShouldBeFalse`, `ShouldBeSameAs`, `ShouldBeOfType`, `ShouldContain`, `ShouldNotContain`, `ShouldBeGreaterThan`, `ShouldBeEmpty`, `ShouldNotBeEmpty`, `ShouldNotBeNullOrEmpty` (string), etc. |
| `using AllOverIt.Fixture.Extensions;` | `ShouldNotBeNullOrEmpty<T>` (IEnumerable), `WithNamedMessageWhenNull`, `WithNamedMessageWhenEmpty`, `WithMessage`, `WithMessageWhenNull`, `WithMessageWhenEmpty`, `WithInnerException`, `ShouldThrow<TException, TResult>`, `ShouldThrowAsync<TException, TResult>`                                |
| `using AllOverIt.Shouldly;`           | `ShouldMatch`, `ShouldNotMatch`, `EquivalenceOptions`, `SequenceOrdering`                                                                                                                                                                                                                          |

## Helper Surface Notes

- `AllOverIt.Shouldly` already supplies recursive comparison support through `ShouldMatch`, `ShouldNotMatch`, and `EquivalenceOptions`.
- `AllOverIt.Fixture` already supplies shared construction and exception-testing helpers that should be reused instead of inventing new test-local patterns.
- `EquivalenceOptionsExtensions.Excluding(...)` currently needs review before it is used broadly, because some call sites do not infer the generic types cleanly.
- If a conversion requires a helper shape that is not already proven by existing tests, flag it instead of forcing it.

## Examples Already Used In The Repo

```csharp
Should.NotThrow(() =>
{
    // act
});
```

```csharp
var exception = Should.Throw<ArgumentNullException>(() =>
{
    // act
});

exception.WithNamedMessageWhenNull("value");
```

```csharp
await Should.ThrowAsync<InvalidOperationException>(async () =>
{
    // act
});
```

```csharp
actual.ShouldMatch(expected);
```

```csharp
result.Errors.ShouldMatch(expected, opts => opts.ExcludeMissingMembers());
```

```csharp
actual.ShouldMatch(expected, options => options.SequenceOrdering = SequenceOrdering.AnyOrder);
```

```csharp
actual.ShouldMatch(expected, opts => opts.IncludeInternalMembers = true);
```

```csharp
value.ShouldBeNull();
value.ShouldBeTrue();
value.ShouldBeFalse();
value.ShouldBeSameAs(expected);
value.ShouldBeOfType(typeof(SomeType));
```

```csharp
// string
value.ShouldNotBeNullOrEmpty();   // using Shouldly;

// IEnumerable<T> including byte[]
value.ShouldNotBeNullOrEmpty();   // using AllOverIt.Fixture.Extensions;
```

```csharp
// ShouldNotMatch (negates ShouldMatch member-by-member)
actual.ShouldNotMatch(other);  // using AllOverIt.Shouldly;
```

## Migration Guidance

- Convert one project or one fixture family at a time.
- Verify each converted file against the build output before widening the scope.
- If a helper creates type inference problems, prefer an explicit local assertion over a new abstraction until the pattern is validated.
- Keep the migration readable for reviewers; avoid cleverness.
- All files must end with no more than one blank line.
- All files must have CR LF line endings

## Review Flags

Raise these for review before converting them broadly:

- `EquivalenceOptionsExtensions.Excluding(...)` call sites with ambiguous type inference.
- Any `ShouldThrow` shape that depends on `Func<T>` rather than `Action`.
- Any assertion that depends on collection property names like `Count` and may accidentally resolve to the LINQ extension method.
- Any use of a new helper that is not already demonstrated by a successful test project.
- If in doubt about a decision, stop and ask, rather than blindly proceeding.

## Project Conversion Checklist

Execution protocol:

- Convert one project at a time.
- Build the project before running tests.
- Do not move to the next project until tests are green.
- Use a new sub-agent per project unless specific learnings need to be carried over.

Project order (check off as each project reaches green):

- [x] AllOverIt.Assertion.Tests
- [x] AllOverIt.Cryptography.Tests
- [x] AllOverIt.Csv.Tests
- [x] AllOverIt.DependencyInjection.Tests
- [x] AllOverIt.EntityFrameworkCore.Diagrams.Tests
- [x] AllOverIt.EntityFrameworkCore.Tests
- [x] AllOverIt.Evaluator.Tests
- [ ] AllOverIt.Filtering.Tests
- [ ] AllOverIt.Fixture.Tests
- [ ] AllOverIt.Logging.Tests
- [ ] AllOverIt.Logging.Testing.Tests
- [ ] AllOverIt.Mapping.Tests
- [ ] AllOverIt.Pagination.Tests
- [ ] AllOverIt.Pipes.Tests
- [ ] AllOverIt.Reactive.Tests
- [ ] AllOverIt.ReactiveUI.Tests
- [ ] AllOverIt.Serialization.Binary.Tests
- [ ] AllOverIt.Serialization.Json.Abstractions.Tests
- [ ] AllOverIt.Serialization.Json.Newtonsoft.Tests
- [ ] AllOverIt.Serialization.Json.SystemText.Tests
- [ ] AllOverIt.Serilog.Tests
- [ ] AllOverIt.Shouldly.Tests
- [ ] AllOverIt.Tests
- [ ] AllOverIt.Validation.Tests

Excluded from migration queue (not standalone test projects):

- AllOverIt.DependencyInjection.Tests.Types
- \_shouldly_eval
