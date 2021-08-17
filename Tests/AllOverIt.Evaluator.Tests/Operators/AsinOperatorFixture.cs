using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class AsinOperatorFixture : FixtureBase
    {
        private readonly double _value;
        private readonly Expression _operand;
        private AsinOperator _operator;

        public AsinOperatorFixture()
        {
            _value = Create<double>();
            _operand = Expression.Constant(_value);
            _operator = new AsinOperator(_operand);
        }

        public class Constructor : AsinOperatorFixture
        {
            [Fact]
            public void Should_Throw_When_Operand_Null()
            {
                Invoking(() => _operator = new AsinOperator(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operand");
            }

            //[Fact]
            //public void Should_Set_Members()
            //{
            //    _operator.Should().BeEquivalentTo(
            //        new
            //        {
            //            Operand = _operand,
            //            OperatorType = default(Func<Expression, Expression>)
            //        },
            //        opt => opt.Excluding(o => o.OperatorType));
            //}
        }

        public class GetExpression : AsinOperatorFixture
        {
            [Fact]
            public void Should_Generate_Ln_Expression()
            {
                var expected = $"Asin({_value})";
                var expression = _operator.GetExpression();

                var actual = expression.ToString();

                actual.Should().Be(expected);
            }
        }
    }
}