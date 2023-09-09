using AllOverIt.Assertion;
using System;

namespace AllOverIt.Evaluator
{
    /// <summary>Contains context information about a formula token and an associated processor.</summary>
    internal sealed class FormulaTokenProcessorContext
    {
        /// <summary>The predicate that determines if the processor should be invoked for the provided token.</summary>
        /// <remarks>The input arguments of the predicate include the next token to be read and a flag to indicate if the
        /// token is within the context of a user-defined method.</remarks>
        public Func<char, bool, bool> Predicate { get; }

        /// <summary>The delegate used for processing a given token.</summary>
        /// <remarks>The input arguments of the processor include the next token to be read and a flag to indicate if the token
        /// is within the context of a user defined method. The processor returns true to indicate processing is to continue or
        /// false to indicate processing of the current scope is complete (such as reading arguments of a user defined method).</remarks>
        public Func<char, bool, bool> Processor { get; }

        /// <summary>Constructor.</summary>
        /// <param name="predicate">The predicate that determines if the processor should be invoked for the provided token.</param>
        /// <param name="processor">The delegate used for processing a given token.</param>
        public FormulaTokenProcessorContext(Func<char, bool, bool> predicate, Func<char, bool, bool> processor)
        {
            Predicate = predicate.WhenNotNull(nameof(predicate));
            Processor = processor.WhenNotNull(nameof(processor));
        }
    }
}