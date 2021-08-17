using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class LogOperatorFixture : FixtureBase
    {
        private readonly double _value;
        private readonly Expression _operand;
        private LogOperator _operator;

        public LogOperatorFixture()
        {
            _value = Create<double>();
            _operand = Expression.Constant(_value);
            _operator = new LogOperator(_operand);
        }

        public class Constructor : LogOperatorFixture
        {
            [Fact]
            public void Should_Throw_When_Operand_Null()
            {
                Invoking(() => _operator = new LogOperator(null))
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

        public class GetExpression : LogOperatorFixture
        {
            [Fact]
            public void Should_Generate_Log_Expression()
            {
                var expected = $"Log10({_value})";
                var expression = _operator.GetExpression();

                var actual = expression.ToString();

                actual.Should().Be(expected);
            }
        }
    }
}
