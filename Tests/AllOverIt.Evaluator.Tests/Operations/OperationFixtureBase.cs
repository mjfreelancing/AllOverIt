using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using AllOverIt.Fixture;
using System.Linq.Expressions;
using Shouldly;
using AllOverIt.Shouldly.Extensions;
namespace AllOverIt.Evaluator.Tests.Operations
{
    public abstract class OperationFixtureBase<TOperationType> : FixtureBase
        where TOperationType : ArithmeticOperationBase, new()
    {
        protected OperationFixtureBase()
        {
            Operation = new TOperationType();
        }

        protected abstract Type OperatorType { get; }

        protected TOperationType Operation { get; private set; }

        [Fact]
        public void Should_Inherit_from_ArithmeticOperationBase()
        {
            // this test is to work around the complexities of validating the result returned from GetExpression()
            // which would either need to be a string comparison of the result, or a decomposition of the Expression
            // which should really be a part of the operator's test.
            Operation.ShouldBeAssignableTo<ArithmeticOperationBase>();
        }

        protected void AssertOperationArgumentCount(int count)
        {
            Operation.ShouldBeEquivalentTo(
                new
                {
                    ArgumentCount = count,
                    Creator = default(Func<Expression[], IOperator>)
                },
                opts => opts.ExcludeMember("Creator"));
        }
    }
}
