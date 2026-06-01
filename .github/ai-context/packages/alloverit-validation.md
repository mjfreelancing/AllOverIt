# AllOverIt.Validation

## Purpose
This package contains additional validators and extensions for use with FluentValidation. It includes an invoker that will select, and invoke, a registered validator based on a provided model. Synchronous and Asynchronous validation is supported, along with extensions that make it easier to pass context information into a validator to keep it stateless. Also available is a validation registrar that makes it possible to auto-register all validators within the same assembly. Validators can also be filtered based on a predicate.

## Package Metadata
- Project: Source/AllOverIt.Validation/AllOverIt.Validation.csproj
- Target frameworks: net10.0;net9.0;net8.0
- Tags: AllOverIt, Utilities, Serialization, Deserialization, JSON, Validation
- Depends on: AllOverIt

## Public API Signals (from source)
### Representative public types
- class RuleBuilderExtensions
- class IsUniqueValidator
- class AbstractValidatorExtensions
- class ContextComparisonValidator
- class ContextRangeValidator
- class ExclusiveBetweenContextValidator
- class GreaterThanContextValidator
- class GreaterThanOrEqualToContextValidator
- class InclusiveBetweenContextValidator
- class LessThanContextValidator
- class LessThanOrEqualToContextValidator
- class LifetimeValidationRegistrarBase

### Representative extension methods
- AddLifetimeValidationInvoker
- AddValidationInvoker

### Active namespaces
- AllOverIt.Validation.Extensions
- AllOverIt.Validation
- AllOverIt.Validation.Validators
- AllOverIt.Validation.Exceptions

## Demo Projects
- Demos/AllOverIt.Validation/BasicValidationDemo/BasicValidationDemo.csproj
- Demos/AllOverIt.Validation/ValidationInvokerDemo/ValidationInvokerDemo.csproj
- Demos/AllOverIt.Validation/ValidationViaDependencyInjectionDemo/ValidationViaDependencyInjectionDemo.csproj

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
