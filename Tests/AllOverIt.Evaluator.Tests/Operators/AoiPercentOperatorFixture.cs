using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class AoiPercentOperatorFixture : AoiFixtureBase
    {
        private readonly double _leftValue;
        private readonly double _rightValue;
        private readonly Expression _leftOperand;
        private readonly Expression _rightOperand;
        private AoiPercentOperator _operator;

        public AoiPercentOperatorFixture()
        {
            _leftValue = Create<double>();
            _rightValue = Create<double>();
            _leftOperand = Expression.Constant(_leftValue);
            _rightOperand = Expression.Constant(_rightValue);
            _operator = new AoiPercentOperator(_leftOperand, _rightOperand);
        }

        public class Constructor : AoiPercentOperatorFixture
        {
            [Fact]
            public void Should_Throw_When_Left_Operand_Null()
            {
                Invoking(() => _operator = new AoiPercentOperator(null, this.CreateStub<Expression>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("leftOperand");
            }

            [Fact]
            public void Should_Throw_When_Right_Operand_Null()
            {
                Invoking(() => _operator = new AoiPercentOperator(this.CreateStub<Expression>(), null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("rightOperand");
            }

            [Fact]
            public void Should_Set_Members()
            {
                _operator.Should().BeEquivalentTo(new
                {
                    LeftOperand = _leftOperand,
                    RightOperand = _rightOperand,
                    OperatorType = default(Func<Expression, Expression>)
                },
                  opt => opt.Excluding(o => o.OperatorType));
            }
        }

        public class GetExpression : AoiPercentOperatorFixture
        {
            [Fact]
            public void Should_Generate_Percent_Expression()
            {
                var expected = $"(({_leftValue} * 100) / {_rightValue})";
                var expression = _operator.GetExpression();

                var actual = expression.ToString();

                actual.Should().Be(expected);
            }
        }
    }
}
