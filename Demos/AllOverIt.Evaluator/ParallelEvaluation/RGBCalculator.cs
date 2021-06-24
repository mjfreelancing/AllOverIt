using AllOverIt.Evaluator;
using AllOverIt.Evaluator.Variables;
using System;

namespace ParallelEvaluation
{
    internal class RGBCalculator
    {
        private IVariableRegistry Variables { get; }
        private readonly Func<double> _redFunc;
        private readonly Func<double> _greenFunc;
        private readonly Func<double> _blueFunc;

        public byte RedValue => CalculateColourValue(_redFunc.Invoke());
        public byte GreenValue => CalculateColourValue(_greenFunc.Invoke());
        public byte BlueValue => CalculateColourValue(_blueFunc.Invoke());

        public RGBCalculator(string redFormula, string greenFormula, string blueFormula)
        {
            var compiler = new FormulaCompiler();

            var variableFactory = new VariableFactory();
            Variables = variableFactory.CreateVariableRegistry();

            Variables.AddVariable(new MutableVariable("x"));
            Variables.AddVariable(new MutableVariable("y"));

            _redFunc = compiler.Compile(redFormula, Variables).Resolver;
            _greenFunc = compiler.Compile(greenFormula, Variables).Resolver;
            _blueFunc = compiler.Compile(blueFormula, Variables).Resolver;
        }

        public void SetX(double value)
        {
            Variables.SetValue("x", value);
        }

        public void SetY(double value)
        {
            Variables.SetValue("y", value);
        }

        private static byte CalculateColourValue(double x)
        {
            if (double.IsNaN(x) || (x < 0.0d))
            {
                x = 0.0d;
            }
            else if (x > 1.0d)
            {
                x = 1.0d;
            }

            return (byte)(x * 255.0d);
        }
    }
}