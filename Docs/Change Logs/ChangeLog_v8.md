#  Version 8.0.0
## XX XXX 2024


### Future Consideration
* Update EnrichedEnumModelBindingDemo so it is based on minimal API and the built-in problem details.
* Run BinarySerializationBenchmark - see if the reader/writer can be made more efficient.
* Add alternative to ServiceRegistrar to provide a fluent syntax that caters for similar functionality.
  plus the ability to resolve the same instance for multiple interfaces, and handle open generics.
* Deprecate FakeItEasy package. Switch to NSubstitute, including incorporating TestUtils project.


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
