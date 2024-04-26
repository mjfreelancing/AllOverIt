using AllOverIt.Assertion;
using AllOverIt.Evaluator.Variables.Extensions;

namespace AllOverIt.Evaluator.Variables
{
    /// <summary>A delegate based variable that is evaluated the first time the <see cref="Value"/> is read.</summary>
    /// <remarks>For a delegate based variable that is re-evaluated each time the <see cref="Value"/> is read,
    /// see <see cref="DelegateVariable"/>.</remarks>
    public sealed class LazyVariable : VariableBase, ILazyVariable
    {
        private readonly Func<double> _valueResolver;
        private readonly bool _threadSafe;
        private Lazy<double> _lazyFunc;

        /// <summary>The value of the variable. This value is evaluated the first time the value is read. Subsequent
        /// reads will return the same value.</summary>
        public override double Value => _lazyFunc.Value;

        /// <summary>Constructor.</summary>
        /// <param name="name">The variable's name.</param>
        /// <param name="valueResolver">The delegate to invoke the first time <see cref="Value"/> is read.</param>
        /// <param name="threadSafe">Indicates if the underlying lazy-evaluator should evaluate in a thread safe manner.</param>
        public LazyVariable(string name, Func<double> valueResolver, bool threadSafe = false)
            : base(name)
        {
            _valueResolver = valueResolver.WhenNotNull(nameof(valueResolver));
            _threadSafe = threadSafe;

            Reset();
        }

        /// <summary>Constructor.</summary>
        /// <param name="name">The variable's name.</param>
        /// <param name="formulaCompilerResult">The compiled result of a formula. The associated resolver will be evaluated
        /// the first time the <see cref="Value"/> is read.</param>
        /// <param name="threadSafe">Indicates if the underlying lazy-evaluator should evaluate in a thread safe manner.</param>
        public LazyVariable(string name, FormulaCompilerResult formulaCompilerResult, bool threadSafe = false)
            : this(name, formulaCompilerResult.Resolver, threadSafe)
        {
            ReferencedVariables = formulaCompilerResult.GetReferencedVariables();
        }

        /// <inheritdoc />
        public void Reset()
        {
            _lazyFunc = new Lazy<double>(_valueResolver, _threadSafe);
        }
    }
}