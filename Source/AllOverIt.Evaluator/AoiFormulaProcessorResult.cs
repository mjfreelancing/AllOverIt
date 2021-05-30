using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator
{
    // Contains the result of parsing and processing a formula.
    public sealed class AoiFormulaProcessorResult
    {
        // Gets the expression built from a processed formula. When this expression is compiled and invoked the value of the formula is returned.
        public Expression<Func<double>> FormulaExpression { get; }

        // Gets an enumerable of all variable names explicitly referenced by the formula.
        public IEnumerable<string> ReferencedVariableNames { get; }

        public AoiFormulaProcessorResult(Expression<Func<double>> formulaExpression, IEnumerable<string> referencedVariableNames)
        {
            FormulaExpression = formulaExpression.WhenNotNull(nameof(formulaExpression));
            ReferencedVariableNames = referencedVariableNames.WhenNotNull(nameof(referencedVariableNames)).AsReadOnlyList();
        }
    }
}