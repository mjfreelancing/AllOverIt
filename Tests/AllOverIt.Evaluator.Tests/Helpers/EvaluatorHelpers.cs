//using System;
//using System.Collections.Generic;
//using System.Linq.Expressions;

//namespace AllOverIt.Evaluator.Tests.Helpers
//{
//    public sealed class EvaluatorHelpers
//    {
//        public static FormulaProcessorResult CreateFormulaProcessorResult(double value, IEnumerable<string> referencedVariableNames)
//        {
//            Expression<Func<double>> expression = () => value;

//            return new FormulaProcessorResult(expression, referencedVariableNames);
//        }

//        public static FormulaCompilerResult CreateFormulaCompilerResult(double value, IEnumerable<string> referencedVariableNames)
//        {

//            return new(() => value, referencedVariableNames);
//        }
//    }
//}