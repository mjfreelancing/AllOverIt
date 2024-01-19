using AllOverIt.Evaluator.Tests.Operators.Dummies;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class OperatorFixture : FixtureBase
    {
        private readonly double _value;
        private DummyOperator _operator;

        public OperatorFixture()
        {
            _value = Create<double>();

            var outOperand = Expression.Constant(_value);
            _operator = new DummyOperator(() => outOperand);
        }

        public class Constructor : OperatorFixture
        {
            [Fact]
            public void Should_Throw_When_Operand_Null()
            {
                Invoking(() => _operator = new DummyOperator(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operatorType");
            }
        }

        public class GetExpression : OperatorFixture
        {
            [Fact]
            public void Should_Generate_Expression()
            {
                var expected = _value.ToString();
                var expression = _operator.GetExpression();

                var actual = expression.ToString();

                actual.Should().Be(expected);
            }
        }
    }
}
