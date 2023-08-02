#  Version 7.0.0
## \<Date>
---

# General Notes
* Multiple dependency package updates across most packages. These are not noted below. Refer to the dependency
  diagram available in the `/Docs/Dependencies` folder.


## AllOverIt
* Added interceptor support to cater for Aspected Oriented Programming (AOP)
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
* Added commandline argument helper to escape strings
* Added `IComparer` extension methods `Reverse()` and `Then()`. The latter allows them to be composed like Chain of Responsibility.
* Added object extension `GetObjectElements()`
* Unseal `ReadOnlyCollection`, `ReadOnlyList`, `ReadOnlyDictionary`
* Changed `RepeatingTask.Start()` to use overloads rather than a default initial delay (previously after the `CancellationToken`)
* Added ColorConsoleLogger
* Moved QueryableExtensions from AllOverIt.Patterns.Specification.Extensions to AllOverIt.Extensions
* Add overloads for RepeatingTask that accept a Timespan
* Added BackgroundTask and BackgroundTask\<TResult>
* Added SemaphoreSlim DisposableWaitAsync() extension method
* Added a CancellationToken to ForEachAsync()
* Added Type extension methods GetEnumerableElementType(), CreateList()
* Added SelectManyAsync() extension method
* Added support for executing a Process and not wait for it to complete
* Added RSA, AES and RSA-AES hybrid encryption helpers


## AllOverIt.AspNetCore
* Unchanged


## AllOverIt.Assertion
* Unchanged


## AllOverIt.Aws.Appsync.Client
* Unchanged


## AllOverIt.Aws.Cdk.Appsync
* Updated from Amazon CDK v1 to Amazon.CDK.Lib v2
* Added an EndpointSource of type "Lookup" that allows for custom values to be looked up when creating a HttpDataSource


## AllOverIt.Cryptography :new:
* Provides a simple approach to using RSA, AES, and RSA-AES hybrid encryption.


## AllOverIt.Csv
* Unchanged


## AllOverIt.DependencyInjection
* Added support for registering named services


## AllOverIt.EntityFrameworkCore
* Added a database migrator that emits all pending migrations to be applied
* Added a utility to get the current migration status of a database


## AllOverIt.EntityFrameworkCore.Diagrams :new:
* Adds support for generating D2 entity relationship diagrams and exporting to text, svg, png, pdf


## AllOverIt.EntityFrameworkCore.Pagination
* Unchanged


## AllOverIt.Evaluator
* Unchanged


## AllOverIt.Filtering
* Unchanged


## AllOverIt.Fixture
* Added exception assertion helpers


## AllOverIt.Fixture.FakeItEasy
* Unchanged


## AllOverIt.GenericHost
* Unchanged


## AllOverIt.Mapping :new:
* Moved `ObjectMapper` from `AllOverIt` into this dedicated package
* Updated support `ArrayList`


## AllOverIt.Pagination
* Added `GetPageResults()` extension method for `IQueryPaginator<TResult>`. This is intended ONLY for memory based queries.
  It Performs the same job as `GetPageResultsAsync()` found in `AllOverIt.EntityFrameworkCore.Pagination`.


## AllOverIt.Pipes
* Provides support for named client and server pipes.


## AllOverIt.Reactive
* Added ObservableObject, ObservableProxy and PropertyNotifyExtensions
* Added EventBusHandler


## AllOverIt.ReactiveUI
* Added support for a `ReactiveCommand` based pipeline (with support for chaining other sync / async pipeline steps)
* Added ReactiveProxy
* Added ViewRegistry so multiple view instances of the same view model type can be tracked


## AllOverIt.Serialization.Binary :new:
* Moved the binary serialization from `AllOverIt` into this dedicated package
* Improved support for IEnumerable and arrays
* Added DynamicBinaryValueReader and DynamicBinaryValueWriter


## AllOverIt.Serialization.Json.Abstractions
* Renamed from `AllOverIt.Serialization.Abstractions`. If upgrading from a previous version then the package reference
  and usages of the `AllOverIt.Serialization.Abstractions` namespace will require manual updates.


## AllOverIt.Serialization.Json.Newtonsoft
* Renamed from `AllOverIt.Serialization.NewtonsoftJson`. If upgrading from a previous version then the package reference
  and usages of the `AllOverIt.Serialization.NewtonsoftJson` namespace will require manual updates.


## AllOverIt.Serialization.Json.SystemText
* Renamed from `AllOverIt.Serialization.SystemTextJson`. If upgrading from a previous version then the package reference
  and usages of the `AllOverIt.Serialization.SystemTextJson` namespace will require manual updates.
* Added an option to `NestedDictionaryConverter` so floating values can be read as double or decimal.


## AllOverIt.Validation
* Unsealed ValidationInvoker so multiple types can be registered with DI.


## AllOverIt.Validation.Options :new:
* Provides support for Configuration Options validation using `FluentValidation` and `AllOverIt.Validation` via the
  `OptionsBuilder<TOptions>` extension method called `UseFluentValidation()`.


## AllOverIt.Wpf :new:
* General purpose WPF utilities


## AllOverIt.Wpf.Controls :new:
* Includes a `PreviewTextBox` control
* Includes a 'ExecuteCommandOnEnterKeyBehavior' behavior
