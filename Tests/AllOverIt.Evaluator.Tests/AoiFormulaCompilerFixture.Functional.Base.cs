using AllOverIt.Evaluator.Extensions;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using FluentAssertions;
using System;

namespace AllOverIt.Evaluator.Tests
{
    public class AoiFormulaCompilerFixtureFunctionalBase : AoiFixtureBase
    {
        protected readonly double _val1;
        protected readonly double _val2;
        protected readonly IAoiVariableRegistry _variableRegistry;
        protected readonly AoiFormulaCompiler _formulaCompiler;

        protected AoiFormulaCompilerFixtureFunctionalBase()
        {
            _val1 = CreateExcluding(0.0);
            _val2 = CreateExcluding(0.0);

            _variableRegistry = new AoiVariableRegistry();
            _variableRegistry.AddVariable(new AoiMutableVariable("x", _val1));
            _variableRegistry.AddVariable(new AoiMutableVariable("y", _val2));

            _formulaCompiler = new AoiFormulaCompiler();
        }

        protected void AssertFormula(string formula, Func<double> func)
        {
            var expected = func.Invoke();
            var actual = _formulaCompiler.GetResult(formula, _variableRegistry);

            actual.Should().Be(expected);
        }
    }
}
