using AllOverIt.Evaluator.Extensions;
using AllOverIt.Evaluator.Variables;
using FluentAssertions;
using System;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalEvaluate : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Evaluate_Methods_With_Case_Insensitive_Comparison()
        {
            const string formula = "sqRT(9)+sqrt(4)+SQRT(16)";

            var actual = FormulaCompiler.GetResult(formula, VariableRegistry);
            actual.Should().Be(9);
        }

        [Fact]
        public void Should_Evaluate_From_Internally_Provided_VariableRegistry()
        {
            var compiled = FormulaCompiler.Compile("x+y");

            var x = Create<int>();
            var y = Create<int>();

            compiled.VariableRegistry.AddVariables(
                new ConstantVariable("x", x),
                new MutableVariable("y", y));

            var expected = x + y;

            var actual = compiled.Resolver.Invoke();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Evaluate_Mixed()
        {
            var c = Create<double>();
            var x = Create<double>();
            var z = Create<double>();
            var expected = 2 * x - c * (5 - Math.Pow(z, 2)) + 1.2E-3;

            var compiled = FormulaCompiler.Compile("2*x-c*(5-z^2)+1.2E-3", VariableRegistry);

            // variables can be defined in the registry after the formula has been compiled
            VariableRegistry.AddVariable(new ConstantVariable("c", c));

            // can also use a variable directly
            var variableZ = new MutableVariable("z");
            VariableRegistry.AddVariable(variableZ);

            // can set values using the registry
            VariableRegistry.SetValue("x", x); // overwrites the value assigned in the setup

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
            var x = Val1;
            var y = Val2;
            var expected = 2 * x - c * (-3.5 - Math.Pow(y, 2)) + Math.Round(x + y - c, 3, MidpointRounding.AwayFromZero) - -5E-3;

            var compiled = FormulaCompiler.Compile("2*x-c*(-3.5-y^2)+ROUND(x+y-c, 3)--5E-3", VariableRegistry);

            // can provide the value at the time of adding the variable
            VariableRegistry.AddVariable(new ConstantVariable("c", c));

            var actual = compiled.Resolver.Invoke();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Evaluate_With_Dependent_Formula()
        {
            const string formulaA = "x*3.8-y";
            const string formulaB = "a-x*z^3";

            var compiledA = FormulaCompiler.Compile(formulaA, VariableRegistry).Resolver;
            var compiledB = FormulaCompiler.Compile(formulaB, VariableRegistry).Resolver;

            // think of these as system variables (x and y were added in the setup)
            VariableRegistry.AddVariable(new MutableVariable("z"));

            // think of these as user variables
            VariableRegistry.AddVariable(new DelegateVariable("a", compiledA)); // must be a FuncVariable so it is re-evaluated when B is evaluated
            VariableRegistry.AddVariable(new DelegateVariable("b", compiledB)); // can be either a FuncVariable or a BasicVariable because nothing depends on it but
                                                                                // making it a FuncVariable means it can immediately be used as the input of another variable

            var z = Create<double>();

            VariableRegistry.SetValue("z", z);

            var x = Val1;
            var y = Val2;
            var expected = (x * 3.8 - y) - x * Math.Pow(z, 3);

            var actual = compiledB.Invoke();

            actual.Should().Be(expected);
        }
    }
}
