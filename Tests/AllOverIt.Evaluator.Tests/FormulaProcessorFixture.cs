using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using Shouldly;
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
                FormulaProcessor.CustomTokens.UserMethod.ShouldBe("$1");
            }

            [Fact]
            public void Should_Define_Custom_Token_UnaryMinus()
            {
                FormulaProcessor.CustomTokens.UnaryMinus.ShouldBe("$2");
            }

            [Fact]
            public void Should_Define_Custom_Token_OpenScope()
            {
                FormulaProcessor.CustomTokens.OpenScope.ShouldBe("(");
            }
        }

        public class Constructor : FormulaProcessorFixture
        {
            [Fact]
            public void Should_Throw_When_Operation_Factory_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() => _formulaProcessor = new FormulaProcessor(null, this.CreateStub<IUserDefinedMethodFactory>()));

                exception.WithNamedMessageWhenNull("operationFactory");
            }

            [Fact]
            public void Should_Throw_When_User_Method_Factory_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() => _formulaProcessor = new FormulaProcessor(this.CreateStub<IArithmeticOperationFactory>(), null));

                exception.WithNamedMessageWhenNull("userDefinedMethodFactory");
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

                negateOperator.ShouldNotBeNull();
                negateOperator._operand.ShouldBeSameAs(expression);
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
                Should.NotThrow(() => _formulaProcessor.Process("1+1", null));
            }

            [Fact]
            public void Should_Have_No_Referenced_Variables()
            {
                var result = _formulaProcessor.Process("1+1", null);

                result.ReferencedVariableNames.ShouldBeEmpty();
            }

            [Fact]
            public void Should_Have_One_Referenced_Variable()
            {
                var result = _formulaProcessor.Process("1+x", null);

                ObjectGraphTestExtensions.ShouldBeEquivalentTo(result.ReferencedVariableNames, new[] { "x" });
            }

            [Fact]
            public void Should_Not_Duplicate_Referenced_Variable()
            {
                var result = _formulaProcessor.Process("x+x", null);

                ObjectGraphTestExtensions.ShouldBeEquivalentTo(result.ReferencedVariableNames, new[] { "x" });
            }

            [Fact]
            public void Should_Have_Two_Referenced_Variables()
            {
                var result = _formulaProcessor.Process("a+b", null);

                ObjectGraphTestExtensions.ShouldBeEquivalentTo(result.ReferencedVariableNames, new[] { "a", "b" });
            }

            [Fact]
            public void Should_Find_Referenced_Variables_In_Method()
            {
                var result = _formulaProcessor.Process("a+round(b,c)/d", null);

                ObjectGraphTestExtensions.ShouldBeEquivalentTo(result.ReferencedVariableNames, new[] { "a", "b", "c", "d" });
            }

            [Fact]
            public void Should_Throw_When_Method_Is_Missing_Argument()
            {
                var exception = Should.Throw<FormulaException>(() =>
                    {
                        _formulaProcessor.Process("round(b)", null);
                    });

                exception.Message.ShouldBe("Invalid expression. See index 8, near 'round(b)'.");

                var inner = exception.InnerException.ShouldBeOfType<FormulaException>();

                inner.Message.ShouldBe("The ROUND method expects 2 parameter(s).");
            }

            [Fact]
            public void Should_Throw_When_Unary_Plus_Method_Is_Missing_Argument()
            {
                var exception = Should.Throw<FormulaException>(() =>
                    {
                        _formulaProcessor.Process("+round(b)", null);
                    });

                exception.Message.ShouldBe("Invalid expression. See index 9, near '+round(b)'.");

                var inner = exception.InnerException.ShouldBeOfType<FormulaException>();

                inner.Message.ShouldBe("The ROUND method expects 2 parameter(s).");
            }

            [Fact]
            public void Should_Throw_When_Unary_Minus_Method_Is_Missing_Argument()
            {
                var exception = Should.Throw<FormulaException>(() =>
                    {
                        _formulaProcessor.Process("-round(b)", null);
                    });

                exception.Message.ShouldBe("Invalid expression. See index 9, near '-round(b)'.");

                var inner = exception.InnerException.ShouldBeOfType<FormulaException>();

                inner.Message.ShouldBe("The ROUND method expects 2 parameter(s).");
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

                value.ShouldBe(expected);
            }

            [Fact]
            public void Should_Process_Double_Unary_Minus()
            {
                var value = Create<int>();
                var result = _formulaProcessor.Process($"--{value}", null);

                result.FormulaExpression
                    .Compile()
                    .Invoke()
                    .ShouldBe(value);
            }

            [Fact]
            public void Should_Process_Double_Unary_Plus()
            {
                var value = Create<int>();
                var result = _formulaProcessor.Process($"++{value}", null);

                result.FormulaExpression
                    .Compile()
                    .Invoke()
                    .ShouldBe(value);
            }

            [Fact]
            public void Should_Process_Plus_Minus_Unary()
            {
                var value = Create<int>();
                var result = _formulaProcessor.Process($"+-{value}", null);

                result.FormulaExpression
                    .Compile()
                    .Invoke()
                    .ShouldBe(-value);
            }

            [Fact]
            public void Should_Process_Minus_Plus_Unary()
            {
                var value = Create<int>();
                var result = _formulaProcessor.Process($"+-{value}", null);

                result.FormulaExpression
                    .Compile()
                    .Invoke()
                    .ShouldBe(-value);
            }

            [Fact]
            public void Should_Throw_Invalid_Expression_When_Adjacent_Tokens()
            {
                var exception = Should.Throw<FormulaException>(() =>
                    {
                        var value = Create<int>();
                        _ = _formulaProcessor.Process($"+/{value}", null);
                    });

                exception.Message.ShouldBe("Invalid expression. See index 2, near '+/'.");

                var inner = exception.InnerException.ShouldBeOfType<FormulaException>();

                inner.Message.ShouldBe("Invalid expression stack.");
            }

            [Fact]
            public void Should_Throw_Invalid_Expression_When_Adjacent_Methods()
            {
                var exception = Should.Throw<FormulaException>(() =>
                    {
                        _ = _formulaProcessor.Process("round(1,1)round(1,1)", null);
                    });

                exception.Message.ShouldBe("Invalid expression. See index 15, near 'round(1,1)round'.");

                var inner = exception.InnerException.ShouldBeOfType<FormulaException>();

                inner.Message.ShouldBe("'round' is a variable or method that does not follow an operator, or is an unregistered operator.");
            }

            [Fact]
            public void Should_Throw_Invalid_Expression_When_Variable_Does_Not_Follow_Operator()
            {
                var exception = Should.Throw<FormulaException>(() =>
                    {
                        _ = _formulaProcessor.Process("1+2b", null);
                    });

                exception.Message.ShouldBe("Invalid expression. See index 4, near '1+2b'.");

                var inner = exception.InnerException.ShouldBeOfType<FormulaException>();

                inner.Message.ShouldBe("'b' is a variable or method that does not follow an operator, or is an unregistered operator.");
            }

            [Fact]
            public void Should_Throw_Invalid_Expression_When_Invalid_Expression_Near_Method()
            {
                var exception = Should.Throw<FormulaException>(() =>
                    {
                        _ = _formulaProcessor.Process("sqrt((", null);
                    });

                exception.Message.ShouldBe("Invalid expression. See index 6, near 'sqrt(('.");

                var inner = exception.InnerException.ShouldBeOfType<FormulaException>();

                inner.Message.ShouldBe("Invalid expression near method: sqrt.");
            }

            [Fact]
            public void Should_Throw_Invalid_Expression_When_Method_Not_Closed()
            {
                var exception = Should.Throw<FormulaException>(() =>
                    {
                        _ = _formulaProcessor.Process("sqrt(9", null);
                    });

                exception.Message.ShouldBe("Invalid expression. See index 6, near 'sqrt(9'.");

                var inner = exception.InnerException.ShouldBeOfType<FormulaException>();

                inner.Message.ShouldBe("Invalid expression near method: sqrt.");
            }

            [Fact]
            public void Should_Process_Expression_When_Method_Argument()
            {
                Should.NotThrow(() =>
                    {
                        _ = _formulaProcessor.Process("sqrt(7+2)", null);
                    });
            }

            [Fact]
            public void Should_Throw_When_Unknown_Method()
            {
                var exception = Should.Throw<FormulaException>(() =>
                    {
                        _ = _formulaProcessor.Process("XYZ()", null);
                    });

                exception.Message.ShouldBe("Invalid expression. See index 3, near 'XYZ'.");

                var inner = exception.InnerException.ShouldBeOfType<FormulaException>();

                inner.Message.ShouldBe("Unknown method: XYZ.");
            }

            [Fact]
            public void Should_Throw_When_Missing_Operand()
            {
                var exception = Should.Throw<FormulaException>(() =>
                    {
                        _ = _formulaProcessor.Process("2+", null);
                    });

                exception.Message.ShouldBe("Invalid expression. See index 2, near '2+'.");

                var inner = exception.InnerException.ShouldBeOfType<FormulaException>();

                inner.Message.ShouldBe("Insufficient expressions in the stack. 1 available, 2 required.");
            }

            [Fact]
            public void Should_Throw_When_Unregistered_Symbol()
            {
                var exception = Should.Throw<FormulaException>(() =>
                    {
                        _ = _formulaProcessor.Process("2_3", null);
                    });

                exception.Message.ShouldBe("Invalid expression. See index 3, near '2_3'.");

                var inner = exception.InnerException.ShouldBeOfType<FormulaException>();

                inner.Message.ShouldBe("'_3' is a variable or method that does not follow an operator, or is an unregistered operator.");
            }
        }
    }
}
