using AllOverIt.Patterns.Pipeline;

namespace PipelineAsyncDemo.Steps
{
    internal sealed class AddFractionStep : IPipelineStepAsync<int, double>
    {
        public Task<double> ExecuteAsync(int input, CancellationToken cancellationToken)
        {
            return Task.FromResult(input + 0.2d);
        }
    }
}