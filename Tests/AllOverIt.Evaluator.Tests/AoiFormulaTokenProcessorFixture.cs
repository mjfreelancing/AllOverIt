using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Stack;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
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
    public class AoiFormulaTokenProcessorFixture : AoiFixtureBase
    {
        private AoiFormulaTokenProcessor _processor;
        private readonly Fake<IList<AoiFormulaTokenProcessorContext>> _tokenProcessorsFake;
        private readonly Fake<IAoiFormulaReader> _formulaReaderFake;
        private readonly Fake<IAoiFormulaProcessor> _formulaProcessorFake;
        private readonly Fake<IAoiFormulaExpressionFactory> _expressionFactoryFake;
        private readonly Fake<IAoiArithmeticOperationFactory> _operationFactoryFake;
        private readonly Fake<IAoiStack<string>> _operatorsFake;
        private readonly Fake<IAoiStack<Expression>> _expressionsFake;
        private readonly bool _isUserDefined;

        public AoiFormulaTokenProcessorFixture()
        {
            this.UseFakeItEasy();

            _isUserDefined = Create<bool>();

            _tokenProcessorsFake = new Fake<IList<AoiFormulaTokenProcessorContext>>();
            _formulaReaderFake = new Fake<IAoiFormulaReader>();
            _formulaProcessorFake = new Fake<IAoiFormulaProcessor>();
            _expressionFactoryFake = new Fake<IAoiFormulaExpressionFactory>();
            _operationFactoryFake = new Fake<IAoiArithmeticOperationFactory>();
            _operatorsFake = new Fake<IAoiStack<string>>();
            _expressionsFake = new Fake<IAoiStack<Expression>>();

            _processor = new AoiFormulaTokenProcessor(_tokenProcessorsFake.FakedObject, _formulaProcessorFake.FakedObject, _formulaReaderFake.FakedObject,
              _expressionFactoryFake.FakedObject, _operationFactoryFake.FakedObject);
        }

        public class Constructor : AoiFormulaTokenProcessorFixture
        {
            [Fact]
            public void Should_Throw_When_TokenProcessors_Null()
            {
                Invoking(() =>
                    {
                        _processor = new AoiFormulaTokenProcessor(null, _formulaProcessorFake.FakedObject, _formulaReaderFake.FakedObject,
                            this.CreateStub<IAoiFormulaExpressionFactory>(), this.CreateStub<IAoiArithmeticOperationFactory>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("tokenProcessors"));
            }

            [Fact]
            public void Should_Throw_When_FormulaProcessor_Null()
            {
                Invoking(() =>
                    {
                        _processor = new AoiFormulaTokenProcessor(CreateMany<AoiFormulaTokenProcessorContext>().AsList(), null,
                            _formulaReaderFake.FakedObject,
                            this.CreateStub<IAoiFormulaExpressionFactory>(), this.CreateStub<IAoiArithmeticOperationFactory>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("formulaProcessor"));
            }

            [Fact]
            public void Should_Throw_When_FormulaReader_Null()
            {
                Invoking(() =>
                    {
                        _processor = new AoiFormulaTokenProcessor(CreateMany<AoiFormulaTokenProcessorContext>().AsList(),
                            _formulaProcessorFake.FakedObject, null,
                            this.CreateStub<IAoiFormulaExpressionFactory>(), this.CreateStub<IAoiArithmeticOperationFactory>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("formulaReader"));
            }

            [Fact]
            public void Should_Throw_When_ExpressionFactory_Null()
            {
                Invoking(() =>
                    {
                        _processor = new AoiFormulaTokenProcessor(CreateMany<AoiFormulaTokenProcessorContext>().AsList(),
                            _formulaProcessorFake.FakedObject, _formulaReaderFake.FakedObject,
                            null, this.CreateStub<IAoiArithmeticOperationFactory>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("expressionFactory"));
            }

            [Fact]
            public void Should_Throw_When_OperationFactory_Null()
            {
                Invoking(() =>
                    {
                        _processor = new AoiFormulaTokenProcessor(CreateMany<AoiFormulaTokenProcessorContext>().AsList(),
                            _formulaProcessorFake.FakedObject, _formulaReaderFake.FakedObject,
                            this.CreateStub<IAoiFormulaExpressionFactory>(), null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("operationFactory"));
            }

            [Fact]
            public void Should_Register_Token_Processors()
            {
                _tokenProcessorsFake
                  .CallsTo(fake => fake.Add(A<AoiFormulaTokenProcessorContext>.Ignored))
                  .MustHaveHappened(6, Times.Exactly);
            }
        }

        public class Constructor_RegisterTokenProcessors : AoiFormulaTokenProcessorFixture
        {
            public Constructor_RegisterTokenProcessors()
            {
                _processor = new AoiFormulaTokenProcessor(_formulaProcessorFake.FakedObject, _formulaReaderFake.FakedObject,
                  _expressionFactoryFake.FakedObject, _operationFactoryFake.FakedObject);
            }

            public class OpenScope : Constructor_RegisterTokenProcessors
            {
                [Fact]
                public void Should_Call_FormulaReader_ReadNext()
                {
                    _processor.ProcessToken('(', Create<bool>());

                    _formulaReaderFake
                      .CallsTo(fake => fake.ReadNext())
                      .MustHaveHappened(1, Times.Exactly);
                }

                [Fact]
                public void Should_Call_FormulaProcessor_ProcessScopeStart()
                {
                    _processor.ProcessToken('(', Create<bool>());

                    _formulaProcessorFake
                      .CallsTo(fake => fake.ProcessScopeStart())
                      .MustHaveHappened(1, Times.Exactly);
                }

                [Fact]
                public void Should_Return_True()
                {
                    var result = _processor.ProcessToken('(', Create<bool>());

                    result.Should().BeTrue();
                }
            }

            public class ScopeEnd : Constructor_RegisterTokenProcessors
            {
                [Fact]
                public void Should_Call_FormulaReader_ReadNext()
                {
                    _processor.ProcessToken(')', _isUserDefined);

                    _formulaReaderFake
                      .CallsTo(fake => fake.ReadNext())
                      .MustHaveHappened(1, Times.Exactly);
                }

                [Fact]
                public void Should_Call_FormulaProcessor_ProcessScopeEnd()
                {
                    _processor.ProcessToken(')', _isUserDefined);

                    _formulaProcessorFake
                      .CallsTo(fake => fake.ProcessScopeEnd(_isUserDefined))
                      .MustHaveHappened(1, Times.Exactly);
                }

                [Fact]
                public void Should_Return_Result()
                {
                    var expected = Create<bool>();

                    _formulaProcessorFake
                      .CallsTo(fake => fake.ProcessScopeEnd(_isUserDefined))
                      .Returns(expected);

                    var result = _processor.ProcessToken(')', _isUserDefined);

                    result.Should().Be(expected);
                }
            }

            public class MethodArgument : Constructor_RegisterTokenProcessors
            {
                [Fact]
                public void Should_Not_Process_When_Not_UserDefined()
                {
                    _processor.ProcessToken(',', false);

                    _formulaProcessorFake
                      .CallsTo(fake => fake.ProcessMethodArgument())
                      .MustHaveHappened(0, Times.Exactly);
                }

                [Fact]
                public void Should_Call_FormulaReader_ReadNext()
                {
                    _processor.ProcessToken(',', true);

                    _formulaReaderFake
                      .CallsTo(fake => fake.ReadNext())
                      .MustHaveHappened(1, Times.Exactly);
                }

                [Fact]
                public void Should_Call_FormulaProcessor_ProcessMethodArgument()
                {
                    _processor.ProcessToken(',', true);

                    _formulaProcessorFake
                      .CallsTo(fake => fake.ProcessMethodArgument())
                      .MustHaveHappened(1, Times.Exactly);
                }

                [Fact]
                public void Should_Return_True()
                {
                    var result = _processor.ProcessToken(',', true);

                    result.Should().BeTrue();
                }
            }

            public class Operator : Constructor_RegisterTokenProcessors
            {
                private readonly char _token;

                public Operator()
                {
                    _token = CreateExcluding('(', ')', ',');

                    _operationFactoryFake
                      .CallsTo(fake => fake.IsCandidate(_token))
                      .Returns(true);
                }

                [Fact]
                public void Should_Not_Process_When_Not_Registered_Token()
                {
                    _operationFactoryFake
                      .CallsTo(fake => fake.IsCandidate(_token))
                      .Returns(false);

                    _processor.ProcessToken(_token, _isUserDefined);

                    _formulaProcessorFake
                      .CallsTo(fake => fake.ProcessOperator())
                      .MustHaveHappened(0, Times.Exactly);
                }

                [Fact]
                public void Should_Not_Call_FormulaReader_ReadNext()
                {
                    _processor.ProcessToken(_token, _isUserDefined);

                    _formulaReaderFake
                      .CallsTo(fake => fake.ReadNext())
                      .MustHaveHappened(0, Times.Exactly);
                }

                [Fact]
                public void Should_Call_FormulaProcessor_ProcessOperator()
                {
                    _processor.ProcessToken(_token, _isUserDefined);

                    _formulaProcessorFake
                      .CallsTo(fake => fake.ProcessOperator())
                      .MustHaveHappened(1, Times.Exactly);
                }

                [Fact]
                public void Should_Return_True()
                {
                    var result = _processor.ProcessToken(_token, _isUserDefined);

                    result.Should().BeTrue();
                }
            }

            public class Numerical : Constructor_RegisterTokenProcessors
            {
                private readonly char _token;

                public Numerical()
                {
                    _token = (char)('0' + (Create<int>() % 10));    // 0 to 9
                }

                [Fact]
                public void Should_Not_Process_When_Not_Numerical_Candidate()
                {
                    _processor.ProcessToken('a', Create<bool>());

                    _formulaProcessorFake
                      .CallsTo(fake => fake.ProcessNumerical())
                      .MustHaveHappened(0, Times.Exactly);
                }

                [Fact]
                public void Should_Not_Call_FormulaReader_ReadNext()
                {
                    _processor.ProcessToken(_token, Create<bool>());

                    _formulaReaderFake
                      .CallsTo(fake => fake.ReadNext())
                      .MustHaveHappened(0, Times.Exactly);
                }

                [Fact]
                public void Should_Call_FormulaProcessor_ProcessNumerical()
                {
                    _processor.ProcessToken(_token, Create<bool>());

                    _formulaProcessorFake
                      .CallsTo(fake => fake.ProcessNumerical())
                      .MustHaveHappened(1, Times.Exactly);
                }

                [Fact]
                public void Should_Return_True()
                {
                    var result = _processor.ProcessToken(_token, Create<bool>());

                    result.Should().BeTrue();
                }
            }

            public class NamedOperand : Constructor_RegisterTokenProcessors
            {
                private readonly char _token;

                public NamedOperand()
                {
                    _token = (char)('a' + (Create<int>() % 26));    // a to z
                }

                [Fact]
                public void Should_Not_Call_FormulaReader_ReadNext()
                {
                    _processor.ProcessToken(_token, Create<bool>());

                    _formulaReaderFake
                      .CallsTo(fake => fake.ReadNext())
                      .MustHaveHappened(0, Times.Exactly);
                }

                [Fact]
                public void Should_Call_FormulaProcessor_ProcessNamedOperand()
                {
                    _processor.ProcessToken(_token, Create<bool>());

                    _formulaProcessorFake
                      .CallsTo(fake => fake.ProcessNamedOperand())
                      .MustHaveHappened(1, Times.Exactly);
                }

                [Fact]
                public void Should_Return_True()
                {
                    var result = _processor.ProcessToken(_token, Create<bool>());

                    result.Should().BeTrue();
                }
            }
        }

        public class ProcessOperators : AoiFormulaTokenProcessorFixture
        {
            [Fact]
            public void Should_Throw_When_Operators_Null()
            {
                Invoking(() => _processor.ProcessOperators(null, this.CreateStub<IAoiStack<Expression>>(), () => false))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("operators"));
            }

            [Fact]
            public void Should_Throw_When_Expressions_Null()
            {
                Invoking(() => _processor.ProcessOperators(this.CreateStub<IAoiStack<string>>(), null, () => false))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("expressions"));
            }

            [Fact]
            public void Should_Throw_When_Condition_Null()
            {
                Invoking(() => _processor.ProcessOperators(this.CreateStub<IAoiStack<string>>(), this.CreateStub<IAoiStack<Expression>>(), null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("condition"));
            }

            [Fact]
            public void Should_Not_Process_When_No_Operators()
            {
                _operatorsFake
                  .CallsTo(fake => fake.Any())
                  .Returns(false);

                _processor.ProcessOperators(_operatorsFake.FakedObject, _expressionsFake.FakedObject, () => true);

                _expressionsFake
                  .CallsTo(fake => fake.Push(A<Expression>.Ignored))
                  .MustHaveHappened(0, Times.Exactly);
            }

            [Fact]
            public void Should_Process_When_Many_Operators()
            {
                var operators = CreateMany<string>(3);
                var expressions = new[] { this.CreateStub<Expression>(), this.CreateStub<Expression>(), this.CreateStub<Expression>() };
                var operations = CreateMany<AoiArithmeticOperation>(3);

                _operatorsFake
                  .CallsTo(fake => fake.Any())
                  .ReturnsNextFromSequence(new[] { true, true, true, false });

                for (var idx = 0; idx < 3; idx++)
                {
                    var op = operators.ElementAt(idx);

                    _operationFactoryFake
                        .CallsTo(fake => fake.GetOperation(op))
                        .Returns(operations.ElementAt(idx));

                    _expressionFactoryFake
                        .CallsTo(fake => fake.CreateExpression(operations.ElementAt(idx), _expressionsFake.FakedObject))
                        .Returns(expressions.ElementAt(idx));
                }

                _processor.ProcessOperators(_operatorsFake.FakedObject, _expressionsFake.FakedObject, () => true);

                _expressionsFake
                  .CallsTo(fake => fake.Push(A<Expression>.Ignored))
                  .MustHaveHappened(3, Times.Exactly);
            }

            [Fact]
            public void Should_Call_Operators_Pop()
            {
                _operatorsFake
                  .CallsTo(fake => fake.Any())
                  .ReturnsNextFromSequence(new[] { true, false });

                _processor.ProcessOperators(_operatorsFake.FakedObject, _expressionsFake.FakedObject, () => true);

                _operatorsFake
                  .CallsTo(fake => fake.Pop())
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Call_OperationFactory_Operation()
            {
                var nextOperator = Create<string>();

                _operatorsFake
                  .CallsTo(fake => fake.Pop())
                  .Returns(nextOperator);

                _operatorsFake
                  .CallsTo(fake => fake.Any())
                  .ReturnsNextFromSequence(new[] { true, false });

                _processor.ProcessOperators(_operatorsFake.FakedObject, _expressionsFake.FakedObject, () => true);

                _operationFactoryFake
                  .CallsTo(fake => fake.GetOperation(nextOperator))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Call_ExpressionFactory_CreateExpression()
            {
                var nextOperator = Create<string>();
                var operation = Create<AoiArithmeticOperation>();

                _operatorsFake
                  .CallsTo(fake => fake.Pop())
                  .Returns(nextOperator);

                _operationFactoryFake
                  .CallsTo(fake => fake.GetOperation(nextOperator))
                  .Returns(operation);

                _operatorsFake
                  .CallsTo(fake => fake.Any())
                  .ReturnsNextFromSequence(new[] { true, false });

                _processor.ProcessOperators(_operatorsFake.FakedObject, _expressionsFake.FakedObject, () => true);

                _expressionFactoryFake
                  .CallsTo(fake => fake.CreateExpression(operation, _expressionsFake.FakedObject))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Call_Expressions_Push()
            {
                var nextOperator = Create<string>();
                var operation = Create<AoiArithmeticOperation>();
                var expression = this.CreateStub<Expression>();

                _operatorsFake
                  .CallsTo(fake => fake.Pop())
                  .Returns(nextOperator);

                _operationFactoryFake
                  .CallsTo(fake => fake.GetOperation(nextOperator))
                  .Returns(operation);

                _expressionFactoryFake
                  .CallsTo(fake => fake.CreateExpression(operation, _expressionsFake.FakedObject))
                  .Returns(expression);

                _operatorsFake
                  .CallsTo(fake => fake.Any())
                  .ReturnsNextFromSequence(new[] { true, false });

                _processor.ProcessOperators(_operatorsFake.FakedObject, _expressionsFake.FakedObject, () => true);

                _expressionsFake
                  .CallsTo(fake => fake.Push(expression))
                  .MustHaveHappened(1, Times.Exactly);
            }
        }

        public class ProcessToken : AoiFormulaTokenProcessorFixture
        {
            [Fact]
            public void Should_Throw_When_No_Tokens_Registered()
            {
                Invoking(() => _processor.ProcessToken(Create<char>(), Create<bool>()))
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage("Sequence contains no elements");
            }

            [Fact]
            public void Should_Invoke_First_Matching_Token_Processor()
            {
                var token = Create<char>();
                var invoked1 = false;
                var invoked2 = false;

                var context1 = new AoiFormulaTokenProcessorContext(
                  (t, u) => t == token,
                  (t, u) =>
                  {
                      invoked1 = true;
                      return false;
                  });

                var context2 = new AoiFormulaTokenProcessorContext(
                  (t, u) => t == token,
                  (t, u) =>
                  {
                      invoked2 = true;
                      return false;
                  });

                var tokenProcessors = new List<AoiFormulaTokenProcessorContext> { context1, context2 };

                _processor = new AoiFormulaTokenProcessor(tokenProcessors, _formulaProcessorFake.FakedObject, _formulaReaderFake.FakedObject,
                  _expressionFactoryFake.FakedObject, _operationFactoryFake.FakedObject);

                _processor.ProcessToken(token, Create<bool>());

                invoked1.Should().BeTrue();
                invoked2.Should().BeFalse();
            }

            [Fact]
            public void Should_Invoke_Processor_With_Arguments()
            {
                var token = Create<char>();
                var userMethod = Create<bool>();

                var tokenMatched = false;
                var userMethodMatched = false;

                var context1 = new AoiFormulaTokenProcessorContext(
                  (t, u) => t == token,
                  (t, u) =>
                  {
                      tokenMatched = t == token;
                      userMethodMatched = u == userMethod;

                      return false;
                  });

                var tokenProcessors = new List<AoiFormulaTokenProcessorContext> { context1 };

                _processor = new AoiFormulaTokenProcessor(tokenProcessors, _formulaProcessorFake.FakedObject, _formulaReaderFake.FakedObject,
                  _expressionFactoryFake.FakedObject, _operationFactoryFake.FakedObject);

                _processor.ProcessToken(token, userMethod);

                tokenMatched.Should().BeTrue();
                userMethodMatched.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_Processor_Result()
            {
                var expected = Create<bool>();

                var context1 = new AoiFormulaTokenProcessorContext(
                  (t, u) => true,
                  (t, u) => expected);

                var tokenProcessors = new List<AoiFormulaTokenProcessorContext> { context1 };

                _processor = new AoiFormulaTokenProcessor(tokenProcessors, _formulaProcessorFake.FakedObject, _formulaReaderFake.FakedObject,
                  _expressionFactoryFake.FakedObject, _operationFactoryFake.FakedObject);

                var result = _processor.ProcessToken(Create<char>(), Create<bool>());

                result.Should().Be(expected);
            }
        }
    }
}
