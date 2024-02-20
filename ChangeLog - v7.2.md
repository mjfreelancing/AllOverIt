#  Version 7.2.0
## XX XXX 2024
---

### AllOverIt
* IAsyncEnumerable extension methods SelectAsListAsync(), SelectAsReadOnlyCollectionAsync(), and SelectAsReadOnlyListAsync() are
  marked as obsolete, and will be removed in v8. They are replaced with SelectToListAsync(), SelectToReadOnlyCollectionAsync(),
  and SelectToReadOnlyListAsync().

  Added new ToListAsync() and SelectToArrayAsync() extension methods.

  Changed return type of ToListAsync() from IList&lt;T&gt; to List&lt;T&gt;.

* IEnumerable extension methods SelectAsList(), SelectAsReadOnlyCollection(), SelectAsReadOnlyList(), SelectAsReadOnlyCollectionAsync(),
  and SelectAsReadOnlyListAsync() are marked as obsolete, and will be removed in v8. They are replaced with SelectToList(),
  SelectToReadOnlyCollection(), SelectToReadOnlyList(), SelectToReadOnlyCollectionAsync(), and SelectToReadOnlyListAsync().

  Added new SelectToArray(), SelectToListAsync() and SelectToArrayAsync() extension methods.