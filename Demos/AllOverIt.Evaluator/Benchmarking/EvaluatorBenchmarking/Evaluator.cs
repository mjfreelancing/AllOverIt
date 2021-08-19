using AllOverIt.Evaluator;
using AllOverIt.Evaluator.Extensions;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Evaluator.Variables.Extensions;
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
            "10/0",
            "22 % 6"
        };

        private readonly IList<double> _lhs;
        private readonly IList<double> _rhs;
        private readonly FormulaCompiler _compiler;
        private readonly VariableRegistry _variableRegistry;

        public Evaluator()
        {
            var rnd = new Random((int) DateTime.Now.Ticks);

            _lhs = Enumerable.Range(1, 10000).Select(_ => rnd.NextDouble()).ToList();
            _rhs = Enumerable.Range(1, 10000).Select(_ => rnd.NextDouble()).ToList();
            _compiler = new FormulaCompiler();
            _variableRegistry = new VariableRegistry();
        }

        [Benchmark]
        public void AddTwoConstantsUsingGetResult()
        {
            AddTwoConstantsUsingGetResult(1);
        }

        [Benchmark]
        public void AddTwoConstantsUsingGetResult10000Times()
        {
            AddTwoConstantsUsingGetResult(10000);
        }

        [Benchmark]
        public void AddTwoVariablesUsingRegistry()
        {
            AddTwoVariablesUsingRegistry(1);
        }

        [Benchmark]
        public void AddTwoVariablesUsingRegistry10000Times()
        {
            AddTwoVariablesUsingRegistry(10000);
        }

        [Benchmark]
        public void EvalMultipleFormulaUsingGetResult()
        {
            foreach (var item in Formula)
            {
                _ = _compiler.GetResult(item);
            }
        }

        [Benchmark]
        public void EvalMultipleFormulaUsingPreCompiled10000Times()
        {
            // using _variableRegistry to prevent multiple instances being created
            var compilerResults = Formula
                .Select(formula => _compiler.Compile(formula))
                .ToList();

            for (var i = 0; i < 10000; i++)
            {
                foreach (var compilerResult in compilerResults)
                {
                    _ = compilerResult.Resolver.Invoke();
                }
            }
        }

        [Benchmark]
        public void CalculateDistancesAndAreaUsingHeronsFormula()
        {
            CalculateDistancesAndAreaUsingHeronsFormula(1);
        }

        [Benchmark]
        public void CalculateDistancesAndAreaUsingHeronsFormula10000Times()
        {
            CalculateDistancesAndAreaUsingHeronsFormula(10000);
        }

        private void AddTwoConstantsUsingGetResult(int iterations)
        {
            for (var i = 0; i < iterations; i++)
            {
                _compiler.GetResult($"{_lhs[i]} + {_rhs[i]}");
            }
        }

        private void AddTwoVariablesUsingRegistry(int iterations)
        {
            var compilerResult = _compiler.Compile("x + y");
            var registry = compilerResult.VariableRegistry;
            var resolver = compilerResult.Resolver;

            var x = registry.AddMutableVariable("x");
            var y = registry.AddMutableVariable("y");

            for (var i = 0; i < iterations; i++)
            {
                x.SetValue(_lhs[i]);
                y.SetValue(_rhs[i]);

                _ = resolver.Invoke();
            }
        }

        private void CalculateDistancesAndAreaUsingHeronsFormula(int iterations)
        {
            // using a variable registry so it can be shared across multiple compilers
            _variableRegistry.Clear();

            // define variables to represent three points of a triangle
            var x1 = _variableRegistry.AddMutableVariable("x1");
            var y1 = _variableRegistry.AddMutableVariable("y1");
            var x2 = _variableRegistry.AddMutableVariable("x2");
            var y2 = _variableRegistry.AddMutableVariable("y2");
            var x3 = _variableRegistry.AddMutableVariable("x3");
            var y3 = _variableRegistry.AddMutableVariable("y3");

            // calculate distance between two points
            var distanceA = _compiler.Compile("sqrt((x2-x1)^2+(y2-y1)^2)", _variableRegistry);
            var distanceB = _compiler.Compile("sqrt((x3-x1)^2+(y3-y1)^2)", _variableRegistry);
            var distanceC = _compiler.Compile("sqrt((x3-x2)^2+(y3-y2)^2)", _variableRegistry);

            // define variables to calculate the length of each side
            var a = _variableRegistry.AddDelegateVariable("a", distanceA);
            var b = _variableRegistry.AddDelegateVariable("b", distanceB);
            var c = _variableRegistry.AddDelegateVariable("c", distanceC);

            // and use this to find the area of the bound triangle using Heron's formula
            var herons = _compiler.Compile("0.25 * sqrt((4*a^2*b^2)-(a^2+b^2-c^2)^2)", _variableRegistry).Resolver;

            for (var i = 0; i < iterations; i++)
            {
                // set the co-ordinates of the 3 points
                x1.SetValue(5 + i);
                y1.SetValue(5 + i);

                x2.SetValue(15 + i);
                y2.SetValue(90 + i);

                x3.SetValue(11 + i);
                y3.SetValue(-6 + i);

                // distance x1,y1 to x2,y2
                var d1 = a.Value;

                // distance x1,y1 to x3,y3
                var d2 = b.Value;

                // distance x2,y2 to x3,y3
                var d3 = c.Value;

                var _ = herons.Invoke();
            }
        }
    }
}