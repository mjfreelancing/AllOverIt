using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaProcessorFixture : FixtureBase
    {
        private readonly Fake<IArithmeticOperationFactory> _operationFactoryFake;
        private readonly Fake<IUserDefinedMethodFactory> _userDefinedMethodFactoryFake;
        private readonly Fake<IVariableRegistry> _variableRegistryFake;
        private FormulaProcessor _formulaProcessor;

        public FormulaProcessorFixture()
        {
            _operationFactoryFake = new Fake<IArithmeticOperationFactory>();
            _userDefinedMethodFactoryFake = new Fake<IUserDefinedMethodFactory>();
            _variableRegistryFake = new Fake<IVariableRegistry>();

            _formulaProcessor = new FormulaProcessor(_operationFactoryFake.FakedObject, _userDefinedMethodFactoryFake.FakedObject);
        }

        public class StaticMembers : FormulaProcessorFixture
        {
            [Fact]
            public void Should_Define_Custom_Token_UserMethod()
            {
                FormulaProcessor.CustomTokens.UserMethod.Should().Be("$1");
            }

            [Fact]
            public void Should_Define_Custom_Token_UnaryMinus()
            {
                FormulaProcessor.CustomTokens.UnaryMinus.Should().Be("$2");
            }

            [Fact]
            public void Should_Define_Custom_Token_OpenScope()
            {
                FormulaProcessor.CustomTokens.OpenScope.Should().Be("(");
            }
        }

        public class Constructor : FormulaProcessorFixture
        {
            [Fact]
            public void Should_Throw_When_Operation_Factory_Null()
            {
                Invoking(() => _formulaProcessor = new FormulaProcessor(null, this.CreateStub<IUserDefinedMethodFactory>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operationFactory");
            }

            [Fact]
            public void Should_Throw_When_User_Method_Factory_Null()
            {
                Invoking(() => _formulaProcessor = new FormulaProcessor(this.CreateStub<IArithmeticOperationFactory>(), null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("userDefinedMethodFactory");
            }

            [Fact]
            public void Should_Register_Unary_Minus()
            {
                _operationFactoryFake
                  .CallsTo(fake => fake.TryRegisterOperation(FormulaProcessor.CustomTokens.UnaryMinus, 4, 1, A<Func<Expression[], IOperator>>.Ignored))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Register_Unary_Minus_Operation()
            {
                var expression = this.CreateStub<Expression>();
                Func<Expression[], IOperator> operation = null;

                _operationFactoryFake
                  .CallsTo(fake => fake.TryRegisterOperation(FormulaProcessor.CustomTokens.UnaryMinus, 4, 1, A<Func<Expression[], IOperator>>.Ignored))
                  .Invokes(call => operation = call.Arguments.Get<Func<Expression[], IOperator>>(3));

                _formulaProcessor = new FormulaProcessor(_operationFactoryFake.FakedObject, _userDefinedMethodFactoryFake.FakedObject);

                var negateOperator = operation.Invoke(new[] { expression }) as NegateOperator;

                negateOperator.Should().NotBeNull();

                var expected = new
                {
                    _operand = expression,
                    OperatorType = default(Func<Expression, Expression>)
                };

                negateOperator.Should().BeEquivalentTo(expected, option => option.Excluding(subject => subject.OperatorType));
            }
        }

        public class Process : FormulaProcessorFixture
        {
            IArithmeticOperationFactory _operationFactory = new ArithmeticOperationFactory();

            public Process()
            {
                _formulaProcessor = new FormulaProcessor(_operationFactory, new UserDefinedMethodFactory());
            }

            [Fact]
            public void Should_Throw_When_Formula_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue => _formulaProcessor.Process(stringValue, _variableRegistryFake.FakedObject),
                    "formula");
            }

            [Fact]
            public void Should_Not_Throw_When_VariableRegistry_Null()
            {
                Invoking(() => _formulaProcessor.Process("1+1", null))
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Have_No_Referenced_Variables()
            {
                var result = _formulaProcessor.Process("1+1", null);

                result.ReferencedVariableNames
                    .Should()
                    .BeEmpty();
            }

            [Fact]
            public void Should_Have_One_Referenced_Variable()
            {
                var result = _formulaProcessor.Process("1+x", null);

                result.ReferencedVariableNames
                    .Should()
                    .BeEquivalentTo("x");
            }

            [Fact]
            public void Should_Not_Duplicate_Referenced_Variable()
            {
                var result = _formulaProcessor.Process("x+x", null);

                result.ReferencedVariableNames
                    .Should()
                    .BeEquivalentTo("x");
            }

            [Fact]
            public void Should_Have_Two_Referenced_Variables()
            {
                var result = _formulaProcessor.Process("a+b", null);

                result.ReferencedVariableNames
                    .Should()
                    .BeEquivalentTo("a", "b");
            }

            [Fact]
            public void Should_Find_Referenced_Variables_In_Method()
            {
                var result = _formulaProcessor.Process("a+round(b,c)/d", null);

                result.ReferencedVariableNames
                    .Should()
                    .BeEquivalentTo("a", "b", "c", "d");
            }

            [Fact]
            public void Should_Throw_When_Method_Is_Missing_Argument()
            {
                Invoking(() =>
                    {
                        _formulaProcessor.Process("round(b)", null);
                    })
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage("Invalid expression. See index 8, near 'round(b)'.")
                    .WithInnerException<FormulaException>()
                    .WithMessage("The ROUND method expects 2 parameter(s).");
            }

            [Fact]
            public void Should_Throw_When_Unary_Plus_Method_Is_Missing_Argument()
            {
                Invoking(() =>
                    {
                        _formulaProcessor.Process("+round(b)", null);
                    })
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage("Invalid expression. See index 9, near '+round(b)'.")
                    .WithInnerException<FormulaException>()
                    .WithMessage("The ROUND method expects 2 parameter(s).");
            }

            [Fact]
            public void Should_Throw_When_Unary_Minus_Method_Is_Missing_Argument()
            {
                Invoking(() =>
                    {
                        _formulaProcessor.Process("-round(b)", null);
                    })
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage("Invalid expression. See index 9, near '-round(b)'.")
                    .WithInnerException<FormulaException>()
                    .WithMessage("The ROUND method expects 2 parameter(s).");
            }





            private sealed class CustomMath
            {
                public static double CustomMin(double value1, double value2)
                {
                    // doesn't take epsilon into account - but this is just for test purposes
                    return value1 < value2
                      ? value1
                      : value2;
                }
            }

            private sealed class CustomMinOperator : BinaryOperator
            {
                public CustomMinOperator(Expression value1, Expression value2)
                    : base(CreateExpression, value1, value2)
                {
                }

                private static Expression CreateExpression(Expression value1, Expression value2)
                {
                    var method = typeof(CustomMath).GetMethod("CustomMin", new[] { typeof(double), typeof(double) });
                    return Expression.Call(method!, value1, value2);
                }
            }

            private sealed class CustomMinOperation : ArithmeticOperationBase
            {
                public CustomMinOperation()
                    : base(2, MakeOperator)
                {
                }

                public static IOperator MakeOperator(Expression[] expressions)
                {
                    return OperatorBase.Create(expressions, e => new CustomMinOperator(e[0], e[1]));
                }
            }

            [Fact]
            public void Should_Process_Custom_Operator()
            {
                _operationFactory.RegisterOperation("??", 3, 2, CustomMinOperation.MakeOperator);

                var val1 = Create<double>();
                var val2 = Create<double>();

                var processorResult = _formulaProcessor.Process($"{val1} ?? {val2}", null);

                var value = processorResult.FormulaExpression.Compile().Invoke();

                var expected = Math.Min(val1, val2);

                value.Should().Be(expected);
            }

            [Fact]
            public void Should_Process_Double_Unary_Minus()
            {
                var value = Create<int>();
                var result = _formulaProcessor.Process($"--{value}", null);

                result.FormulaExpression
                    .Compile()
                    .Invoke()
                    .Should()
                    .Be(value);
            }

            [Fact]
            public void Should_Process_Double_Unary_Plus()
            {
                var value = Create<int>();
                var result = _formulaProcessor.Process($"++{value}", null);

                result.FormulaExpression
                    .Compile()
                    .Invoke()
                    .Should()
                    .Be(value);
            }

            [Fact]
            public void Should_Process_Plus_Minus_Unary()
            {
                var value = Create<int>();
                var result = _formulaProcessor.Process($"+-{value}", null);

                result.FormulaExpression
                    .Compile()
                    .Invoke()
                    .Should()
                    .Be(-value);
            }

            [Fact]
            public void Should_Process_Minus_Plus_Unary()
            {
                var value = Create<int>();
                var result = _formulaProcessor.Process($"+-{value}", null);

                result.FormulaExpression
                    .Compile()
                    .Invoke()
                    .Should()
                    .Be(-value);
            }

            [Fact]
            public void Should_Throw_Invalid_Expression_When_Adjacent_Tokens()
            {
                Invoking(() =>
                    {
                        var value = Create<int>();
                        _ = _formulaProcessor.Process($"+/{value}", null);
                    })
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage("Invalid expression. See index 2, near '+/'.")
                    .WithInnerException<FormulaException>()
                    .WithMessage("Invalid expression stack.");
            }

            [Fact]
            public void Should_Throw_Invalid_Expression_When_Adjacent_Methods()
            {
                Invoking(() =>
                    {
                        _ = _formulaProcessor.Process("round(1,1)round(1,1)", null);
                    })
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage("Invalid expression. See index 15, near 'round(1,1)round'.")
                    .WithInnerException<FormulaException>()
                    .WithMessage("'round' is a variable or method that does not follow an operator, or is an unregistered operator.");
            }

            [Fact]
            public void Should_Throw_Invalid_Expression_When_Variable_Does_Not_Follow_Operator()
            {
                Invoking(() =>
                    {
                        _ = _formulaProcessor.Process("1+2b", null);
                    })
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage("Invalid expression. See index 4, near '1+2b'.")
                    .WithInnerException<FormulaException>()
                    .WithMessage("'b' is a variable or method that does not follow an operator, or is an unregistered operator.");
            }

            [Fact]
            public void Should_Throw_Invalid_Expression_When_Invalid_Expression_Near_Method()
            {
                Invoking(() =>
                    {
                        _ = _formulaProcessor.Process("sqrt((", null);
                    })
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage("Invalid expression. See index 6, near 'sqrt(('.")
                    .WithInnerException<FormulaException>()
                    .WithMessage("Invalid expression near method: sqrt.");
            }

            [Fact]
            public void Should_Throw_Invalid_Expression_When_Method_Not_Closed()
            {
                Invoking(() =>
                    {
                        _ = _formulaProcessor.Process("sqrt(9", null);
                    })
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage("Invalid expression. See index 6, near 'sqrt(9'.")
                    .WithInnerException<FormulaException>()
                    .WithMessage("Invalid expression near method: sqrt.");
            }

            [Fact]
            public void Should_Process_Expression_When_Method_Argument()
            {
                Invoking(() =>
                    {
                        _ = _formulaProcessor.Process("sqrt(7+2)", null);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_Unknown_Method()
            {
                Invoking(() =>
                    {
                        _ = _formulaProcessor.Process("XYZ()", null);
                    })
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage("Invalid expression. See index 3, near 'XYZ'.")
                    .WithInnerException<FormulaException>()
                    .WithMessage("Unknown method: XYZ.");
            }

            [Fact]
            public void Should_Throw_When_Missing_Operand()
            {
                Invoking(() =>
                    {
                        _ = _formulaProcessor.Process("2+", null);
                    })
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage("Invalid expression. See index 2, near '2+'.")
                    .WithInnerException<FormulaException>()
                    .WithMessage("Insufficient expressions in the stack. 1 available, 2 required.");
            }

            [Fact]
            public void Should_Throw_When_Unregistered_Symbol()
            {
                Invoking(() =>
                    {
                        _ = _formulaProcessor.Process("2_3", null);
                    })
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage("Invalid expression. See index 3, near '2_3'.")
                    .WithInnerException<FormulaException>()
                    .WithMessage("'_3' is a variable or method that does not follow an operator, or is an unregistered operator.");
            }
        }
    }
}
