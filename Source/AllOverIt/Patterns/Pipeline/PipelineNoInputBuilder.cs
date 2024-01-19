using AllOverIt.Assertion;
using System;

namespace AllOverIt.Patterns.Pipeline
{
    // Begins a new pipline that takes no initial input
    internal sealed class PipelineNoInputBuilder<TOut> : IPipelineBuilder<TOut>
    {
        private readonly Func<TOut> _step;

        public PipelineNoInputBuilder(Func<TOut> step)
        {
            _step = step.WhenNotNull(nameof(step));
        }

        public Func<TOut> Build()
        {
            return _step;
        }
    }

    // Appends to a pipeline that takes no initial input
    internal sealed class PipelineNoInputBuilder<TPrevOut, TNextOut> : IPipelineBuilder<TNextOut>
    {
        private readonly IPipelineBuilder<TPrevOut> _prevStep;
        private readonly Func<TPrevOut, TNextOut> _step;

        public PipelineNoInputBuilder(IPipelineBuilder<TPrevOut> prevStep, Func<TPrevOut, TNextOut> step)
        {
            _prevStep = prevStep.WhenNotNull(nameof(prevStep));
            _step = step.WhenNotNull(nameof(step));
        }

        // Create a func that invokes the previous func and uses its result as the input to the next func (step)
        public Func<TNextOut> Build()
        {
            TNextOut func()
            {
                var prevOutput = _prevStep
                    .Build()
                    .Invoke();

                return _step.Invoke(prevOutput);
            }

            return func;
        }
    }
}
