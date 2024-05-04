using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Operators;
using AllOverIt.Evaluator.Tests.Operators.Dummies;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class OperatorBaseFixture : FixtureBase
    {
        public class Create_Operator : OperatorBaseFixture
        {
            private readonly Expression _expression1;
            private readonly Expression _expression2;
            private readonly DummyOperatorBase1 _operator;
            private Func<Expression[], DummyOperatorBase1> _creator;

            public Create_Operator()
            {
                _expression1 = this.CreateStub<Expression>();
                _expression2 = this.CreateStub<Expression>();
                _operator = new DummyOperatorBase1(_expression1, _expression2);
                _creator = e => _operator;
            }

            [Fact]
            public void Should_Throw_When_Expressions_Null()
            {
                Invoking(() => OperatorBase.Create(null, _creator))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expressions");
            }

            [Fact]
            public void Should_Throw_When_Expressions_Empty()
            {
                Invoking(() => OperatorBase.Create(Array.Empty<Expression>(), _creator))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("expressions");
            }

            [Fact]
            public void Should_Throw_When_Creator_Null()
            {
                Invoking(() => OperatorBase.Create(new[] { _expression1 }, (Func<Expression[], DummyOperatorBase1>) null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("creator");
            }

            [Fact]
            public void Should_Throw_When_Multiple_Constructors()
            {
                DummyOperatorBase2 Creator(Expression[] e) => Create<DummyOperatorBase2>();

                Invoking(() => OperatorBase.Create(new[] { _expression1 }, Creator))
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage("Sequence contains more than one element");
            }

            [Fact]
            public void Should_Not_Throw_When_Correct_Constructor_Argument_Count()
            {
                Invoking(() => OperatorBase.Create(new[] { _expression1, _expression2 }, _creator))
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Constructors_Has_Incorrect_Argument_Count()
            {
                Invoking(() => OperatorBase.Create(new[] { _expression1 }, _creator))
                    .Should()
                    .Throw<OperatorException>()
                    .WithMessage("Invalid number of arguments. 2 expressions expected, 1 provided.");
            }

            [Fact]
            public void Should_Invoke_Creator_Using_Expressions()
            {
                List<Expression> expressions = null;

                _creator = e =>
                {
                    expressions = new List<Expression>(e.ToArray());
                    return _operator;
                };

                OperatorBase.Create(new[] { _expression1, _expression2 }, _creator);

                var expected = new[] { _expression1, _expression2 };

                expected.Should().BeEquivalentTo(expressions);
            }

            [Fact]
            public void Should_Return_Operator()
            {
                var actual = OperatorBase.Create(new[] { _expression1, _expression2 }, _creator);

                actual.Should().BeSameAs(_operator);
            }
        }
    }
}
