﻿using AllOverIt.Evaluator.Tests.Helpers;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class AoiFormulaCompilerFixture : AoiFixtureBase
    {
        private readonly Fake<IAoiFormulaParser> _parserFake;
        private readonly IAoiVariableRegistry _variableRegistry;
        private readonly string _formula;
        private AoiFormulaCompiler _formulaCompiler;

        public AoiFormulaCompilerFixture()
        {
            this.UseFakeItEasy();

            _parserFake = this.CreateFake<IAoiFormulaParser>(true);         // freeze it so it can be used when creating the SUT, via Create<AoiFormulaCompiler>()
            _variableRegistry = this.CreateStub<IAoiVariableRegistry>();
            _formula = Create<string>();
        }

        public class Constructor : AoiFormulaCompilerFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Parser_Null()
            {
                Invoking(() => _formulaCompiler = new AoiFormulaCompiler(null))
                    .Should()
                    .NotThrow();
            }
        }

        public class Compiler : AoiFormulaCompilerFixture
        {
            private readonly double _value;
            private readonly IEnumerable<string> _referencedVariableNames;

            public Compiler()
            {
                _formulaCompiler = Create<AoiFormulaCompiler>();

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
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("formula"));
            }

            [Fact]
            public void Should_Throw_When_Formula_Empty()
            {
                Invoking(() => _formulaCompiler.Compile(string.Empty, _variableRegistry))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage(GetExpectedArgumentCannotBeEmptyExceptionMessage("formula"));
            }

            [Fact]
            public void Should_Throw_When_Formula_Whitespace()
            {
                Invoking(() => _formulaCompiler.Compile(" ", _variableRegistry))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage(GetExpectedArgumentCannotBeEmptyExceptionMessage("formula"));
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
