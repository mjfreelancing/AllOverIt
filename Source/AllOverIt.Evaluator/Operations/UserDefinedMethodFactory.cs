using AllOverIt.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Evaluator.Operations
{
    /// <summary>A factory containing user-defined methods that can be evaluated as part of a formula.</summary>
    /// <remarks>
    /// The factory includes the following pre-defined methods:
    /// <para>ROUND: Rounds a number to a specified number of decimal places.</para>
    /// <para>SQRT: Calculate the square root of a number.</para>
    /// <para>CBRT: Calculate the cube root of a number.</para>
    /// <para>LOG10: Calculate the log10 of a number.</para>
    /// <para>LOG2: Calculate the log2 of a number.</para>
    /// <para>LOG: Calculate the natural log of a number.</para>
    /// <para>EXP: Raise 'e' to a specified power.</para>
    /// <para>PERC: Calculate the percentage that one operand is of another.</para>
    /// <para>SIN: Calculate the sine of an angle (in radians).</para>
    /// <para>COS: Calculate the cosine of an angle (in radians).</para>
    /// <para>TAN: Calculate the tangent of an angle (in radians).</para>
    /// <para>SINH: Calculate the hyperbolic sine of an angle (in radians).</para>
    /// <para>COSH: Calculate the hyperbolic cosine of an angle (in radians).</para>
    /// <para>TANH: Calculate the hyperbolic tangent of an angle (in radians).</para>
    /// <para>ASIN: Calculate the angle (in radians) of a sine value.</para>
    /// <para>ACOS: Calculate the angle (in radians) of a cosine value.</para>
    /// <para>ATAN: Calculate the angle (in radians) of a tangent value.</para>
    /// <para>MIN: Returns the minimum of two values.</para>
    /// <para>MAX: Returns the maximum of two values.</para>
    /// <para>ABS: Returns the absolute value of a number.</para>
    /// <para>CEIL: Returns the smallest integral value greater than or equal to a given number.</para>
    /// <para>FLOOR: Returns the largest integral value greater than or equal to a given number.</para>
    /// </remarks>
    public sealed class UserDefinedMethodFactory : IUserDefinedMethodFactory
    {
        // shared across all instances
        private static readonly IDictionary<string, Lazy<ArithmeticOperationBase>> BuiltInMethodsRegistry = new Dictionary<string, Lazy<ArithmeticOperationBase>>();

        // unique to each instance created (unless created as a Singleton of course) - created when the first method is registered
        private IDictionary<string, Lazy<ArithmeticOperationBase>> _userMethodsRegistry;

        /// <summary>Provides a list of all built-in and custom registered methods.</summary>
        public IEnumerable<string> RegisteredMethods => BuiltInMethodsRegistry.Keys
            .Concat(_userMethodsRegistry?.Keys ?? Enumerable.Empty<string>())
            .AsReadOnlyCollection();

        static UserDefinedMethodFactory()
        {
            RegisterMethod<RoundOperation>(BuiltInMethodsRegistry, "ROUND");
            RegisterMethod<SqrtOperation>(BuiltInMethodsRegistry, "SQRT");
            RegisterMethod<CubeRootOperation>(BuiltInMethodsRegistry, "CBRT");
            RegisterMethod<Log10Operation>(BuiltInMethodsRegistry, "LOG10");
            RegisterMethod<Log2Operation>(BuiltInMethodsRegistry, "LOG2");
            RegisterMethod<LogOperation>(BuiltInMethodsRegistry, "LOG");
            RegisterMethod<ExpOperation>(BuiltInMethodsRegistry, "EXP");
            RegisterMethod<PercentOperation>(BuiltInMethodsRegistry, "PERC");
            RegisterMethod<SinOperation>(BuiltInMethodsRegistry, "SIN");
            RegisterMethod<CosOperation>(BuiltInMethodsRegistry, "COS");
            RegisterMethod<TanOperation>(BuiltInMethodsRegistry, "TAN");
            RegisterMethod<SinhOperation>(BuiltInMethodsRegistry, "SINH");
            RegisterMethod<CoshOperation>(BuiltInMethodsRegistry, "COSH");
            RegisterMethod<TanhOperation>(BuiltInMethodsRegistry, "TANH");
            RegisterMethod<AsinOperation>(BuiltInMethodsRegistry, "ASIN");
            RegisterMethod<AcosOperation>(BuiltInMethodsRegistry, "ACOS");
            RegisterMethod<AtanOperation>(BuiltInMethodsRegistry, "ATAN");
            RegisterMethod<MinOperation>(BuiltInMethodsRegistry, "MIN");
            RegisterMethod<MaxOperation>(BuiltInMethodsRegistry, "MAX");
            RegisterMethod<AbsOperation>(BuiltInMethodsRegistry, "ABS");
            RegisterMethod<CeilingOperation>(BuiltInMethodsRegistry, "CEIL");
            RegisterMethod<FloorOperation>(BuiltInMethodsRegistry, "FLOOR");
        }

        /// <summary>Registers an operation type that provides the implementation for a named method.</summary>
        /// <typeparam name="TOperationType">The type implementing the registered method.</typeparam>
        /// <param name="methodName">The case-insensitive method name being registered.</param>
        /// <remarks>The operation type is expected to be thread-safe and should therefore not store state.</remarks>
        public void RegisterMethod<TOperationType>(string methodName) where TOperationType : ArithmeticOperationBase, new()
        {
            _userMethodsRegistry ??=  new Dictionary<string, Lazy<ArithmeticOperationBase>>();

            RegisterMethod<TOperationType>(_userMethodsRegistry, methodName, true);
        }

        /// <summary>Indicates if the requested method name has been registered.</summary>
        /// <param name="methodName">The case-insensitive method name being queried.</param>
        /// <returns>True if the requested method name has been registered, otherwise false.</returns>
        public bool IsRegistered(string methodName)
        {
            var upperMethodName = methodName.ToUpperInvariant();

            return BuiltInMethodsRegistry.ContainsKey(upperMethodName) ||
                   _userMethodsRegistry != null && _userMethodsRegistry.ContainsKey(upperMethodName);
        }

        /// <summary>Gets an instance of the operation type that was registered using the provided method name.</summary>
        /// <param name="methodName">The registered method name associated with the operation type to be returned.</param>
        /// <returns>An instance of the operation type that was registered using the provided method name.</returns>
        /// <remarks>The operation type is only ever created once (per factory instance).</remarks>
        public ArithmeticOperationBase GetMethod(string methodName)
        {
            var upperMethodName = methodName.ToUpperInvariant();

            if (BuiltInMethodsRegistry.TryGetValue(upperMethodName, out var builtInOperation))
            {
                return builtInOperation.Value;
            }

            if (_userMethodsRegistry != null && _userMethodsRegistry.TryGetValue(upperMethodName, out var userDefinedOperation))
            {
                return userDefinedOperation.Value;
            }

            throw new KeyNotFoundException($"The '{methodName}' method is not registered with the {nameof(UserDefinedMethodFactory)}.");
        }

        private static void RegisterMethod<TOperationType>(IDictionary<string, Lazy<ArithmeticOperationBase>> operationRegistry, string methodName,
            bool requiresUppercase = false)
            where TOperationType : ArithmeticOperationBase, new()
        {
            // minor allocation improvement if this can be avoided
            if (requiresUppercase)
            {
                methodName = methodName.ToUpperInvariant();
            }

            operationRegistry.Add(methodName, MakeLazyOperation<TOperationType>());
        }

        private static Lazy<ArithmeticOperationBase> MakeLazyOperation<TOperationType>() where TOperationType : ArithmeticOperationBase, new()
        {
            // user defined methods are only ever created once, if ever.
            return new Lazy<ArithmeticOperationBase>(() => new TOperationType());
        }
    }
}