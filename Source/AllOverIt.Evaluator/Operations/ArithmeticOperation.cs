using AllOverIt.Evaluator.Operators;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // Represents an arithmetic operator or a user defined method.
    public sealed class ArithmeticOperation : ArithmeticOperationBase
    {
        public int Precedence { get; }

        public ArithmeticOperation(int precedence, int argumentCount, Func<Expression[], IOperator> creator)
          : base(argumentCount, creator)
        {
            Precedence = precedence;
        }
    }
}