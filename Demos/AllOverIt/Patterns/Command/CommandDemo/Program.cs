using AllOverIt.Patterns.Command;
using System;

namespace CommandDemo
{
    internal class Program
    {
        static void Main()
        {
            var times2 = new MultiplyCommand(2);
            var times3 = new MultiplyCommand(3);
            var times4 = new MultiplyCommand(4);
            var times5 = new MultiplyCommand(5);

            var pipeline = new CommandPipeline<int>();

            var result = pipeline
                .Append(times2)
                .Append(times3)
                .Append(times4, times5)
                .Execute(7);

            Console.WriteLine();
            Console.WriteLine($"Total = {result}");

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}
