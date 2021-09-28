using AllOverIt.Evaluator.Variables;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator
{
    /// <summary>Contains the result of parsing and processing a formula.</summary>
    public sealed record FormulaProcessorResult
    {
        /// <summary>Gets the expression built from a processed formula. When this expression is compiled and invoked
        /// the value of the formula is returned.</summary>
        public Expression<Func<double>> FormulaExpression { get; }

        /// <summary>Gets an enumerable of all variable names explicitly referenced by the formula.</summary>
        public IReadOnlyCollection<string> ReferencedVariableNames { get; }

        /// <summary>The variable registry that will be referenced by the compiled expression during evaluation.
        /// This may be null if the formula does not contain any variables.</summary>
        /// <remarks>If the compiler is provided a variable registry then this property will refer to the same reference
        /// even if the formula did not contain any variables.</remarks>
        public IVariableRegistry VariableRegistry { get; }

        /// <summary>Constructor.</summary>
        /// <param name="formulaExpression">The expression built from a processed formula.</param>
        /// <param name="referencedVariableNames">A collection of all variable names explicitly referenced by the formula.</param>
        /// <param name="variableRegistry">The variable registry that will be referenced by the compiled expression during evaluation.</param>
        internal FormulaProcessorResult(Expression<Func<double>> formulaExpression, IReadOnlyCollection<string> referencedVariableNames, IVariableRegistry variableRegistry)
        {
            // Note: referencedVariableNames is passed as IReadOnlyCollection<string> for performance reasons (from the FormulaProcessor)
            FormulaExpression = formulaExpression.WhenNotNull(nameof(formulaExpression));
            ReferencedVariableNames = referencedVariableNames.WhenNotNull(nameof(referencedVariableNames));
            VariableRegistry = variableRegistry;    // can be NULL if there were no variables in the formula
        }
    }
}