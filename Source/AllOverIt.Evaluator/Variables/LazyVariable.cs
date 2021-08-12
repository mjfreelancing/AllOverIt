using AllOverIt.Helpers;
using System;
using System.Collections.Generic;

namespace AllOverIt.Evaluator.Variables
{
    // Implements a read-only variable that obtains its value via a deferred delegate. The value is evaluated when the variable is first accessed.
    public sealed record LazyVariable : VariableBase, ILazyVariable
    {
        private readonly Func<double> _valueResolver;
        private readonly bool _threadSafe;
        private Lazy<double> _lazyFunc;

        public override double Value => _lazyFunc.Value;

        // 'referencedVariableNames' is an optional list of variable names that this variable depends on to calculate its value.
        public LazyVariable(string name, Func<double> valueResolver, IEnumerable<string> referencedVariableNames = null, bool threadSafe = false)
            : base(name, referencedVariableNames)
        {
            _valueResolver = valueResolver.WhenNotNull(nameof(valueResolver));
            _threadSafe = threadSafe;

            Reset();
        }

        /// <summary>Resets the variable to force its value to be re-evaluated.</summary>
        public void Reset()
        {
            _lazyFunc = new Lazy<double>(_valueResolver, _threadSafe);
        }
    }
}