﻿using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Stack;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class AoiFormulaExpressionFactoryFixture : AoiFixtureBase
    {
        private readonly int _argumentCount;
        private readonly Fake<IAoiArithmeticOperation> _operationFake;
        private readonly Fake<IAoiStack<Expression>> _expressionStackFake;
        private readonly AoiFormulaExpressionFactory _factory;
        private readonly Expression[] _expectedExpressions;

        public AoiFormulaExpressionFactoryFixture()
        {
            _argumentCount = Create<int>() % 20 + 1;
            _operationFake = new Fake<IAoiArithmeticOperation>();
            _expressionStackFake = new Fake<IAoiStack<Expression>>();

            _expressionStackFake
              .CallsTo(fake => fake.Count)
              .Returns(_argumentCount);

            _operationFake
              .CallsTo(fake => fake.ArgumentCount)
              .Returns(_argumentCount);

            var expressions = new Stack<Expression>();

            for (var i = 0; i < _argumentCount; ++i)
            {
                expressions.Push(this.CreateStub<Expression>());
            }

            _expectedExpressions = expressions.Reverse().ToArray();

            _expressionStackFake
              .CallsTo(fake => fake.Pop())
              .ReturnsNextFromSequence(_expectedExpressions);

            _factory = new AoiFormulaExpressionFactory();
        }

        public class CreateExpression_OperationBase : AoiFormulaExpressionFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Operation_Null()
            {
                Invoking(() => _factory.CreateExpression(null, this.CreateStub<IAoiStack<Expression>>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operation");
            }

            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() => _factory.CreateExpression(this.CreateStub<AoiArithmeticOperationBase>(), null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expressionStack");
            }

            [Fact]
            public void Should_Throw_When_Insufficient_Parameters()
            {
                _operationFake
                  .CallsTo(fake => fake.ArgumentCount)
                  .Returns(_argumentCount + 1);

                Invoking(() => _factory.CreateExpression(_operationFake.FakedObject, _expressionStackFake.FakedObject))
                    .Should()
                    .Throw<AoiFormulaException>()
                    .WithMessage($"Insufficient expressions in the stack. {_argumentCount} available, {_argumentCount + 1} required.");
            }

            [Fact]
            public void Should_Not_Process_Loop_When_No_Expressions()
            {
                _operationFake
                  .CallsTo(fake => fake.ArgumentCount)
                  .Returns(0);

                _factory.CreateExpression(_operationFake.FakedObject, _expressionStackFake.FakedObject);

                _expressionStackFake
                  .CallsTo(fake => fake.Pop())
                  .MustHaveHappened(0, Times.Exactly);
            }

            [Fact]
            public void Should_Process_Loop_Expected_Times()
            {
                _factory.CreateExpression(_operationFake.FakedObject, _expressionStackFake.FakedObject);

                _expressionStackFake
                  .CallsTo(fake => fake.Pop())
                  .MustHaveHappened(_argumentCount, Times.Exactly);
            }

            [Fact]
            public void Should_Call_Operation_GetExpression()
            {
                Expression[] actualExpressions = null;

                _operationFake
                  .CallsTo(fake => fake.GetExpression(A<Expression[]>.Ignored))
                  .Invokes(call => actualExpressions = call.Arguments.Get<Expression[]>(0));

                _factory.CreateExpression(_operationFake.FakedObject, _expressionStackFake.FakedObject);

                actualExpressions.Should().BeEquivalentTo(_expectedExpressions);
            }

            [Fact]
            public void Should_Return_Expected_Expression()
            {
                var expected = this.CreateStub<Expression>();

                _operationFake
                  .CallsTo(fake => fake.GetExpression(A<Expression[]>.Ignored))
                  .Returns(expected);

                var actual = _factory.CreateExpression(_operationFake.FakedObject, _expressionStackFake.FakedObject);

                actual.Should().BeSameAs(expected);
            }
        }

        public class CreateExpression_Variable : AoiFormulaExpressionFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Variable_Null()
            {
                Invoking(() => _factory.CreateExpression(null, this.CreateStub<IAoiVariableRegistry>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("variableName");
            }

            [Fact]
            public void Should_Throw_When_Variable_Empty()
            {
                Invoking(() => _factory.CreateExpression(string.Empty, this.CreateStub<IAoiVariableRegistry>()))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("variableName");
            }

            [Fact]
            public void Should_Throw_When_Variable_Whitespace()
            {
                Invoking(() => _factory.CreateExpression(" ", this.CreateStub<IAoiVariableRegistry>()))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("variableName");
            }

            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() => _factory.CreateExpression(Create<string>(), null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("variableRegistry");
            }

            [Fact]
            public void Should_Return_Variable_Expression()
            {
                var variableName = Create<string>();
                var variableRegistry = new AoiVariableRegistry();

                var expected = CreateVariableExpression(variableName, variableRegistry).ToString();
                var actual = _factory.CreateExpression(variableName, variableRegistry).ToString();

                actual.Should().Be(expected);
            }

            private Expression CreateVariableExpression(string variableName, IAoiVariableRegistry variableRegistry)
            {
                var registry = Expression.Constant(variableRegistry);

                var getValueMethod = typeof(IAoiReadableVariableRegistry).GetMethod("GetValue", new[] { typeof(string) });
                var variableExpression = new Expression[] { Expression.Constant(variableName) };

                return Expression.Call(registry, getValueMethod, variableExpression);
            }
        }
    }
}
