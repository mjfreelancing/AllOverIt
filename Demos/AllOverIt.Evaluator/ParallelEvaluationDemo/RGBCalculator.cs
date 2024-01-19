using AllOverIt.Evaluator;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Evaluator.Variables.Extensions;
using System;

namespace ParallelEvaluationDemo
{
    internal sealed class RGBCalculator
    {
        private static readonly VariableFactory VariableFactory = new();       // thread safe
        private readonly IVariableRegistry _variables;
        private readonly Func<double> _redFunc;
        private readonly Func<double> _greenFunc;
        private readonly Func<double> _blueFunc;

        public byte RedValue => CalculateColorValue(_redFunc.Invoke());
        public byte GreenValue => CalculateColorValue(_greenFunc.Invoke());
        public byte BlueValue => CalculateColorValue(_blueFunc.Invoke());

        public RGBCalculator(string redFormula, string greenFormula, string blueFormula)
        {
            _variables = VariableFactory.CreateVariableRegistry();

            _variables.AddMutableVariable("x");
            _variables.AddMutableVariable("y");

            var compiler = new FormulaCompiler();

            _redFunc = compiler.Compile(redFormula, _variables).Resolver;
            _greenFunc = compiler.Compile(greenFormula, _variables).Resolver;
            _blueFunc = compiler.Compile(blueFormula, _variables).Resolver;
        }

        public void SetX(double value)
        {
            _variables.SetValue("x", value);
        }

        public void SetY(double value)
        {
            _variables.SetValue("y", value);
        }

        private static byte CalculateColorValue(double x)
        {
            if (double.IsNaN(x) || x < 0.0d)
            {
                x = 0.0d;
            }
            else if (x > 1.0d)
            {
                x = 1.0d;
            }

            return (byte) (x * 255.0d);
        }
    }
}