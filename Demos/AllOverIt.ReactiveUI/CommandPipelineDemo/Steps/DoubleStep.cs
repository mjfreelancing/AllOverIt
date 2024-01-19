using AllOverIt.Patterns.Pipeline;

namespace CommandPipelineDemo.Steps
{
    internal sealed class DoubleStep : IPipelineStep<int, int>
    {
        public int Execute(int input)
        {
            return input * 2;
        }
    }
}