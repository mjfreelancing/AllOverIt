# AllOverIt.Filtering

## Purpose
This package adds comprehensive filtering support to any IQueryable with built-in provision for operations such as Contains, StartsWith, EndsWith, In, NotIn, EqualTo, NotEqualTo, GreaterThan, LessThan, GreaterThanOrEqual, and LessThanOrEqual. It's also possible to create a custom predicate based on a Specification and there are options such as ignoring default (null) filter values and parameterizing query inputs that makes it ideal for use with EntityFramework Core.

## Package Metadata
- Project: Source/AllOverIt.Filtering/AllOverIt.Filtering.csproj
- Target frameworks: net10.0;net9.0;net8.0
- Tags: AllOverIt, Utilities, Query, Filtering
- Depends on: AllOverIt

## Public API Signals (from source)
### Representative public types
- interface IArrayFilterOperation
- interface IBasicFilterOperation
- class Contains
- class DefaultQueryFilterOptions
- class EndsWith
- class EqualTo
- class FilterExtensions
- class FilterSpecificationExtensions
- class GreaterThan
- class GreaterThanOrEqual
- class In
- class LessThan

### Representative extension methods
- HasValue

### Active namespaces
- AllOverIt.Filtering.Filters
- AllOverIt.Filtering.Operations
- AllOverIt.Filtering.Builders
- AllOverIt.Filtering.Options
- AllOverIt.Filtering.Extensions
- AllOverIt.Filtering.Exceptions

## Demo Projects
- Demos/AllOverIt.Filtering/FilteringDemo/FilteringDemo.csproj

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
