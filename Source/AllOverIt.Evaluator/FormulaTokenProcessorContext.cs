using AllOverIt.Helpers;
using System;

namespace AllOverIt.Evaluator
{
    // Contains context information about a formula token and an associated processor.
    internal sealed class FormulaTokenProcessorContext
    {
        // Gets the predicate that determines if the processor should be invoked for the provided token.
        // The input arguments of the predicate include the next token to be read and a flag to indicate if the token is within
        // the context of a user defined method. The processor is invoked if the predicate returns true.
        public Func<char, bool, bool> Predicate { get; }

        // Gets delegate used for processing a given token.
        // The input arguments of the processor include the next token to be read and a flag to indicate if the token is within
        // the context of a user defined method. The processor returns true to indicate processing is to continue or false
        // to indicate processing of the current scope is complete (such as reading arguments of a user defined method).
        public Func<char, bool, bool> Processor { get; }

        public FormulaTokenProcessorContext(Func<char, bool, bool> predicate, Func<char, bool, bool> processor)
        {
            Predicate = predicate.WhenNotNull(nameof(predicate));
            Processor = processor.WhenNotNull(nameof(processor));
        }
    }
}