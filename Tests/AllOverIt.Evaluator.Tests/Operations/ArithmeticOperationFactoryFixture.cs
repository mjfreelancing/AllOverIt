using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class ArithmeticOperationFactoryFixture : FixtureBase
    {
        private readonly ArithmeticOperationFactory _operationFactory;

        protected ArithmeticOperationFactoryFixture()
        {
            _operationFactory = new ArithmeticOperationFactory();
        }

        public class Constructor : ArithmeticOperationFactoryFixture
        {
            [Fact]
            public void Should_Register_Default_Operations()
            {
                var expected = new[] { "+", "-", "*", "/", "%", "^" };
                _operationFactory.RegisteredOperations.Should().BeEquivalentTo(expected);
            }
        }

        public class RegisterDefaultOperations : ArithmeticOperationFactoryFixture
        {

            [Fact]
            public void Should_Register_AddOperator()
            {
                AssertRegisteredOperation<AddOperator>("+", 5, 2);
            }

            [Fact]
            public void Should_Register_SubtractOperator()
            {
                AssertRegisteredOperation<SubtractOperator>("-", 5, 2);
            }

            [Fact]
            public void Should_Register_MultiplyOperator()
            {
                AssertRegisteredOperation<MultiplyOperator>("*", 3, 2);
            }

            [Fact]
            public void Should_Register_DivideOperator()
            {
                AssertRegisteredOperation<DivideOperator>("/", 3, 2);
            }

            [Fact]
            public void Should_Register_ModuloOperator()
            {
                AssertRegisteredOperation<ModuloOperator>("%", 3, 2);
            }

            [Fact]
            public void Should_Register_PowerOperator()
            {
                AssertRegisteredOperation<PowerOperator>("^", 2, 2);
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

        public class TryRegisterOperation : ArithmeticOperationFactoryFixture
        {
            [Fact]
            public void Should_Not_Register_Duplicate_Operation()
            {
                IOperator Creator(Expression[] e) => this.CreateStub<IOperator>();

                _operationFactory.RegisterOperation("xyz", Create<int>(), Create<int>(), Creator);

                var actual = _operationFactory.TryRegisterOperation("xyz", Create<int>(), Create<int>(), Creator);

                actual.Should().BeFalse();
            }
        }

        public class RegisterOperation : ArithmeticOperationFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Register_Duplicate_Operation()
            {
                IOperator Creator(Expression[] e) => this.CreateStub<IOperator>();

                _operationFactory.RegisterOperation("xyz", Create<int>(), Create<int>(), Creator);

                Invoking(
                        () => _operationFactory.RegisterOperation("xyz", Create<int>(), Create<int>(), Creator))
                    .Should()
                    .Throw<OperationFactoryException>()
                    .WithMessage("Operation already registered for the 'xyz' operator");
            }

            [Fact]
            public void Should_Register_Operation()
            {
                var symbol = Create<string>();
                var precedence = Create<int>();
                var arguments = Create<int>();

                IOperator Creator(Expression[] e) => this.CreateStub<IOperator>();

                _operationFactory.RegisterOperation(symbol, precedence, arguments, Creator);

                var actual = _operationFactory.GetOperation(symbol);

                actual.Should().BeEquivalentTo(new
                {
                    ArgumentCount = arguments,
                    Precedence = precedence,
                    Creator = (Func<Expression[], IOperator>) Creator
                });
            }
        }

        public class GetOperation : ArithmeticOperationFactoryFixture
        {
            [Fact]
            public void Should_Return_Registered_Operation()
            {
                var operation = _operationFactory.GetOperation("+");

                var expressions = from index in Enumerable.Range(1, 2)
                                  select Expression.Constant(Create<double>());

                var creator = operation.Creator.Invoke(expressions.Cast<Expression>().ToArray());

                creator.Should().BeOfType<AddOperator>();
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
                    .Throw<OperationFactoryException>()
                    .WithMessage("Operation not found for the 'xyz' operator");
            }
        }
    }
}
