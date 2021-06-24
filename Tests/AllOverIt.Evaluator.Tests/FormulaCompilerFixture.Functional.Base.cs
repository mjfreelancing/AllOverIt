using AllOverIt.Evaluator.Extensions;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using FluentAssertions;
using System;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalBase : FixtureBase
    {
        protected readonly double _val1;
        protected readonly double _val2;
        protected readonly IVariableRegistry _variableRegistry;
        protected readonly FormulaCompiler _formulaCompiler;

        protected FormulaCompilerFixtureFunctionalBase()
        {
            _val1 = CreateExcluding(0.0);
            _val2 = CreateExcluding(0.0);

            _variableRegistry = new VariableRegistry();
            _variableRegistry.AddVariable(new MutableVariable("x", _val1));
            _variableRegistry.AddVariable(new MutableVariable("y", _val2));

            _formulaCompiler = new FormulaCompiler();
        }

        protected void AssertFormula(string formula, Func<double> func)
        {
            var expected = func.Invoke();
            var actual = _formulaCompiler.GetResult(formula, _variableRegistry);

            actual.Should().Be(expected);
        }
    }
}
