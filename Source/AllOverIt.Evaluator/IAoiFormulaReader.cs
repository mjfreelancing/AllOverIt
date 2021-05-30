using AllOverIt.Evaluator.Operations;
using System;

namespace AllOverIt.Evaluator
{
    // An interface representing a reader that is used to tokenize a formula into operators, operands and method names.
    public interface IAoiFormulaReader : IDisposable
    {
        // Reads the next character without changing the state of the reader.
        // Return an integer representing the next character to be read.
        int PeekNext();

        // Reads the next character from the reader and advances the character position by one character.
        // Returns the next character from the reader.
        int ReadNext();

        // Reads the numerical value at the current position of the reader.
        // Returns the value read as a double.
        double ReadNumerical();

        // Reads a variable or method name starting at the current reader position.
        // Returns the name of the variable or method name read.
        string ReadNamedOperand(IAoiArithmeticOperationFactory operationFactory);

        // Reads an operator starting at the current reader position.
        // Returns the operator symbol read.
        string ReadOperator(IAoiArithmeticOperationFactory operationFactory);
    }
}