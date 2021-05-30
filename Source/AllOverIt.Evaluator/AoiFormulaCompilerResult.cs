using AllOverIt.Helpers;
using System;
using System.Collections.Generic;

namespace AllOverIt.Evaluator
{
    // A class containing a delegate and a list of referenced variables resulting from the compilation of a formula.
    public sealed class AoiFormulaCompilerResult
    {
        // The delegate that returns the result of the formula when invoked.
        public Func<double> Resolver { get; }

        // An enumerable of all variable names explicitly referenced by the formula.
        public IEnumerable<string> ReferencedVariableNames { get; }

        public AoiFormulaCompilerResult(Func<double> resolver, IEnumerable<string> referencedVariableNames)
        {
            Resolver = resolver.WhenNotNull(nameof(resolver));
            ReferencedVariableNames = referencedVariableNames.WhenNotNull(nameof(referencedVariableNames));
        }
    }
}