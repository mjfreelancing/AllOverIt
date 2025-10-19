# NET 10 Updates - v9.0.0-rc.x

### AllOverIt
* EnumerableExtensions / AsyncEnumerableExtensions updates - Change Task to ValueTask and remove ToListAsync()
  and ToArrayAsync() methods for NET 10 and above as these are not available in the runtime.
* Added support for ReadOnlyMemory<T> as an input type in relevant methods.
* Included extension methods for IAsyncEnumerable<T>.
* Added a mockable ZipPackage to support the creation of zip files.

### AllOverIt.ReactiveUI
* Updated CommandFactory to support providing a scheduler when creating cancellable commands.

---

#  Version 8.1.0
## 24 Mar 2025

### General
* Multiple packages updated to use the latest version of their dependencies

### AllOverIt
* Made CreatePropertyGetterExpressionLambda<TType>(PropertyInfo propertyInfo) public
* Added CreatePropertyGetterExpressionLambda<TType, TProperty>(string propertyName)
* Added CreatePropertyGetter<TType, TProperty>(string propertyName)
* Made CreatePropertySetterExpressionLambda<TType>(PropertyInfo propertyInfo) public
* Made CreatePropertySetterExpressionLambda<TType, TProperty>(PropertyInfo propertyInfo) public
* Added CreatePropertySetter<TType, TProperty>(string propertyName()
* Applied some nullable reference corrections in EnrichedResult
* Added a IsUnique() validator that caters for up to four properties
* Added SetPropertyPathValue() and TrySetPropertyPathValue() object extensions that set a property value based on its path.
* Add support for binary read/write DateOnly and TimeOnly

### AllOverIt.EntityFrameworkCore.Pagination
* Add support for DateOnly and TimeOnly

### AllOverIt.Pagination
* Add support for DateOnly and TimeOnly

### AllOverIt.Validation
* Fixed issue with the lifetime validation invoker disposing of IDisposable dependencies on the validator before the validation was invoked.

---

#  Version 8.0.0
## 26 Nov 2024


## .NET Versions Supported
* NET 8
* NET 9


### Applicable to all packages
* Dropped support for NET Standard 2.1, NET 6 and NET 7.


### AllOverIt
* EnrichedEnum previously threw ArgumentNullException when comparing to a null.
* Updated ObjectExtensions.InvokeMethod() to support return values.
* Added LockableList.
* Moved event bus and handlers to the AllOverIt.Reactive.Messaging namespace.
* Renamed `ElementDictionaryExtensions.TryGetManyObjectArrayValues<TValue>()` to `ElementDictionaryExtensions.TryGetManyObjectValues<TValue>()`.
* Methods such as `ElementDictionaryExtensions.TryGetDescendantObjectArray()` previously returned an empty array for
  the out argument when the method returned false. These methods now return null for this argument.
* Removed extension methods already available as properties on `Type`: IsGenericType(), IsEnumType(), IsClassType(), IsValueType(), IsPrimitiveType()


### AllOverIt
* Added overloads for each of the Throw<T> methods that allows exception arguments to be lazily resolved.


### AllOverIt.Aws.Cdk.AppSync
* [Breaking Change] Complete rework of how resolvers and request/response handlers are configured. Datasources can
  now be shared and resolvers now support VTL and JS Code based request/response handlers.
* Added support for event bridge datasources.


### AllOverIt.Cryptography
* Refactored RsaAesHybridEncryptor to use CryptographicOperations.HashData() for NET 9.


### AllOverIt.Pipes
* Internal improvements to anonymous and named pipes.


### AllOverIt.Reactive
* Moved event bus and handlers to the AllOverIt.Reactive.Messaging namespace.


### AllOverIt.Serilog
* Added ObservableSink.

---

### For Future Consideration
* Update EnrichedEnumModelBindingDemo so it is based on minimal API and the built-in problem details.
* Run BinarySerializationBenchmark - see if the reader/writer can be made more efficient.
* Add alternative to ServiceRegistrar to provide a fluent syntax that caters for similar functionality.
  plus the ability to resolve the same instance for multiple interfaces, and handle open generics.
* Deprecate FakeItEasy package. Switch to NSubstitute, including incorporating TestUtils project.
