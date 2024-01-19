using AllOverIt.Evaluator.Operators;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>Represents an arithmetic operation factory for registering and creating instances of an ArithmeticOperation
    /// that implements an associated operator..</summary>
    /// <remarks>It is recommended to use a single instance of the factory.</remarks>
    public interface IArithmeticOperationFactory
    {
        /// <summary>Gets all registered operations based on their associated symbol.</summary>
        IEnumerable<string> RegisteredOperations { get; }

        /// <summary>Attempts to register a new operation in terms of its operator symbol, precedence level and a factory used for
        /// creating the required operation. If the symbol is already registered then the request is ignored.</summary>
        /// <param name="symbol">The symbol that identifies the operation being registered.</param>
        /// <param name="precedence">The precedence level that determines the order of operations. A lower value indicates a higher precedence.</param>
        /// <param name="argumentCount">The number of required arguments.</param>
        /// <param name="operatorCreator">The factory used to create the operation using the provided arguments (as expressions).</param>
        /// <returns><see langword="true" /> if the symbol was registered, otherwise <see langword="false" />.</returns>
        bool TryRegisterOperation(string symbol, int precedence, int argumentCount, Func<Expression[], IOperator> operatorCreator);

        /// <summary>Registers a new operation in terms of its operator symbol, precedence level and a factory used for creating the
        /// required operation.</summary>
        /// <param name="symbol">The symbol that identifies the operation being registered.</param>
        /// <param name="precedence">The precedence level that determines the order of operations. A lower value indicates a higher precedence.</param>
        /// <param name="argumentCount">The number of required arguments.</param>
        /// <param name="operatorCreator">The factory used to create the operation using the provided arguments (as expressions).</param>
        void RegisterOperation(string symbol, int precedence, int argumentCount, Func<Expression[], IOperator> operatorCreator);

        /// <summary>Gets the operation object associated with a specified operator symbol.</summary>
        /// <param name="symbol">The symbol that identifies the operation to be created.</param>
        /// <returns>The constructed operation.</returns>
        ArithmeticOperation GetOperation(string symbol);
    }
}