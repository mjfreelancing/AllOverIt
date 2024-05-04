using AllOverIt.Patterns.Pipeline;

namespace PipelineAsyncDemo.Steps
{
    internal sealed class DoubleStepAsync : IPipelineStepAsync<int, int>
    {
        public Task<int> ExecuteAsync(int input, CancellationToken cancellationToken)
        {
            return Task.FromResult(input * 2);
        }
    }
}