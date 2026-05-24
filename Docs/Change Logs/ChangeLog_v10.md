# Version 10.0.0

## XX XXX 2026

# AllOverIt.Fixture

- Replaced FluentAssertions with Shouldly

Migration Notes:

- Invoking(() => {}).Should().Throw() and related methods have been removed since Shouldly provides Should.Throw(() => {})

---
