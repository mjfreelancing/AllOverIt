using AllOverIt.Evaluator;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Evaluator.Variables.Extensions;
using System;

namespace VariableEvaluation
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new VariableFactory();
            var registry = factory.CreateVariableRegistry();

            // create a formula to calculate the slope between two points
            var compiler = new FormulaCompiler();
            var slope = compiler.Compile("(y2 - y1) / (x2 - x1)", registry).Resolver;

            var x1 = factory.CreateConstantVariable("x1");
            var y1 = factory.CreateConstantVariable("y1", 1.0d);

            var x2 = factory.CreateMutableVariable("x2");
            var y2 = factory.CreateMutableVariable("y2");

            // populate the variables registry
            registry.Add(
              // x1, y1 will be a constant point
              x1, y1,

              // x2, y2 will vary between calls
              x2, y2);

            for (var x = 0; x <= 9; x += 3)
            {
                for (var y = 0; y <= 9; y += 3)
                {
                    // update the mutable variables
                    x2.SetValue(x);
                    y2.SetValue(y);

                    // calculate the slope
                    var gradient = slope.Invoke();

                    var question = $"Slope between ({x1.Value}, {y1.Value}) and ({x2.Value}, {y2.Value}) is ";
                    var answer = $"{gradient}";

                    if (double.IsInfinity(gradient))
                    {
                        answer = $"{(gradient < 0 ? "Negative" : "Positive")} Infinity";
                    }

                    Console.WriteLine($"{question} is {answer}");
                }
            }

            Console.WriteLine("");
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}
