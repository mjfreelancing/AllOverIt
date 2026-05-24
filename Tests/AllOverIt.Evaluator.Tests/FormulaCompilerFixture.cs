using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.FakeItEasy;
using Shouldly;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixture : FixtureBase
    {
        private readonly IVariableRegistry _variableRegistry;
        private FormulaCompiler _formulaCompiler;

        public FormulaCompilerFixture()
        {
            this.UseFakeItEasy();

            _variableRegistry = this.CreateStub<IVariableRegistry>();
        }

        public class Compiler : FormulaCompilerFixture
        {
            public Compiler()
            {
                _formulaCompiler = new FormulaCompiler();
            }

            [Fact]
            public void Should_Throw_When_Formula_Null()
            {
                var exception = Should.Throw<FormatException>(() => _formulaCompiler.Compile(null, _variableRegistry));

                exception.Message.ShouldBe("The formula is empty.");
            }

            [Fact]
            public void Should_Throw_When_Formula_Empty()
            {
                var exception = Should.Throw<FormatException>(() => _formulaCompiler.Compile(string.Empty, _variableRegistry));

                exception.Message.ShouldBe("The formula is empty.");
            }

            [Fact]
            public void Should_Throw_When_Formula_Whitespace()
            {
                var exception = Should.Throw<FormatException>(() => _formulaCompiler.Compile(" ", _variableRegistry));

                exception.Message.ShouldBe("The formula is empty.");
            }

            [Fact]
            public void Should_Return_The_Same_Variable_Registry()
            {
                var compilerResult = _formulaCompiler.Compile("1+2", _variableRegistry);

                ReferenceEquals(compilerResult.VariableRegistry, _variableRegistry).ShouldBeTrue();
            }

            [Fact]
            public void Should_Create_Variable_Registry()
            {
                var compilerResult = _formulaCompiler.Compile("x+y", null);

                compilerResult.VariableRegistry.ShouldNotBeNull();
            }

            [Fact]
            public void Should_Not_Create_Variable_Registry()
            {
                var compilerResult = _formulaCompiler.Compile("1+2", null);

                compilerResult.VariableRegistry.ShouldBeNull();
            }

            [Fact]
            public void Should_Throw_When_Invalid_Expression_1()
            {
                var exception = Should.Throw<FormulaException>(() =>
                    {
                        var formula = "1d-4a";

                        _ = _formulaCompiler.Compile(formula, _variableRegistry);
                    });

                exception.Message.ShouldBe("Invalid expression. See index 2, near '1d'.");

                var inner = exception.InnerException.ShouldBeOfType<FormulaException>();

                inner.Message.ShouldBe("'d' is a variable or method that does not follow an operator, or is an unregistered operator.");
            }

            [Fact]
            public void Should_Throw_When_Invalid_Expression_2()
            {
                var exception = Should.Throw<FormulaException>(() =>
                    {
                        var formula = "1 4 + 2";

                        _ = _formulaCompiler.Compile(formula, _variableRegistry);
                    });

                exception.Message.ShouldBe("Invalid expression. See index 3, near '1 4'.");

                var inner = exception.InnerException.ShouldBeOfType<FormulaException>();

                inner.Message.ShouldBe("The number '4' did not follow an operator.");
            }

            [Fact]
            public void Should_Return_Compiled_Expression()
            {
                var val1 = Create<int>();
                var val2 = Create<int>();
                var expected = val1 + val2;

                var compilerResult = _formulaCompiler.Compile($"{val1}+{val2}");

                var value = compilerResult.Resolver.Invoke();

                value.ShouldBe(expected);
            }
        }
    }
}
