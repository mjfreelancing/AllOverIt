using AllOverIt.Evaluator.Variables;
using AllOverIt.Helpers;
using System;

namespace AllOverIt.Evaluator
{
    public sealed class AoiFormulaCompiler : IAoiFormulaCompiler
    {
        // the default registry is used when compiled formulas are not dependent on variables
        private readonly Lazy<IAoiVariableRegistry> _defaultRegistry = new(() => new AoiVariableRegistry());

        private IAoiFormulaParser FormulaParser { get; }

        public AoiFormulaCompiler(IAoiFormulaParser formulaParser = null)
        {
            formulaParser ??= AoiFormulaParser.CreateDefault();

            FormulaParser = formulaParser;
        }

        public AoiFormulaCompilerResult Compile(string formula, IAoiVariableRegistry variableRegistry = null)
        {
            _ = formula.WhenNotNullOrEmpty(nameof(formula));

            var processorResult = FormulaParser.Parse(formula, variableRegistry ?? _defaultRegistry.Value);
            var compiledExpression = processorResult.FormulaExpression.Compile();

            return new AoiFormulaCompilerResult(compiledExpression, processorResult.ReferencedVariableNames);
        }
    }
}