#  Version 7.22.1
## 09 Jul 2024

### AllOverIt.EntityFrameworkCore.Diagrams

* Fixed PreserveColumnOrder issue so base class properties are listed first.

---


#  Version 7.22.0
## 07 Jul 2024

### AllOverIt.Serialization.Binary

* Fixed issue with writing an empty string via WriteSafeString() that was being read back as null.

---


#  Version 7.21.0
## 02 Jul 2024

### AllOverIt.Fixture

* Refactored ClassPropertyAssertions (and related) to cater for open generics via typeof(T).
* Added MeetsCriteria() on PropertyInfoAssertions.

---


#  Version 7.20.1
## 01 Jul 2024

### AllOverIt.Logging.Testing

* Added AssertMessageWithArgumentsEntry() and GetExpectedLogTemplateWithArgumentsMetadata() overloads that will allow
  destructured argument names to be provided via a dictionary.

---


#  Version 7.20.0
## 24 Jun 2024

### AllOverIt

* Added IsCompilerGenerated() extension method on PropertyInfo.

### AllOverIt.Fixture

* Updated ClassProperties to exclude properties that are compiler generated, such as 'EqualityContract' on records.
* Added MatchNames() to ClassPropertiesAssertions so tests can detect when a property is added or removed from a class.

---


#  Version 7.19.0
## 14 Jun 2024

### AllOverIt.Fixture

* Updated ClassProperties to provide the ability to include / exclude properties using expressions.
* The ClassProperties Including() and Excluding() methods now allow for cumulative filtering by chaining multiple calls.
* Changed PropertyInfoAssertions and NullabilityInfoAssertions to use "Not Nullable" instead of "NotNull" when reporting a
  nullability assertion failure.

---


#  Version 7.18.1
## 12 Jun 2024

### AllOverIt.Fixture

* Improved property assertion differentiation between Set and Init.
* Improved property accessor assertion messages to indicate what was actually found.

---


#  Version 7.18.0
## 11 Jun 2024

### AllOverIt.Fixture

* Updated ClassProperties (for property assertions) to provide the ability to assert properties only declared on the class
  (exclude base class properties)
* Breaking: Removed HasInitOnlyAccessor() and HasNoInitOnlyAccessor() from PropertyInfoAssertions. Replaced with an additional
  enum value of 'Init' on PropertyAccessor that can be passed to IsPublic(), IsProtected(), IsPrivate(), and IsInternal().
  This better reflects how code is naturally written, and therefore tested.

---


#  Version 7.17.0
## 11 Jun 2024

### AllOverIt.Fixture

* Breaking: Renamed PropertyInfoAssertions IsNotNull() to IsNotNullable() to align it with NullabilityInfo Assertions.

---


#  Version 7.16.0
## 11 Jun 2024

### AllOverIt.Mapping

* Refactored to cater for required properties. This involved removing the `new()` constraint which means classes with a default
  constructor now use reflection - slightly slower and allocates a little more memory.

* `ObjectMapperConfiguration` has been updated to provide the ability to register custom type factories. This is completely optional
  for classes with a default constructor, but if registered during configuration then there is a small performance improvement when
  a target type needs to be constructed for the first time. This option also makes it possible to register an external factory within
  DI setup.

* Added a `Map()` overload that allows for specifying source and target types.

### AllOverIt.Fixture

* Removed HasRequiredModifier() and HasNoRequiredModifier() from PropertyInfoAssertions from NET 6.
* Updated PropertyInfoAssertions to include IsOfType(), IsNullable(), IsNotNullable().
* Added NullabilityInfoAssertions

---


#  Version 7.15.1
## 03 Jun 2024

### AllOverIt

* BreadcrumbData updated so Tags defaults to [].

---


#  Version 7.15.0
## 03 Jun 2024

### AllOverIt

