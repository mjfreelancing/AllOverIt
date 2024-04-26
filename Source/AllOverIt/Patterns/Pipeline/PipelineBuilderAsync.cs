using AllOverIt.Assertion;

namespace AllOverIt.Patterns.Pipeline
{
    // Begins a new asynchronous pipline that takes an initial input
    internal sealed class PipelineBuilderAsync<TIn, TOut> : IPipelineBuilderAsync<TIn, TOut>
    {
        private readonly Func<TIn, CancellationToken, Task<TOut>> _step;

        public PipelineBuilderAsync(Func<TIn, CancellationToken, Task<TOut>> step)
        {
            _step = step.WhenNotNull(nameof(step));
        }

        public Func<TIn, CancellationToken, Task<TOut>> Build()
        {
            return _step;
        }
    }

    // Appends to an asynchronous pipline that takes an initial input
    internal sealed class PipelineBuilderAsync<TIn, TPrevOut, TNextOut> : IPipelineBuilderAsync<TIn, TNextOut>
    {
        private readonly IPipelineBuilderAsync<TIn, TPrevOut> _prevStep;
        private readonly Func<TPrevOut, CancellationToken, Task<TNextOut>> _step;

        public PipelineBuilderAsync(IPipelineBuilderAsync<TIn, TPrevOut> prevStep, Func<TPrevOut, CancellationToken, Task<TNextOut>> step)
        {
            _prevStep = prevStep.WhenNotNull(nameof(prevStep));
            _step = step.WhenNotNull(nameof(step));
        }

        // Create a func that invokes the previous func and uses its result as the input to the next func (step)
        public Func<TIn, CancellationToken, Task<TNextOut>> Build()
        {
            async Task<TNextOut> func(TIn input, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var prevOutput = await _prevStep
                    .Build()
                    .Invoke(input, cancellationToken)
                    .ConfigureAwait(false);

                return await _step
                    .Invoke(prevOutput, cancellationToken)
                    .ConfigureAwait(false);
            }

            return func;
        }
    }
}
