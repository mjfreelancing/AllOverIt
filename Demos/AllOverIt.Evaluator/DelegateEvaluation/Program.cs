using AllOverIt.Evaluator;
using AllOverIt.Evaluator.Variables;
using System;

namespace DelegateEvaluation
{
    class Program
    {
        static void Main(string[] args)
        {
            var factorial = new Factorial();

            var factory = new AoiVariableFactory();
            var registry = factory.CreateVariableRegistry();

            // 'f' will be a variable that has its value determined by a delegate
            // this represents any situation where a value comes from an independant source (such as a database)
            registry.AddVariable(factory.CreateDelegateVariable("f", factorial.Calculate));

            var compiler = new AoiFormulaCompiler();
            var compiled = compiler.Compile("5 * f", registry).Resolver;

            for (var i = 0u; i <= 10; ++i)
            {
                // update the factorial value to be calculated (simply to show the evaluation is re-performed)
                factorial.Value = i;

                // this will result in factorial.Calculate() being called when 'f' is re-evaluated
                var result = compiled.Invoke();

                Console.WriteLine($"5 * Factorial({i}) = {result}");
            }

            Console.WriteLine("");
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}
