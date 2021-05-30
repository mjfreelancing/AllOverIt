using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Operators;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Operations
{
    // Implements a factory used for registering and creating instances of an AoiArithmeticOperation that implements an associated operator.
    // Refer to RegisterDefaultOperations() for the registered built-in operations.
    // 
    // This factory assumes a lower precedence value indicates a higher priority (refer to http://en.wikipedia.org/wiki/Order_of_operations).
    public sealed class AoiArithmeticOperationFactory : IAoiArithmeticOperationFactory
    {
        internal IDictionary<string, Lazy<AoiArithmeticOperation>> Operations { get; }

        /// <summary>Initializes a new <c>AoiArithmeticOperationFactory</c> instance.</summary>
        public AoiArithmeticOperationFactory()
          : this(new Dictionary<string, Lazy<AoiArithmeticOperation>>())
        {
            RegisterDefaultOperations();
        }

        internal AoiArithmeticOperationFactory(IDictionary<string, Lazy<AoiArithmeticOperation>> operations)
        {
            Operations = operations.WhenNotNull(nameof(operations));
        }

        public bool IsCandidate(char symbol)
        {
            return Operations.Keys.Any(k => k.Contains(symbol));
        }

        public bool IsRegistered(string symbol)
        {
            return Operations.ContainsKey(symbol);
        }

        public void RegisterOperation(string symbol, int precedence, int argumentCount, Func<Expression[], IAoiOperator> operatorCreator)
        {
            if (Operations.ContainsKey(symbol))
            {
                throw new AoiOperationFactoryException($"Operation already registered for the '{symbol}' operator");
            }

            Operations[symbol] = new Lazy<AoiArithmeticOperation>(() => new AoiArithmeticOperation(precedence, argumentCount, operatorCreator));
        }

        // Creates the operation instances on demand. Only one instance per type is ever created.
        public AoiArithmeticOperation GetOperation(string symbol)
        {
            if (Operations.TryGetValue(symbol, out var result))
            {
                return result.Value;
            }

            throw new AoiOperationFactoryException($"Operation not found for the '{symbol}' operator");
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

        private static IAoiOperator MakeAddOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiAddOperator(e[0], e[1]));
        }

        private static IAoiOperator MakeSubtractOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiSubtractOperator(e[0], e[1]));
        }

        private static IAoiOperator MakeMultiplyOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiMultiplyOperator(e[0], e[1]));
        }

        private static IAoiOperator MakeDivideOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiDivideOperator(e[0], e[1]));
        }

        private static IAoiOperator MakeModuloOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiModuloOperator(e[0], e[1]));
        }

        private static IAoiOperator MakePowerOperator(Expression[] expressions)
        {
            return AoiOperatorBase.Create(expressions, e => new AoiPowerOperator(e[0], e[1]));
        }
    }
}
