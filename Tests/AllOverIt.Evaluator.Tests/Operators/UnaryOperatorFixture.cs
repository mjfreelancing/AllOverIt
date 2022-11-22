using AllOverIt.Evaluator.Tests.Operators.Dummies;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class UnaryOperatorFixture : FixtureBase
    {
        private readonly Expression _operand;
        private readonly Expression _resultExpression;
        private readonly Func<Expression, Expression> _operatorType;
        private UnaryOperatorDummy _operator;

        public UnaryOperatorFixture()
        {
            _operand = this.CreateStub<Expression>();
            _resultExpression = this.CreateStub<Expression>();

            _operatorType = e => _resultExpression;

            _operator = new UnaryOperatorDummy(_operatorType, _operand);
        }

        public class Constructor : UnaryOperatorFixture
        {
            [Fact]
            public void Should_Throw_When_OperatorType_Is_Null()
            {
                Invoking(() => _operator = new UnaryOperatorDummy(null, _operand))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operatorType");
            }

            [Fact]
            public void Should_Throw_When_Operand_Is_Null()
            {
                Invoking(() => _operator = new UnaryOperatorDummy(_operatorType, null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operand");
            }

            [Fact]
            public void Should_Set_Members()
            {
                _operator._operand.Should().BeSameAs(_operand);
            }
        }

        public class GetExpression : UnaryOperatorFixture
        {
            [Fact]
            public void Should_Return_Expected_expression()
            {
                var actual = _operator.GetExpression();

                actual.Should().BeSameAs(_resultExpression);
            }
        }
    }
}
