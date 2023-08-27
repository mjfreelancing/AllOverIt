using AllOverIt.Assertion;
using AllOverIt.Evaluator.Variables.Extensions;
using System;

namespace AllOverIt.Evaluator.Variables
{
    /// <summary>A delegate based variable that is re-evaluated each time the <see cref="Value"/> is read.</summary>
    /// <remarks>For a delegate based variable that is only evaluated the first time the <see cref="Value"/> is read,
    /// see <see cref="LazyVariable"/>.</remarks>
    public sealed record DelegateVariable : VariableBase
    {
        private readonly Func<double> _valueResolver;

        /// <summary>The current value of the variable. The value may change on each evaluation depending on how the
        /// delegate is implemented.</summary>
        public override double Value => _valueResolver.Invoke();

        /// <summary>Constructor.</summary>
        /// <param name="name">The variable's name.</param>
        /// <param name="valueResolver">The delegate to invoke each time the <see cref="Value"/> is read.</param>
        public DelegateVariable(string name, Func<double> valueResolver)
            : base(name)
        {
            _valueResolver = valueResolver.WhenNotNull(nameof(valueResolver));
        }

        /// <summary>Constructor.</summary>
        /// <param name="name">The variable's name.</param>
        /// <param name="formulaCompilerResult">The compiled result of a formula. The associated resolver will be re-evaluated
        /// each time the <see cref="Value"/> is read.</param>
        public DelegateVariable(string name, FormulaCompilerResult formulaCompilerResult)
            : this(name, formulaCompilerResult.Resolver)
        {
            ReferencedVariables = formulaCompilerResult.GetReferencedVariables();
        }
    }
}