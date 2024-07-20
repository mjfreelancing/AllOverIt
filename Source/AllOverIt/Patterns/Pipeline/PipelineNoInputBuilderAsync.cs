using AllOverIt.Assertion;

namespace AllOverIt.Patterns.Pipeline
{
    // Begins a new asynchronous pipeline that takes no initial input
    internal sealed class PipelineNoInputBuilderAsync<TOut> : IPipelineBuilderAsync<TOut>
    {
        private readonly Func<CancellationToken, Task<TOut>> _step;

        public PipelineNoInputBuilderAsync(Func<CancellationToken, Task<TOut>> step)
        {
            _step = step.WhenNotNull();
        }

        public Func<CancellationToken, Task<TOut>> Build()
        {
            return _step;
        }
    }

    // Appends to an asynchronous pipeline that takes no initial input
    internal sealed class PipelineNoInputBuilderAsync<TPrevOut, TNextOut> : IPipelineBuilderAsync<TNextOut>
    {
        private readonly IPipelineBuilderAsync<TPrevOut> _prevStep;
        private readonly Func<TPrevOut, CancellationToken, Task<TNextOut>> _step;

        public PipelineNoInputBuilderAsync(IPipelineBuilderAsync<TPrevOut> prevStep, Func<TPrevOut, CancellationToken, Task<TNextOut>> step)
        {
            _prevStep = prevStep.WhenNotNull();
            _step = step.WhenNotNull();
        }

        // Create a func that invokes the previous func and uses its result as the input to the next func (step)
        public Func<CancellationToken, Task<TNextOut>> Build()
        {
            async Task<TNextOut> func(CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var prevOutput = await _prevStep
                    .Build()
                    .Invoke(cancellationToken)
                    .ConfigureAwait(false);

                return await _step
                    .Invoke(prevOutput, cancellationToken)
                    .ConfigureAwait(false);
            }

            return func;
        }
    }
}
