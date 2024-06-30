using AllOverIt.Assertion;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Extensions;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator
{
    /// <summary>Contains the result of parsing and processing a formula.</summary>
    public sealed class FormulaProcessorResult
    {
        private readonly string[]? _referencedVariableNames;

        /// <summary>Gets the expression built from a processed formula. When this expression is compiled and invoked
        /// the value of the formula is returned.</summary>
        public Expression<Func<double>> FormulaExpression { get; }

        /// <summary>Gets an enumerable of all variable names explicitly referenced by the formula.</summary>
        public string[] ReferencedVariableNames => _referencedVariableNames ?? [];

        /// <summary>The variable registry that will be referenced by the compiled expression during evaluation.
        /// This may be <see langword="null"/> if the formula does not contain any variables.</summary>
        /// <remarks>If the compiler is provided a variable registry then this property will refer to the same reference
        /// even if the formula did not contain any variables.</remarks>
        public IVariableRegistry? VariableRegistry { get; }

        /// <summary>Constructor.</summary>
        /// <param name="formulaExpression">The expression built from a processed formula.</param>
        /// <param name="referencedVariableNames">When not <see langword="null"/>, a collection of all variable names explicitly referenced by the formula.</param>
        /// <param name="variableRegistry">When not <see langword="null"/>, the variable registry that will be referenced by the compiled expression during evaluation.</param>
        internal FormulaProcessorResult(Expression<Func<double>> formulaExpression, string[]? referencedVariableNames, IVariableRegistry? variableRegistry)
        {
            FormulaExpression = formulaExpression.WhenNotNull(nameof(formulaExpression));

            // Can be NULL if there were no variables in the formula
            _referencedVariableNames = referencedVariableNames?.AsArray();
            VariableRegistry = variableRegistry;
        }
    }
}