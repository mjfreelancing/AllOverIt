using System.Collections.Generic;

namespace AllOverIt.Evaluator.Stack
{
    // An interface representing a variable size last-in-first-out (LIFO) stack of instances of the same specified type.
    public interface IEvaluatorStack<TType> : IEnumerable<TType>
    {
        // Gets the number of items in the stack.
        int Count { get; }

        // Clears the contents of the stack without changing the current capacity.
        void Clear();

        // Indicates if the stack contains one or more items.
        bool Any();

        // Gets the item at the top of the stack without removing it.
        TType Peek();

        // Removes and returns the item at the top of the stack.
        TType Pop();

        // Inserts a new item at the top of the stack.
        void Push(TType item);
    }
}