using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using AllOverIt.Evaluator.Stack;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator
{
    public sealed class FormulaProcessor : IFormulaProcessor
    {
        // Tags used to internally identify custom operators within the stack. Can be anything other than the operators defined in ArithmeticOperationFactory
        internal static class CustomTokens
        {
            internal const string UserMethod = "$$";
            internal const string UnaryMinus = "##";
            internal const string OpenScope = "(";
        }

        private readonly IEvaluatorStack<string> _operatorStack;
        private readonly IEvaluatorStack<Expression> _expressionStack;
        private readonly IFormulaExpressionFactory _formulaExpressionFactory;
        private readonly IArithmeticOperationFactory _operationFactory;
        private readonly IUserDefinedMethodFactory _userDefinedMethodFactory;

        private readonly Func<IFormulaProcessor, IFormulaReader, IFormulaExpressionFactory, IArithmeticOperationFactory, IFormulaTokenProcessor>
            _tokenProcessorFactory;

        private IFormulaTokenProcessor _tokenProcessor;
        private IList<string> _referencedVariableNames;
        private IVariableRegistry _variableRegistry;
        private IFormulaReader _formulaReader;

        // tracks whether the last processed token was an operator or an expression so unary plus and unary minus can be handled.
        internal bool LastPushIsOperator { get; set; }

        public FormulaProcessor(IArithmeticOperationFactory operationFactory, IUserDefinedMethodFactory userDefinedMethodFactory)
            : this(new EvaluatorStack<string>(), new EvaluatorStack<Expression>(), new FormulaExpressionFactory(),
                operationFactory, userDefinedMethodFactory, (p, r, e, o) => new FormulaTokenProcessor(p, r, e, o))
        {
        }

        internal FormulaProcessor(IEvaluatorStack<string> operatorStack, IEvaluatorStack<Expression> expressionStack, IFormulaExpressionFactory formulaExpressionFactory,
          IArithmeticOperationFactory operationFactory, IUserDefinedMethodFactory userDefinedMethodFactory,
          Func<IFormulaProcessor, IFormulaReader, IFormulaExpressionFactory, IArithmeticOperationFactory, IFormulaTokenProcessor> tokenProcessorFactory)
        {
            _operatorStack = operatorStack.WhenNotNull(nameof(operatorStack));
            _expressionStack = expressionStack.WhenNotNull(nameof(expressionStack));
            _formulaExpressionFactory = formulaExpressionFactory.WhenNotNull(nameof(formulaExpressionFactory));
            _operationFactory = operationFactory.WhenNotNull(nameof(operationFactory));
            _userDefinedMethodFactory = userDefinedMethodFactory.WhenNotNull(nameof(userDefinedMethodFactory));
            _tokenProcessorFactory = tokenProcessorFactory.WhenNotNull(nameof(tokenProcessorFactory));

            // custom operator registration
            _operationFactory.RegisterOperation(CustomTokens.UnaryMinus, 4, 1, e => new NegateOperator(e[0]));
        }

        public FormulaProcessorResult Process(IFormulaReader formulaReader, IVariableRegistry variableRegistry)
        {
            _formulaReader = formulaReader.WhenNotNull(nameof(formulaReader));
            _variableRegistry = variableRegistry.WhenNotNull(nameof(variableRegistry));

            _operatorStack.Clear();
            _expressionStack.Clear();
            LastPushIsOperator = true;
            _referencedVariableNames = new List<string>();

            _tokenProcessor = _tokenProcessorFactory.Invoke(this, _formulaReader, _formulaExpressionFactory, _operationFactory);

            ParseContent(false);

            _tokenProcessor.ProcessOperators(_operatorStack, _expressionStack, () => true);

            var lastExpression = _expressionStack.Pop();
            var funcExpression = Expression.Lambda<Func<double>>(lastExpression);

            return new FormulaProcessorResult(funcExpression, _referencedVariableNames);
        }

        public void PushOperator(string operatorToken)
        {
            _ = operatorToken.WhenNotNullOrEmpty(nameof(operatorToken));

            _operatorStack.Push(operatorToken);
            LastPushIsOperator = true;
        }

        public void PushExpression(Expression expression)
        {
            expression.WhenNotNull(nameof(expression));

            _expressionStack.Push(expression);
            LastPushIsOperator = false;
        }

        public void ProcessScopeStart()
        {
            PushOperator(CustomTokens.OpenScope);
        }

        public bool ProcessScopeEnd(bool isUserMethod)
        {
            _tokenProcessor.ProcessOperators(_operatorStack, _expressionStack, () => _operatorStack.Peek() != CustomTokens.OpenScope);

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

        public void ProcessMethodArgument()
        {
            LastPushIsOperator = true;     // Cater for when an argument may be a unary plus/minus

            // A parameter to a method may itself be an expression.
            // For example: 15.4 + ROUND(3.4355, 3) in
            //              ROUND(15.4 + ROUND(3.4355, 3), 2)
            _tokenProcessor.ProcessOperators(_operatorStack, _expressionStack, () => _operatorStack.Peek() != CustomTokens.OpenScope);
        }

        public void ProcessOperator()
        {
            // must be called indirectly via Process()
            _formulaReader.CheckNotNull(nameof(_formulaReader));

            var operatorToken = _formulaReader.ReadOperator(_operationFactory);

            if (LastPushIsOperator)
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

                _tokenProcessor.ProcessOperators(_operatorStack, _expressionStack, () =>
                {
                    var next = _operatorStack.Peek();
                    return (next != CustomTokens.OpenScope) && (currentOperation.Precedence >= _operationFactory.GetOperation(next).Precedence);
                });
            }

            PushOperator(operatorToken);
        }

        public void ProcessNumerical()
        {
            // must be called indirectly via Process()
            _formulaReader.CheckNotNull(nameof(_formulaReader));

            // starting at the current reader position, read a numerical result and return it as an expression
            var value = _formulaReader.ReadNumerical();
            var numericalExpression = Expression.Constant(value);

            PushExpression(numericalExpression);
        }

        public void ProcessNamedOperand()
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
                if (!_tokenProcessor.ProcessToken((char)next, isUserMethod))
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
                    _formulaReader.ReadNext();

                    // the token processor consumes the trailing ')'
                    return ParseMethodToExpression(namedOperand);
                }

                throw new FormulaException($"Unknown method: {namedOperand}");
            }

            // For everything else, assume it must be a variable - not validating it exists since we are 
            // only compiling the expression.  It will be validated at runtime when the variables are available.
            _referencedVariableNames.Add(namedOperand);

            return _formulaExpressionFactory.CreateExpression(namedOperand, _variableRegistry);
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

            return _formulaExpressionFactory.CreateExpression(operation, _expressionStack);
        }
    }
}