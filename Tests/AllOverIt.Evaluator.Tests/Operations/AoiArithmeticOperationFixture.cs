using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AoiArithmeticOperationFixture : AoiFixtureBase
    {
        private AoiArithmeticOperation _operation;

        public class Constructor : AoiArithmeticOperationFixture
        {
            [Fact]
            public void Should_Throw_When_Creator_Null()
            {
                Invoking(
                        () => _operation = new AoiArithmeticOperation(Create<int>(), Create<int>(), null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("creator"));
            }

            [Fact]
            public void Should_Assign_Members()
            {
                var precedence = Create<int>();
                var argumentCount = Create<int>();

                IAoiOperator Creator(Expression[] e) => this.CreateStub<IAoiOperator>();

                _operation = new AoiArithmeticOperation(precedence, argumentCount, Creator);

                _operation.Should().BeEquivalentTo(
                    new
                    {
                        Precedence = precedence,
                        ArgumentCount = argumentCount,
                        Creator = (Func<Expression[], IAoiOperator>) Creator
                    });
            }
        }
    }
}
