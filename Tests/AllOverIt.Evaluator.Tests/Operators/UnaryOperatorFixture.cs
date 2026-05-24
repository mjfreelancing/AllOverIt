using AllOverIt.Evaluator.Tests.Operators.Dummies;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using System.Linq.Expressions;
using Shouldly;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class UnaryOperatorFixture : FixtureBase
    {
        private readonly Expression _operand;
        private readonly Expression _resultExpression;
        private readonly Func<Expression, Expression> _operatorType;
        private DummyUnaryOperator _operator;

        public UnaryOperatorFixture()
        {
            _operand = this.CreateStub<Expression>();
            _resultExpression = this.CreateStub<Expression>();

            _operatorType = e => _resultExpression;

            _operator = new DummyUnaryOperator(_operatorType, _operand);
        }

        public class Constructor : UnaryOperatorFixture
        {
            [Fact]
            public void Should_Throw_When_OperatorType_Is_Null()
            {
                Should.Throw<ArgumentNullException>(() => _operator = new DummyUnaryOperator(null, _operand))
                    .WithNamedMessageWhenNull("operatorType");
            }

            [Fact]
            public void Should_Throw_When_Operand_Is_Null()
            {
                Should.Throw<ArgumentNullException>(() => _operator = new DummyUnaryOperator(_operatorType, null))
                    .WithNamedMessageWhenNull("operand");
            }

            [Fact]
            public void Should_Set_Members()
            {
                _operator._operand.ShouldBeSameAs(_operand);
            }
        }

        public class GetExpression : UnaryOperatorFixture
        {
            [Fact]
            public void Should_Return_Expected_expression()
            {
                var actual = _operator.GetExpression();

                actual.ShouldBeSameAs(_resultExpression);
            }
        }
    }
}
