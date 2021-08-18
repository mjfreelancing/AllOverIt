using AllOverIt.Evaluator.Variables;
using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;

namespace AllOverIt.Evaluator
{
    // A class containing a delegate and a list of referenced variables resulting from the compilation of a formula.
    public sealed record FormulaCompilerResult
    {
        /// <summary>The variable registry reference by the compiled formula. This will be null if the formula compiler was not provided
        /// a variable registry and the formula did not contain any variables.</summary>
        public IVariableRegistry VariableRegistry { get; }

        // The delegate that returns the result of the formula when invoked.
        public Func<double> Resolver { get; }

        // An enumerable of all variable names explicitly referenced by the formula.
        public IReadOnlyCollection<string> ReferencedVariableNames { get; }

        public FormulaCompilerResult(IVariableRegistry variableRegistry, Func<double> resolver, IEnumerable<string> referencedVariableNames)
        {
            VariableRegistry = variableRegistry;                    // will be null if the formula contained no variables
            Resolver = resolver.WhenNotNull(nameof(resolver));

            ReferencedVariableNames = referencedVariableNames
                .WhenNotNull(nameof(referencedVariableNames))
                .AsReadOnlyCollection();
        }
    }
}