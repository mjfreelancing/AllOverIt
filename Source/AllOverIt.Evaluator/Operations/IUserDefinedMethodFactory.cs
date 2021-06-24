namespace AllOverIt.Evaluator.Operations
{
    // Represents a user defined method factory.
    public interface IUserDefinedMethodFactory
    {
        // Registers a user defined method with the factory using an associated name.
        // TOperationType is the concrete operation class type to be registered (must inherit from AoiArithmeticOperationBase and implement a default constructor.
        void RegisterMethod<TOperationType>(string methodName) where TOperationType : ArithmeticOperationBase, new();

        // Indicates if a user defined method is registered with the factory.
        bool IsRegistered(string methodName);

        // Gets an instance of the class implementing a user defined method based on its registered name.
        ArithmeticOperationBase GetMethod(string methodName);
    }
}