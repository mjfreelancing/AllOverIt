using AllOverIt.Evaluator.Variables;
using AllOverIt.Helpers;
using System;

namespace AllOverIt.Evaluator
{
    public sealed class FormulaCompiler : IFormulaCompiler
    {
        // the default registry is used when compiled formulas are not dependent on variables
        private readonly Lazy<IVariableRegistry> _defaultRegistry = new(() => new VariableRegistry());

        private IFormulaParser FormulaParser { get; }

        public FormulaCompiler(IFormulaParser formulaParser = null)
        {
            formulaParser ??= Evaluator.FormulaParser.CreateDefault();

            FormulaParser = formulaParser;
        }

        public FormulaCompilerResult Compile(string formula, IVariableRegistry variableRegistry = null)
        {
            _ = formula.WhenNotNullOrEmpty(nameof(formula));

            var processorResult = FormulaParser.Parse(formula, variableRegistry ?? _defaultRegistry.Value);
            var compiledExpression = processorResult.FormulaExpression.Compile();

            return new FormulaCompilerResult(compiledExpression, processorResult.ReferencedVariableNames);
        }
    }
}