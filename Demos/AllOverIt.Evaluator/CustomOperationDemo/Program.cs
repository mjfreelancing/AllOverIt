using AllOverIt.Evaluator;
using AllOverIt.Evaluator.Extensions;
using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Extensions;
using CustomOperationDemo.Operations;
using System;
using System.Collections.Generic;

namespace CustomOperationDemo
{
    class Program
    {
        static void Main()
        {
            EvaluateCustomMin();          // example 1: MIN()
            EvaluateUniqueAngles();       // example 2: GCD()

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static void EvaluateCustomMin()
        {
            // Custom operator: registering '?' in this demo to return the minimum of two values:  a ? b will return the minimum of a and b
            // Custom method: registering MIN() to perform the same as above (is implemented in terms of 'CustomMinOperator' and 'CustomMinOperation')

            // will be equated to 10 + 1.2 - 6.2 = 5
            const string formula = "10 + min(1.2, 3.9) + 3.4 ? -6.2"; // the method names are parsed without case-sensitivity

            // NOTE: This demo deliberately ensures unique ArithmeticOperationFactory instances are created for each approach because
            //       FormulaProcessor registers an internal operation to handle negative values. If the same instance was shared
            //       between two processors an exception would be thrown due to a duplicate registration.

            var manualResult = EvaluateManually(formula);
            Console.WriteLine($"{formula} = {manualResult}");
        }

        private static void EvaluateUniqueAngles()
        {
            // determining unique angles in a XY plane based by checking for unique GCD values

            var xValues = new List<int>();
            var yValues = new List<int>();

            var gcdCompiler = new FormulaCompiler(userMethodFactory: CreateUserDefinedMethodFactory<GreatestCommonDenominatorOperation>("GCD"));

            var xValue = new MutableVariable("x");
            var yValue = new MutableVariable("y");

            var gcdFactory = new VariableFactory();
            var gcdRegistry = gcdFactory.CreateVariableRegistry();
            gcdRegistry.AddVariables(xValue, yValue);

            var gcdResolver = gcdCompiler
              .Compile("GCD(x, y)", gcdRegistry)
              .Resolver;

            for (var i = 0; i <= 100; i++)
            {
                xValue.SetValue(i);

                for (var j = 0; j <= 100; j++)
                {
                    yValue.SetValue(j);

                    if (gcdResolver.Invoke().IsEqualTo(1.0d))
                    {
                        xValues.Add(i);
                        yValues.Add(j);
                    }
                }
            }

            for (var idx = 0; idx < xValues.Count; idx++)
            {
                Console.WriteLine($"({xValues[idx]}, {yValues[idx]})");
            }
        }

        private static ArithmeticOperationFactory CreateArithmeticOperationFactory()
        {
            var operationFactory = new ArithmeticOperationFactory();

            operationFactory.RegisterOperation(
              "?",                                  // the mathematical operator symbol
              3,                                    // the precedence level (see ArithmeticOperationFactory for suggested levels to use)
              2,                                    // the number of expected arguments
              CustomMinOperation.MakeOperator);     // the lazily invoked factory method

            return operationFactory;
        }

        private static UserDefinedMethodFactory CreateUserDefinedMethodFactory<TOperationType>(string name)
          where TOperationType : ArithmeticOperationBase, new()
        {
            var userDefinedMethodFactory = new UserDefinedMethodFactory();
            userDefinedMethodFactory.RegisterMethod<TOperationType>(name);

            return userDefinedMethodFactory;
        }

        private static double EvaluateManually(string formula)
        {
            var compiler = new FormulaCompiler(
                CreateArithmeticOperationFactory(),
                CreateUserDefinedMethodFactory<CustomMinOperation>("MIN"));

            return compiler.GetResult(formula);
        }
    }
}
