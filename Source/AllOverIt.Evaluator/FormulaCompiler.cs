using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Extensions;

namespace AllOverIt.Evaluator
{
    /// <summary>Compiles a mathematical expression provided as a string to a delegate that can be repeatedly invoked for re-evaluation.</summary>
    public sealed class FormulaCompiler
    {
        private readonly FormulaProcessor _formulaProcessor;

        /// <summary>Constructor.</summary>
        /// <param name="operationFactory">The arithmetic operation factory used for building expressions.</param>
        /// <param name="userMethodFactory">The user-defined method factory used for evaluating higher level operations.</param>
        public FormulaCompiler(IArithmeticOperationFactory? operationFactory = default, IUserDefinedMethodFactory? userMethodFactory = default)
        {
            // Note: FormulaCompiler must be created per-thread because the FormulaProcessor maintains state between each call to Process().
            //       The FormulaCompiler *could* be made thread safe by creating a new FormulaProcessor instance each time Process() was
            //       called but this results in a lot more memory allocations. Considering it is far more common to call Process() multiple
            //       times within a given thread, the implementation should be left as-is.
            _formulaProcessor = new FormulaProcessor(
                operationFactory ?? new ArithmeticOperationFactory(),
                userMethodFactory ?? new UserDefinedMethodFactory());
        }

        /// <summary>Compiles a formula represented as a string into a delegate that can be repeatedly invoked for re-evaluation.</summary>
        /// <param name="formula">The mathematical formula to be compiled.</param>
        /// <param name="variableRegistry">The registry of variables referenced by the formula that are required for evaluation. This
        /// can be populated with all required variables before or after compilation, but before evaluation. If a registry is not provided
        /// and the formula contains variables then an instance will be created and provided as part of the result.</param>
        /// <returns>The compiler result containing the variable registry (if originally provided or variables are found in the formula),
        /// the compiled delegate, and a list of variables referenced by the formula (if applicable).</returns>
        public FormulaCompilerResult Compile(string formula, IVariableRegistry? variableRegistry = default)
        {
            if (formula.IsNullOrEmpty())
            {
                throw new FormatException("The formula is empty.");
            }

            var processorResult = _formulaProcessor.Process(formula, variableRegistry);
            var compiledExpression = processorResult.FormulaExpression.Compile();
            var referencedVariableNames = processorResult.ReferencedVariableNames;  // will be [] if there were no variables

            // The result's variable registry will be the same reference as the passed in variableRegistry.
            // If the caller did not provide a registry but the formula contained variables then the _formulaProcessor will have created a registry.
            // It's the caller's responsibility to populate the registry (with IVariable and suitable values) before attempting to evaluate the formula.
            variableRegistry = processorResult.VariableRegistry;

            return new FormulaCompilerResult(variableRegistry, compiledExpression, referencedVariableNames);
        }
    }
}