using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests.Helpers
{
    public sealed class EvaluatorHelpers
    {
        public static AoiFormulaProcessorResult CreateFormulaProcessorResult(double value, IEnumerable<string> referencedVariableNames)
        {
            Expression<Func<double>> expression = () => value;

            return new AoiFormulaProcessorResult(expression, referencedVariableNames);
        }

        public static AoiFormulaCompilerResult CreateFormulaCompilerResult(double value, IEnumerable<string> referencedVariableNames)
        {

            return new AoiFormulaCompilerResult(() => value, referencedVariableNames);
        }
    }
}