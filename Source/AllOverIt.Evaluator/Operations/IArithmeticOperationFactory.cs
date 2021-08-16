using AllOverIt.Evaluator.Operators;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>Represents an arithmetic operation factory.</summary>
    /// <remarks>It is recommended to use a single instance of the factory.</remarks>
    public interface IArithmeticOperationFactory
    {
        /// <summary>Gets all registered operations based on their associated symbol.</summary>
        IEnumerable<string> RegisteredOperations { get; }

        // todo: requires a test
        bool TryRegisterOperation(string symbol, int precedence, int argumentCount, Func<Expression[], IOperator> operatorCreator);

        // Registers a new operation in terms of its operator symbol, precedence level and a factory used for creating the required operation.
        void RegisterOperation(string symbol, int precedence, int argumentCount, Func<Expression[], IOperator> operatorCreator);

        // Gets the operation object associated with a specified operator symbol.
        ArithmeticOperation GetOperation(string symbol);
    }
}