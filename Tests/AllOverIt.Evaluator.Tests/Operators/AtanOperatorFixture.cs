using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class AtanOperatorFixture : FixtureBase
    {
        private readonly double _value;
        private readonly Expression _operand;
        private AtanOperator _operator;

        public AtanOperatorFixture()
        {
            _value = Create<double>();
            _operand = Expression.Constant(_value);
            _operator = new AtanOperator(_operand);
        }

        public class Constructor : AtanOperatorFixture
        {
            [Fact]
            public void Should_Throw_When_Operand_Null()
            {
                Invoking(() => _operator = new AtanOperator(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operand");
            }

            //[Fact]
            //public void Should_Set_Members()
            //{
            //    _operator.Should().BeEquivalentTo(new
            //    {
            //        Operand = _operand,
            //        OperatorType = default(Func<Expression, Expression>)
            //    },
            //      opt => opt.Excluding(o => o.OperatorType));
            //}
        }

        public class GetExpression : AtanOperatorFixture
        {
            [Fact]
            public void Should_Generate_Ln_Expression()
            {
                var expected = $"Atan({_value})";
                var expression = _operator.GetExpression();

                var actual = expression.ToString();

                actual.Should().Be(expected);
            }
        }
    }
}