* Minor performance improvements
* PropertyInfo extension methods used to obtain information about a property can now specify if the request is for the getter, setter, or both.
* Added optional Tags to breadcrumb data
* Updated GetFriendlyName() to optionally include declaring types and namespace
* Breaking: Updated BindingOptions.GetMethod and BindingOptions.SetMethod to provide the ability to filter for properties based on their Get and Set
  methods. Previously, all filtering was based solely on a property's getter. If BindingOptions.All is used, then property setters will also be
  considered. The TypeExtensions.GetPropertyInfo() may return additional properties if BindingOptions.All is used.
* Breaking: Removed ability to explicitly set breadcrumb data CallerName, FilePath, LineNumber.
  Metadata and Tags can also be set via a fluent syntax after calling one of the Add() extension methods.

### AllOverIt.Fixture

* Added new property-based assertions, based on FluentAssertions. Refer to Properties.For<T>().

---


#  Version 7.14.1
## 27 May 2024

### AllOverIt.Fixture

* Moved EnrichedEnumCustomization and changed its namespace.

---


#  Version 7.14.0
## 26 May 2024

### AllOverIt.Fixture

* Added support for customizing AutoFixture's Fixture so that is can create EnrichedEnum instances.
* Added AssertThrowsWhenStringNullOrEmptyOrWhitespace() to simplify asserting an exception will be thrown when a string argument is null,
  empty, or whitespace.

---


#  Version 7.13.0
## 16 May 2024

### AllOverIt

* Added IAsyncEnumerable extension methods that have a non-async selector: SelectManyAsync(), SelectManyToArrayAsync(), SelectManyToListAsync(),
  SelectManyToReadOnlyCollectionAsync(), SelectToArrayAsync(), SelectToListAsync(), and SelectToReadOnlyCollectionAsync().

---


#  Version 7.12.1
## 15 May 2024

### AllOverIt.Logging.Testing

* Changed LogCallExpectation from internal to public

---


#  Version 7.12.0
## 15 May 2024

### AllOverIt
* Updates to EnumerableExtensions
  - Added SelectManyToArray(), SelectManyToList(), and SelectManyToReadOnlyCollection()
  - Added SelectManyAsync(), SelectManyToArrayAsync(), SelectManyToListAsync(), and SelectManyToReadOnlyCollectionAsync()

* Updates to AsyncEnumerableExtensions
  - Breaking: Corrected signature of SelectManyAsync() selector to return Task<IEnumerable<TResult>>, instead of IEnumerable<TResult>.
  - Breaking: Updated SelectAsync(), SelectToArrayAsync(), SelectToListAsync(), and SelectToReadOnlyCollectionAsync() to forward
              the CancellationToken to the selector.
  - Added ToReadOnlyCollectionAsync(), SelectManyToArrayAsync(), SelectManyToListAsync(), and SelectManyToReadOnlyCollectionAsync()

### AllOverIt.Logging :new:
* A new package containing extension methods to provide additional logging functionality.

### AllOverIt.Logging.Testing :new:
* A new package containing helper methods that assist with asserting logger calls.

---


#  Version 7.11.0
## 08 May 2024

### AllOverIt
* Fixed progress text reported by ProgressUpdater when the caller returns null so it reports the previous progress text.
* Breaking: Updated SelectManyAsync() to forward the CancellationToken to the selector.

### AllOverIt.ReactiveUI
* Added CommandFactory to create cancellable commands, and cancel commands that can automatically cancel one or more
  cancellable commands.

---


#  Version 7.10.0
## 04 May 2024

### AllOverIt
* Corrected namespace for VoidType (now AllOverIt.Types).
* Breaking: Changed name of TypeExtensions.CreateList() to TypeExtensions.CreateListOf()
* Breaking: Several sync methods updated to forward the CancellationToken to the selector: SelectAsync(),
            SelectAsReadOnlyCollectionAsync(), SelectAsReadOnlyListAsync(), SelectToReadOnlyCollectionAsync(), SelectToArrayAsync(),
            and SelectToListAsync().
* Added SelectAsParallelAsync() to asynchronously invoke selectors in parallel, and with a maximum degree of parallelism.

---


#  Version 7.9.0
## 22 Apr 2024

### AllOverIt
* Added a 'VoidType' that can be useful for observable subjects that need to emit 'no value'.
* Added an implementation of the Result pattern.

