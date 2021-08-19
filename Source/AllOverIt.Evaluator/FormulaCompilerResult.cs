using AllOverIt.Evaluator.Variables;
using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;

namespace AllOverIt.Evaluator
{
    /// <summary>A class containing a delegate and a list of referenced variables resulting from the compilation of a formula.</summary>
    public sealed record FormulaCompilerResult
    {
        /// <summary>The variable registry reference by the compiled formula. This will be null if the formula compiler was not provided
        /// a variable registry and the formula did not contain any variables.</summary>
        public IVariableRegistry VariableRegistry { get; }

        /// <summary>The compiled delegate that returns the result of the formula when invoked.</summary>
        public Func<double> Resolver { get; }

        /// <summary>An enumerable of all variable names explicitly referenced by the formula.
        /// This will be null if the formula did not have any variables.</summary>
        public IReadOnlyCollection<string> ReferencedVariableNames { get; }

        /// <summary>Constructor.</summary>
        /// <param name="variableRegistry">The variable registry reference by the compiled formula. Variables can be added at any time
        /// prior to evaluation.</param>
        /// <param name="resolver">The compiled delegate that when invoked will return the result of the formula.</param>
        /// <param name="referencedVariableNames">An enumerable of all variable names explicitly referenced by the formula. This will be
        /// null if the formula did not have any variables.</param>
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