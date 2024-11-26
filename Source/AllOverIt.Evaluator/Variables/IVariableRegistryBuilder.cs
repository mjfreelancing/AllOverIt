using AllOverIt.Evaluator.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.Evaluator.Variables
{
    /// <summary>Represents a builder for an <see cref="IVariableRegistry"/> that will only allow the registry to be
    /// constructed if all variables referenced by all formulae are registered. If none of the variables are formula
    /// based then <see cref="IVariableRegistry"/> can be used directly.</summary>
    public interface IVariableRegistryBuilder
    {
        /// <summary>Adds a new constant variable for inclusion in the variable registry.</summary>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="value">The constant value to be assigned to the variable.</param>
        /// <returns>The current builder to provide a fluent syntax.</returns>
        IVariableRegistryBuilder AddConstantVariable(string name, double value);

        /// <summary>Adds a new delegate-based variable for inclusion in the variable registry.</summary>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="valueResolver">The value delegate to be assigned to the variable. This delegate will be called
        /// each time the variable is evaluated.</param>
        /// <returns>The current builder to provide a fluent syntax.</returns>
        IVariableRegistryBuilder AddDelegateVariable(string name, Func<double> valueResolver);

        /// <summary>Adds a new delegate-based variable for inclusion in the variable registry.</summary>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="formulaCompilerResult">The result of a compiled formula.</param>
        /// <returns>The current builder to provide a fluent syntax.</returns>
        IVariableRegistryBuilder AddDelegateVariable(string name, FormulaCompilerResult formulaCompilerResult);

        /// <summary>Adds a new delegate-based variable for inclusion in the variable registry.</summary>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="formulaCompilerResultResolver">A delegate that will return a formula compiled result.
        /// This delegate will be called each time the variable is evaluated so only use this overload if that
        /// is the intended behaviour.</param>
        /// <returns>The current builder to provide a fluent syntax.</returns>
        IVariableRegistryBuilder AddDelegateVariable(string name, Func<IVariableRegistry, FormulaCompilerResult> formulaCompilerResultResolver);

        /// <summary>Adds a new lazily-initialized variable for inclusion in the variable registry.</summary>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="valueResolver">The initial lazily-evaluated delegate to be assigned to the variable.</param>
        /// <param name="threadSafe">Indicates if the underlying lazy-evaluator should evaluate in a thread safe manner.</param>
        /// <returns>The current builder to provide a fluent syntax.</returns>
        IVariableRegistryBuilder AddLazyVariable(string name, Func<double> valueResolver, bool threadSafe = false);

        /// <summary>Adds a new lazily-initialized variable for inclusion in the variable registry.</summary>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="formulaCompilerResult">The compiled result of a formula. The associated resolver will be evaluated
        /// the first time the variable is evaluated.</param>
        /// <param name="threadSafe">Indicates if the underlying lazy-evaluator should evaluate in a thread safe manner.</param>
        /// <returns>The current builder to provide a fluent syntax.</returns>
        IVariableRegistryBuilder AddLazyVariable(string name, FormulaCompilerResult formulaCompilerResult, bool threadSafe = false);

        /// <summary>Adds a new lazily-initialized variable for inclusion in the variable registry.</summary>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="formulaCompilerResultResolver">A delegate that will return a formula compiled result. The associated resolver
        /// will be evaluated the first time the variable is evaluated.</param>
        /// <param name="threadSafe">Indicates if the underlying lazy-evaluator should evaluate in a thread safe manner.</param>
        /// <returns>The current builder to provide a fluent syntax.</returns>
        IVariableRegistryBuilder AddLazyVariable(string name, Func<IVariableRegistry, FormulaCompilerResult> formulaCompilerResultResolver, bool threadSafe = false);

        /// <summary>Adds a new mutable variable for inclusion in the variable registry.</summary>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="value">The constant value to be assigned to the variable.</param>
        /// <returns>The current builder to provide a fluent syntax.</returns>
        IVariableRegistryBuilder AddMutableVariable(string name, double value = 0);

        /// <summary>Confirms all formula-based referenced variables are resolvable and returns the fully populated variable registry.</summary>
        /// <returns>The current builder to provide a fluent syntax.</returns>
        /// <exception cref="VariableRegistryBuilderException">When one or more formula-based referenced variables has not been registered.</exception>
        [return: NotNull]
        IVariableRegistry Build();

        /// <summary>Confirms all formula-based referenced variables are resolvable and returns the fully populated variable registry.</summary>
        /// <param name="variableRegistry">When successfully built, the fully populated variable registry, otherwise <see langword="null"/>.</param>
        /// <returns><see langword="True"/> when the variable registry can be returned, otherwise <see langword="False"/>.</returns>
        bool TryBuild([NotNullWhen(true)] out IVariableRegistry? variableRegistry);

        /// <summary>Gets a collection of all unregistered variable names referenced by any formula-based variables.</summary>
        string[] GetUnregisteredVariableNames();
    }
}