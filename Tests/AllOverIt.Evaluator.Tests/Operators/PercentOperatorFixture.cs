using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class PercentOperatorFixture : FixtureBase
    {
        private readonly double _leftValue;
        private readonly double _rightValue;
        private readonly Expression _leftOperand;
        private readonly Expression _rightOperand;
        private PercentOperator _operator;

        public PercentOperatorFixture()
        {
            _leftValue = Create<double>();
            _rightValue = Create<double>();
            _leftOperand = Expression.Constant(_leftValue);
            _rightOperand = Expression.Constant(_rightValue);
            _operator = new PercentOperator(_leftOperand, _rightOperand);
        }

        public class Constructor : PercentOperatorFixture
        {
            [Fact]
            public void Should_Throw_When_Left_Operand_Null()
            {
                Invoking(() => _operator = new PercentOperator(null, this.CreateStub<Expression>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("leftOperand");
            }

            [Fact]
            public void Should_Throw_When_Right_Operand_Null()
            {
                Invoking(() => _operator = new PercentOperator(this.CreateStub<Expression>(), null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("rightOperand");
            }

            [Fact]
            public void Should_Set_Members()
            {
                _operator._leftOperand.Should().BeSameAs(_leftOperand);
                _operator._rightOperand.Should().BeSameAs(_rightOperand);
            }
        }

        public class GetExpression : PercentOperatorFixture
        {
            [Fact]
            public void Should_Generate_Expression()
            {
                var expected = $"(({_leftValue} * 100) / {_rightValue})";
                var expression = _operator.GetExpression();

                var actual = expression.ToString();

                actual.Should().Be(expected);
            }
        }
    }
}
