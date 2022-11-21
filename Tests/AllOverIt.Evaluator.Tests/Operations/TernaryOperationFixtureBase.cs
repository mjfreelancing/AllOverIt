using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using FluentAssertions;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public abstract class TernaryOperationFixtureBase<TOperationType> : OperationFixtureBase<TOperationType>
        where TOperationType : ArithmeticOperationBase, new()
    {
        [Fact]
        public void Should_Create_Expected_Operator()
        {
            var operands = new[]
            {
                Expression.Constant(Create<bool>()),
                Expression.Constant(Create<double>()),
                Expression.Constant(Create<double>())
            };

            var creator = Operation._creator;

            var operation = creator.Invoke(operands);
            operation.Should().BeOfType(OperatorType);

            var symbol = operation as TernaryOperator;

            symbol!._operand1.Should().BeSameAs(operands[0]);
            symbol!._operand2.Should().BeSameAs(operands[1]);
            symbol._operand3.Should().BeSameAs(operands[2]);
        }

        [Fact]
        public void Should_Assign_Base_Members()
        {
            AssertOperationArgumentCount(3);
        }
    }
}