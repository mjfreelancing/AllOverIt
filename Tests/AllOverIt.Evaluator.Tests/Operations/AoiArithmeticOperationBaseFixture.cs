using AllOverIt.Evaluator.Operators;
using AllOverIt.Evaluator.Tests.Operations.Dummies;
using AllOverIt.Fixture;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AoiArithmeticOperationBaseFixture : AoiFixtureBase
    {
        private AoiArithmeticOperationDummy _operation;

        public class Constructor : AoiArithmeticOperationBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Creator_Null()
            {
                Invoking(
                    () => _operation = new AoiArithmeticOperationDummy(Create<int>(), null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("creator"));
            }

            [Fact]
            public void Should_Assign_Members()
            {
                var argumentCount = Create<int>();
                
                IAoiOperator Creator(Expression[] e) => this.CreateStub<IAoiOperator>();

                _operation = new AoiArithmeticOperationDummy(argumentCount, Creator);

                _operation.Should().BeEquivalentTo(new
                {
                    ArgumentCount = argumentCount,
                    Creator = (Func<Expression[], IAoiOperator>)Creator
                });
            }
        }

        public class GetExpression : AoiArithmeticOperationBaseFixture
        {
            [Fact]
            public void Should_Get_Expression()
            {
                var expressionIn = this.CreateStub<Expression>();
                var expressionOut = this.CreateStub<Expression>();
                var operatorFake = this.CreateStub<IAoiOperator>();

                A.CallTo(() => operatorFake.GetExpression()).Returns(expressionOut);

                IList<Expression> expressionsIn = null;

                Func<Expression[], IAoiOperator> creator = e =>
                {
                    expressionsIn = new List<Expression>(e);
                    return operatorFake;
                };

                _operation = new AoiArithmeticOperationDummy(Create<int>(), creator);

                var result = _operation.GetExpression(new[] { expressionIn });
                object[] expected = { expressionIn };

                expressionsIn.Should().BeEquivalentTo(expected);
                result.Should().BeEquivalentTo(expressionOut);
            }
        }
    }
}
