using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using AllOverIt.Evaluator.Stack;
using AllOverIt.Evaluator.Tests.Operations.Dummies;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class AoiFormulaProcessorFixture : AoiFixtureBase
    {
        private readonly Expression _expression;
        private readonly Fake<IAoiStack<string>> _operatorStackFake;
        private readonly Fake<IAoiStack<Expression>> _expressionStackFake;
        private readonly Fake<IAoiArithmeticOperationFactory> _operationFactoryFake;
        private readonly Fake<IAoiFormulaExpressionFactory> _formulaExpressionFactoryFake;
        private readonly Fake<IAoiFormulaTokenProcessor> _tokenProcessorFake;
        private readonly Fake<IAoiUserDefinedMethodFactory> _userDefinedMethodFactoryFake;
        private readonly Fake<IAoiFormulaReader> _formulaReaderFake;
        private readonly Fake<IAoiVariableRegistry> _variableRegistryFake;
        private AoiFormulaProcessor _formulaProcessor;

        public AoiFormulaProcessorFixture()
        {
            _expression = Expression.Constant(Create<double>());

            _operatorStackFake = new Fake<IAoiStack<string>>();
            _expressionStackFake = new Fake<IAoiStack<Expression>>();
            _operationFactoryFake = new Fake<IAoiArithmeticOperationFactory>();
            _formulaExpressionFactoryFake = new Fake<IAoiFormulaExpressionFactory>();
            _tokenProcessorFake = new Fake<IAoiFormulaTokenProcessor>();
            _userDefinedMethodFactoryFake = new Fake<IAoiUserDefinedMethodFactory>();
            _formulaReaderFake = new Fake<IAoiFormulaReader>();
            _variableRegistryFake = new Fake<IAoiVariableRegistry>();

            _expressionStackFake
              .CallsTo(fake => fake.Pop())
              .Returns(_expression);

            _formulaProcessor = new AoiFormulaProcessor(_operatorStackFake.FakedObject, _expressionStackFake.FakedObject, _formulaExpressionFactoryFake.FakedObject,
              _operationFactoryFake.FakedObject, _userDefinedMethodFactoryFake.FakedObject, (p, r, e, o) => _tokenProcessorFake.FakedObject);
        }

        public class StaticMembers : AoiFormulaProcessorFixture
        {
            [Fact]
            public void Should_Define_Custom_Token_UserMethod()
            {
                AoiFormulaProcessor.CustomTokens.UserMethod.Should().Be("$$");
            }

            [Fact]
            public void Should_Define_Custom_Token_UnaryMinus()
            {
                AoiFormulaProcessor.CustomTokens.UnaryMinus.Should().Be("##");
            }

            [Fact]
            public void Should_Define_Custom_Token_OpenScope()
            {
                AoiFormulaProcessor.CustomTokens.OpenScope.Should().Be("(");
            }
        }

        public class Constructor : AoiFormulaProcessorFixture
        {
            [Fact]
            public void Should_Throw_When_Operation_Factory_Null()
            {
                Invoking(() => _formulaProcessor = new AoiFormulaProcessor(null, this.CreateStub<IAoiUserDefinedMethodFactory>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operationFactory");
            }

            [Fact]
            public void Should_Throw_When_User_Method_Factory_Null()
            {
                Invoking(() => _formulaProcessor = new AoiFormulaProcessor(this.CreateStub<IAoiArithmeticOperationFactory>(), null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("userDefinedMethodFactory");
            }

            [Fact]
            public void Should_Throw_When_OperatorStack_Null()
            {
                Invoking(() =>
                    {
                        _formulaProcessor = new AoiFormulaProcessor(null, _expressionStackFake.FakedObject, _formulaExpressionFactoryFake.FakedObject,
                            _operationFactoryFake.FakedObject, _userDefinedMethodFactoryFake.FakedObject,
                            (p, r, e, o) => _tokenProcessorFake.FakedObject);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operatorStack");
            }

            [Fact]
            public void Should_Throw_When_ExpressionStack_Null()
            {
                Invoking(() =>
                    {
                        _formulaProcessor = new AoiFormulaProcessor(_operatorStackFake.FakedObject, null, _formulaExpressionFactoryFake.FakedObject,
                            _operationFactoryFake.FakedObject, _userDefinedMethodFactoryFake.FakedObject,
                            (p, r, e, o) => _tokenProcessorFake.FakedObject);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expressionStack");
            }

            [Fact]
            public void Should_Throw_When_ExpressionFactory_Null()
            {
                Invoking(() =>
                    {
                        _formulaProcessor = new AoiFormulaProcessor(_operatorStackFake.FakedObject, _expressionStackFake.FakedObject, null,
                            _operationFactoryFake.FakedObject, _userDefinedMethodFactoryFake.FakedObject,
                            (p, r, e, o) => _tokenProcessorFake.FakedObject);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("formulaExpressionFactory");
            }

            [Fact]
            public void Should_Throw_When_TokenProcessorFactory_Null()
            {
                Invoking(() =>
                    {
                        _formulaProcessor = new AoiFormulaProcessor(_operatorStackFake.FakedObject, _expressionStackFake.FakedObject,
                            _formulaExpressionFactoryFake.FakedObject, _operationFactoryFake.FakedObject, _userDefinedMethodFactoryFake.FakedObject,
                            null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("tokenProcessorFactory");
            }

            [Fact]
            public void Should_Create_Token_Processor()
            {
                _formulaReaderFake
                  .CallsTo(fake => fake.PeekNext())
                  .Returns(-1);

                _expressionStackFake
                  .CallsTo(fake => fake.Pop())
                  .Returns(Expression.Constant(Create<double>()));

                _formulaProcessor.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject);

                _tokenProcessorFake
                  .CallsTo(fake => fake.ProcessOperators(_operatorStackFake.FakedObject, _expressionStackFake.FakedObject, A<Func<bool>>.Ignored))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Register_Unary_Minus()
            {
                _operationFactoryFake
                  .CallsTo(fake => fake.RegisterOperation(AoiFormulaProcessor.CustomTokens.UnaryMinus, 4, 1, A<Func<Expression[], IAoiOperator>>.Ignored))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Register_Unary_Minus_Operation()
            {
                var expression = this.CreateStub<Expression>();
                Func<Expression[], IAoiOperator> operation = null;

                _operationFactoryFake
                  .CallsTo(fake => fake.RegisterOperation(AoiFormulaProcessor.CustomTokens.UnaryMinus, 4, 1, A<Func<Expression[], IAoiOperator>>.Ignored))
                  .Invokes(call => operation = call.Arguments.Get<Func<Expression[], IAoiOperator>>(3));

                _formulaProcessor = new AoiFormulaProcessor(_operatorStackFake.FakedObject, _expressionStackFake.FakedObject, _formulaExpressionFactoryFake.FakedObject,
                  _operationFactoryFake.FakedObject, _userDefinedMethodFactoryFake.FakedObject, (p, r, e, o) => _tokenProcessorFake.FakedObject);

                var negateOperator = operation.Invoke(new[] { expression }) as AoiNegateOperator;

                negateOperator.Should().NotBeNull();

                negateOperator.Should().BeEquivalentTo(new
                {
                    Operand = expression,
                    OperatorType = default(Func<Expression, Expression>)
                },
                opt => opt.Excluding(s => s.OperatorType));

                negateOperator.OperatorType.Should().BeOfType<Func<Expression, Expression>>();
            }
        }

        public class Process : AoiFormulaProcessorFixture
        {
            public Process()
            {
                _expressionStackFake
                  .CallsTo(fake => fake.Pop())
                  .Returns(Expression.Constant(Create<double>()));
            }

            [Fact]
            public void Should_Throw_When_FormulaReader_Null()
            {
                Invoking(() => _formulaProcessor.Process(null, _variableRegistryFake.FakedObject))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("formulaReader");
            }

            [Fact]
            public void Should_Throw_When_VariableRegistry_Null()
            {
                Invoking(() => _formulaProcessor.Process(_formulaReaderFake.FakedObject, null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("variableRegistry");
            }

            [Fact]
            public void Should_Clear_Symbol_Stack()
            {
                _formulaProcessor.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject);

                _operatorStackFake
                  .CallsTo(fake => fake.Clear())
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Clear_Expression_Stack()
            {
                _formulaProcessor.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject);

                _expressionStackFake
                  .CallsTo(fake => fake.Clear())
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Set_LastPushIsOperator_True()
            {
                _formulaProcessor.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject);

                _formulaProcessor.LastPushIsOperator.Should().BeTrue();
            }

            [Fact]
            public void Should_Invoke_TokenProcessorFactory()
            {
                var created = false;

                Func<IAoiFormulaProcessor, IAoiFormulaReader, IAoiFormulaExpressionFactory, IAoiArithmeticOperationFactory, IAoiFormulaTokenProcessor>
                  factory = (p, r, e, o) =>
                {
                    created = true;
                    return _tokenProcessorFake.FakedObject;
                };

                _formulaProcessor = new AoiFormulaProcessor(_operatorStackFake.FakedObject, _expressionStackFake.FakedObject,
                  _formulaExpressionFactoryFake.FakedObject, _operationFactoryFake.FakedObject, _userDefinedMethodFactoryFake.FakedObject, factory);

                _formulaProcessor.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject);

                created.Should().BeTrue();
            }

            [Fact]
            public void Should_Parse_Content_PeekNext()
            {
                _formulaReaderFake
                  .CallsTo(fake => fake.PeekNext())
                  .Returns(-1);

                _formulaProcessor.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject);

                _formulaReaderFake
                  .CallsTo(fake => fake.PeekNext())
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Parse_Content_PeekNext_Until_End()
            {
                var charsToRead = CreateMany<int>().ToList();
                charsToRead.Add(-1);

                _formulaReaderFake
                  .CallsTo(fake => fake.PeekNext())
                  .ReturnsNextFromSequence(charsToRead.ToArray());

                charsToRead.ForEach(ch =>
                {
                    _tokenProcessorFake
              .CallsTo(fake => fake.ProcessToken((char)ch, false))
              .Returns(true);
                });

                _formulaProcessor.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject);

                _formulaReaderFake
                  .CallsTo(fake => fake.PeekNext())
                  .MustHaveHappened(charsToRead.Count, Times.Exactly);
            }

            [Fact]
            public void Should_Call_TokenProcessor_ProcessOperators()
            {
                _formulaProcessor.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject);

                _tokenProcessorFake
                  .CallsTo(fake => fake.ProcessOperators(_operatorStackFake.FakedObject, _expressionStackFake.FakedObject, A<Func<bool>>.Ignored))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Return_ProcessorResult()
            {
                var namedOperand = Create<string>();

                _formulaReaderFake
                  .CallsTo(fake => fake.ReadNamedOperand(_operationFactoryFake.FakedObject))
                  .Returns(namedOperand);

                _formulaReaderFake
                  .CallsTo(fake => fake.PeekNext())
                  .ReturnsNextFromSequence('(', -1);

                _tokenProcessorFake
                  .CallsTo(fake => fake.ProcessToken('(', false))
                  .Invokes(call => _formulaProcessor.ProcessNamedOperand());

                var expected = Create<double>();
                var expression = Expression.Constant(expected);

                _expressionStackFake
                  .CallsTo(fake => fake.Pop())
                  .Returns(expression);

                var processorResult = _formulaProcessor.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject);

                var compiled = processorResult.FormulaExpression.Compile();
                var result = compiled.Invoke();

                result.Should().Be(expected);

                processorResult.Should().BeEquivalentTo(new
                {
                    ReferencedVariableNames = new[] { namedOperand },
                    FormulaExpression = default(Expression<Func<double>>)
                }, option => option.Excluding(prop => prop.FormulaExpression));
            }
        }

        public class PushOperator : AoiFormulaProcessorFixture
        {
            [Fact]
            public void Should_Throw_When_Token_Null()
            {
                Invoking(() => _formulaProcessor.PushOperator(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operatorToken");
            }

            [Fact]
            public void Should_Throw_When_Token_Empty()
            {
                Invoking(() => _formulaProcessor.PushOperator(string.Empty))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("operatorToken");
            }

            [Fact]
            public void Should_Throw_When_Token_Whitespace()
            {
                Invoking(() => _formulaProcessor.PushOperator("   "))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("operatorToken");
            }

            [Fact]
            public void Should_Push_Symbol_Stack()
            {
                var token = Create<string>();

                _formulaProcessor.PushOperator(token);

                _operatorStackFake
                  .CallsTo(fake => fake.Push(token))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Set_LastPushIsOperator_True()
            {
                _formulaProcessor.PushOperator(Create<string>());

                _formulaProcessor.LastPushIsOperator.Should().BeTrue();
            }
        }

        public class PushExpression : AoiFormulaProcessorFixture
        {
            public PushExpression()
            {
                _formulaProcessor.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject);
            }

            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() => _formulaProcessor.PushExpression(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Push_Expression()
            {
                _formulaProcessor.PushExpression(_expression);

                _expressionStackFake
                  .CallsTo(fake => fake.Push(_expression))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Set_LastPushIsOperator_False()
            {
                _formulaProcessor.PushExpression(_expression);

                _formulaProcessor.LastPushIsOperator.Should().BeFalse();
            }
        }

        public class ProcessScopeStart : AoiFormulaProcessorFixture
        {
            public ProcessScopeStart()
            {
                _formulaProcessor.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject);
            }

            [Fact]
            public void Should_Push_Symbol_OpenScope()
            {
                _formulaProcessor.ProcessScopeStart();

                _operatorStackFake
                  .CallsTo(fake => fake.Push(AoiFormulaProcessor.CustomTokens.OpenScope))
                  .MustHaveHappened(1, Times.Exactly);
            }
        }

        public class ProcessScopeEnd : AoiFormulaProcessorFixture
        {
            public ProcessScopeEnd()
            {
                _expressionStackFake
                  .CallsTo(fake => fake.Pop())
                  .Returns(Expression.Constant(Create<double>()));

                _formulaProcessor.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject);
            }

            [Fact]
            public void Should_Process_Operators_When_OperatorStack_Has_No_OpenScope()
            {
                var token = Create<string>();   // any thing other than FormulaProcessor.CustomTokens.OpenScope
                Func<bool> condition = null;

                _operatorStackFake
                  .CallsTo(fake => fake.Any())
                  .Returns(true);

                _expressionStackFake
                  .CallsTo(fake => fake.Any())
                  .Returns(true);

                _operatorStackFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(token);

                _tokenProcessorFake
                  .CallsTo(fake => fake.ProcessOperators(_operatorStackFake.FakedObject, _expressionStackFake.FakedObject, A<Func<bool>>.Ignored))
                  .Invokes(call => condition = call.Arguments.Get<Func<bool>>(2));

                _formulaProcessor.ProcessScopeEnd(false);

                var result = condition.Invoke();

                result.Should().BeTrue();
            }

            [Fact]
            public void Should_Process_Operators_When_OperatorStack_Has_OpenScope()
            {
                Func<bool> condition = null;

                _operatorStackFake
                  .CallsTo(fake => fake.Any())
                  .Returns(true);

                _expressionStackFake
                  .CallsTo(fake => fake.Any())
                  .Returns(true);

                _operatorStackFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(AoiFormulaProcessor.CustomTokens.OpenScope);

                _tokenProcessorFake
                  .CallsTo(fake => fake.ProcessOperators(_operatorStackFake.FakedObject, _expressionStackFake.FakedObject, A<Func<bool>>.Ignored))
                  .Invokes(call => condition = call.Arguments.Get<Func<bool>>(2));

                _formulaProcessor.ProcessScopeEnd(false);

                var result = condition.Invoke();

                result.Should().BeFalse();
            }

            [Fact]
            public void Should_Call_OperatorStack_Pop()
            {
                _formulaProcessor.ProcessScopeEnd(false);

                _operatorStackFake
                  .CallsTo(fake => fake.Pop())
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Not_Call_OperatorStack_Any_When_Not_User_Defined_Method()
            {
                _formulaProcessor.ProcessScopeEnd(false);

                _operatorStackFake
                  .CallsTo(fake => fake.Any())
                  .MustHaveHappened(0, Times.Exactly);
            }

            [Fact]
            public void Should_Call_OperatorStack_Any_When_User_Defined_Method()
            {
                _operatorStackFake
                  .CallsTo(fake => fake.Any())
                  .Returns(true);

                _formulaProcessor.ProcessScopeEnd(true);

                _operatorStackFake
                  .CallsTo(fake => fake.Any())
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Throw_When_User_Defined_Method_And_Invalid_Stack()
            {
                _operatorStackFake
                  .CallsTo(fake => fake.Any())
                  .Returns(false);

                Invoking(() => _formulaProcessor.ProcessScopeEnd(true))
                    .Should()
                    .Throw<AoiFormulaException>()
                    .WithMessage("Invalid expression stack");
            }

            [Fact]
            public void Should_Return_False_When_User_Defined_Method_Peek_UserMethod()
            {
                _operatorStackFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(AoiFormulaProcessor.CustomTokens.UserMethod);

                _operatorStackFake
                  .CallsTo(fake => fake.Any())
                  .Returns(true);

                var result = _formulaProcessor.ProcessScopeEnd(true);

                result.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_False_When_User_Defined_Method_Not_Peek_UserMethod()
            {
                _operatorStackFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(AoiFormulaProcessor.CustomTokens.OpenScope); // anything other than UserMethod

                _operatorStackFake
                  .CallsTo(fake => fake.Any())
                  .Returns(true);

                var result = _formulaProcessor.ProcessScopeEnd(true);

                result.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_True()
            {
                var result = _formulaProcessor.ProcessScopeEnd(false);

                result.Should().BeTrue();
            }
        }

        public class ProcessMethodArgument : AoiFormulaProcessorFixture
        {
            public ProcessMethodArgument()
            {
                _expressionStackFake
                  .CallsTo(fake => fake.Pop())
                  .Returns(Expression.Constant(Create<double>()));

                _formulaProcessor.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject);
            }

            [Fact]
            public void Should_Set_LastPushIsOperator_True()
            {
                _formulaProcessor.ProcessMethodArgument();

                _formulaProcessor.LastPushIsOperator.Should().BeTrue();
            }

            [Fact]
            public void Should_Process_Operators_When_OperatorStack_Has_No_OpenScope()
            {
                var token = Create<string>();   // any thing other than FormulaProcessor.CustomTokens.OpenScope
                Func<bool> condition = null;

                _operatorStackFake
                  .CallsTo(fake => fake.Any())
                  .Returns(true);

                _expressionStackFake
                  .CallsTo(fake => fake.Any())
                  .Returns(true);

                _operatorStackFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(token);

                _tokenProcessorFake
                  .CallsTo(fake => fake.ProcessOperators(_operatorStackFake.FakedObject, _expressionStackFake.FakedObject, A<Func<bool>>.Ignored))
                  .Invokes(call => condition = call.Arguments.Get<Func<bool>>(2));

                _formulaProcessor.ProcessMethodArgument();

                var result = condition.Invoke();

                result.Should().BeTrue();
            }

            [Fact]
            public void Should_Process_Operators_When_OperatorStack_Has_OpenScope()
            {
                Func<bool> condition = null;

                _operatorStackFake
                  .CallsTo(fake => fake.Any())
                  .Returns(true);

                _expressionStackFake
                  .CallsTo(fake => fake.Any())
                  .Returns(true);

                _operatorStackFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(AoiFormulaProcessor.CustomTokens.OpenScope);

                _tokenProcessorFake
                  .CallsTo(fake => fake.ProcessOperators(_operatorStackFake.FakedObject, _expressionStackFake.FakedObject, A<Func<bool>>.Ignored))
                  .Invokes(call => condition = call.Arguments.Get<Func<bool>>(2));

                _formulaProcessor.ProcessMethodArgument();

                var result = condition.Invoke();

                result.Should().BeFalse();
            }
        }

        public class ProcessOperator : AoiFormulaProcessorFixture
        {
            public ProcessOperator()
            {
                _expressionStackFake
                  .CallsTo(fake => fake.Pop())
                  .Returns(Expression.Constant(Create<double>()));

                _formulaProcessor.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject);
            }

            [Fact]
            public void Should_Throw_When_Process_Not_Called()
            {
                _formulaProcessor = new AoiFormulaProcessor(_operatorStackFake.FakedObject, _expressionStackFake.FakedObject, _formulaExpressionFactoryFake.FakedObject,
                  _operationFactoryFake.FakedObject, _userDefinedMethodFactoryFake.FakedObject, (p, r, e, o) => _tokenProcessorFake.FakedObject);

                Invoking(() => _formulaProcessor.ProcessOperator())
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithNamedMessageWhenNull("_formulaReader");
            }

            [Fact]
            public void Should_Call_FormulaReader_ReadOperator()
            {
                _formulaProcessor.ProcessOperator();

                _formulaReaderFake
                  .CallsTo(fake => fake.ReadOperator(_operationFactoryFake.FakedObject))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Throw_When_User_Method_And_Invalid_Stack()
            {
                _formulaProcessor.LastPushIsOperator = true;

                _formulaReaderFake
                  .CallsTo(fake => fake.ReadOperator(_operationFactoryFake.FakedObject))
                  .Returns("*");    // anything other than '-' or '+'

                Invoking(() => _formulaProcessor.ProcessOperator())
                    .Should()
                    .Throw<AoiFormulaException>()
                    .WithMessage("Invalid expression stack");
            }

            [Fact]
            public void Should_Not_Throw_When_User_Method_And_Valid_Stack()
            {
                _formulaProcessor.LastPushIsOperator = true;

                _formulaReaderFake
                  .CallsTo(fake => fake.ReadOperator(_operationFactoryFake.FakedObject))
                  .Returns("-");

                Invoking(() => _formulaProcessor.ProcessOperator())
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Push_Unary_Minus()
            {
                _formulaProcessor.LastPushIsOperator = true;

                _formulaReaderFake
                  .CallsTo(fake => fake.ReadOperator(_operationFactoryFake.FakedObject))
                  .Returns("-");

                _formulaProcessor.ProcessOperator();

                _operatorStackFake
                  .CallsTo(fake => fake.Push(AoiFormulaProcessor.CustomTokens.UnaryMinus))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Not_Push_Unary_Plus()
            {
                _formulaProcessor.LastPushIsOperator = true;

                _formulaReaderFake
                  .CallsTo(fake => fake.ReadOperator(_operationFactoryFake.FakedObject))
                  .Returns("+");

                _formulaProcessor.ProcessOperator();

                _operatorStackFake
                  .CallsTo(fake => fake.Push(AoiFormulaProcessor.CustomTokens.UnaryMinus))
                  .MustHaveHappened(0, Times.Exactly);
            }

            [Fact]
            public void Should_Throw_When_Not_User_Method_And_Symbol_Not_Registered()
            {
                var symbol = Create<string>();
                _formulaProcessor.LastPushIsOperator = false;

                _operationFactoryFake
                  .CallsTo(fake => fake.IsRegistered(symbol))
                  .Returns(false);

                _formulaReaderFake
                  .CallsTo(fake => fake.ReadOperator(_operationFactoryFake.FakedObject))
                  .Returns(symbol);

                Invoking(() => _formulaProcessor.ProcessOperator())
                    .Should()
                    .Throw<AoiFormulaException>()
                    .WithMessage($"Unknown operator: {symbol}");
            }

            [Fact]
            public void Should_Call_OperationFactory_GetOperation_When_Not_User_Method()
            {
                var symbol = Create<string>();
                _formulaProcessor.LastPushIsOperator = false;

                _operationFactoryFake
                  .CallsTo(fake => fake.IsRegistered(symbol))
                  .Returns(true);

                _formulaReaderFake
                  .CallsTo(fake => fake.ReadOperator(_operationFactoryFake.FakedObject))
                  .Returns(symbol);

                _formulaProcessor.ProcessOperator();

                _operationFactoryFake
                  .CallsTo(fake => fake.GetOperation(symbol))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Call_TokenProcessor_ProcessOperators_When_Not_User_Method()
            {
                var symbol = Create<string>();
                _formulaProcessor.LastPushIsOperator = false;

                _operationFactoryFake
                  .CallsTo(fake => fake.IsRegistered(symbol))
                  .Returns(true);

                _formulaReaderFake
                  .CallsTo(fake => fake.ReadOperator(_operationFactoryFake.FakedObject))
                  .Returns(symbol);

                _formulaProcessor.ProcessOperator();

                // one call is from Process() and the other is from ProcessOperator()
                _tokenProcessorFake
                  .CallsTo(fake => fake.ProcessOperators(_operatorStackFake.FakedObject, _expressionStackFake.FakedObject, A<Func<bool>>.Ignored))
                  .MustHaveHappened(2, Times.Exactly);
            }

            [Fact]
            public void Should_Process_Operators_When_Next_Is_Not_OpenScope()
            {
                Func<bool> condition = null;
                var symbol = Create<string>();
                var peekedOp = Create<string>();

                _formulaProcessor.LastPushIsOperator = false;

                _operatorStackFake.CallsTo(fake => fake.Peek())
                  .Returns(peekedOp);

                var currentOperation = new AoiArithmeticOperation(1, 1, e => this.CreateStub<IAoiOperator>());
                var peekedOperation = new AoiArithmeticOperation(0, 1, e => this.CreateStub<IAoiOperator>());

                _operationFactoryFake
                  .CallsTo(fake => fake.GetOperation(symbol))
                  .Returns(currentOperation);

                _operationFactoryFake
                  .CallsTo(fake => fake.GetOperation(peekedOp))
                  .Returns(peekedOperation);

                _operationFactoryFake
                  .CallsTo(fake => fake.IsRegistered(symbol))
                  .Returns(true);

                _formulaReaderFake
                  .CallsTo(fake => fake.ReadOperator(_operationFactoryFake.FakedObject))
                  .Returns(symbol);

                _tokenProcessorFake
                  .CallsTo(fake => fake.ProcessOperators(_operatorStackFake.FakedObject, _expressionStackFake.FakedObject, A<Func<bool>>.Ignored))
                  .Invokes(call => condition = call.Arguments.Get<Func<bool>>(2));

                _formulaProcessor.ProcessOperator();

                var result = condition.Invoke();

                result.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Process_Operators_When_Next_Is_OpenScope()
            {
                Func<bool> condition = null;
                var symbol = Create<string>();

                _formulaProcessor.LastPushIsOperator = false;

                _operatorStackFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(AoiFormulaProcessor.CustomTokens.OpenScope);

                _operationFactoryFake
                  .CallsTo(fake => fake.IsRegistered(symbol))
                  .Returns(true);

                _formulaReaderFake
                  .CallsTo(fake => fake.ReadOperator(_operationFactoryFake.FakedObject))
                  .Returns(symbol);

                _tokenProcessorFake
                  .CallsTo(fake => fake.ProcessOperators(_operatorStackFake.FakedObject, _expressionStackFake.FakedObject, A<Func<bool>>.Ignored))
                  .Invokes(call => condition = call.Arguments.Get<Func<bool>>(2));

                _formulaProcessor.ProcessOperator();

                var result = condition.Invoke();

                result.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Process_Operators_When_Equal_Precedence()
            {
                Func<bool> condition = null;
                var symbol = Create<string>();
                var peekedOp = Create<string>();

                _formulaProcessor.LastPushIsOperator = false;

                _operatorStackFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(peekedOp);

                var currentOperation = new AoiArithmeticOperation(1, 1, e => this.CreateStub<IAoiOperator>());
                var peekedOperation = new AoiArithmeticOperation(1, 1, e => this.CreateStub<IAoiOperator>());

                _operationFactoryFake
                  .CallsTo(fake => fake.GetOperation(symbol))
                  .Returns(currentOperation);

                _operationFactoryFake
                  .CallsTo(fake => fake.GetOperation(peekedOp))
                  .Returns(peekedOperation);

                _operationFactoryFake
                  .CallsTo(fake => fake.IsRegistered(symbol))
                  .Returns(true);

                _formulaReaderFake
                  .CallsTo(fake => fake.ReadOperator(_operationFactoryFake.FakedObject))
                  .Returns(symbol);

                _tokenProcessorFake
                  .CallsTo(fake => fake.ProcessOperators(_operatorStackFake.FakedObject, _expressionStackFake.FakedObject, A<Func<bool>>.Ignored))
                  .Invokes(call => condition = call.Arguments.Get<Func<bool>>(2));

                _formulaProcessor.ProcessOperator();

                var result = condition.Invoke();

                result.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Process_Operators_When_Lower_Precedence()
            {
                Func<bool> condition = null;
                var symbol = Create<string>();
                var peekedOp = Create<string>();

                _formulaProcessor.LastPushIsOperator = false;

                _operatorStackFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(peekedOp);

                var currentOperation = new AoiArithmeticOperation(1, 1, e => this.CreateStub<IAoiOperator>());
                var peekedOperation = new AoiArithmeticOperation(2, 1, e => this.CreateStub<IAoiOperator>());

                _operationFactoryFake
                  .CallsTo(fake => fake.GetOperation(symbol))
                  .Returns(currentOperation);

                _operationFactoryFake
                  .CallsTo(fake => fake.GetOperation(peekedOp))
                  .Returns(peekedOperation);

                _operationFactoryFake
                  .CallsTo(fake => fake.IsRegistered(symbol))
                  .Returns(true);

                _formulaReaderFake
                  .CallsTo(fake => fake.ReadOperator(_operationFactoryFake.FakedObject))
                  .Returns(symbol);

                _tokenProcessorFake
                  .CallsTo(fake => fake.ProcessOperators(_operatorStackFake.FakedObject, _expressionStackFake.FakedObject, A<Func<bool>>.Ignored))
                  .Invokes(call => condition = call.Arguments.Get<Func<bool>>(2));

                _formulaProcessor.ProcessOperator();

                var result = condition.Invoke();

                result.Should().BeFalse();
            }

            [Fact]
            public void Should_Push_Operator()
            {
                var symbol = Create<string>();
                _formulaProcessor.LastPushIsOperator = false;

                _operationFactoryFake
                  .CallsTo(fake => fake.IsRegistered(symbol))
                  .Returns(true);

                _formulaReaderFake
                  .CallsTo(fake => fake.ReadOperator(_operationFactoryFake.FakedObject))
                  .Returns(symbol);

                _formulaProcessor.ProcessOperator();

                _operatorStackFake
                  .CallsTo(fake => fake.Push(symbol))
                  .MustHaveHappened(1, Times.Exactly);
            }
        }

        public class ProcessNumerical : AoiFormulaProcessorFixture
        {
            public ProcessNumerical()
            {
                _expressionStackFake
                  .CallsTo(fake => fake.Pop())
                  .Returns(Expression.Constant(Create<double>()));

                _formulaProcessor.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject);
            }

            [Fact]
            public void Should_Throw_When_Process_Not_Called()
            {
                _formulaProcessor = new AoiFormulaProcessor(_operatorStackFake.FakedObject, _expressionStackFake.FakedObject, _formulaExpressionFactoryFake.FakedObject,
                  _operationFactoryFake.FakedObject, _userDefinedMethodFactoryFake.FakedObject, (p, r, e, o) => _tokenProcessorFake.FakedObject);

                Invoking(() => _formulaProcessor.ProcessNumerical())
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithNamedMessageWhenNull("_formulaReader");
            }

            [Fact]
            public void Should_Call_FormulaReader_ReadNumerical()
            {
                _formulaProcessor.ProcessNumerical();

                _formulaReaderFake
                  .CallsTo(fake => fake.ReadNumerical())
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Push_Expression()
            {
                Expression expression = null;
                var value = Create<double>();

                _formulaReaderFake
                  .CallsTo(fake => fake.ReadNumerical())
                  .Returns(value);

                _expressionStackFake
                  .CallsTo(fake => fake.Push(A<Expression>.Ignored))
                  .Invokes(call => expression = call.Arguments.Get<Expression>(0));

                _formulaProcessor.ProcessNumerical();

                var expected = expression.ToString();

                expected.Should().Be(value.ToString());
            }
        }

        public class ProcessNamedOperand : AoiFormulaProcessorFixture
        {
            private readonly string _namedOperand;

            public ProcessNamedOperand()
            {
                _namedOperand = Create<string>();

                _expressionStackFake
                  .CallsTo(fake => fake.Pop())
                  .Returns(Expression.Constant(Create<double>()));
            }

            [Fact]
            public void Should_Throw_When_Process_Not_Called()
            {
                _formulaProcessor = new AoiFormulaProcessor(_operatorStackFake.FakedObject, _expressionStackFake.FakedObject, _formulaExpressionFactoryFake.FakedObject,
                  _operationFactoryFake.FakedObject, _userDefinedMethodFactoryFake.FakedObject, (p, r, e, o) => _tokenProcessorFake.FakedObject);

                Invoking(() => _formulaProcessor.ProcessNamedOperand())
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithNamedMessageWhenNull("_formulaReader");
            }

            [Fact]
            public void Should_Call_FormulaReader_ReadNamedOperand()
            {
                _formulaProcessor.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject);
                _formulaProcessor.ProcessNamedOperand();

                _formulaReaderFake
                  .CallsTo(fake => fake.ReadNamedOperand(_operationFactoryFake.FakedObject))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Call_UserDefinedMethodFactory_IsRegistered_When_Peek_OpenScope()
            {
                ConfigureProcess();

                _operatorStackFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(AoiFormulaProcessor.CustomTokens.UserMethod);

                _formulaProcessor.ProcessNamedOperand();

                _userDefinedMethodFactoryFake
                  .CallsTo(fake => fake.IsRegistered(_namedOperand))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Call_ReadNext_When_Peek_OpenScope_And_User_Method_Registered()
            {
                ConfigureProcess();

                _operatorStackFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(AoiFormulaProcessor.CustomTokens.UserMethod);

                _formulaProcessor.ProcessNamedOperand();

                _formulaReaderFake
                  .CallsTo(fake => fake.ReadNext())
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Throw_When_Peek_OpenScope_And_Not_User_Method_Registered()
            {
                ConfigureProcess();

                _userDefinedMethodFactoryFake
                  .CallsTo(fake => fake.IsRegistered(_namedOperand))
                  .Returns(false);

                Invoking(() => _formulaProcessor.ProcessNamedOperand())
                    .Should()
                    .Throw<AoiFormulaException>()
                    .WithMessage($"Unknown method: {_namedOperand}");
            }

            [Fact]
            public void Should_Return_Expression_When_Not_Peek_OpenScope()
            {
                ConfigureProcess();

                _formulaExpressionFactoryFake
                  .CallsTo(fake => fake.CreateExpression(_namedOperand, _variableRegistryFake.FakedObject))
                  .Returns(_expression);

                _formulaReaderFake
                  .CallsTo(fake => fake.PeekNext())
                  .Returns(-1);

                _formulaProcessor.ProcessNamedOperand();

                _expressionStackFake
                  .CallsTo(fake => fake.Push(_expression))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void When_ParseMethodToExpression_Should_Call_PushOperator_UserMethod()
            {
                ConfigureProcess();

                _operatorStackFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(AoiFormulaProcessor.CustomTokens.UserMethod);

                _formulaProcessor.ProcessNamedOperand();

                _operatorStackFake
                  .CallsTo(fake => fake.Push(AoiFormulaProcessor.CustomTokens.UserMethod))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void When_ParseMethodToExpression_Should_Call_PushOperator_OpenScope()
            {
                ConfigureProcess();

                _operatorStackFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(AoiFormulaProcessor.CustomTokens.UserMethod);

                _formulaProcessor.ProcessNamedOperand();

                _operatorStackFake
                  .CallsTo(fake => fake.Push(AoiFormulaProcessor.CustomTokens.OpenScope))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void When_ParseMethodToExpression_Should_Parse_IsUserMethod()
            {
                ConfigureProcess();

                var next = Create<char>();

                _operatorStackFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(AoiFormulaProcessor.CustomTokens.UserMethod);

                _formulaReaderFake
                  .CallsTo(fake => fake.PeekNext())
                  .ReturnsNextFromSequence('(', next, -1);

                _tokenProcessorFake
                  .CallsTo(fake => fake.ProcessToken(next, true))
                  .Returns(false);

                _formulaProcessor.ProcessNamedOperand();

                _tokenProcessorFake
                  .CallsTo(fake => fake.ProcessToken(next, true))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void When_ParseMethodToExpression_Should_Throw_When_Not_Peek_UserMethod()
            {
                ConfigureProcess();

                _operatorStackFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(string.Empty);

                Invoking(() => _formulaProcessor.ProcessNamedOperand())
                    .Should()
                    .Throw<AoiFormulaException>()
                    .WithMessage($"Invalid expression near method: {_namedOperand}");
            }

            [Fact]
            public void When_ParseMethodToExpression_Should_Call_OperatorStack_Pop()
            {
                ConfigureProcess();

                _operatorStackFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(AoiFormulaProcessor.CustomTokens.UserMethod);

                _formulaProcessor.ProcessNamedOperand();

                _operatorStackFake
                  .CallsTo(fake => fake.Pop())
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void When_ParseMethodToExpression_Should_Call_UserDefinedMethodFactory_GetMethod()
            {
                ConfigureProcess();

                _operatorStackFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(AoiFormulaProcessor.CustomTokens.UserMethod);

                _formulaProcessor.ProcessNamedOperand();

                _userDefinedMethodFactoryFake
                  .CallsTo(fake => fake.GetMethod(_namedOperand))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void When_ParseMethodToExpression_Should_Throw_When_Invalid_Expression_Count()
            {
                ConfigureProcess();

                var expressionsRequired = Create<int>();
                var operation = new AoiArithmeticOperationDummy(expressionsRequired);

                _expressionStackFake
                  .CallsTo(fake => fake.Count)
                  .ReturnsNextFromSequence(expressionsRequired, 0);  // ensure expression count comparison fails

                _userDefinedMethodFactoryFake
                  .CallsTo(fake => fake.GetMethod(_namedOperand))
                  .Returns(operation);

                _operatorStackFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(AoiFormulaProcessor.CustomTokens.UserMethod);

                Invoking(() => _formulaProcessor.ProcessNamedOperand())
                    .Should()
                    .Throw<AoiFormulaException>()
                    .WithMessage($"Expected {operation.ArgumentCount} parameters");
            }

            [Fact]
            public void When_ParseMethodToExpression_Should_Return_Created_Expression()
            {
                ConfigureProcess();

                var currentExpressionCount = Create<int>();
                var expressionsRequired = Create<int>();
                var operation = new AoiArithmeticOperationDummy(expressionsRequired);

                _expressionStackFake
                  .CallsTo(fake => fake.Count)
                  .ReturnsNextFromSequence(currentExpressionCount, expressionsRequired + currentExpressionCount);

                _userDefinedMethodFactoryFake
                  .CallsTo(fake => fake.GetMethod(_namedOperand)).Returns(operation);

                _operatorStackFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(AoiFormulaProcessor.CustomTokens.UserMethod);

                _formulaExpressionFactoryFake
                  .CallsTo(fake => fake.CreateExpression(operation, _expressionStackFake.FakedObject))
                  .Returns(_expression);

                _formulaProcessor.ProcessNamedOperand();

                _expressionStackFake
                  .CallsTo(fake => fake.Push(_expression))
                  .MustHaveHappened(1, Times.Exactly);
            }

            private void ConfigureProcess()
            {
                _formulaReaderFake
                  .CallsTo(fake => fake.PeekNext())
                  .Returns('(');

                _formulaReaderFake
                  .CallsTo(fake => fake.ReadNamedOperand(_operationFactoryFake.FakedObject))
                  .Returns(_namedOperand);

                _userDefinedMethodFactoryFake
                  .CallsTo(fake => fake.IsRegistered(_namedOperand))
                  .Returns(true);

                _formulaProcessor.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject);
            }
        }
    }
}
