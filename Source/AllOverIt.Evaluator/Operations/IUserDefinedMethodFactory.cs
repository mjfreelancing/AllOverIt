using System.Collections.Generic;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>Represents a user defined method factory.</summary>
    /// <remarks>It is recommended to use a single instance of the factory.</remarks>
    public interface IUserDefinedMethodFactory
    {
        /// <summary>Gets all registered user-defined method names.</summary>
        IEnumerable<string> RegisteredMethods { get; }

        /// <summary>Registers a user defined method with the factory using an associated name and operation type.</summary>
        /// <typeparam name="TOperationType">The operation type (inheriting ArithmeticOperationBase) that implements the user-defined method.</typeparam>
        /// <param name="methodName">The name of the user-defined method.</param>
        /// <remarks>The operation type must not maintain state (and therefore be thread safe) as the factory will only ever create a single instance.</remarks>
        void RegisterMethod<TOperationType>(string methodName) where TOperationType : ArithmeticOperationBase, new();

        /// <summary>Indicates if the provided user-defined method name is registered with the factory.</summary>
        /// <param name="methodName">The name of the user-defined method to check.</param>
        /// <returns>True if the user-defined method name is registered.</returns>
        bool IsRegistered(string methodName);

        /// <summary>Gets an instance of the class implementing the user-defined method identified by the provided method name.</summary>
        /// <param name="methodName">The name of the user-defined method identifying the operation type to return.</param>
        /// <returns>The concrete type implementing the user-defined method.</returns>
        ArithmeticOperationBase GetMethod(string methodName);
    }
}