using System;
using System.Linq.Expressions;
using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using Xunit;

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
                Invoking(() => _operator = new IfOperator(null, this.CreateStub<Expression>(), this.CreateStub<Expression>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operand1");
            }

            [Fact]
            public void Should_Throw_When_True_Operand_Null()
            {
                Invoking(() => _operator = new IfOperator(this.CreateStub<Expression>(), null, this.CreateStub<Expression>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operand2");
            }

            [Fact]
            public void Should_Throw_When_False_Operand_Null()
            {
                Invoking(() => _operator = new IfOperator(this.CreateStub<Expression>(), this.CreateStub<Expression>(), null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operand3");
            }

            [Fact]
            public void Should_Set_Members()
            {
                _operator._operand1.Should().BeSameAs(_conditionOperand);
                _operator._operand2.Should().BeSameAs(_trueOperand);
                _operator._operand3.Should().BeSameAs(_falseOperand);
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

                actual.Should().Be(expected);
            }
        }
    }
}