using AllOverIt.Evaluator.Tests.Operators.Dummies;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using System.Linq.Expressions;
using Shouldly;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class TernaryOperatorFixture : FixtureBase
    {
        private readonly Expression _operand1;
        private readonly Expression _operand2;
        private readonly Expression _operand3;
        private readonly Expression _resultExpression;
        private readonly Func<Expression, Expression, Expression, Expression> _operatorType;
        private DummyTernaryOperator _operator;

        public TernaryOperatorFixture()
        {
            _operand1 = this.CreateStub<Expression>();
            _operand2 = this.CreateStub<Expression>();
            _operand3 = this.CreateStub<Expression>();
            _resultExpression = this.CreateStub<Expression>();

            _operatorType = (e1, e2, e3) => _resultExpression;

            _operator = new DummyTernaryOperator(_operatorType, _operand1, _operand2, _operand3);
        }

        public class Constructor : TernaryOperatorFixture
        {
            [Fact]
            public void Should_Throw_When_OperatorType_Is_Null()
            {
                Should.Throw<ArgumentNullException>(() => _operator = new DummyTernaryOperator(null, _operand1, _operand2, _operand3))
                    .WithNamedMessageWhenNull("operatorType");
            }

            [Fact]
            public void Should_Throw_When_Operand1_Is_Null()
            {
                Should.Throw<ArgumentNullException>(() => _operator = new DummyTernaryOperator(_operatorType, null, _operand2, _operand3))
                    .WithNamedMessageWhenNull("operand1");
            }

            [Fact]
            public void Should_Throw_When_Operand2_Is_Null()
            {
                Should.Throw<ArgumentNullException>(() => _operator = new DummyTernaryOperator(_operatorType, _operand1, null, _operand3))
                    .WithNamedMessageWhenNull("operand2");
            }

            [Fact]
            public void Should_Throw_When_Operand3_Is_Null()
            {
                Should.Throw<ArgumentNullException>(() => _operator = new DummyTernaryOperator(_operatorType, _operand1, _operand2, null))
                    .WithNamedMessageWhenNull("operand3");
            }

            [Fact]
            public void Should_Set_Members()
            {
                _operator._operand1.ShouldBeSameAs(_operand1);
                _operator._operand2.ShouldBeSameAs(_operand2);
                _operator._operand3.ShouldBeSameAs(_operand3);
            }
        }

        public class GetExpression : TernaryOperatorFixture
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
