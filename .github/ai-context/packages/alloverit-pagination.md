# AllOverIt.Pagination

## Purpose
An advanced keyset-based pagination package that uses a fluent-style builder to create queries that cater for forward and backward navigation for any given deterministic base query. Each page of results includes a 'continuation token' for the current, next, and previous page. These tokens can be used at a later time to navigate to the same location within the dataset, thereby somewhat overcoming the inability of random access traditionally associated with keyset-based navigation.

## Package Metadata
- Project: Source/AllOverIt.Pagination/AllOverIt.Pagination.csproj
- Target frameworks: net10.0;net9.0;net8.0
- Tags: AllOverIt, Utilities, Query, Pagination, KeySet
- Depends on: AllOverIt.Serialization.Binary

## Public API Signals (from source)
### Representative public types
- class ContinuationTokenOptions
- class ContinuationTokenValidator
- class PageResult
- class PaginationException
- class QueryPaginator
- class QueryPaginatorBase
- class QueryPaginatorConfiguration
- class QueryPaginatorExtensions
- class QueryPaginatorFactory
- class ServiceCollectionExtensions
- enum PaginationDirection
- interface IColumnDefinition

### Representative extension methods
- AddQueryPagination

### Active namespaces
- AllOverIt.Pagination
- AllOverIt.Pagination.TokenEncoding
- AllOverIt.Pagination.Extensions
- AllOverIt.Pagination.Exceptions

## Demo Projects
- Demos/AllOverIt.Pagination/MemoryPaginationDemo/MemoryPaginationDemo.csproj

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
