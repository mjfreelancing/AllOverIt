using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public abstract class AoiUnaryOperationFixtureBase<TOperationType> : AoiOperationFixtureBase<TOperationType>
        where TOperationType : AoiArithmeticOperationBase, new()
    {
        [Fact]
        public void Should_Assign_Base_Members()
        {
            AssertOperationArgumentCount(1);
        }

        [Fact]
        public void Should_Create_Expected_Operator()
        {
            var operand = this.CreateStub<Expression>();
            var creator = Operation.Creator;

            var operation = creator.Invoke(new[] { operand });
            operation.Should().BeOfType(OperatorType);

            var symbol = operation as AoiUnaryOperator;

            symbol!.Operand.Should().BeSameAs(operand);
        }
    }
}
