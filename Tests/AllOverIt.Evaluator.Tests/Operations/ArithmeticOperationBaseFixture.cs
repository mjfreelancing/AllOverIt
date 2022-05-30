using AllOverIt.Evaluator.Operators;
using AllOverIt.Evaluator.Tests.Operations.Dummies;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class ArithmeticOperationBaseFixture : FixtureBase
    {
        private ArithmeticOperationDummy _operation;

        public class Constructor : ArithmeticOperationBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Creator_Null()
            {
                Invoking(
                        () => _operation = new ArithmeticOperationDummy(Create<int>(), null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("creator");
            }

            [Fact]
            public void Should_Assign_Members()
            {
                var argumentCount = Create<int>();

                IOperator Creator(Expression[] e) => this.CreateStub<IOperator>();

                _operation = new ArithmeticOperationDummy(argumentCount, Creator);

                var expected = new
                {
                    ArgumentCount = argumentCount,
                    Creator = (Func<Expression[], IOperator>) Creator
                };

                expected.Should().BeEquivalentTo(_operation);
            }
        }

        public class GetExpression : ArithmeticOperationBaseFixture
        {
            [Fact]
            public void Should_Get_Expression()
            {
                var expressionIn = this.CreateStub<Expression>();
                var expressionOut = this.CreateStub<Expression>();
                var operatorFake = this.CreateStub<IOperator>();

                A.CallTo(() => operatorFake.GetExpression()).Returns(expressionOut);

                IList<Expression> expressionsIn = null;

                IOperator creator(Expression[] e)
                {
                    expressionsIn = new List<Expression>(e);
                    return operatorFake;
                }

                _operation = new ArithmeticOperationDummy(Create<int>(), creator);

                var result = _operation.GetExpression(new[] { expressionIn });
                object[] expected = { expressionIn };

                expressionsIn.Should().BeEquivalentTo(expected);
                result.Should().BeEquivalentTo(expressionOut);
            }
        }
    }
}
