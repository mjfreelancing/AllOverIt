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

        /// <summary>Constructor.</summary>
        /// <param name="formulaExpression">The expression built from a processed formula.</param>
        /// <param name="referencedVariableNames">A collection of all variable names explicitly referenced by the formula.</param>
        internal FormulaProcessorResult(Expression<Func<double>> formulaExpression, IReadOnlyCollection<string> referencedVariableNames)
        {
            // Note: referencedVariableNames is passed as IReadOnlyCollection<string> for performance reasons (from the FormulaProcessor)
            FormulaExpression = formulaExpression.WhenNotNull(nameof(formulaExpression));
            ReferencedVariableNames = referencedVariableNames.WhenNotNull(nameof(referencedVariableNames));
        }
    }
}