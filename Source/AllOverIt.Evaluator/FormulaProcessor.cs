using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator
{
    public sealed class FormulaProcessor
    {
        // Tags used to internally identify custom operators within the stack. Can be anything other than the operators defined in ArithmeticOperationFactory
        private static class CustomTokens
        {
            internal const string UserMethod = "$$";
            internal const string UnaryMinus = "##";
            internal const string OpenScope = "(";
        }

        private static readonly FormulaExpressionFactory FormulaExpressionFactory = new();
        private readonly Stack<string> _operatorStack = new();
        private readonly Stack<Expression> _expressionStack = new();
        private readonly IList<FormulaTokenProcessorContext> _tokenProcessors = new List<FormulaTokenProcessorContext>();
        private readonly IArithmeticOperationFactory _operationFactory;
        private readonly IUserDefinedMethodFactory _userDefinedMethodFactory;
        private readonly IList<string> _referencedVariableNames = new List<string>();
        private IVariableRegistry _variableRegistry;
        private FormulaReader _formulaReader;

        // tracks whether the last processed token was an operator or an expression so unary plus and unary minus can be handled.
        private bool _lastPushIsOperator;

        public FormulaProcessor(IArithmeticOperationFactory operationFactory, IUserDefinedMethodFactory userDefinedMethodFactory)
        {
            _operationFactory = operationFactory.WhenNotNull(nameof(operationFactory));
            _userDefinedMethodFactory = userDefinedMethodFactory.WhenNotNull(nameof(userDefinedMethodFactory));

            // custom operator registration
            _operationFactory.RegisterOperation(CustomTokens.UnaryMinus, 4, 1, e => new NegateOperator(e[0]));
        }

        public FormulaProcessorResult Process(FormulaReader formulaReader, IVariableRegistry variableRegistry)
        {
            _formulaReader = formulaReader.WhenNotNull(nameof(formulaReader));
            _variableRegistry = variableRegistry.WhenNotNull(nameof(variableRegistry));

            _operatorStack.Clear();
            _expressionStack.Clear();
            _lastPushIsOperator = true;

            RegisterTokenProcessors();
            ParseContent(false);
            ProcessOperators(_operatorStack, _expressionStack, () => true);

            var lastExpression = _expressionStack.Pop();
            var funcExpression = Expression.Lambda<Func<double>>(lastExpression);

            return new FormulaProcessorResult(funcExpression, _referencedVariableNames);
        }

        private void PushOperator(string operatorToken)
        {
            _ = operatorToken.WhenNotNullOrEmpty(nameof(operatorToken));

            _operatorStack.Push(operatorToken);
            _lastPushIsOperator = true;
        }

        private void PushExpression(Expression expression)
        {
            expression.WhenNotNull(nameof(expression));

            _expressionStack.Push(expression);
            _lastPushIsOperator = false;
        }

        private void ProcessScopeStart()
        {
            PushOperator(CustomTokens.OpenScope);
        }

        private bool ProcessScopeEnd(bool isUserMethod)
        {
            ProcessOperators(_operatorStack, _expressionStack, () => _operatorStack.Peek() != CustomTokens.OpenScope);

            _operatorStack.Pop();   // pop the (

            if (isUserMethod)
            {
                // we should at least have a 'UserMethod' in the stack to indicate a user method is being parsed
                if (!_operatorStack.Any())
                {
                    throw new FormulaException("Invalid expression stack");
                }

                // methods pushed onto the operator stack have been prefixed with 'UserMethod' to identify them as methods since they 
                // may take parameters, in which case it is time to pop the expressions so they can be used for the input.
                if (_operatorStack.Peek() == CustomTokens.UserMethod)
                {
                    // ParseMethodToExpression will now pop all parameters and build an expression for the current 'method'
                    return false;     // abort further processing
                }
            }

            return true;
        }

        private void ProcessMethodArgument()
        {
            _lastPushIsOperator = true;     // Cater for when an argument may be a unary plus/minus

            // A parameter to a method may itself be an expression.
            // For example: 15.4 + ROUND(3.4355, 3) in
            //              ROUND(15.4 + ROUND(3.4355, 3), 2)
            ProcessOperators(_operatorStack, _expressionStack, () => _operatorStack.Peek() != CustomTokens.OpenScope);
        }

        private void ProcessOperator()
        {
            // must be called indirectly via Process()
            _formulaReader.CheckNotNull(nameof(_formulaReader));

            var operatorToken = _formulaReader.ReadOperator(_operationFactory);

            if (_lastPushIsOperator)
            {
                if (!"-+".Contains(operatorToken))
                {
                    throw new FormulaException("Invalid expression stack");
                }

                if (operatorToken == "-")
                {
                    operatorToken = CustomTokens.UnaryMinus;
                }
                else
                {
                    // ignore unary-plus
                    return;
                }
            }
            else
            {
                // validate the symbol is valid
                if (!_operationFactory.IsRegistered(operatorToken))
                {
                    throw new FormulaException($"Unknown operator: {operatorToken}");
                }

                var currentOperation = _operationFactory.GetOperation(operatorToken);

                ProcessOperators(_operatorStack, _expressionStack, () =>
                {
                    var next = _operatorStack.Peek();
                    return (next != CustomTokens.OpenScope) && (currentOperation.Precedence >= _operationFactory.GetOperation(next).Precedence);
                });
            }

            PushOperator(operatorToken);
        }

        private void ProcessNumerical()
        {
            // must be called indirectly via Process()
            _formulaReader.CheckNotNull(nameof(_formulaReader));

            // starting at the current reader position, read a numerical result and return it as an expression
            var value = _formulaReader.ReadNumerical();
            var numericalExpression = Expression.Constant(value);

            PushExpression(numericalExpression);
        }

        private void ProcessNamedOperand()
        {
            // must be called indirectly via Process()
            _formulaReader.CheckNotNull(nameof(_formulaReader));

            var operandExpression = GetNamedOperandExpression();

            PushExpression(operandExpression);
        }

        private void ParseContent(bool isUserMethod)
        {
            var next = _formulaReader.PeekNext();

            while (next > -1)
            {
                if (!ProcessToken((char)next, isUserMethod))
                {
                    return;
                }

                next = _formulaReader.PeekNext();
            }
        }

        // Reads the parameters of a user defined method, returning the final expression for the parsed method including all of its parameters.
        // get the expression for a variable or method at the current position
        private Expression GetNamedOperandExpression()
        {
            // continue to scan until we get a full word (which may include the full stop character)
            var namedOperand = _formulaReader.ReadNamedOperand(_operationFactory);

            // we need to peek ahead to see if the next character is a '('
            if (_formulaReader.PeekNext() == '(')
            {
                // the current 'word' represents the name of a method
                if (_userDefinedMethodFactory.IsRegistered(namedOperand))
                {
                    // consume the opening (
                    _formulaReader.ConsumeNext();

                    // the token processor consumes the trailing ')'
                    return ParseMethodToExpression(namedOperand);
                }

                throw new FormulaException($"Unknown method: {namedOperand}");
            }

            // For everything else, assume it must be a variable - not validating it exists since we are 
            // only compiling the expression.  It will be validated at runtime when the variables are available.
            _referencedVariableNames.Add(namedOperand);

            return FormulaExpressionFactory.CreateExpression(namedOperand, _variableRegistry);
        }

        private Expression ParseMethodToExpression(string methodName)
        {
            PushOperator(CustomTokens.UserMethod);      // to indicate a method operation
            PushOperator(CustomTokens.OpenScope);       // used to track nested argument expressions

            // capture the current count of expressions - it should increase by the number of parameters expected by the method
            var currentExpressionCount = _expressionStack.Count;

            // passing true to indicate it is a method being parsed (so parameters can be parsed / read correctly)
            ParseContent(true);

            // check if the expression is missing the required )
            if (_operatorStack.Peek() != CustomTokens.UserMethod)
            {
                throw new FormulaException($"Invalid expression near method: {methodName}");
            }

            _operatorStack.Pop();                        // pop the 'UserMethod'

            // determine how many arguments we need to pop 
            var operation = _userDefinedMethodFactory.GetMethod(methodName);    // will throw if not registered
            var expressionsRequired = operation.ArgumentCount;

            if (_expressionStack.Count - currentExpressionCount != expressionsRequired)
            {
                throw new FormulaException($"Expected {operation.ArgumentCount} parameters");
            }

            return FormulaExpressionFactory.CreateExpression(operation, _expressionStack);
        }

        private void RegisterTokenProcessors()
        {
            // start of a new scope
            RegisterTokenProcessor(
              (token, _) => token == '(',
              (_, _) =>
              {
                  _formulaReader.ConsumeNext();   // consume the '('
                  ProcessScopeStart();
                  return true;
              });

            // end of a scope
            RegisterTokenProcessor(
              (token, _) => token == ')',
              (_, isUserDefined) =>
              {
                  _formulaReader.ConsumeNext();   // consume the ')'
                  return ProcessScopeEnd(isUserDefined);
              });

            // arguments of a method
            RegisterTokenProcessor(
              (token, isUserDefined) => isUserDefined && token == ',',
              (_, _) =>
              {
                  _formulaReader.ConsumeNext();
                  ProcessMethodArgument();
                  return true;
              });

            // an operator
            RegisterTokenProcessor(
              (token, _) => _operationFactory.IsCandidate(token),
              (_, _) =>
              {
                  ProcessOperator();
                  return true;
              });

            // numerical constant
            RegisterTokenProcessor(
              (token, _) => FormulaReader.IsNumericalCandidate(token),
              (_, _) =>
              {
                  ProcessNumerical();
                  return true;
              });

            // everything else - variables and methods
            RegisterTokenProcessor(
              (_, _) => true,
              (_, _) =>
              {
                  ProcessNamedOperand();
                  return true;
              });
        }

        // The predicate is used to determine if the associated processor will be invoked. The input arguments of the predicate include the
        // next token to be read and a flag to indicate if the token is within the context of a user defined method. The processor 
        // is invoked if the predicate returns true.
        // The input arguments of the processor include the next token to be read and a flag to indicate if the token is within the context
        // of a user defined method. The processor returns true to indicate processing is to continue or false to indicate processing of the
        // current scope is complete (such as reading arguments of a user defined method).
        private void RegisterTokenProcessor(Func<char, bool, bool> predicate, Func<char, bool, bool> processor)
        {
            _tokenProcessors.Add(new FormulaTokenProcessorContext(predicate, processor));
        }

        private void ProcessOperators(Stack<string> operators, Stack<Expression> expressions, Func<bool> condition)
        {
            _ = operators.WhenNotNull(nameof(operators));
            _ = expressions.WhenNotNull(nameof(expressions));
            _ = condition.WhenNotNull(nameof(condition));

            while (operators.Any() && condition.Invoke())
            {
                var nextOperator = operators.Pop();
                var operation = _operationFactory.GetOperation(nextOperator);
                var expression = FormulaExpressionFactory.CreateExpression(operation, expressions);

                expressions.Push(expression);
            }
        }

        private bool ProcessToken(char token, bool isUserMethod)
        {
            var processor = _tokenProcessors
                .SkipWhile(p => !p.Predicate.Invoke(token, isUserMethod))
                .First();     // process the first match found

            // returns true to indicate processing should continue
            return processor.Processor.Invoke(token, isUserMethod);
        }
    }
}