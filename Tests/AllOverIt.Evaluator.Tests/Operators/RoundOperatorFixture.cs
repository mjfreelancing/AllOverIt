using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using System.Linq.Expressions;
using Shouldly;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class RoundOperatorFixture : FixtureBase
    {
        private readonly double _value;
        private readonly int _decimals;
        private readonly Expression _leftOperand;
        private readonly Expression _rightOperand;
        private RoundOperator _operator;

        public RoundOperatorFixture()
        {
            _value = Create<double>();
            _decimals = Create<int>();
            _leftOperand = Expression.Constant(_value);
            _rightOperand = Expression.Constant(_decimals);
            _operator = new RoundOperator(_leftOperand, _rightOperand);
        }

        public class Constructor : RoundOperatorFixture
        {
            [Fact]
            public void Should_Throw_When_Left_Operand_Null()
            {
                Should.Throw<ArgumentNullException>(() => _operator = new RoundOperator(null, this.CreateStub<Expression>()))
                    .WithNamedMessageWhenNull("leftOperand");
            }

            [Fact]
            public void Should_Throw_When_Right_Operand_Null()
            {
                Should.Throw<ArgumentNullException>(() => _operator = new RoundOperator(this.CreateStub<Expression>(), null))
                    .WithNamedMessageWhenNull("rightOperand");
            }

            [Fact]
            public void Should_Set_Members()
            {
                _operator._leftOperand.ShouldBeSameAs(_leftOperand);
                _operator._rightOperand.ShouldBeSameAs(_rightOperand);
            }
        }

        public class GetExpression : RoundOperatorFixture
        {
            [Fact]
            public void Should_Generate_Expression()
            {
                var expected = $"Round({_value}, Convert({_decimals}, Int32), AwayFromZero)";
                var expression = _operator.GetExpression();

                var actual = expression.ToString();

                actual.ShouldBe(expected);
            }
        }
    }
}
