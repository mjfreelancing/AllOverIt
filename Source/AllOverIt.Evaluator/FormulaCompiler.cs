using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Extensions;
using System;
using System.Text.RegularExpressions;

namespace AllOverIt.Evaluator
{
    public sealed class FormulaCompiler : IFormulaCompiler
    {
        private readonly FormulaProcessor _formulaProcessor;

        public FormulaCompiler(IArithmeticOperationFactory arithmeticFactory = null, IUserDefinedMethodFactory userMethodFactory = null)
        {
            _formulaProcessor = new FormulaProcessor(
                arithmeticFactory ?? new ArithmeticOperationFactory(),
                userMethodFactory ?? new UserDefinedMethodFactory());
        }

        public FormulaCompilerResult Compile(string formula, IVariableRegistry variableRegistry = null)
        {
            if (formula != null)
            {
                // remove any extraneous whitespace
                formula = Regex.Replace(formula, @"\s+", "");
            }

            if (formula.IsNullOrEmpty())
            {
                throw new FormatException("The formula is empty.");
            }

            variableRegistry ??= new VariableRegistry();
            var processorResult = _formulaProcessor.Process(formula, variableRegistry);
            var compiledExpression = processorResult.FormulaExpression.Compile();

            return new FormulaCompilerResult(variableRegistry, compiledExpression, processorResult.ReferencedVariableNames);
        }
    }
}