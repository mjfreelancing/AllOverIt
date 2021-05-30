using AllOverIt.Evaluator.Stack;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator
{
    // An interface representing a formula token processor that performs an operation based on the next token to be read from a formula.
    public interface IAoiFormulaTokenProcessor
    {
        // Processors a stack of operators and expressions while a condition evaluates true.
        void ProcessOperators(IAoiStack<string> operators, IAoiStack<Expression> expressions, Func<bool> condition);

        // Performs some processing based on a given token.
        bool ProcessToken(char token, bool isUserMethod);
    }
}