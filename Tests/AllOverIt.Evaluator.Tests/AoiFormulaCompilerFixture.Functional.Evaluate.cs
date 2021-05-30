using AllOverIt.Evaluator.Extensions;
using AllOverIt.Evaluator.Variables;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class AoiFormulaCompilerFixtureFunctionalEvaluate : AoiFormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Evaluate_Methods_With_Case_Insenstive_Comparison()
        {
            const string formula = "sqRT(9)+sqrt(4)+SQRT(16)";

            var actual = _formulaCompiler.GetResult(formula, _variableRegistry);
            actual.Should().Be(9);
        }

        [Fact]
        public void Should_Evaluate_Mixed()
        {
            var c = Create<double>();
            var x = Create<double>();
            var z = Create<double>();
            var expected = 2 * x - c * (5 - Math.Pow(z, 2)) + 1.2E-3;

            var compiled = _formulaCompiler.Compile("2*x-c*(5-z^2)+1.2E-3", _variableRegistry);

            // variables can be defined in the registry after the formula has been compiled
            _variableRegistry.AddVariable(new AoiConstantVariable("c", c));

            // can also use a variable directly
            var variableZ = new AoiMutableVariable("z");
            _variableRegistry.AddVariable(variableZ);

            // can set values using the registry
            _variableRegistry.SetValue("x", x); // overwrites the value assigned in the setup

            // or a variable directly
            variableZ.SetValue(z);

            // setting new variable values and recalculating the result can be performed as often as required
            var actual = compiled.Resolver.Invoke();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Evaluate_Formula_With_Method()
        {
            var c = Create<double>();
            var x = _val1;
            var y = _val2;
            var expected = 2 * x - c * (-3.5 - Math.Pow(y, 2)) + Math.Round(x + y - c, MidpointRounding.AwayFromZero) - -5E-3;

            var compiled = _formulaCompiler.Compile("2*x-c*(-3.5-y^2)+ROUND(x+y-c, 3)--5E-3", _variableRegistry);

            // can provide the value at the time of adding the variable
            _variableRegistry.AddVariable(new AoiConstantVariable("c", c));

            var actual = compiled.Resolver.Invoke();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Evaluate_With_Dependent_Formula()
        {
            const string formulaA = "x*3.8-y";
            const string formulaB = "a-x*z^3";

            var compiledA = _formulaCompiler.Compile(formulaA, _variableRegistry).Resolver;
            var compiledB = _formulaCompiler.Compile(formulaB, _variableRegistry).Resolver;

            // think of these as system variables (x and y were added in the setup)
            _variableRegistry.AddVariable(new AoiMutableVariable("z"));

            // think of these as user variables
            _variableRegistry.AddVariable(new AoiDelegateVariable("a", compiledA)); // must be a FuncVariable so it is re-evaluated when B is evaluated
            _variableRegistry.AddVariable(new AoiDelegateVariable("b", compiledB)); // can be either a FuncVariable or a BasicVariable because nothing depends on it but
                                                                                    // making it a FuncVariable means it can immediately be used as the input of another variable

            var z = Create<double>();

            _variableRegistry.SetValue("z", z);

            var x = _val1;
            var y = _val2;
            var expected = (x * 3.8 - y) - x * Math.Pow(z, 3);

            var actual = compiledB.Invoke();

            actual.Should().Be(expected);
        }
    }
}
