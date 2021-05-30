using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using FluentAssertions;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public abstract class AoiBinaryOperationFixtureBase<TOperationType> : AoiOperationFixtureBase<TOperationType>
        where TOperationType : AoiArithmeticOperationBase, new()
    {
        [Fact]
        public void Should_Create_Expected_Operator()
        {
            var operands = (from index in Enumerable.Range(1, 2)
                            select Expression.Constant(Create<double>())).ToArray();

            var creator = Operation.Creator;

            var operation = creator.Invoke(operands);
            operation.Should().BeOfType(OperatorType);

            var symbol = operation as AoiBinaryOperator;

            symbol!.LeftOperand.Should().BeSameAs(operands[0]);
            symbol.RightOperand.Should().BeSameAs(operands[1]);
        }

        [Fact]
        public void Should_Assign_Base_Members()
        {
            AssertOperationArgumentCount(2);
        }
    }
}
