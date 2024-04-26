using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class ArithmeticOperationFixture : FixtureBase
    {
        private ArithmeticOperation _operation;

        public class Constructor : ArithmeticOperationFixture
        {
            [Fact]
            public void Should_Throw_When_Creator_Null()
            {
                Invoking(
                        () => _operation = new ArithmeticOperation(Create<int>(), Create<int>(), null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("creator");
            }

            [Fact]
            public void Should_Assign_Members()
            {
                var precedence = Create<int>();
                var argumentCount = Create<int>();

                IOperator Creator(Expression[] e) => this.CreateStub<IOperator>();

                _operation = new ArithmeticOperation(precedence, argumentCount, Creator);

                var expected = new
                {
                    Precedence = precedence,
                    ArgumentCount = argumentCount,
                    Creator = (Func<Expression[], IOperator>) Creator
                };

                expected.Should().BeEquivalentTo(_operation);
            }
        }
    }
}
