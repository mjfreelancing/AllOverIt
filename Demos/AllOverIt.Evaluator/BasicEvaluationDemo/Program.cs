using AllOverIt.Evaluator;
using AllOverIt.Evaluator.Extensions;
using System;

namespace BasicEvaluationDemo
{
    class Program
    {
        static void Main()
        {
            var formulas = new[]
            {
                "11 + 77",
                "11 - 77",
                "11 * 77",
                "11 / 77",
                "5.105 ^ -3",
                "(5 ^ -2 ^ 3) * (10 ^ 5)",
                "-(5 ^ 3 / (12 - 5) + 34.6) / (-423.3 / -.1) * 10 ^ 3",
                "round(2.45678, 3)",
                "sqrt(15)",
                "sqrt(3) / (2 * sqrt(2))",
                "log10(19)",
                "log(2.7)",
                "exp(3)",
                "10/0",
                "IF(EQ(1,2),50*2,40*3)"         // 120
            };

            var compiler = new FormulaCompiler();

            foreach (var item in formulas)
            {
                var result = compiler.GetResult(item);

                Console.WriteLine($"{item} = {result}");
            }

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}
