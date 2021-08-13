using AllOverIt.Evaluator;
using AllOverIt.Evaluator.Extensions;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Evaluator.Variables.Extensions;
using AllOverIt.Extensions;
using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EvaluatorBenchmarking
{
    // all benchmarks are purely for comparison with an updated implementation
    [MemoryDiagnoser]
    public class Evaluator
    {
        private static readonly string[] Formula = new[]
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
            "log(19)",
            "ln(2.7)",
            "exp(3)",
            "10/0"
        };

        private readonly IList<double> _lhs;
        private readonly IList<double> _rhs;
        private readonly FormulaCompiler _compiler;
        private readonly VariableFactory _variableFactory;

        public Evaluator()
        {
            var rnd = new Random((int) DateTime.Now.Ticks);

            _lhs = Enumerable.Range(1, 100).SelectAsList(item => rnd.NextDouble());
            _rhs = Enumerable.Range(1, 100).SelectAsList(item => rnd.NextDouble());
            _compiler = new FormulaCompiler();
            _variableFactory = new VariableFactory();
        }

        [Benchmark]
        public void Add100RandomPairsWithoutCompilation()
        {
            for (var i = 0; i < 100; i++)
            {
                var lhs = _lhs[i];
                var rhs = _rhs[i];
                _compiler.GetResult($"{lhs} + {rhs}");
            }
        }

        [Benchmark]
        public void Add100RandomPairsWithCompilation()
        {
            var registry = _variableFactory.CreateVariableRegistry();

            var formula = _compiler.Compile("x + y", registry).Resolver;

            var x = _variableFactory.CreateMutableVariable("x");
            var y = _variableFactory.CreateMutableVariable("y");

            registry.Add(x, y);

            for (var i = 0; i < 100; i++)
            {
                x.SetValue(_lhs[i]);
                y.SetValue(_rhs[i]);

                _ = formula.Invoke();
            }
        }

        [Benchmark]
        public void MultipleFormulaeWithoutCompilation()
        {
            foreach (var item in Formula)
            {
                _ = _compiler.GetResult(item);
            }
        }

        [Benchmark]
        public void CalculateAreaUsingHeronsFormula()
        {
            var registry = _variableFactory.CreateVariableRegistry();

            // define variables to represent three points of a triangle
            var x1 = new MutableVariable("x1");
            var y1 = new MutableVariable("y1");
            var x2 = new MutableVariable("x2");
            var y2 = new MutableVariable("y2");
            var x3 = new MutableVariable("x3");
            var y3 = new MutableVariable("y3");

            // calculate distance between two points
            var distanceA = _compiler.Compile("sqrt((x2-x1)^2+(y2-y1)^2)", registry).Resolver;
            var distanceB = _compiler.Compile("sqrt((x3-x1)^2+(y3-y1)^2)", registry).Resolver;
            var distanceC = _compiler.Compile("sqrt((x3-x2)^2+(y3-y2)^2)", registry).Resolver;

            // define variables to calculate the length of each side
            var a = new DelegateVariable("a", distanceA);
            var b = new DelegateVariable("b", distanceB);
            var c = new DelegateVariable("c", distanceC);

            // and use this to find the area of the bound triangle using Heron's formula
            var herons = _compiler.Compile("0.25 * sqrt((4*a^2*b^2)-(a^2+b^2-c^2)^2)", registry).Resolver;

            // define all variables
            registry.Add(x1, y1, x2, y2, x3, y3, a, b, c);

            // set the co-ordinates of the 3 points
            x1.SetValue(5);
            y1.SetValue(5);

            x2.SetValue(15);
            y2.SetValue(90);

            x3.SetValue(11);
            y3.SetValue(-6);

            // distance x1,y1 to x2,y2
            var d1 = a.Value;

            // distance x1,y1 to x3,y3
            var d2 = b.Value;

            // distance x2,y2 to x3,y3
            var d3 = c.Value;

            // area
            var _ = herons.Invoke();
        }
    }
}