---


#  Version 7.8.1
## 18 Apr 2024

### AllOverIt.Aws.AppSync.Client
* Explicit disposal of HttpClient

---


#  Version 7.8.0
## 12 Apr 2024

### AllOverIt
* Breaking: ChainOfResponsibility async handlers updated to support CancellationToken.

---


#  Version 7.7.0
## 10 Apr 2024

### AllOverIt
* Breaking: More appropriate implementation of exception handler in BackgroundTask and the Task extension method, FireAndForget().
  They now provides the actual Exception instead of ExceptionDispatchInfo(), which the caller can create if they need to re-throw
  later (or on a different thread).

* Added RepeatingTask.StartAsync() to replace the various Start() methods. StartAsync() accepts an options class. The Start()
  methods will be removed in v8.

### AllOverIt.Serialization.Json.Newtonsoft
* Added a default constructor to NewtonsoftJsonSerializer so it can be used in a generic constraint.

### AllOverIt.Serialization.Json.SystemText
* Added a default constructor to SystemTextJsonSerializer so it can be used in a generic constraint.

---


#  Version 7.6.0
## 28 Mar 2024

### AllOverIt.Serilog
* Added an overload for AddSerilogCircularBuffer() that allows the capacity to be resolved from IServiceProvider.

---


#  Version 7.5.1
## 28 Mar 2024

### AllOverIt.Aws.Cdk.AppSync
* Maintenance package updates

### AllOverIt.ReactiveUI
* Maintenance package updates

### AllOverIt.ReactiveUI.Wpf
* Maintenance package updates

---


#  Version 7.5.0
## 27 Mar 2024

### AllOverIt
* Added an AsArray() extension method for IEnumerable.
* Breaking: Changed return type of GetEnumValues() from IReadOnlyCollection<TEnumType> to TEnumType[].
* Added new methods to extract an array of enum values for a given numerical or Enum mask value;
  GetValuesFromBitMask() and GetValuesFromEnumWithFlags(). NET 7.0 and above.

### AllOverIt.EntityFrameworkCore.Diagrams
* Change in behaviour: Added support for preserving the order of columns as they are defined on entities. Default is true.
* Changed default entity nullable column options to be Visible and use NullableColumnMode.NotNull.

### AllOverIt.Fixture.FakeItEasy
* Breaking: Changed optional ICustomization in UseFakeItEasy() to AutoFakeItEasyCustomization

---


#  Version 7.3.0 - 7.4.1
## 04 Mar 2024

### AllOverIt.Aws.Cdk.AppSync
* v7.3.0 - General cleanup / maintenance


### AllOverIt.EntityFrameworkCore.Diagrams
* v7.3.0 - Added support for grouping entities.
* v7.3.1 - Fixed an issue with a default style on grouped entities not generating a valid configuration. Added a new overload not requiring a style.
* v7.3.2 - Correct a regression with group titles not being included in the output.
* v7.4.0 - Added an option to copy global entity options when first retrieving them for customization. This is a change in behaviour, but it is more appropriate.
* v7.4.1 - Corrected copying of global 'NotNull' label.

---


#  Version 7.2.0
## 23 Feb 2024

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


---


#  Version 7.1.1
## 19 Feb 2024

### AllOverIt.Validation
* Fixed issue with scoped / transient validators being incorrectly registered with LifetimeValidationInvoker.


---


#  Version 7.1.0
## 09 Feb 2024

### AllOverIt
* Added CircularBuffer&lt;TType&gt; now inherits from ICircularBuffer&lt;TType&gt;


### AllOverIt.Validation
* Added `ILifetimeValidationInvoker` as an alternative to `IValidationInvoker`. The existing `ValidationInvoker` can only be used
  with validators containing a default constructor, and all validators are treated as a Singleton. `ILifetimeValidationInvoker`
  adds support for registering validators with a Transient, Scoped, or Singleton scope, and they can be injected with dependencies.
  A `ILifetimeValidationInvoker` is registered as a Singleton via the `AddLifetimeValidationInvoker()` extension method on
  `IServiceCollection`. A `AddValidationInvoker()` extension method has also been added to simplify using `IValidationInvoker`.

  The extension methods return `IValidationRegistry`/ `ILifetimeValidationRegistry` so additional validator registrations can be
  added after the invoker has been registered, but before the `IServiceCollection` has been built.


