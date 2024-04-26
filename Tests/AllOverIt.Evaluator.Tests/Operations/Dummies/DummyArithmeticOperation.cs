using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Operations.Dummies
{
    internal class DummyArithmeticOperation : ArithmeticOperationBase
    {
        public DummyArithmeticOperation()
            : this(1, e => null)
        {
        }

        public DummyArithmeticOperation(int argumentCount)
            : this(argumentCount, e => null)
        {
        }

        public DummyArithmeticOperation(int argumentCount, Func<Expression[], IOperator> creator)
            : base(argumentCount, creator)
        {
        }
    }
}
