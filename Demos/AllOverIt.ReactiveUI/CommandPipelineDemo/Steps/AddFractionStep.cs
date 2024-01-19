using AllOverIt.Patterns.Pipeline;

namespace CommandPipelineDemo.Steps
{
    internal sealed class AddFractionStep : IPipelineStep<int, double>
    {
        public double Execute(int input)
        {
            return input + 0.2d;
        }
    }
}