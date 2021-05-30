using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Stack;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator
{
    internal sealed class AoiFormulaTokenProcessor
    : IAoiFormulaTokenProcessor
    {
        private IAoiFormulaProcessor FormulaProcessor { get; }
        private IList<AoiFormulaTokenProcessorContext> TokenProcessors { get; }
        private IAoiFormulaReader FormulaReader { get; }
        private IAoiFormulaExpressionFactory ExpressionFactory { get; }
        private IAoiArithmeticOperationFactory OperationFactory { get; }

        public AoiFormulaTokenProcessor(IAoiFormulaProcessor formulaProcessor, IAoiFormulaReader formulaReader,
          IAoiFormulaExpressionFactory formulaExpressionFactory, IAoiArithmeticOperationFactory operationFactory)
          : this(new List<AoiFormulaTokenProcessorContext>(), formulaProcessor, formulaReader, formulaExpressionFactory, operationFactory)
        { }

        internal AoiFormulaTokenProcessor(IList<AoiFormulaTokenProcessorContext> tokenProcessors, IAoiFormulaProcessor formulaProcessor,
          IAoiFormulaReader formulaReader, IAoiFormulaExpressionFactory expressionFactory, IAoiArithmeticOperationFactory operationFactory)
        {
            TokenProcessors = tokenProcessors.WhenNotNull(nameof(tokenProcessors));
            FormulaProcessor = formulaProcessor.WhenNotNull(nameof(formulaProcessor));
            FormulaReader = formulaReader.WhenNotNull(nameof(formulaReader));
            ExpressionFactory = expressionFactory.WhenNotNull(nameof(expressionFactory));
            OperationFactory = operationFactory.WhenNotNull(nameof(operationFactory));

            RegisterTokenProcessors();
        }

        public void ProcessOperators(IAoiStack<string> operators, IAoiStack<Expression> expressions, Func<bool> condition)
        {
            _ = operators.WhenNotNull(nameof(operators));
            _ = expressions.WhenNotNull(nameof(expressions));
            _ = condition.WhenNotNull(nameof(condition));

            while (operators.Any() && condition.Invoke())
            {
                var nextOperator = operators.Pop();
                var operation = OperationFactory.GetOperation(nextOperator);
                var expression = ExpressionFactory.CreateExpression(operation, expressions);

                expressions.Push(expression);
            }
        }

        public bool ProcessToken(char token, bool isUserMethod)
        {
            var processor = TokenProcessors
              .SkipWhile(p => !p.Predicate.Invoke(token, isUserMethod))
              .First();     // process the first match found

            // returns true to indicate processing should continue
            return processor.Processor.Invoke(token, isUserMethod);
        }

        private void RegisterTokenProcessors()
        {
            // start of a new scope
            Register(
              (token, isUserDefined) => token == '(',
              (token, isUserDefined) =>
              {
                  FormulaReader.ReadNext();   // consume the '('
                  FormulaProcessor.ProcessScopeStart();
                  return true;
              });

            // end of a scope
            Register(
              (token, isUserDefined) => token == ')',
              (token, isUserDefined) =>
              {
                  FormulaReader.ReadNext();   // consume the ')'
                  return FormulaProcessor.ProcessScopeEnd(isUserDefined);
              });

            // arguments of a method
            Register(
              (token, isUserDefined) => isUserDefined && token == ',',
              (token, isUserDefined) =>
              {
                  FormulaReader.ReadNext();
                  FormulaProcessor.ProcessMethodArgument();
                  return true;
              });

            // an operator
            Register(
              (token, isUserDefined) => OperationFactory.IsCandidate(token),
              (token, isUserDefined) =>
              {
                  FormulaProcessor.ProcessOperator();
                  return true;
              });

            // numerical constant
            Register(
              (token, isUserDefined) => AoiFormulaReader.IsNumericalCandidate(token),
              (token, isUserDefined) =>
              {
                  FormulaProcessor.ProcessNumerical();
                  return true;
              });

            // everything else - variables and methods
            Register(
              (token, isUserDefined) => true,
              (token, isUserDefined) =>
              {
                  FormulaProcessor.ProcessNamedOperand();
                  return true;
              });
        }

        // The predicate is used to determine if the associated processor will be invoked. The input arguments of the predicate include the
        // next token to be read and a flag to indicate if the token is within the context of a user defined method. The processor 
        // is invoked if the predicate returns true.
        // The input arguments of the processor include the next token to be read and a flag to indicate if the token is within the context
        // of a user defined method. The processor returns true to indicate processing is to continue or false to indicate processing of the
        // current scope is complete (such as reading arguments of a user defined method).
        private void Register(Func<char, bool, bool> predicate, Func<char, bool, bool> processor)
        {
            TokenProcessors.Add(new AoiFormulaTokenProcessorContext(predicate, processor));
        }
    }
}