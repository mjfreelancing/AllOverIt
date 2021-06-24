using AllOverIt.Evaluator.Tests.Helpers;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixture : FixtureBase
    {
        private readonly Fake<IFormulaParser> _parserFake;
        private readonly IVariableRegistry _variableRegistry;
        private readonly string _formula;
        private FormulaCompiler _formulaCompiler;

        public FormulaCompilerFixture()
        {
            this.UseFakeItEasy();

            _parserFake = this.CreateFake<IFormulaParser>(true);         // freeze it so it can be used when creating the SUT, via Create<FormulaCompiler>()
            _variableRegistry = this.CreateStub<IVariableRegistry>();
            _formula = Create<string>();
        }

        public class Constructor : FormulaCompilerFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Parser_Null()
            {
                Invoking(() => _formulaCompiler = new FormulaCompiler(null))
                    .Should()
                    .NotThrow();
            }
        }

        public class Compiler : FormulaCompilerFixture
        {
            private readonly double _value;
            private readonly IEnumerable<string> _referencedVariableNames;

            public Compiler()
            {
                _formulaCompiler = Create<FormulaCompiler>();

                _value = Create<double>();
                _referencedVariableNames = CreateMany<string>();
                var processorResult = EvaluatorHelpers.CreateFormulaProcessorResult(_value, _referencedVariableNames);

                _parserFake
                  .CallsTo(fake => fake.Parse(_formula, _variableRegistry))
                  .Returns(processorResult);
            }

            [Fact]
            public void Should_Throw_When_Formula_Null()
            {
                Invoking(() => _formulaCompiler.Compile(null, _variableRegistry))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("formula");
            }

            [Fact]
            public void Should_Throw_When_Formula_Empty()
            {
                Invoking(() => _formulaCompiler.Compile(string.Empty, _variableRegistry))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("formula");
            }

            [Fact]
            public void Should_Throw_When_Formula_Whitespace()
            {
                Invoking(() => _formulaCompiler.Compile(" ", _variableRegistry))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("formula");
            }

            [Fact]
            public void Should_Call_Parser_Parse()
            {
                _formulaCompiler.Compile(_formula, _variableRegistry);

                _parserFake
                  .CallsTo(fake => fake.Parse(_formula, _variableRegistry))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Return_Compiled_Expression()
            {
                var compilerResult = _formulaCompiler.Compile(_formula, _variableRegistry);

                var value = compilerResult.Resolver.Invoke();

                value.Should().Be(_value);
                compilerResult.ReferencedVariableNames.Should().BeSameAs(_referencedVariableNames);
            }
        }
    }
}
