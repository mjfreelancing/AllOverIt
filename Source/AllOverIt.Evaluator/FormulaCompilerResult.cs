using AllOverIt.Assertion;
using AllOverIt.Evaluator.Variables;

namespace AllOverIt.Evaluator
{
    /// <summary>Contains a compiled delegate (resolver) and a list of referenced variables resulting from the compilation of a formula.</summary>
    public sealed class FormulaCompilerResult
    {
        /// <summary>The variable registry reference by the compiled formula. This will be <see langword="null"/> if the formula compiler was not provided
        /// a variable registry and the formula did not contain any variables.</summary>
        /// <remarks>Expected variables (as per ReferencedVariableNames) can be added to the registry after compilation if required.</remarks>
        public IVariableRegistry? VariableRegistry { get; }

        /// <summary>The compiled delegate that returns the result of the formula when invoked.</summary>
        public Func<double> Resolver { get; }

        /// <summary>An array of all variable names explicitly referenced by the formula.</summary>
        public string[] ReferencedVariableNames { get; }

        /// <summary>Constructor.</summary>
        /// <param name="variableRegistry">The variable registry reference by the compiled formula. Variables can be added at any time
        /// prior to evaluation.</param>
        /// <param name="resolver">The compiled delegate that when invoked will return the result of the formula.</param>
        /// <param name="referencedVariableNames">An array of all variable names explicitly referenced by the formula.</param>
        internal FormulaCompilerResult(IVariableRegistry? variableRegistry, Func<double> resolver, string[] referencedVariableNames)
        {
            VariableRegistry = variableRegistry;                    // will be null if the formula contained no variables
            Resolver = resolver.WhenNotNull(nameof(resolver));
            ReferencedVariableNames = referencedVariableNames.WhenNotNull(nameof(referencedVariableNames));
        }
    }
}