using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Helpers;
using System;
using System.Text.RegularExpressions;

namespace AllOverIt.Evaluator
{
    public sealed class FormulaParser : IFormulaParser
    {
        private readonly IFormulaProcessor _formulaProcessor;
        private readonly Func<string, IFormulaReader> _formulaReaderCreator;
        private IFormulaReader _formulaReader;

        internal static IFormulaParser CreateDefault()
        {
            return Create(
              new ArithmeticOperationFactory(),
              new UserDefinedMethodFactory());
        }

        // This factory method can be used to build a custom parser based on extended versions of the arithmetic operation and user-defined
        // method factories. A 'null' formula processor will result in a AoiFormulaProcessor being used.
        public static IFormulaParser Create(IArithmeticOperationFactory arithmeticFactory, IUserDefinedMethodFactory userMethodFactory,
          Func<IArithmeticOperationFactory, IUserDefinedMethodFactory, IFormulaProcessor> processorCreator = null)
        {
            _ = arithmeticFactory.WhenNotNull(nameof(arithmeticFactory));
            _ = userMethodFactory.WhenNotNull(nameof(userMethodFactory));

            processorCreator ??= ((arithmetic, userMethod) => new FormulaProcessor(arithmetic, userMethod));

            var processor = processorCreator.Invoke(arithmeticFactory, userMethodFactory);

            return new FormulaParser(processor);
        }

        public FormulaParser(IFormulaProcessor formulaProcessor)
            : this(formulaProcessor, formula => new FormulaReader(formula))
        {
        }

        internal FormulaParser(IFormulaProcessor formulaProcessor, Func<string, IFormulaReader> formulaReaderCreator)
        {
            _formulaProcessor = formulaProcessor.WhenNotNull(nameof(formulaProcessor));
            _formulaReaderCreator = formulaReaderCreator.WhenNotNull(nameof(formulaReaderCreator));
        }

        public FormulaProcessorResult Parse(string formula, IVariableRegistry variableRegistry)
        {
            _ = formula.WhenNotNullOrEmpty(nameof(formula));
            _ = variableRegistry.WhenNotNull(nameof(variableRegistry));

            // remove any extraneous whitespace
            formula = Regex.Replace(formula, @"\s+", "");

            using (_formulaReader = _formulaReaderCreator.Invoke(formula))
            {
                return _formulaProcessor.Process(_formulaReader, variableRegistry);
            }
        }
    }
}