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
    public sealed class AoiFormulaProcessor : IAoiFormulaProcessor
    {
        // Tags used to internally identify custom operators within the stack. Can be anything other than the operators defined in ArithmeticOperationFactory
        internal static class CustomTokens
        {
            internal const string UserMethod = "$$";
            internal const string UnaryMinus = "##";
            internal const string OpenScope = "(";
        }

        private IAoiVariableRegistry VariableRegistry { get; set; }
        private IAoiStack<string> OperatorStack { get; }
        private IAoiStack<Expression> ExpressionStack { get; }
        private IAoiFormulaExpressionFactory FormulaExpressionFactory { get; }
        private IAoiArithmeticOperationFactory OperationFactory { get; }
        private IAoiUserDefinedMethodFactory UserDefinedMethodFactory { get; }
        private Func<IAoiFormulaProcessor, IAoiFormulaReader, IAoiFormulaExpressionFactory, IAoiArithmeticOperationFactory, IAoiFormulaTokenProcessor> TokenProcessorFactory { get; }
        private IAoiFormulaReader FormulaReader { get; set; }
        private IAoiFormulaTokenProcessor TokenProcessor { get; set; }
        private IList<string> ReferencedVariableNames { get; set; }

        // tracks whether the last processed token was an operator or an expression so unary plus and unary minus can be handled.
        internal bool LastPushIsOperator { get; set; }

        public AoiFormulaProcessor(IAoiArithmeticOperationFactory operationFactory, IAoiUserDefinedMethodFactory userDefinedMethodFactory)
            : this(new AoiStack<string>(), new AoiStack<Expression>(), new AoiFormulaExpressionFactory(),
                operationFactory, userDefinedMethodFactory, (p, r, e, o) => new AoiFormulaTokenProcessor(p, r, e, o))
        {
        }

        internal AoiFormulaProcessor(IAoiStack<string> operatorStack, IAoiStack<Expression> expressionStack, IAoiFormulaExpressionFactory formulaExpressionFactory,
          IAoiArithmeticOperationFactory operationFactory, IAoiUserDefinedMethodFactory userDefinedMethodFactory,
          Func<IAoiFormulaProcessor, IAoiFormulaReader, IAoiFormulaExpressionFactory, IAoiArithmeticOperationFactory, IAoiFormulaTokenProcessor> tokenProcessorFactory)
        {
            OperatorStack = operatorStack.WhenNotNull(nameof(operatorStack));
            ExpressionStack = expressionStack.WhenNotNull(nameof(expressionStack));
            FormulaExpressionFactory = formulaExpressionFactory.WhenNotNull(nameof(formulaExpressionFactory));
            OperationFactory = operationFactory.WhenNotNull(nameof(operationFactory));
            UserDefinedMethodFactory = userDefinedMethodFactory.WhenNotNull(nameof(userDefinedMethodFactory));
            TokenProcessorFactory = tokenProcessorFactory.WhenNotNull(nameof(tokenProcessorFactory));

            // custom operator registration
            OperationFactory.RegisterOperation(CustomTokens.UnaryMinus, 4, 1, e => new AoiNegateOperator(e[0]));
        }

        public AoiFormulaProcessorResult Process(IAoiFormulaReader formulaReader, IAoiVariableRegistry variableRegistry)
        {
            FormulaReader = formulaReader.WhenNotNull(nameof(formulaReader));
            VariableRegistry = variableRegistry.WhenNotNull(nameof(variableRegistry));

            OperatorStack.Clear();
            ExpressionStack.Clear();
            LastPushIsOperator = true;
            ReferencedVariableNames = new List<string>();

            TokenProcessor = TokenProcessorFactory.Invoke(this, FormulaReader, FormulaExpressionFactory, OperationFactory);

            ParseContent(false);

            TokenProcessor.ProcessOperators(OperatorStack, ExpressionStack, () => true);

            var lastExpression = ExpressionStack.Pop();
            var funcExpression = Expression.Lambda<Func<double>>(lastExpression);

            return new AoiFormulaProcessorResult(funcExpression, ReferencedVariableNames);
        }

        public void PushOperator(string operatorToken)
        {
            _ = operatorToken.WhenNotNullOrEmpty(nameof(operatorToken));

            OperatorStack.Push(operatorToken);
            LastPushIsOperator = true;
        }

        public void PushExpression(Expression expression)
        {
            expression.WhenNotNull(nameof(expression));

            ExpressionStack.Push(expression);
            LastPushIsOperator = false;
        }

        public void ProcessScopeStart()
        {
            PushOperator(CustomTokens.OpenScope);
        }

        public bool ProcessScopeEnd(bool isUserMethod)
        {
            TokenProcessor.ProcessOperators(OperatorStack, ExpressionStack, () => OperatorStack.Peek() != CustomTokens.OpenScope);

            OperatorStack.Pop();   // pop the (

            if (isUserMethod)
            {
                // we should at least have a 'UserMethod' in the stack to indicate a user method is being parsed
                if (!OperatorStack.Any())
                {
                    throw new AoiFormulaException("Invalid expression stack");
                }

                // methods pushed onto the operator stack have been prefixed with 'UserMethod' to identify them as methods since they 
                // may take parameters, in which case it is time to pop the expressions so they can be used for the input.
                if (OperatorStack.Peek() == CustomTokens.UserMethod)
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
            TokenProcessor.ProcessOperators(OperatorStack, ExpressionStack, () => OperatorStack.Peek() != CustomTokens.OpenScope);
        }

        public void ProcessOperator()
        {
            // must be called indirectly via Process()
            _ = FormulaReader.WhenNotNull(nameof(FormulaReader));

            var operatorToken = FormulaReader.ReadOperator(OperationFactory);

            if (LastPushIsOperator)
            {
                if (!"-+".Contains(operatorToken))
                {
                    throw new AoiFormulaException("Invalid expression stack");
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
                if (!OperationFactory.IsRegistered(operatorToken))
                {
                    throw new AoiFormulaException($"Unknown operator: {operatorToken}");
                }

                var currentOperation = OperationFactory.GetOperation(operatorToken);

                TokenProcessor.ProcessOperators(OperatorStack, ExpressionStack, () =>
                {
                    var next = OperatorStack.Peek();
                    return (next != CustomTokens.OpenScope) && (currentOperation.Precedence >= OperationFactory.GetOperation(next).Precedence);
                });
            }

            PushOperator(operatorToken);
        }

        public void ProcessNumerical()
        {
            // must be called indirectly via Process()
            _ = FormulaReader.WhenNotNull(nameof(FormulaReader));

            // starting at the current reader position, read a numerical result and return it as an expression
            var value = FormulaReader.ReadNumerical();
            var numericalExpression = Expression.Constant(value);

            PushExpression(numericalExpression);
        }

        public void ProcessNamedOperand()
        {
            // must be called indirectly via Process()
            _ = FormulaReader.WhenNotNull(nameof(FormulaReader));

            var operandExpression = GetNamedOperandExpression();

            PushExpression(operandExpression);
        }

        private void ParseContent(bool isUserMethod)
        {
            var next = FormulaReader.PeekNext();

            while (next > -1)
            {
                if (!TokenProcessor.ProcessToken((char)next, isUserMethod))
                {
                    return;
                }

                next = FormulaReader.PeekNext();
            }
        }

        // Reads the parameters of a user defined method, returning the final expression for the parsed method including all of its parameters.
        // get the expression for a variable or method at the current position
        private Expression GetNamedOperandExpression()
        {
            // continue to scan until we get a full word (which may include the full stop character)
            var namedOperand = FormulaReader.ReadNamedOperand(OperationFactory);

            // we need to peek ahead to see if the next character is a '('
            if (FormulaReader.PeekNext() == '(')
            {
                // the current 'word' represents the name of a method
                if (UserDefinedMethodFactory.IsRegistered(namedOperand))
                {
                    // consume the opening (
                    FormulaReader.ReadNext();

                    // the token processor consumes the trailing ')'
                    return ParseMethodToExpression(namedOperand);
                }

                throw new AoiFormulaException($"Unknown method: {namedOperand}");
            }

            // For everything else, assume it must be a variable - not validating it exists since we are 
            // only compiling the expression.  It will be validated at runtime when the variables are available.
            ReferencedVariableNames.Add(namedOperand);

            return FormulaExpressionFactory.CreateExpression(namedOperand, VariableRegistry);
        }

        private Expression ParseMethodToExpression(string methodName)
        {
            PushOperator(CustomTokens.UserMethod);      // to indicate a method operation
            PushOperator(CustomTokens.OpenScope);       // used to track nested argument expressions

            // capture the current count of expressions - it should increase by the number of parameters expected by the method
            var currentExpressionCount = ExpressionStack.Count;

            // passing true to indicate it is a method being parsed (so parameters can be parsed / read correctly)
            ParseContent(true);

            // check if the expression is missing the required )
            if (OperatorStack.Peek() != CustomTokens.UserMethod)
            {
                throw new AoiFormulaException($"Invalid expression near method: {methodName}");
            }

            OperatorStack.Pop();                        // pop the 'UserMethod'

            // determine how many arguments we need to pop 
            var operation = UserDefinedMethodFactory.GetMethod(methodName);    // will throw if not registered
            var expressionsRequired = operation.ArgumentCount;

            if (ExpressionStack.Count - currentExpressionCount != expressionsRequired)
            {
                throw new AoiFormulaException($"Expected {operation.ArgumentCount} parameters");
            }

            return FormulaExpressionFactory.CreateExpression(operation, ExpressionStack);
        }
    }
}