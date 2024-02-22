#  Version 7.2.0
## XX XXX 2024
---

### AllOverIt
* `IEnumerable<T>` extension methods SelectAsList(), SelectAsReadOnlyCollection(), SelectAsReadOnlyList(), SelectAsReadOnlyCollectionAsync(),
  and SelectAsReadOnlyListAsync() are marked as obsolete, and will be removed in v8. They are replaced with:
  
  | Obsolete | New Method | Comment |
  | - | - |
  | SelectAsList() | SelectToList() | Returns List&lt;T&gt;, not IList&lt;T&gt; |
  | SelectAsReadOnlyCollection() | SelectToReadOnlyCollection() | Returns ReadOnlyCollection&lt;T&gt;, not IReadOnlyCollection&lt;T&gt; |
  | SelectAsReadOnlyList() | SelectToReadOnlyCollection() | ReadOnlyCollection&lt;T&gt; is a IReadOnlyList&lt;T&gt; |
  | SelectAsReadOnlyCollectionAsync() | SelectToReadOnlyCollectionAsync() | Returns ReadOnlyCollection&lt;T&gt;, not IReadOnlyCollection&lt;T&gt; |
  | SelectAsReadOnlyListAsync() | SelectToReadOnlyCollectionAsync() | ReadOnlyCollection&lt;T&gt; is a IReadOnlyList&lt;T&gt; |

  Added new `SelectToArray()`, `SelectToListAsync()` and `SelectToArrayAsync()` extension methods.

* `IAsyncEnumerable<T>` extension methods `SelectAsListAsync()`, `SelectAsReadOnlyCollectionAsync()`, and `SelectAsReadOnlyListAsync()` are
  marked as obsolete, and will be removed in v8. They are replaced with:

  | Obsolete | New Method | Comment |
  | - | - |
  | SelectAsList() | SelectToList() | Returns List&lt;T&gt;, not IList&lt;T&gt; |
  | SelectAsReadOnlyCollection() | SelectToReadOnlyCollection() | Returns ReadOnlyCollection&lt;T&gt;, not IReadOnlyCollection&lt;T&gt; |
  | SelectAsReadOnlyList() | SelectToReadOnlyCollection() | ReadOnlyCollection&lt;T&gt; is a IReadOnlyList&lt;T&gt; |
  | SelectAsReadOnlyCollectionAsync() | SelectToReadOnlyCollectionAsync() | Returns ReadOnlyCollection&lt;T&gt;, not IReadOnlyCollection&lt;T&gt; |
  | SelectAsReadOnlyListAsync() | SelectToReadOnlyCollectionAsync() | ReadOnlyCollection&lt;T&gt; is a IReadOnlyList&lt;T&gt; |


  Added new `ToArrayAsync()`, `ToListAsync()` and `SelectToArrayAsync()` extension methods.


  ### AllOverIt.Validation
  * Added `AbstractValidator<T>` extension methods `CustomRuleFor()`, `CustomRuleForAsync()`, `ConditionalCustomRuleFor()`, and `ConditionalCustomRuleForAsync()`.
