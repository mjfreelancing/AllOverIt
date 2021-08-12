using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Helpers;
using System;

namespace AllOverIt.Evaluator
{
    public sealed class FormulaCompiler : IFormulaCompiler
    {
        // todo: consider making this available via a property (save user from having to create it) - need to consider cross over with the alternative ctor
        // the default registry is used when compiled formulas are not dependent on variables
        private readonly Lazy<IVariableRegistry> _defaultRegistry = new(() => new VariableRegistry());

        private readonly FormulaParser _formulaParser;

        public FormulaCompiler(IArithmeticOperationFactory arithmeticFactory = null, IUserDefinedMethodFactory userMethodFactory = null)//FormulaParser formulaParser = null)
        {
            _formulaParser = new FormulaParser(arithmeticFactory, userMethodFactory);
        }

        public FormulaCompilerResult Compile(string formula, IVariableRegistry variableRegistry = null)
        {
            _ = formula.WhenNotNullOrEmpty(nameof(formula));

            var processorResult = _formulaParser.Parse(formula, variableRegistry ?? _defaultRegistry.Value);
            var compiledExpression = processorResult.FormulaExpression.Compile();

            return new FormulaCompilerResult(compiledExpression, processorResult.ReferencedVariableNames);
        }
    }
}