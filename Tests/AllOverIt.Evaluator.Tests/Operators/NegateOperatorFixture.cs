using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class NegateOperatorFixture : FixtureBase
    {
        private readonly double _value;
        private readonly Expression _operand;
        private NegateOperator _operator;

        public NegateOperatorFixture()
        {
            _value = Create<double>();
            _operand = Expression.Constant(_value);
            _operator = new NegateOperator(_operand);
        }

        public class Constructor : NegateOperatorFixture
        {
            [Fact]
            public void Should_Throw_When_Operand_Null()
            {
                Invoking(() => _operator = new NegateOperator(null))
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

        public class GetExpression : NegateOperatorFixture
        {
            [Fact]
            public void Should_Generate_Expression()
            {
                var expected = $"-{_value}";
                var expression = _operator.GetExpression();

                var actual = expression.ToString();

                actual.Should().Be(expected);
            }
        }
    }
}
