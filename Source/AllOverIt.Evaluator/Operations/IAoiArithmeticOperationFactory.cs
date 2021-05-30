using AllOverIt.Evaluator.Operators;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // Represents an arithmetic operation factory.
    public interface IAoiArithmeticOperationFactory
    {
        // Indicates if the provided symbol represents any character of the registered operations.
        bool IsCandidate(char symbol);

        // Indicates if the specified operation (operator symbol) is registered with the factory.
        bool IsRegistered(string symbol);

        // Registers a new operation in terms of its operator symbol, precedence level and a factory used for creating the required operation.
        void RegisterOperation(string symbol, int precedence, int argumentCount, Func<Expression[], IAoiOperator> operatorCreator);

        // Gets the operation object associated with a specified operator symbol.
        AoiArithmeticOperation GetOperation(string symbol);
    }
}