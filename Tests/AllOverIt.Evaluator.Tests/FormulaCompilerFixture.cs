using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixture : FixtureBase
    {
        private readonly IVariableRegistry _variableRegistry;
        private readonly string _formula;
        private FormulaCompiler _formulaCompiler;

        public FormulaCompilerFixture()
        {
            this.UseFakeItEasy();

            _variableRegistry = this.CreateStub<IVariableRegistry>();
            _formula = Create<string>();
        }

        public class Compiler : FormulaCompilerFixture
        {
            private readonly double _value;
            private readonly IEnumerable<string> _referencedVariableNames;

            public Compiler()
            {
                _formulaCompiler = new FormulaCompiler();

                _value = Create<double>();
                _referencedVariableNames = CreateMany<string>();
            }

            [Fact]
            public void Should_Throw_When_Formula_Null()
            {
                Invoking(() => _formulaCompiler.Compile(null, _variableRegistry))
                    .Should()
                    .Throw<FormatException>()
                    .WithMessage("The formula is empty.");
            }

            [Fact]
            public void Should_Throw_When_Formula_Empty()
            {
                Invoking(() => _formulaCompiler.Compile(string.Empty, _variableRegistry))
                    .Should()
                    .Throw<FormatException>()
                    .WithMessage("The formula is empty.");
            }

            [Fact]
            public void Should_Throw_When_Formula_Whitespace()
            {
                Invoking(() => _formulaCompiler.Compile(" ", _variableRegistry))
                    .Should()
                    .Throw<FormatException>()
                    .WithMessage("The formula is empty.");
            }

            [Fact]
            public void Should_Return_The_Same_Variable_Registry()
            {
                var compilerResult = _formulaCompiler.Compile("1+2", _variableRegistry);

                ReferenceEquals(compilerResult.VariableRegistry, _variableRegistry)
                    .Should()
                    .BeTrue();
            }

            [Fact]
            public void Should_Create_Variable_Registry()
            {
                var compilerResult = _formulaCompiler.Compile("x+y", null);

                compilerResult.VariableRegistry
                    .Should()
                    .NotBeNull();
            }

            [Fact]
            public void Should_Not_Create_Variable_Registry()
            {
                var compilerResult = _formulaCompiler.Compile("1+2", null);

                compilerResult.VariableRegistry
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void Should_Throw_When_Invalid_Expression()
            {
                Invoking(() =>
                    {
                        var formula = "1d-4a";

                        _ = _formulaCompiler.Compile(formula, _variableRegistry);
                    })
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage("Invalid expression. See index 2, near '1d'.")
                    .WithInnerException<FormulaException>()
                    .WithMessage("'d' is a variable or method that does not follow an operator, or is an unregistered operator.");
            }

            [Fact]
            public void Should_Throw_When_Invalid_Expression0000()
            {
                Invoking(() =>
                    {
                        var formula = "1 4 + 2";

                        _ = _formulaCompiler.Compile(formula, _variableRegistry);
                    })
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage("Invalid expression. See index 3, near '1 4'.")
                    .WithInnerException<FormulaException>()
                    .WithMessage("The number '4' did not follow an operator.");
            }

            [Fact]
            public void Should_Return_Compiled_Expression()
            {
                var val1 = Create<int>();
                var val2 = Create<int>();
                var expected = val1 + val2;

                var compilerResult = _formulaCompiler.Compile($"{val1}+{val2}");

                var value = compilerResult.Resolver.Invoke();

                value.Should().Be(expected);
            }
        }
    }
}
