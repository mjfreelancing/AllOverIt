using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class AoiRoundOperatorFixture : AoiFixtureBase
    {
        private readonly double _value;
        private readonly int _decimals;
        private readonly Expression _leftOperand;
        private readonly Expression _rightOperand;
        private AoiRoundOperator _operator;

        public AoiRoundOperatorFixture()
        {
            _value = Create<double>();
            _decimals = Create<int>();
            _leftOperand = Expression.Constant(_value);
            _rightOperand = Expression.Constant(_decimals);
            _operator = new AoiRoundOperator(_leftOperand, _rightOperand);
        }

        public class Constructor : AoiRoundOperatorFixture
        {
            [Fact]
            public void Should_Throw_When_Left_Operand_Null()
            {
                Invoking(() => _operator = new AoiRoundOperator(null, this.CreateStub<Expression>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("leftOperand"));
            }

            [Fact]
            public void Should_Throw_When_Right_Operand_Null()
            {
                Invoking(() => _operator = new AoiRoundOperator(this.CreateStub<Expression>(), null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("rightOperand"));
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

        public class GetExpression : AoiRoundOperatorFixture
        {
            [Fact]
            public void Should_Generate_Round_Expression()
            {
                var expected = $"Round({_value}, Convert({_decimals}, Int32), AwayFromZero)";
                var expression = _operator.GetExpression();

                var actual = expression.ToString();

                actual.Should().Be(expected);
            }
        }
    }
}
