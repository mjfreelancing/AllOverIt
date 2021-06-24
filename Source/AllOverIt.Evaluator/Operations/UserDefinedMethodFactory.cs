using System;
using System.Collections.Generic;

namespace AllOverIt.Evaluator.Operations
{
    public sealed class UserDefinedMethodFactory : IUserDefinedMethodFactory
    {
        private IDictionary<string, Lazy<ArithmeticOperationBase>> OperationRegistry { get; }

        public UserDefinedMethodFactory()
            : this(null)
        {
        }

        internal UserDefinedMethodFactory(IDictionary<string, Lazy<ArithmeticOperationBase>> operationRegistry)
        {
            OperationRegistry = operationRegistry ?? new Dictionary<string, Lazy<ArithmeticOperationBase>>();

            RegisterBuiltInMethods();
        }

        // The method name is considered case-insensitive.
        public void RegisterMethod<TOperationType>(string methodName) where TOperationType : ArithmeticOperationBase, new()
        {
            OperationRegistry.Add(methodName.ToUpper(), MakeLazyOperation<TOperationType>());
        }

        // The method name is considered case-insensitive.
        public bool IsRegistered(string methodName)
        {
            return OperationRegistry.ContainsKey(methodName.ToUpper());
        }

        // The method name is considered case-insensitive. The object returned is expected to be thread-safe and should therefore not store state.
        public ArithmeticOperationBase GetMethod(string methodName)
        {
            return OperationRegistry[methodName.ToUpper()].Value;
        }

        private void RegisterBuiltInMethods()
        {
            RegisterMethod<RoundOperation>("ROUND");
            RegisterMethod<SqrtOperation>("SQRT");
            RegisterMethod<LogOperation>("LOG");
            RegisterMethod<LnOperation>("LN");
            RegisterMethod<ExpOperation>("EXP");
            RegisterMethod<PercentOperation>("PERC");
            RegisterMethod<SinOperation>("SIN");
            RegisterMethod<CosOperation>("COS");
            RegisterMethod<TanOperation>("TAN");
            RegisterMethod<SinhOperation>("SINH");
            RegisterMethod<CoshOperation>("COSH");
            RegisterMethod<TanhOperation>("TANH");
            RegisterMethod<AsinOperation>("ASIN");
            RegisterMethod<AcosOperation>("ACOS");
            RegisterMethod<AtanOperation>("ATAN");
        }

        private static Lazy<ArithmeticOperationBase> MakeLazyOperation<TOperationType>() where TOperationType : ArithmeticOperationBase, new()
        {
            // user defined methods are only ever created once, if ever.
            return new(() => new TOperationType());
        }
    }
}