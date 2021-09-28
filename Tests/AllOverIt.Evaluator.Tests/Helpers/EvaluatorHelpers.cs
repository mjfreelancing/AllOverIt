using AllOverIt.Evaluator.Variables;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Helpers
{
    public sealed class EvaluatorHelpers
    {
        public static FormulaProcessorResult CreateFormulaProcessorResult(double value, IReadOnlyCollection<string> referencedVariableNames, IVariableRegistry variableRegistry)
        {
            Expression<Func<double>> expression = () => value;

            return new FormulaProcessorResult(expression, referencedVariableNames, variableRegistry);
        }

        public static FormulaCompilerResult CreateFormulaCompilerResult(IVariableRegistry variableRegistry, double value, IReadOnlyCollection<string> referencedVariableNames)
        {
            return new FormulaCompilerResult(variableRegistry, () => value, referencedVariableNames);
        }
    }
}