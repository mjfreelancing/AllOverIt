using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using System.Linq.Expressions;
using Shouldly;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class NotEqualOperatorFixture : FixtureBase
    {
        private readonly double _leftValue;
        private readonly double _rightValue;
        private readonly Expression _leftOperand;
        private readonly Expression _rightOperand;
        private NotEqualOperator _operator;

        public NotEqualOperatorFixture()
        {
            _leftValue = Create<double>();
            _rightValue = Create<double>();
            _leftOperand = Expression.Constant(_leftValue);
            _rightOperand = Expression.Constant(_rightValue);
            _operator = new NotEqualOperator(_leftOperand, _rightOperand);
        }

        public class Constructor : NotEqualOperatorFixture
        {
            [Fact]
            public void Should_Throw_When_Left_Operand_Null()
            {
                Should.Throw<ArgumentNullException>(() => _operator = new NotEqualOperator(null, this.CreateStub<Expression>()))
                    .WithNamedMessageWhenNull("leftOperand");
            }

            [Fact]
            public void Should_Throw_When_Right_Operand_Null()
            {
                Should.Throw<ArgumentNullException>(() => _operator = new NotEqualOperator(this.CreateStub<Expression>(), null))
                    .WithNamedMessageWhenNull("rightOperand");
            }

            [Fact]
            public void Should_Set_Members()
            {
                _operator._leftOperand.ShouldBeSameAs(_leftOperand);
                _operator._rightOperand.ShouldBeSameAs(_rightOperand);
            }
        }

        public class GetExpression : NotEqualOperatorFixture
        {
            [Fact]
            public void Should_Generate_Expression()
            {
                var expected = $"({_leftValue} != {_rightValue})";
                var expression = _operator.GetExpression();

                var actual = expression.ToString();

                actual.ShouldBe(expected);
            }
        }
    }
}
