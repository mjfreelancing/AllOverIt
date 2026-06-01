# AllOverIt.DependencyInjection

## Purpose
This package provides useful dependency injection related utilities such as registering and resolving 'named' services, auto service discovery and registration via a service registrar, and service decoration.

## Package Metadata
- Project: Source/AllOverIt.DependencyInjection/AllOverIt.DependencyInjection.csproj
- Target frameworks: net10.0;net9.0;net8.0
- Tags: AllOverIt, Utilities, DependencyInjection
- Depends on: AllOverIt

## Public API Signals (from source)
### Representative public types
- class ServiceCollectionExtensions
- interface INamedServiceResolver
- class DependencyRegistrationException
- class ServiceProviderExtensions
- class ServiceRegistrarBase
- interface INamedServiceBuilder
- interface INamedServiceRegistration
- interface IServiceRegistrar
- interface IServiceRegistrarOptions

### Active namespaces
- AllOverIt.DependencyInjection
- AllOverIt.DependencyInjection.Extensions
- AllOverIt.DependencyInjection.Exceptions

## Demo Projects
- Demos/AllOverIt.DependencyInjection/AutoRegistrationDemo/AutoRegistrationDemo.csproj
- Demos/AllOverIt.DependencyInjection/ExternalDependencies/ExternalDependencies.csproj
- Demos/AllOverIt.DependencyInjection/NamedDependenciesDemo/NamedDependenciesDemo.csproj

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
