using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AoiArithmeticOperationFactoryFixture : AoiFixtureBase
    {
        private AoiArithmeticOperationFactory _operationFactory;

        public AoiArithmeticOperationFactoryFixture()
        {
            _operationFactory = new AoiArithmeticOperationFactory();
        }

        public class Constructor : AoiArithmeticOperationFactoryFixture
        {
            [Fact]
            public void Should_Call_RegisterDefaultOperations()
            {
                var expected = new[] { "+", "-", "*", "/", "%", "^" };
                _operationFactory.Operations.Keys.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Assign_Operations()
            {
                var operations = new Dictionary<string, Lazy<AoiArithmeticOperation>>();

                _operationFactory = new AoiArithmeticOperationFactory(operations);

                _operationFactory.Operations.Should().BeSameAs(operations);
            }
        }

        public class RegisterDefaultOperations : AoiArithmeticOperationFactoryFixture
        {

            [Fact]
            public void Should_Register_AddOperator()
            {
                AssertRegisteredOperation<AoiAddOperator>("+", 5, 2);
            }

            [Fact]
            public void Should_Register_SubtractOperator()
            {
                AssertRegisteredOperation<AoiSubtractOperator>("-", 5, 2);
            }

            [Fact]
            public void Should_Register_MultiplyOperator()
            {
                AssertRegisteredOperation<AoiMultiplyOperator>("*", 3, 2);
            }

            [Fact]
            public void Should_Register_DivideOperator()
            {
                AssertRegisteredOperation<AoiDivideOperator>("/", 3, 2);
            }

            [Fact]
            public void Should_Register_ModuloOperator()
            {
                AssertRegisteredOperation<AoiModuloOperator>("%", 3, 2);
            }

            [Fact]
            public void Should_Register_PowerOperator()
            {
                AssertRegisteredOperation<AoiPowerOperator>("^", 2, 2);
            }

            private void AssertRegisteredOperation<TOperation>(string symbol, int precedence, int argumentCount)
            {
                var operation = _operationFactory.GetOperation(symbol);

                var expressions = from index in Enumerable.Range(1, argumentCount)
                                  select Expression.Constant(Create<double>());

                var creator = operation.Creator.Invoke(expressions.Cast<Expression>().ToArray());

                operation.Precedence.Should().Be(precedence);
                creator.Should().BeOfType<TOperation>();
            }
        }

        public class IsCandidate : AoiArithmeticOperationFactoryFixture
        {
            public IsCandidate()
            {
                var operations = new Dictionary<string, Lazy<AoiArithmeticOperation>>
                {
                    ["abcdef"] = new Lazy<AoiArithmeticOperation>()
                };

                _operationFactory = new AoiArithmeticOperationFactory(operations);
            }

            [Fact]
            public void Should_Succeed_For_Registered_Operator()
            {
                var result = _operationFactory.IsCandidate('a');

                result.Should().BeTrue();
            }

            [Fact]
            public void Should_Fail_For_Registered_Operator()
            {
                var result = _operationFactory.IsCandidate('z');

                result.Should().BeFalse();
            }
        }

        public class IsRegistered : AoiArithmeticOperationFactoryFixture
        {
            [Fact]
            public void Should_Call_Operations_ContainsKey()
            {
                var operationFake = this.CreateStub<IDictionary<string, Lazy<AoiArithmeticOperation>>>();
                var operation = Create<string>();

                _operationFactory = new AoiArithmeticOperationFactory(operationFake);

                _operationFactory.IsRegistered(operation);

                A.CallTo(() => operationFake.ContainsKey(operation)).MustHaveHappened(1, Times.Exactly);
            }
        }

        public class RegisterOperation : AoiArithmeticOperationFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Register_Duplicate_Operation()
            {
                IAoiOperator Creator(Expression[] e) => this.CreateStub<IAoiOperator>();

                _operationFactory.RegisterOperation("xyz", Create<int>(), Create<int>(), Creator);

                Invoking(
                        () => _operationFactory.RegisterOperation("xyz", Create<int>(), Create<int>(), Creator))
                    .Should()
                    .Throw<AoiOperationFactoryException>()
                    .WithMessage("Operation already registered for the 'xyz' operator");
            }

            [Fact]
            public void Should_Register_Operation()
            {
                var symbol = Create<string>();
                var precedence = Create<int>();
                var arguments = Create<int>();

                IAoiOperator Creator(Expression[] e) => this.CreateStub<IAoiOperator>();

                _operationFactory.RegisterOperation(symbol, precedence, arguments, Creator);

                var actual = _operationFactory.GetOperation(symbol);

                actual.Should().BeEquivalentTo(new
                {
                    ArgumentCount = arguments,
                    Precedence = precedence,
                    Creator = (Func<Expression[], IAoiOperator>)Creator
                });
            }
        }

        public class GetOperation : AoiArithmeticOperationFactoryFixture
        {
            [Fact]
            public void Should_Return_Registered_Operation()
            {
                var operation = _operationFactory.GetOperation("+");

                var expressions = from index in Enumerable.Range(1, 2)
                                  select Expression.Constant(Create<double>());

                var creator = operation.Creator.Invoke(expressions.Cast<Expression>().ToArray());

                creator.Should().BeOfType<AoiAddOperator>();
            }

            [Fact]
            public void Should_Return_Same_Operation_Instance()
            {
                var operation1 = _operationFactory.GetOperation("+");
                var operation2 = _operationFactory.GetOperation("+");

                operation1.Should().BeSameAs(operation2);
            }

            [Fact]
            public void Should_Throw_When_Operation_Not_Registered()
            {
                Invoking(() => _operationFactory.GetOperation("xyz"))
                    .Should()
                    .Throw<AoiOperationFactoryException>()
                    .WithMessage("Operation not found for the 'xyz' operator");
            }
        }
    }
}
