using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;
using Shouldly;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public abstract class BinaryOperationFixtureBase<TOperationType> : OperationFixtureBase<TOperationType>
        where TOperationType : ArithmeticOperationBase, new()
    {
        [Fact]
        public void Should_Create_Expected_Operator()
        {
            var operands = new[]
            {
                Expression.Constant(Create<double>()),
                Expression.Constant(Create<double>())
            };

            var creator = Operation._creator;

            var operation = creator.Invoke(operands);
            operation.ShouldBeOfType(OperatorType);

            var symbol = operation as BinaryOperator;

            symbol!._leftOperand.ShouldBeSameAs(operands[0]);
            symbol._rightOperand.ShouldBeSameAs(operands[1]);
        }

        [Fact]
        public void Should_Assign_Base_Members()
        {
            AssertOperationArgumentCount(2);
        }
    }
}
