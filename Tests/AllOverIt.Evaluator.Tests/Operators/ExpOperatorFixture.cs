using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class ExpOperatorFixture : FixtureBase
    {
        private readonly double _value;
        private readonly Expression _operand;
        private ExpOperator _operator;

        public ExpOperatorFixture()
        {
            _value = Create<double>();
            _operand = Expression.Constant(_value);
            _operator = new ExpOperator(_operand);
        }

        public class Constructor : ExpOperatorFixture
        {
            [Fact]
            public void Should_Throw_When_Operand_Null()
            {
                Invoking(() => _operator = new ExpOperator(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operand");
            }

            [Fact]
            public void Should_Set_Members()
            {
                _operator._operand.Should().BeSameAs(_operand);
            }
        }

        public class GetExpression : ExpOperatorFixture
        {
            [Fact]
            public void Should_Generate_Expression()
            {
                var expected = $"Exp({_value})";
                var expression = _operator.GetExpression();

                var actual = expression.ToString();

                actual.Should().Be(expected);
            }
        }
    }
}
