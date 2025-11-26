#  Version 9.0.0
## 26 Nov 2025

# Updated to support NET 10

### AllOverIt
* EnumerableExtensions / AsyncEnumerableExtensions updates - Change Task to ValueTask and remove ToListAsync()
  and ToArrayAsync() methods for NET 10 and above as these are not available in the runtime.
* Added support for ReadOnlyMemory<T> as an input type in relevant methods.
* Included extension methods for IAsyncEnumerable<T>.
* Added a mockable ZipPackage to support the creation of zip files.

### AllOverIt.ReactiveUI
* Updated CommandFactory to support providing a scheduler when creating cancellable commands.

### AllOverIt.EntityFrameworkCore.Diagrams
* Added support for join tables added via UsingEntity() in many-to-many relationships.
* Added support for defining groups based on the table name rather than a generic - for when shadow tables are used.

---
