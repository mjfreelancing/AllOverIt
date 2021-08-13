using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Operations;
using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace AllOverIt.Evaluator
{
    public sealed class FormulaReader : IDisposable
    {
        private static readonly StringBuilder TokenBuilder = new();
        private static readonly char DecimalSeparator = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        private TextReader _reader;

        public FormulaReader(string formula)
        {
            _ = formula.WhenNotNullOrEmpty(nameof(formula));

            _reader = new StringReader(formula);
        }

        public int PeekNext()
        {
            return _reader.Peek();
        }

        public void ConsumeNext()
        {
            _reader.Read();
        }

        // Supports exponent values
        public double ReadNumerical()
        {
            var previousTokenWasExponent = false;

            // operand
            TokenBuilder.Clear();

            var peek = PeekNext();

            if (peek == -1)
            {
                throw new FormulaException("Nothing to read");
            }

            // begin by reading tokens that could make up a numerical value, including support for exponent values
            while (peek > -1)
            {
                var next = (char)peek;

                var isExponent = "eE".ContainsChar(next);

                var allowMinus = previousTokenWasExponent && (next == '-');

                if (IsNumericalCandidate(next) || isExponent || allowMinus)
                {
                    ConsumeNext();
                    TokenBuilder.Append(next);
                    previousTokenWasExponent = isExponent;
                }
                else
                {
                    break;
                }

                peek = PeekNext();
            }

            var operand = $"{TokenBuilder}";

            if (string.IsNullOrWhiteSpace(operand))
            {
                throw new FormulaException("Unexpected non-numerical token");
            }

            double value;

            try
            {
                // will throw 'FormatException' if invalid - such as multiple decimal points
                value = double.Parse(operand, NumberStyles.Float); // supports numbers such as 3.9E7
            }

            catch (FormatException exception)
            {
                throw new FormulaException($"Invalid numerical value: {operand}", exception);
            }

            return value;
        }

        public string ReadNamedOperand(IArithmeticOperationFactory operationFactory)
        {
            _ = operationFactory.WhenNotNull(nameof(operationFactory));

            TokenBuilder.Clear();

            var peek = PeekNext();

            if (peek == -1)
            {
                throw new FormulaException("Nothing to read");
            }

            while (peek > -1)
            {
                var next = (char)peek;

                if (next != '(' &&
                    next != ')' &&
                    next != ',' &&
                    !operationFactory.IsCandidate(next))
                {
                    ConsumeNext();
                    TokenBuilder.Append($"{next}");
                }
                else
                {
                    break;
                }

                peek = PeekNext();
            }

            var variableOrMethod = $"{TokenBuilder}";

            if (string.IsNullOrWhiteSpace(variableOrMethod))
            {
                throw new FormulaException("Unexpected empty named operand");
            }

            return variableOrMethod;
        }

        public string ReadOperator(IArithmeticOperationFactory operationFactory)
        {
            _ = operationFactory.WhenNotNull(nameof(operationFactory));

            // operation
            TokenBuilder.Clear();

            var peek = PeekNext();

            if (peek == -1)
            {
                throw new FormulaException("Nothing to read");
            }

            while (peek > -1)
            {
                var next = (char)peek;

                // keep reading while ever the characters read are part of a registered operator
                if (operationFactory.IsCandidate(next))
                {
                    // check for unary plus/minus
                    if ("-+".ContainsChar(next) && TokenBuilder.Length > 0)
                    {
                        // 3 * -7 would have read "*-"
                        break;
                    }

                    ConsumeNext();
                    TokenBuilder.Append($"{next}");
                }
                else
                {
                    break;
                }

                peek = PeekNext();
            }

            var operation = $"{TokenBuilder}";

            if (string.IsNullOrWhiteSpace(operation))
            {
                throw new FormulaException("Unexpected empty operation");
            }

            return operation;
        }

        // Indicates if the provided token is a digit character or decimal separator.
        public static bool IsNumericalCandidate(char token)
        {
            return char.IsDigit(token) || (token == DecimalSeparator);
        }

        public void Dispose()
        {
            _reader?.Dispose();
            _reader = null;
        }
    }
}