### AllOverIt.Serilog :new:
* Provides a serilog sink that will capture log events and store them in a fixed-size circular buffer. That buffer can be
  independently injected elsewhere in the application for the purpose of further diagnostic processing.

  Provides a ThreadIEnricher that captures the current ThreadId.


---


#  Version 7.0.0
## 20 Jan 2024

### General Notes
* Multiple dependency package updates across most packages. These are not noted below. Refer to the dependency
  diagram available in the `\Docs\Reference\Content\images\dependencies` folder.


### AllOverIt
* Added interceptor support to cater for Aspect Oriented Programming (AOP)
* Added string extension `IsEmpty()`
* `IAsyncEnumerable` extension updates
* Added async version of `ChainOfResponsibilityHandler` and `ChainOfResponsibilityComposer`
* Added a pipeline pattern implementation
* Added `AllOverIt.Plugin.PluginLoadContext` to dynamically load assemblies (for .NET Core 3.1 and above)
* Fixed some `Breadcrumbs` overloads that were not recording the caller details
* Updates to string comparison utils (expressions) to detect null constant values and convert / throw as required
* General tidy up of all exception classes
* Deprecated `AllOverIt.Process`, replacing it with `ProcessExecutor`
* Added Task extensions `FireAndForget()`
* Removed unnecessary 'continueOnCapturedContext' argument from `TaskHelper.WhenAll()`
* Added FileUtils `CreateFileWithContentAsync()`
* Added CommandLine argument helper to escape strings
* Added `IComparer` extension methods `Reverse()` and `Then()`. The latter allows them to be composed like Chain of Responsibility.
* Added object extension `GetObjectElements()`
* Unseal `ReadOnlyCollection`, `ReadOnlyList`, `ReadOnlyDictionary`
* Changed `RepeatingTask.Start()` to use overloads rather than a default initial delay (previously after the `CancellationToken`)
* Added ColorConsoleLogger
* Moved QueryableExtensions from AllOverIt.Patterns.Specification.Extensions to AllOverIt.Extensions
* Add overloads for RepeatingTask that accept a Timespan
* Added BackgroundTask and BackgroundTask\<TResult>
* Added SemaphoreSlim DisposableWaitAsync() extension method
* Added a CancellationToken to ForEachAsync(), ForEachAsTaskAsync(), ForEachAsParallelAsync()
* Added Type extension methods GetEnumerableElementType(), CreateList()
* Added SelectManyAsync() extension method
* Added support for executing a Process and not wait for it to complete
* Added RSA, AES and RSA-AES hybrid encryption helpers
* Added Stream extensions to convert to and from a byte array
* Added ProgressUpdater
* Added cancellation support in the async pipeline builder
* Updated IElementDictionary to be enumerable, provide Names and Values, and the ability to change values within the json helper
* Added DisposeWith() for any IAsyncDisposable, using CompositeAsyncDisposable
* Removed TextStreamer since System.IO.StringWriter provides an almost identical interface
* Added ToMemoryStream() extension method for string.
* Renamed IDictionary extension method from Concat() to Combine() to avoid namespace clashes
* Renamed IList extension method from AddRange() to AddMany() to avoid namespace clashes
* Changed generic cache keys to be records
* Added support for custom value converters in LinqSpecificationVisitor
* Added object extensions to invoke methods using reflection


### AllOverIt.AspNetCore
* Unchanged


### AllOverIt.Assertion
* Added WhenNotNullOrEmpty() overloads to the static Throw class.


### AllOverIt.Aws.AppSync.Client
* Update to detection of execution timeout due to AWS changes


### AllOverIt.Aws.Cdk.AppSync
* Updated from Amazon CDK v1 to Amazon.CDK.Lib v2
* Added an EndpointSource of type "Lookup" that allows for custom values to be looked up when creating a HttpDataSource


