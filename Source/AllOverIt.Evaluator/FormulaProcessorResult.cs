using AllOverIt.Assertion;
using AllOverIt.Collections;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Extensions;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator
{
    /// <summary>Contains the result of parsing and processing a formula.</summary>
    public sealed class FormulaProcessorResult
    {
        private readonly IReadOnlyCollection<string> _referencedVariableNames;

        /// <summary>Gets the expression built from a processed formula. When this expression is compiled and invoked
        /// the value of the formula is returned.</summary>
        public Expression<Func<double>> FormulaExpression { get; }

        /// <summary>Gets an enumerable of all variable names explicitly referenced by the formula.</summary>
        public IReadOnlyCollection<string> ReferencedVariableNames => _referencedVariableNames ?? Collection.EmptyReadOnly<string>();

        /// <summary>The variable registry that will be referenced by the compiled expression during evaluation.
        /// This may be null if the formula does not contain any variables.</summary>
        /// <remarks>If the compiler is provided a variable registry then this property will refer to the same reference
        /// even if the formula did not contain any variables.</remarks>
        public IVariableRegistry VariableRegistry { get; }

        /// <summary>Constructor.</summary>
        /// <param name="formulaExpression">The expression built from a processed formula.</param>
        /// <param name="referencedVariableNames">When not null, a collection of all variable names explicitly referenced by the formula.</param>
        /// <param name="variableRegistry">When not null, the variable registry that will be referenced by the compiled expression during evaluation.</param>
        internal FormulaProcessorResult(Expression<Func<double>> formulaExpression, IEnumerable<string> referencedVariableNames, IVariableRegistry variableRegistry)
        {
            FormulaExpression = formulaExpression.WhenNotNull(nameof(formulaExpression));

            // can be NULL if there were no variables in the formula
            _referencedVariableNames = referencedVariableNames?.AsReadOnlyCollection();
            VariableRegistry = variableRegistry;
        }
    }
}