using AllOverIt.Extensions;
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
        /// <param name="referencedVariableNames">An enumerable of all variable names explicitly referenced by the formula.</param>
        public FormulaProcessorResult(Expression<Func<double>> formulaExpression, IEnumerable<string> referencedVariableNames)
        {
            FormulaExpression = formulaExpression.WhenNotNull(nameof(formulaExpression));

            ReferencedVariableNames = referencedVariableNames
                .WhenNotNull(nameof(referencedVariableNames))
                .AsReadOnlyCollection();
        }
    }
}