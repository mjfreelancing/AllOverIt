using System;
using System.Linq.Expressions;
using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class FloorOperatorFixture : FixtureBase
    {
        private readonly double _value;
        private readonly Expression _operand;
        private FloorOperator _operator;

        public FloorOperatorFixture()
        {
            _value = Create<double>();
            _operand = Expression.Constant(_value);
            _operator = new FloorOperator(_operand);
        }

        public class Constructor : FloorOperatorFixture
        {
            [Fact]
            public void Should_Throw_When_Operand_Null()
            {
                Invoking(() => _operator = new FloorOperator(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operand");
            }

            [Fact]
            public void Should_Set_Members()
            {
                _operator.Should().BeEquivalentTo(new
                    {
                        Operand = _operand,
                        OperatorType = default(Func<Expression, Expression>)
                    },
                    opt => opt.Excluding(o => o.OperatorType));
            }
        }

        public class GetExpression : FloorOperatorFixture
        {
            [Fact]
            public void Should_Generate_Expression()
            {
                var expected = $"Floor({_value})";
                var expression = _operator.GetExpression();

                var actual = expression.ToString();

                actual.Should().Be(expected);
            }
        }
    }
}