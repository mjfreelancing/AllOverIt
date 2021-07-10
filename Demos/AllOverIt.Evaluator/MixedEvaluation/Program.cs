using AllOverIt.Evaluator;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Evaluator.Variables.Extensions;
using System;

namespace MixedEvaluation
{
    class Program
    {
        static void Main(string[] args)
        {
            var compiler = new FormulaCompiler();
            var factory = new VariableFactory();
            var registry = factory.CreateVariableRegistry();

            // define variables to represent three points of a triangle
            var x1 = new MutableVariable("x1");
            var y1 = new MutableVariable("y1");
            var x2 = new MutableVariable("x2");
            var y2 = new MutableVariable("y2");
            var x3 = new MutableVariable("x3");
            var y3 = new MutableVariable("y3");

            // calculate distance between two points
            var distanceA = compiler.Compile("sqrt((x2-x1)^2+(y2-y1)^2)", registry).Resolver;
            var distanceB = compiler.Compile("sqrt((x3-x1)^2+(y3-y1)^2)", registry).Resolver;
            var distanceC = compiler.Compile("sqrt((x3-x2)^2+(y3-y2)^2)", registry).Resolver;

            // define variables to calculate the length of each side
            var a = new DelegateVariable("a", distanceA);
            var b = new DelegateVariable("b", distanceB);
            var c = new DelegateVariable("c", distanceC);

            // and use this to find the area of the bound triangle using Heron's formula
            var herons = compiler.Compile("0.25 * sqrt((4*a^2*b^2)-(a^2+b^2-c^2)^2)", registry).Resolver;

            // define all variables
            registry.Add(x1, y1, x2, y2, x3, y3, a, b, c);

            // set the co-ordinates of the 3 points
            x1.SetValue(5);
            y1.SetValue(5);

            x2.SetValue(15);
            y2.SetValue(90);

            x3.SetValue(11);
            y3.SetValue(-6);

            Console.WriteLine($"(x1, y1) = ({x1.Value}, {y1.Value})");
            Console.WriteLine($"(x2, y2) = ({x2.Value}, {y2.Value})");
            Console.WriteLine($"(x3, y3) = ({x3.Value}, {y3.Value})");
            Console.WriteLine();
            Console.WriteLine($"Distance between ({x1.Value}, {y1.Value}) and ({x2.Value}, {y2.Value}) = {a.Value}");
            Console.WriteLine($"Distance between ({x1.Value}, {y1.Value}) and ({x3.Value}, {y3.Value}) = {b.Value}");
            Console.WriteLine($"Distance between ({x2.Value}, {y2.Value}) and ({x3.Value}, {y3.Value}) = {c.Value}");
            Console.WriteLine();
            Console.WriteLine($"Area = {herons.Invoke()}");

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}
