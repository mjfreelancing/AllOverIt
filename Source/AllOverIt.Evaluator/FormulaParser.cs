using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Helpers;
using System.Text.RegularExpressions;

namespace AllOverIt.Evaluator
{
    internal sealed class FormulaParser
    {
        private readonly FormulaProcessor _formulaProcessor;

        public FormulaParser(IArithmeticOperationFactory arithmeticFactory = null, IUserDefinedMethodFactory userMethodFactory = null)
        {
            _formulaProcessor = new FormulaProcessor(
                arithmeticFactory ?? new ArithmeticOperationFactory(),
                userMethodFactory ?? new UserDefinedMethodFactory());
        }

        public FormulaProcessorResult Parse(string formula, IVariableRegistry variableRegistry)
        {
            _ = formula.WhenNotNullOrEmpty(nameof(formula));
            _ = variableRegistry.WhenNotNull(nameof(variableRegistry));

            // remove any extraneous whitespace
            formula = Regex.Replace(formula, @"\s+", "");

            using (var formulaReader = new FormulaReader(formula))
            {
                return _formulaProcessor.Process(formulaReader, variableRegistry);
            }
        }
    }
}