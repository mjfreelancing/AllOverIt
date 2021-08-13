﻿using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Extensions;
using System;
using System.Text.RegularExpressions;

namespace AllOverIt.Evaluator
{
    public sealed class FormulaCompiler : IFormulaCompiler
    {
        // todo: consider making this available via a property (save user from having to create it) - need to consider cross over with the alternative ctor
        // the default registry is used when compiled formulas are not dependent on variables
        //private readonly Lazy<IVariableRegistry> _defaultRegistry = new(() => new VariableRegistry());

        private readonly FormulaProcessor _formulaProcessor;


        public FormulaCompiler(IArithmeticOperationFactory arithmeticFactory = null, IUserDefinedMethodFactory userMethodFactory = null)
        {
            _formulaProcessor = new FormulaProcessor(
                arithmeticFactory ?? new ArithmeticOperationFactory(),
                userMethodFactory ?? new UserDefinedMethodFactory());
        }

        // A new VariableRegistry will be created if variableRegistry is null, and it will be returned as part of the FormulaCompilerResult.
        // The variable registry does not have to be populated at the time of compilation.
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
            var processorResult = Parse(formula, variableRegistry);
            var compiledExpression = processorResult.FormulaExpression.Compile();

            return new FormulaCompilerResult(variableRegistry, compiledExpression, processorResult.ReferencedVariableNames);
        }

        private FormulaProcessorResult Parse(string formula, IVariableRegistry variableRegistry)
        {
            //using (var formulaReader = new FormulaReader(formula))
            //{
                return _formulaProcessor.Process(/*formulaReader,*/ formula, variableRegistry);
            //}
        }
    }
}