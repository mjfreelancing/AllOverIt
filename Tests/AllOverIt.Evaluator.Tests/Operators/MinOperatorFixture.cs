using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using System.Linq.Expressions;
using Shouldly;

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
                Should.Throw<ArgumentNullException>(() => _operator = new MinOperator(null, this.CreateStub<Expression>()))
                    .WithNamedMessageWhenNull("leftOperand");
            }

            [Fact]
            public void Should_Throw_When_Right_Operand_Null()
            {
                Should.Throw<ArgumentNullException>(() => _operator = new MinOperator(this.CreateStub<Expression>(), null))
                    .WithNamedMessageWhenNull("rightOperand");
            }

            [Fact]
            public void Should_Set_Members()
            {
                _operator._leftOperand.ShouldBeSameAs(_leftOperand);
                _operator._rightOperand.ShouldBeSameAs(_rightOperand);
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

                actual.ShouldBe(expected);
            }
        }
    }
}
