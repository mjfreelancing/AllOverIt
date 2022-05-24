using AllOverIt.Evaluator.Extensions;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using FluentAssertions;
using System;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalBase : FixtureBase
    {
        protected readonly double Val1;
        protected readonly double Val2;
        protected readonly IVariableRegistry VariableRegistry;
        protected readonly FormulaCompiler FormulaCompiler;

        protected FormulaCompilerFixtureFunctionalBase()
        {
            Val1 = CreateExcluding(0.0);
            Val2 = CreateExcluding(0.0);

            VariableRegistry = new VariableRegistry();
            VariableRegistry.AddVariable(new MutableVariable("x", Val1));
            VariableRegistry.AddVariable(new MutableVariable("y", Val2));

            FormulaCompiler = new FormulaCompiler();
        }

        protected void AssertFormula(string formula, Func<double> func, double precision = 1E-7)
        {
            var expected = func.Invoke();
            var actual = FormulaCompiler.GetResult(formula, VariableRegistry);

            actual.Should().BeApproximately(expected, precision);
        }
    }
}
