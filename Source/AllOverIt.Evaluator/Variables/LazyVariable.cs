using AllOverIt.Evaluator.Variables.Extensions;
using AllOverIt.Helpers;
using System;

namespace AllOverIt.Evaluator.Variables
{
    // Implements a read-only variable that obtains its value via a deferred delegate. The value is evaluated when the variable is first accessed.
    public sealed record LazyVariable : VariableBase, ILazyVariable
    {
        private readonly Func<double> _valueResolver;
        private readonly bool _threadSafe;
        private Lazy<double> _lazyFunc;

        public override double Value => _lazyFunc.Value;

        public LazyVariable(string name, Func<double> valueResolver, bool threadSafe = false)
            : base(name)
        {
            _valueResolver = valueResolver.WhenNotNull(nameof(valueResolver));
            _threadSafe = threadSafe;

            Reset();
        }

        public LazyVariable(string name, FormulaCompilerResult compilerResult, bool threadSafe = false)
            : this(name, compilerResult.Resolver, threadSafe)
        {
            ReferencedVariables = compilerResult.GetReferencedVariables();
        }

        /// <summary>Resets the variable to force its value to be re-evaluated.</summary>
        public void Reset()
        {
            _lazyFunc = new Lazy<double>(_valueResolver, _threadSafe);
        }
    }
}