using System;
using System.Collections.Generic;

namespace AllOverIt.Evaluator.Variables
{
    /// <summary>Represents a factory for creating variables.</summary>
    public interface IVariableFactory
    {
        /// <summary>Creates a new instance of a variable registry.</summary>
        /// <returns>A new instance of a variable registry.</returns>
        IVariableRegistry CreateVariableRegistry();

        /// <summary>Creates a new constant variable.</summary>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="value">The constant value to be assigned to the variable.</param>
        /// <returns>The new variable instance.</returns>
        IVariable CreateConstantVariable(string name, double value = default);

        /// <summary>Creates a new mutable variable.</summary>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="value">The initial value to be assigned to the variable.</param>
        /// <returns>The new variable instance.</returns>
        IMutableVariable CreateMutableVariable(string name, double value = default);

        /// <summary>Creates a new delegate variable that will be re-evaluated each time its value is read.</summary>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="valueResolver">The initial delegate to be assigned to the variable.</param>
        /// <returns>The new variable instance.</returns>
        IVariable CreateDelegateVariable(string name, Func<double> valueResolver);

        /// <summary>Creates a new lazily-evaluated delegate variable.</summary>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="valueResolver">The initial lazily-evaluated delegate to be assigned to the variable.</param>
        /// <returns>The new variable instance.</returns>
        ILazyVariable CreateLazyVariable(string name, Func<double> valueResolver, bool threadSafe = false);

        /// <summary>Creates a delegate-based variable that will invoke one or more value resolvers and aggregate their values.</summary>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="valueResolvers">The value resolvers to be evaluated and aggregated. These will be re-evaluated each
        ///  time the created variable is evaluated.</param>
        /// <returns>The new variable instance.</returns>
        IVariable CreateAggregateVariable(string name, params Func<double>[] valueResolvers);

        /// <summary>Creates a delegate-based variable that calculates the sum of registered variables by their name.</summary>
        /// <param name="name">The name to be assigned to the variable.</param>
        /// <param name="variableRegistry">The variable registry containing the named variables to be aggregated.</param>
        /// <param name="variableNames">The names of the variables to be aggregated. This cannot be an empty collection.
        /// If null is provided then all variables within the registry will be aggregated.</param>
        /// <returns>The new variable instance.</returns>
        IVariable CreateAggregateVariable(string name, IVariableRegistry variableRegistry, IEnumerable<string> variableNames = null);
    }
}