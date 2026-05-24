using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using System.Linq.Expressions;
using Shouldly;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class LessThanOrEqualOperatorFixture : FixtureBase
    {
        private readonly double _leftValue;
        private readonly double _rightValue;
        private readonly Expression _leftOperand;
        private readonly Expression _rightOperand;
        private LessThanOrEqualOperator _operator;

        public LessThanOrEqualOperatorFixture()
        {
            _leftValue = Create<double>();
            _rightValue = Create<double>();
            _leftOperand = Expression.Constant(_leftValue);
            _rightOperand = Expression.Constant(_rightValue);
            _operator = new LessThanOrEqualOperator(_leftOperand, _rightOperand);
        }

        public class Constructor : LessThanOrEqualOperatorFixture
        {
            [Fact]
            public void Should_Throw_When_Left_Operand_Null()
            {
                Should.Throw<ArgumentNullException>(() => _operator = new LessThanOrEqualOperator(null, this.CreateStub<Expression>()))
                    .WithNamedMessageWhenNull("leftOperand");
            }

            [Fact]
            public void Should_Throw_When_Right_Operand_Null()
            {
                Should.Throw<ArgumentNullException>(() => _operator = new LessThanOrEqualOperator(this.CreateStub<Expression>(), null))
                    .WithNamedMessageWhenNull("rightOperand");
            }

            [Fact]
            public void Should_Set_Members()
            {
                _operator._leftOperand.ShouldBeSameAs(_leftOperand);
                _operator._rightOperand.ShouldBeSameAs(_rightOperand);
            }
        }

        public class GetExpression : LessThanOrEqualOperatorFixture
        {
            [Fact]
            public void Should_Generate_Expression()
            {
                var expected = $"({_leftValue} <= {_rightValue})";
                var expression = _operator.GetExpression();

                var actual = expression.ToString();

                actual.ShouldBe(expected);
            }
        }
    }
}
