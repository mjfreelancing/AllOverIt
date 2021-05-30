using AllOverIt.Evaluator.Tests.Operators.Dummies;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class AoiOperatorFixture : AoiFixtureBase
    {
        private readonly double _value;
        private OperatorDummy _operator;

        public AoiOperatorFixture()
        {
            _value = Create<double>();

            var outOperand = Expression.Constant(_value);
            _operator = new OperatorDummy(() => outOperand);
        }

        public class Constructor : AoiOperatorFixture
        {
            [Fact]
            public void Should_Throw_When_Operand_Null()
            {
                Invoking(() => _operator = new OperatorDummy(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("operatorType"));
            }
        }

        public class GetExpression : AoiOperatorFixture
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
