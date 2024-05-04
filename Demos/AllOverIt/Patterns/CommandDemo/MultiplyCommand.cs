using AllOverIt.Patterns.Command;

namespace CommandDemo
{
    internal sealed class MultiplyCommand : ICommand<int, int>
    {
        private readonly int _factor;

        public MultiplyCommand(int factor)
        {
            _factor = factor;
        }

        public int Execute(int input)
        {
            var result = input * _factor;

            // just to show the demo executing each command
            Console.WriteLine($"{input} * {_factor} = {result}");

            return result;
        }
    }
}
