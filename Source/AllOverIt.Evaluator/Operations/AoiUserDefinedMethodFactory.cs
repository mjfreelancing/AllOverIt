using System;
using System.Collections.Generic;

namespace AllOverIt.Evaluator.Operations
{
    public sealed class AoiUserDefinedMethodFactory : IAoiUserDefinedMethodFactory
    {
        private IDictionary<string, Lazy<AoiArithmeticOperationBase>> OperationRegistry { get; }

        public AoiUserDefinedMethodFactory()
            : this(null)
        {
        }

        internal AoiUserDefinedMethodFactory(IDictionary<string, Lazy<AoiArithmeticOperationBase>> operationRegistry)
        {
            OperationRegistry = operationRegistry ?? new Dictionary<string, Lazy<AoiArithmeticOperationBase>>();

            RegisterBuiltInMethods();
        }

        // The method name is considered case-insensitive.
        public void RegisterMethod<TOperationType>(string methodName) where TOperationType : AoiArithmeticOperationBase, new()
        {
            OperationRegistry.Add(methodName.ToUpper(), MakeLazyOperation<TOperationType>());
        }

        // The method name is considered case-insensitive.
        public bool IsRegistered(string methodName)
        {
            return OperationRegistry.ContainsKey(methodName.ToUpper());
        }

        // The method name is considered case-insensitive. The object returned is expected to be thread-safe and should therefore not store state.
        public AoiArithmeticOperationBase GetMethod(string methodName)
        {
            return OperationRegistry[methodName.ToUpper()].Value;
        }

        private void RegisterBuiltInMethods()
        {
            RegisterMethod<AoiRoundOperation>("ROUND");
            RegisterMethod<AoiSqrtOperation>("SQRT");
            RegisterMethod<AoiLogOperation>("LOG");
            RegisterMethod<AoiLnOperation>("LN");
            RegisterMethod<AoiExpOperation>("EXP");
            RegisterMethod<AoiPercentOperation>("PERC");
            RegisterMethod<AoiSinOperation>("SIN");
            RegisterMethod<AoiCosOperation>("COS");
            RegisterMethod<AoiTanOperation>("TAN");
            RegisterMethod<AoiSinhOperation>("SINH");
            RegisterMethod<AoiCoshOperation>("COSH");
            RegisterMethod<AoiTanhOperation>("TANH");
            RegisterMethod<AoiAsinOperation>("ASIN");
            RegisterMethod<AoiAcosOperation>("ACOS");
            RegisterMethod<AoiAtanOperation>("ATAN");
        }

        private static Lazy<AoiArithmeticOperationBase> MakeLazyOperation<TOperationType>() where TOperationType : AoiArithmeticOperationBase, new()
        {
            // user defined methods are only ever created once, if ever.
            return new(() => new TOperationType());
        }
    }
}