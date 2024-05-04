using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class MinOperatorFixture : FixtureBase
    {
        private readonly double _leftValue;
        private readonly double _rightValue;
        private readonly Expression _leftOperand;
        private readonly Expression _rightOperand;
        private MinOperator _operator;

        public MinOperatorFixture()
        {
            _leftValue = Create<double>();
            _rightValue = Create<double>();
            _leftOperand = Expression.Constant(_leftValue);
            _rightOperand = Expression.Constant(_rightValue);
            _operator = new MinOperator(_leftOperand, _rightOperand);
        }

        public class Constructor : MinOperatorFixture
        {
            [Fact]
            public void Should_Throw_When_Left_Operand_Null()
            {
                Invoking(() => _operator = new MinOperator(null, this.CreateStub<Expression>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("leftOperand");
            }

            [Fact]
            public void Should_Throw_When_Right_Operand_Null()
            {
                Invoking(() => _operator = new MinOperator(this.CreateStub<Expression>(), null))
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

        public class GetExpression : MinOperatorFixture
        {
            [Fact]
            public void Should_Generate_Expression()
            {
                var expected = $"Min({_leftValue}, {_rightValue})";
                var expression = _operator.GetExpression();

                var actual = expression.ToString();

                actual.Should().Be(expected);
            }
        }
    }
}