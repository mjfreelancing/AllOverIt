using AllOverIt.Evaluator.Operators;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // Represents an arithmetic operator or a user defined method.
    public sealed class AoiArithmeticOperation : AoiArithmeticOperationBase
    {
        public int Precedence { get; }

        public AoiArithmeticOperation(int precedence, int argumentCount, Func<Expression[], IAoiOperator> creator)
          : base(argumentCount, creator)
        {
            Precedence = precedence;
        }
    }
}