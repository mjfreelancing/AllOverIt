using AllOverIt.Helpers;
using System;
using System.Collections.Generic;

namespace AllOverIt.Evaluator.Variables
{
    // Implements a read-only variable that obtains its value via a deferred delegate. The value is evaluated when the variable is first accessed.
    public sealed class AoiLazyVariable
      : AoiVariableBase, IAoiLazyVariable
    {
        private Lazy<double> LazyFunc { get; set; }
        private Func<double> ValueResolver { get; }
        internal bool ThreadSafe { get; }

        public override double Value => LazyFunc.Value;

        // 'referencedVariableNames' is an optional list of variable names that this variable depends on to calculate its value.
        public AoiLazyVariable(string name, Func<double> valueResolver, IEnumerable<string> referencedVariableNames = null, bool threadSafe = false)
            : base(name, referencedVariableNames)
        {
            ValueResolver = valueResolver.WhenNotNull(nameof(valueResolver));
            ThreadSafe = threadSafe;

            Reset();
        }

        /// <summary>Resets the variable to force its value to be re-evaluated.</summary>
        public void Reset()
        {
            LazyFunc = new Lazy<double>(ValueResolver, ThreadSafe);
        }
    }
}