using AllOverIt.Patterns.Pipeline;
using System.Threading;
using System.Threading.Tasks;

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