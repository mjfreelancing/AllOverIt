using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Operators;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // Implements a factory used for registering and creating instances of an ArithmeticOperation that implements an associated operator.
    // Refer to RegisterDefaultOperations() for the registered built-in operations.
    // 
    // This factory assumes a lower precedence value indicates a higher priority (refer to http://en.wikipedia.org/wiki/Order_of_operations).
    public sealed class ArithmeticOperationFactory : IArithmeticOperationFactory
    {
        private readonly IDictionary<string, Lazy<ArithmeticOperation>> _operations = new Dictionary<string, Lazy<ArithmeticOperation>>();

        public IEnumerable<string> RegisteredOperations => _operations.Keys;

        public ArithmeticOperationFactory()
        {
            RegisterDefaultOperations();
        }

        public bool TryRegisterOperation(string symbol, int precedence, int argumentCount, Func<Expression[], IOperator> operatorCreator)
        {
            return TryRegisterOperation(symbol, precedence, argumentCount, operatorCreator, false);
        }

        public void RegisterOperation(string symbol, int precedence, int argumentCount, Func<Expression[], IOperator> operatorCreator)
        {
            TryRegisterOperation(symbol, precedence, argumentCount, operatorCreator, true);
        }

        // Creates the operation instances on demand. Only one instance per type is ever created.
        public ArithmeticOperation GetOperation(string symbol)
        {
            if (_operations.TryGetValue(symbol, out var result))
            {
                return result.Value;
            }

            throw new OperationFactoryException($"Operation not found for the '{symbol}' operator");
        }

        private void RegisterDefaultOperations()
        {
            // Note: The MakeXXX() methods are used to ensure the operator contains a constructor with the required number of arguments
            // Using order of precedence levels as detailed at http://en.wikipedia.org/wiki/Order_of_operations
            RegisterOperation("+", 5, 2, MakeAddOperator);
            RegisterOperation("-", 5, 2, MakeSubtractOperator);
            RegisterOperation("*", 3, 2, MakeMultiplyOperator);
            RegisterOperation("/", 3, 2, MakeDivideOperator);
            RegisterOperation("%", 3, 2, MakeModuloOperator);
            RegisterOperation("^", 2, 2, MakePowerOperator);
        }

        private bool TryRegisterOperation(string symbol, int precedence, int argumentCount, Func<Expression[], IOperator> operatorCreator, bool throwIfRegistered)
        {
            if (_operations.ContainsKey(symbol))
            {
                if (throwIfRegistered)
                {
                    throw new OperationFactoryException($"Operation already registered for the '{symbol}' operator");
                }

                return false;
            }

            _operations[symbol] = new Lazy<ArithmeticOperation>(() => new ArithmeticOperation(precedence, argumentCount, operatorCreator));
            return true;
        }

        private static IOperator MakeAddOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new AddOperator(e[0], e[1]));
        }

        private static IOperator MakeSubtractOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new SubtractOperator(e[0], e[1]));
        }

        private static IOperator MakeMultiplyOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new MultiplyOperator(e[0], e[1]));
        }

        private static IOperator MakeDivideOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new DivideOperator(e[0], e[1]));
        }

        private static IOperator MakeModuloOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new ModuloOperator(e[0], e[1]));
        }

        private static IOperator MakePowerOperator(Expression[] expressions)
        {
            return OperatorBase.Create(expressions, e => new PowerOperator(e[0], e[1]));
        }
    }
}
