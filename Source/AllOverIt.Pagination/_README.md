# AllOverIt.Pagination

An advanced keyset-based pagination package designed to efficiently handle large datasets. This package provides a fluent-style API to build paginated queries that support forward and backward navigation. It is ideal for scenarios where offset-based pagination is inefficient or impractical.

## Key Features

- **Keyset-Based Pagination**: Avoids the performance issues of offset-based pagination by using deterministic ordering.
- **Fluent API**: Easily configure pagination queries with a chainable, intuitive API.
- **Continuation Tokens**: Each page includes tokens for the current, next, and previous pages, enabling stateless navigation.
- **Bidirectional Navigation**: Supports both forward and backward pagination.
- **Multi-Column Ordering**: Allows ordering by multiple columns, including mixed ascending and descending orders.
- **Integration with Entity Framework Core**: Includes extensions for seamless integration with EF Core.
- **Memory-Based Pagination**: Supports in-memory pagination for non-database scenarios.
- **Extensibility**: Built with interfaces and dependency injection for easy customization and testing.

## Benefits

- **Performance**: Keyset-based pagination is faster and more efficient for large datasets compared to offset-based pagination.
- **Deterministic Results**: Ensures consistent ordering and navigation, even with concurrent data changes.
- **Stateless Navigation**: Continuation tokens allow navigation without maintaining server-side state.
- **Flexibility**: Works with any `IQueryable` source, including databases, in-memory collections, and more.
- **Ease of Use**: The fluent API simplifies query configuration and reduces boilerplate code.

---

## How to Use

### Installation

Install the package via NuGet:

``` csharp
dotnet add package AllOverIt.Pagination
```


---

### Basic Usage

#### 1. **Define Your Query**
Start with a base query, such as an `IQueryable` from a database or an in-memory collection.

``` csharp
var query = from person in dbContext.Persons
            select person;
```


#### 2. **Configure the Paginator**
Create a paginator using the `IQueryPaginatorFactory` or the static `QueryPaginator.Create` method.

``` csharp
var paginatorConfig = new QueryPaginatorConfiguration
{
    PageSize = 20,
    PaginationDirection = PaginationDirection.Forward
};

var queryPaginator = QueryPaginator<Person>
    .Create(query, paginatorConfig)
    .ColumnAscending(person => person.LastName)
    .ColumnAscending(person => person.FirstName)
    .ColumnAscending(person => person.Id); // Ensure the last column is unique
```


#### 3. **Fetch Paginated Results**
Use the paginator to fetch results for a specific page.

``` csharp
var continuationToken = ""; // Use null or an empty string for the first page
var pageResults = queryPaginator.GetPageResults(continuationToken);

foreach (var person in pageResults.Results)
{
    Console.WriteLine($"{person.LastName}, {person.FirstName}");
}

// Access continuation tokens for navigation
var nextToken = pageResults.NextToken;
var previousToken = pageResults.PreviousToken;
```

---

### Entity Framework Core Integration

For EF Core, use the `AllOverIt.EntityFrameworkCore.Pagination` package to simplify integration.

``` csharp
var pageResults = await queryPaginator.GetPageResultsAsync(continuationToken);
```

---

### Memory-Based Pagination

For in-memory collections, the process is similar. Use `AsQueryable()` to create a queryable source.

``` csharp
var persons = GetInMemoryData();
var query = persons.AsQueryable();

var queryPaginator = QueryPaginator<Person>
    .Create(query, paginatorConfig)
    .ColumnAscending(person => person.LastName)
    .ColumnAscending(person => person.FirstName)
    .ColumnAscending(person => person.Id);

var pageResults = queryPaginator.GetPageResults(continuationToken);
```

---

### Continuation Tokens

Continuation tokens are used to navigate between pages. They encode the state of the pagination and can be stored or passed between API calls.

- **Current Page**: Use `pageResults.CurrentToken` to represent the current page's position.
- **Next Page**: Use `pageResults.NextToken` to represent the next page.
- **Previous Page**: Use `pageResults.PreviousToken` to represent the previous page.

Additionally, you can manually generate tokens for the first and last pages using the `TokenEncoder`:

- **First Page**: Use `queryPaginator.TokenEncoder.EncodeFirstPage()` to generate a token for the first page.
- **Last Page**: Use `queryPaginator.TokenEncoder.EncodeLastPage()` to generate a token for the last page.
