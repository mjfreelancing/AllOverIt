using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using System.Linq.Expressions;
using Shouldly;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class IfOperatorFixture : FixtureBase
    {
        private readonly bool _condition;
        private readonly double _trueValue;
        private readonly double _falseValue;
        private readonly Expression _conditionOperand;
        private readonly Expression _trueOperand;
        private readonly Expression _falseOperand;
        private IfOperator _operator;

        public IfOperatorFixture()
        {
            _condition = Create<bool>();
            _trueValue = Create<double>();
            _falseValue = Create<double>();
            _conditionOperand = Expression.Constant(_condition);
            _trueOperand = Expression.Constant(_trueValue);
            _falseOperand = Expression.Constant(_falseValue);
            _operator = new IfOperator(_conditionOperand, _trueOperand, _falseOperand);
        }

        public class Constructor : IfOperatorFixture
        {
            [Fact]
            public void Should_Throw_When_Condition_Null()
            {
                Should.Throw<ArgumentNullException>(() => _operator = new IfOperator(null, this.CreateStub<Expression>(), this.CreateStub<Expression>()))
                    .WithNamedMessageWhenNull("operand1");
            }

            [Fact]
            public void Should_Throw_When_True_Operand_Null()
            {
                Should.Throw<ArgumentNullException>(() => _operator = new IfOperator(this.CreateStub<Expression>(), null, this.CreateStub<Expression>()))
                    .WithNamedMessageWhenNull("operand2");
            }

            [Fact]
            public void Should_Throw_When_False_Operand_Null()
            {
                Should.Throw<ArgumentNullException>(() => _operator = new IfOperator(this.CreateStub<Expression>(), this.CreateStub<Expression>(), null))
                    .WithNamedMessageWhenNull("operand3");
            }

            [Fact]
            public void Should_Set_Members()
            {
                _operator._operand1.ShouldBeSameAs(_conditionOperand);
                _operator._operand2.ShouldBeSameAs(_trueOperand);
                _operator._operand3.ShouldBeSameAs(_falseOperand);
            }
        }

        public class GetExpression : IfOperatorFixture
        {
            [Fact]
            public void Should_Generate_Expression()
            {
                var expected = $"IIF({_condition}, {_trueValue}, {_falseValue})";
                var expression = _operator.GetExpression();

                var actual = expression.ToString();

                actual.ShouldBe(expected);
            }
        }
    }
}
