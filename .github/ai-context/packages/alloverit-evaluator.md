# AllOverIt.Evaluator

## Purpose
This package contains a highly-extendable mathematical expression compiler and evaluator that includes constant, mutable, delegate, and lazily evaluated variables, offers implicit and explicit variable lookup, and supports custom arithmetic operators and operations.

## Package Metadata
- Project: Source/AllOverIt.Evaluator/AllOverIt.Evaluator.csproj
- Target frameworks: net10.0;net9.0;net8.0
- Tags: AllOverIt, Utilities, Formula, Mathematical, Compiler, Evaluator
- Depends on: AllOverIt

## Public API Signals (from source)
### Representative public types
- class AbsOperation
- class AbsOperator
- class AcosOperation
- class AcosOperator
- class AddOperator
- class ArithmeticOperation
- class ArithmeticOperationBase
- class ArithmeticOperationFactory
- class AsinOperation
- class AsinOperator
- class AtanOperation
- class AtanOperator

### Representative extension methods
- Add
- AddDelegateVariable
- AddLazyVariable
- AddConstantVariable
- AddMutableVariable
- GetAllReferencedVariables
- GetReferencedVariables
- GetResult
- SetVariableRegistry

### Active namespaces
- AllOverIt.Evaluator.Operators
- AllOverIt.Evaluator.Operations
- AllOverIt.Evaluator.Variables
- AllOverIt.Evaluator
- AllOverIt.Evaluator.Exceptions
- AllOverIt.Evaluator.Variables.Extensions
- AllOverIt.Evaluator.Extensions

## Demo Projects
- Demos/AllOverIt.Evaluator/BasicEvaluationDemo/BasicEvaluationDemo.csproj
- Demos/AllOverIt.Evaluator/CustomOperationDemo/CustomOperationDemo.csproj
- Demos/AllOverIt.Evaluator/DelegateEvaluationDemo/DelegateEvaluationDemo.csproj
- Demos/AllOverIt.Evaluator/MixedEvaluationDemo/MixedEvaluationDemo.csproj
- Demos/AllOverIt.Evaluator/ParallelEvaluationDemo/ParallelEvaluationDemo.csproj
- Demos/AllOverIt.Evaluator/VariableEvaluationDemo/VariableEvaluationDemo.csproj
- Demos/AllOverIt.Evaluator/VariableLookupDemo/VariableLookupDemo.csproj

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
