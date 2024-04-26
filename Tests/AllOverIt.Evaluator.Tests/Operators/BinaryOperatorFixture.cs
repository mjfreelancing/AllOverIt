using AllOverIt.Evaluator.Tests.Operators.Dummies;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operators
{
    public class BinaryOperatorFixture : FixtureBase
    {
        private readonly Expression _leftOperand;
        private readonly Expression _rightOperand;
        private readonly Expression _resultExpression;
        private readonly Func<Expression, Expression, Expression> _operatorType;
        private DummyBinaryOperator _operator;

        public BinaryOperatorFixture()
        {
            _leftOperand = this.CreateStub<Expression>();
            _rightOperand = this.CreateStub<Expression>();
            _resultExpression = this.CreateStub<Expression>();

            _operatorType = (e1, e2) => _resultExpression;

            _operator = new DummyBinaryOperator(_operatorType, _leftOperand, _rightOperand);
        }

        public class Constructor : BinaryOperatorFixture
        {
            [Fact]
            public void Should_Throw_When_OperatorType_Is_Null()
            {
                Invoking(() => _operator = new DummyBinaryOperator(null, _leftOperand, _rightOperand))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operatorType");
            }

            [Fact]
            public void Should_Throw_When_Left_Operand_Is_Null()
            {
                Invoking(() => _operator = new DummyBinaryOperator(_operatorType, null, _rightOperand))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("leftOperand");
            }

            [Fact]
            public void Should_Throw_When_Right_Operand_Is_Null()
            {
                Invoking(() => _operator = new DummyBinaryOperator(_operatorType, _leftOperand, null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("rightOperand");
            }

            [Fact]
            public void Should_Set_Members()
            {
                var expected = new
                {
                    _leftOperand = _leftOperand,
                    _rightOperand = _rightOperand,
                    OperatorType = _operatorType
                };

                _operator.Should().BeEquivalentTo(expected);
            }
        }

        public class GetExpression : BinaryOperatorFixture
        {
            [Fact]
            public void Should_Return_Expected_expression()
            {
                var actual = _operator.GetExpression();

                actual.Should().BeSameAs(_resultExpression);
            }
        }
    }
}
