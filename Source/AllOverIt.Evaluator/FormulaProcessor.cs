using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Operators;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator
{
    /// <summary>Parses a mathematical formula and compiles it to an expression that can be later evaluated.</summary>
    /// <remarks>A compiled expression provides a vast performance benefit when the formula needs to be evaluated multiple times.</remarks>
    public sealed class FormulaProcessor
    {
        // Tags used to internally identify custom operators within the stack. Can be anything other than the operators defined in ArithmeticOperationFactory
        internal static class CustomTokens
        {
            internal const string UserMethod = "$1";
            internal const string UnaryMinus = "$2";
            internal const string OpenScope = "(";
        }

        private static readonly IReadOnlyCollection<string> EmptyReadOnlyCollection = new List<string>();
        private static readonly char DecimalSeparator = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        private readonly IList<FormulaTokenProcessorContext> _tokenProcessors = new List<FormulaTokenProcessorContext>();
        private readonly Stack<string> _operatorStack = new();
        private readonly Stack<Expression> _expressionStack = new();
        private readonly HashSet<string> _referencedVariableNames = new();
        private readonly IArithmeticOperationFactory _operationFactory;
        private readonly IUserDefinedMethodFactory _userDefinedMethodFactory;

        private IVariableRegistry _variableRegistry;
        private IVariableRegistry VariableRegistry
        {
            get
            {
                // Using Lazy<IVariableRegistry> will achieve the same thing but this uses slightly less memory
                _variableRegistry ??= new VariableRegistry();
                return _variableRegistry;
            }
        }

        private string _formula;
        private int _currentIndex;

        // tracks whether the last processed token was an operator or an expression so unary plus and unary minus can be handled.
        private bool _lastPushIsOperator;

        /// <summary>Constructor.</summary>
        /// <param name="operationFactory">Provides the required support for performing mathematical operations, such as addition, multiplication,
        /// and so on.</param>
        /// <param name="userDefinedMethodFactory">Provides support for user-defined methods in addition to the built-in library of methods. Refer
        /// to the <c>UserDefinedMethodFactory</c> for a list of available built-in methods.</param>
        public FormulaProcessor(IArithmeticOperationFactory operationFactory, IUserDefinedMethodFactory userDefinedMethodFactory)
        {
            _operationFactory = operationFactory.WhenNotNull(nameof(operationFactory));
            _userDefinedMethodFactory = userDefinedMethodFactory.WhenNotNull(nameof(userDefinedMethodFactory));

            // custom operator registration (using TryRegisterOperation as the factory can be shared across threads)
            _operationFactory.TryRegisterOperation(CustomTokens.UnaryMinus, 4, 1, e => new NegateOperator(e[0]));

            RegisterTokenProcessors();
        }

        /// <summary>Parses a provided formula to create a compiled expression that can later be evaluated.</summary>
        /// <param name="formula">The formula to be parsed and compiled into an expression.</param>
        /// <param name="variableRegistry">The variable registry to be referenced during the formula's evaluation (after compilation).</param>
        /// <returns>The processed result, including the formula's compiled expression, a collection of any referenced variables, and a variable
        /// registry that will be referenced by the compiled expression when it is evaluated.</returns>
        /// <remarks>The variable registry does not need to be provided if the formula does not contain any variables. If the formula does contain
        /// variables then there are two use cases. First, if a variable registry is not provided then an instance will be created during processing
        /// and will be returned as part of the result. Second, if a variable registry is provided then that instance will be used (and will always
        /// be returned with the result, even if there are no variables in the formula). It is the caller's responsibility to populate the variable
        /// registry before the compiled expression is evaluated.</remarks>
        public FormulaProcessorResult Process(string formula, IVariableRegistry variableRegistry)
        {
            _formula = formula.WhenNotNullOrEmpty(nameof(formula));

            _variableRegistry = variableRegistry;   // can be null
            _lastPushIsOperator = true;
            _currentIndex = 0;

            try
            {
                ParseContent(false);
                ProcessOperators(_operatorStack, _expressionStack, () => true);

                var lastExpression = _expressionStack.Pop();
                var funcExpression = Expression.Lambda<Func<double>>(lastExpression);

                var referencedVariableNames = _referencedVariableNames.Any()

                    // must return a copy of the referenced variable names
                    ? new ReadOnlyCollection<string>(_referencedVariableNames.ToList())

                    // prevent allocating multiple collections when there's nothing in them
                    : EmptyReadOnlyCollection;

                // If the caller did not provide a registry (it was null) and
                //  - there were no variables then returning null for the registry.
                //  - there were variables then this processor will have created one, so return it.
                // If the caller provided a registry, just return it
                var registry = variableRegistry ?? _variableRegistry;

                return new FormulaProcessorResult(funcExpression, referencedVariableNames, registry);
            }
            catch (Exception exception)
            {
                var span = _formula.AsSpan();
                var processed = span[.._currentIndex].ToString();

                throw new FormulaException($"Invalid expression. See index {_currentIndex}, near '{processed}'.", exception);
            }
            finally
            {
                ClearState();
            }
        }

        private void ClearState()
        {
            _operatorStack.Clear();
            _expressionStack.Clear();
            _referencedVariableNames.Clear();
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
                    throw new FormulaException("Invalid expression stack.");
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
            var operatorToken = ReadOperator();

            if (_lastPushIsOperator)
            {
                var tokenSpan = operatorToken.AsSpan();

                if (tokenSpan.Length != 1 || !IsUnaryPlusOrMinus(tokenSpan[0]))
                {
                    throw new FormulaException("Invalid expression stack.");
                }

                if (tokenSpan[0] == '-')
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
                // validate the symbol is valid - throws if it isn't registered
                var currentOperation = _operationFactory.GetOperation(operatorToken);

                ProcessOperators(_operatorStack, _expressionStack, () =>
                {
                    var next = _operatorStack.Peek();

                    return next != CustomTokens.OpenScope && 
                           currentOperation.Precedence >= _operationFactory.GetOperation(next).Precedence;
                });
            }

            PushOperator(operatorToken);
        }

        private void ProcessNumerical()
        {
            // starting at the current reader position, read a numerical result and return it as an expression
            var value = ReadNumerical();
            var numericalExpression = Expression.Constant(value);

            PushExpression(numericalExpression);
        }

        private void ProcessNamedOperand()
        {
            if (!_lastPushIsOperator)
            {
                var namedOperand = ReadNamedOperand();

                throw new FormulaException($"'{namedOperand}' is a variable or method that does not follow an operator, or is an unregistered operator.");
            }

            var operandExpression = GetNamedOperandExpression();

            PushExpression(operandExpression);
        }

        private void ParseContent(bool isUserMethod)
        {
            var span = _formula.AsSpan();

            while (_currentIndex != span.Length)
            {
                var next = span[_currentIndex];

                if (!ProcessToken(next, isUserMethod))
                {
                    return;
                }
            }
        }

        // Reads the parameters of a user defined method, returning the final expression for the parsed method including all of its parameters.
        // get the expression for a variable or method at the current position
        private Expression GetNamedOperandExpression()
        {
            // continue to scan until we get a full word (which may include the full stop character)
            var namedOperand = ReadNamedOperand();

            var span = _formula.AsSpan();

            // In the case of a method, we need to peek ahead to see if the next character is a '('.
            // We need to also make sure we are not at the end of a formula (the named operand will be a variable)
            if (_currentIndex < span.Length && span[_currentIndex] == '(')
            {
                // the current 'word' represents the name of a method
                if (_userDefinedMethodFactory.IsRegistered(namedOperand))
                {
                    // consume the opening (
                    _currentIndex++;

                    // the token processor consumes the trailing ')'
                    return ParseMethodToExpression(namedOperand);
                }

                throw new FormulaException($"Unknown method: {namedOperand}.");
            }

            // For everything else, assume it must be a variable - not validating it exists since we are 
            // only compiling the expression.  It will be validated at runtime when the variables are available.
            _referencedVariableNames.Add(namedOperand);

            return FormulaExpressionFactory.CreateExpression(namedOperand, VariableRegistry);
        }

        private Expression ParseMethodToExpression(string methodName)
        {
            PushOperator(CustomTokens.UserMethod);      // to indicate a method operation
            PushOperator(CustomTokens.OpenScope);       // used to track nested argument expressions

            // capture the current count of expressions - it should increase by the number of parameters expected by the method
            var currentExpressionCount = _expressionStack.Count;

            // passing true to indicate it is a method being parsed (so parameters can be parsed / read correctly)
            ParseContent(true);

            // check if the expression is missing the required user method token
            if (_operatorStack.Peek() != CustomTokens.UserMethod)
            {
                throw new FormulaException($"Invalid expression near method: {methodName}.");
            }

            _operatorStack.Pop();                        // pop the 'UserMethod'

            // determine how many arguments we need to pop 
            var operation = _userDefinedMethodFactory.GetMethod(methodName);    // will throw if not registered
            var expressionsRequired = operation.ArgumentCount;

            if (_expressionStack.Count - currentExpressionCount != expressionsRequired)
            {
                throw new FormulaException($"The {methodName.ToUpper()} method expects {operation.ArgumentCount} parameter(s).");
            }

            return FormulaExpressionFactory.CreateExpression(operation, _expressionStack);
        }

        private void RegisterTokenProcessors()
        {
            // args are (token, isUserDefined)

            // start of a new scope
            RegisterTokenProcessor(
              (token, _) => token == '(',
              (_, _) =>
              {
                  // consume the '('
                  _currentIndex++;
                  
                  ProcessScopeStart();
                  return true;
              });

            // end of a scope
            RegisterTokenProcessor(
              (token, _) => token == ')',
              (_, isUserDefined) =>
              {
                  // consume the ')'
                  _currentIndex++;
                  
                  return ProcessScopeEnd(isUserDefined);
              });

            // arguments of a method
            RegisterTokenProcessor(
              (token, isUserDefined) => isUserDefined && token == ',',
              (_, _) =>
              {
                  _currentIndex++;
                  ProcessMethodArgument();
                  return true;
              });

            // an operator
            RegisterTokenProcessor(
              (token, _) => IsCandidateOperation(token),
              (_, _) =>
              {
                  ProcessOperator();
                  return true;
              });

            // numerical constant
            RegisterTokenProcessor(
              (token, _) => IsNumericalCandidate(token),
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
                .SkipWhile(context => !context.Predicate.Invoke(token, isUserMethod))
                .First();     // process the first match found

            // returns true to indicate processing should continue
            return processor.Processor.Invoke(token, isUserMethod);
        }

        private double ReadNumerical()
        {
            var previousTokenWasExponent = false;

            var span = _formula.AsSpan();

            if (_currentIndex == span.Length)
            {
                throw new FormulaException("Nothing to read.");
            }

            var startIndex = _currentIndex;

            // begin by reading tokens that could make up a numerical value, including support for exponent values
            while (_currentIndex != span.Length)
            {
                var next = span[_currentIndex];

                var isExponent = "eE".ContainsChar(next);

                var allowMinus = previousTokenWasExponent && (next == '-');

                if (IsNumericalCandidate(next) || isExponent || allowMinus)
                {
                    _currentIndex++;
                    previousTokenWasExponent = isExponent;
                }
                else
                {
                    break;
                }
            }

            if (startIndex == _currentIndex)
            {
                throw new FormulaException("Unexpected non-numerical token.");
            }

            var operand = span[startIndex.._currentIndex];

            double value;

            try
            {
                // will throw 'FormatException' if invalid - such as multiple decimal points
                value = double.Parse(operand, NumberStyles.Float); // supports numbers such as 3.9E7
            }

            catch (FormatException exception)
            {
                throw new FormulaException($"Invalid numerical value: {operand.ToString()}.", exception);
            }

            return value;
        }

        private string ReadNamedOperand()
        {
            var span = _formula.AsSpan();

            if (_currentIndex == span.Length)
            {
                throw new FormulaException("Nothing to read");
            }

            var startIndex = _currentIndex;

            while (_currentIndex != span.Length)
            {
                var next = span[_currentIndex];

                // numerical characters are not checked since variables can contain numbers
                if (next != '(' &&
                    next != ')' &&
                    next != ',' &&
                    !IsCandidateOperation(next))   // only looking to see if 'next' is the start of a new operation
                {
                    _currentIndex++;
                }
                else
                {
                    break;
                }
            }

            if (startIndex == _currentIndex)
            {
                throw new FormulaException("Unexpected empty named operand.");
            }

            return span[startIndex.._currentIndex].ToString();
        }

        private string ReadOperator()
        {
            var span = _formula.AsSpan();

            if (_currentIndex == span.Length)
            {
                throw new FormulaException("Nothing to read");
            }

            var startIndex = _currentIndex;

            while (_currentIndex != span.Length)
            {
                var next = span[_currentIndex];

                // check for unary plus/minus
                if (_currentIndex > startIndex && IsUnaryPlusOrMinus(next))
                {
                    // 3 * -7 would have read "*-"
                    break;
                }

                // keep reading while ever the characters read are part of a registered operator
                // (almost always a single character, but supports multi-character)
                var isCandidate = startIndex == _currentIndex
                    ? IsCandidateOperation(span[_currentIndex])        // avoid creation of a string
                    : IsCandidateOperation(span.Slice(startIndex, _currentIndex - startIndex + 1).ToString());

                if (isCandidate)
                {
                    _currentIndex++;
                }
                else
                {
                    break;
                }
            }

            if (startIndex == _currentIndex)
            {
                throw new FormulaException("Unexpected empty operation.");
            }

            return span[startIndex.._currentIndex].ToString();
        }

        private static bool IsNumericalCandidate(char token)
        {
            return char.IsDigit(token) || (token == DecimalSeparator);
        }

        private static bool IsUnaryPlusOrMinus(char token)
        {
            return token is '-' or '+';
        }

        private bool IsCandidateOperation(char symbol)
        {
            return _operationFactory.RegisteredOperations.Any(key => key[0] == symbol);
        }

        private bool IsCandidateOperation(string token)
        {
            return _operationFactory.RegisteredOperations.Any(key => key.StartsWith(token));
        }
    }
}