### AllOverIt.Cryptography :new:
* Provides a simple approach to using RSA, AES, and RSA-AES hybrid encryption.


### AllOverIt.Csv
* Renamed CsvExportException to CsvSerializerException
* Added buffered CSV exporter base classes


### AllOverIt.DependencyInjection
* Added support for registering named services


### AllOverIt.EntityFrameworkCore
* Added a database migrator that emits all pending migrations to be applied
* Added a utility to get the current migration status of a database


### AllOverIt.EntityFrameworkCore.Diagrams :new:
* Adds support for generating D2 entity relationship diagrams and exporting to text, svg, png, pdf


### AllOverIt.EntityFrameworkCore.Pagination
* Unchanged


### AllOverIt.Evaluator
* Modified VariableRegistry to be enumerable (removed Variables property)
* Added methods to IVariableRegistry: TryGetVariable(), GetVariable(), ContainsVariable()
* Added a VariableRegistryBuilder that allows registrations to be added out of sequence


### AllOverIt.Filtering
* Unchanged


### AllOverIt.Fixture
* Added exception assertion helpers


### AllOverIt.Fixture.FakeItEasy
* Unchanged


### AllOverIt.GenericHost
* Unchanged


### AllOverIt.Mapping :new:
* Moved `ObjectMapper` from `AllOverIt` into this dedicated package
* Updated support `ArrayList`


### AllOverIt.Pagination
* Added `GetPageResults()` extension method for `IQueryPaginator<TResult>`. This is intended ONLY for memory based queries.
  It Performs the same job as `GetPageResultsAsync()` found in `AllOverIt.EntityFrameworkCore.Pagination`.


### AllOverIt.Pipes
* Provides support for named client and server pipes (Windows only).


### AllOverIt.Reactive
* Added ObservableObject, ObservableProxy and PropertyNotifyExtensions
* Added EventBusHandler


### AllOverIt.ReactiveUI
* Added support for a `ReactiveCommand` based pipeline (with support for chaining other sync / async pipeline steps)
* Added ReactiveProxy
* Added ViewRegistry so multiple view instances of various ViewModel types can be created and tracked


### AllOverIt.ReactiveUI.Wpf :new:
* Added WpfViewHandler for use with ViewRegistry in AllOverIt.ReactiveUI
* Added helpers to register ReactiveWindow / ReactiveUserControl and their associated ViewModel with IServiceCollection


### AllOverIt.Serialization.Binary :new:
* Moved the binary serialization from `AllOverIt` into this dedicated package
* Improved support for IEnumerable and arrays
* Added DynamicBinaryValueReader and DynamicBinaryValueWriter


### AllOverIt.Serialization.Json.Abstractions
* Renamed from `AllOverIt.Serialization.Abstractions`. If upgrading from a previous version then the package reference
  and usages of the `AllOverIt.Serialization.Abstractions` namespace will require manual updates.


### AllOverIt.Serialization.Json.Newtonsoft
* Renamed from `AllOverIt.Serialization.NewtonsoftJson`. If upgrading from a previous version then the package reference
  and usages of the `AllOverIt.Serialization.NewtonsoftJson` namespace will require manual updates.


### AllOverIt.Serialization.Json.SystemText
* Renamed from `AllOverIt.Serialization.SystemTextJson`. If upgrading from a previous version then the package reference
  and usages of the `AllOverIt.Serialization.SystemTextJson` namespace will require manual updates.
* Added an option to `NestedDictionaryConverter` so floating values can be read as double or decimal.


### AllOverIt.Validation
* Unsealed ValidationInvoker so multiple types can be registered with DI.


### AllOverIt.Validation.Options :new:
* Provides support for Configuration Options validation using `FluentValidation` and `AllOverIt.Validation` via the
  `OptionsBuilder<TOptions>` extension method called `UseFluentValidation()`.


### AllOverIt.Wpf :new:
* General purpose WPF utilities


### AllOverIt.Wpf.Controls :new:
* Includes a `PreviewTextBox` control
* Includes a 'ExecuteCommandOnEnterKeyBehavior' behavior
