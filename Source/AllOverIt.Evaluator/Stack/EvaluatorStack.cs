using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Evaluator.Stack
{
    internal sealed class EvaluatorStack<TType> : IEvaluatorStack<TType>
    {
        private Stack<TType> Stack { get; }

        public int Count => Stack.Count;

        public EvaluatorStack()
        {
            Stack = new Stack<TType>();
        }

        public EvaluatorStack(IEnumerable<TType> items)
        {
            Stack = new Stack<TType>(items);
        }

        public void Clear()
        {
            Stack.Clear();
        }

        public bool Any()
        {
            return Stack.Any();
        }

        public TType Peek()
        {
            return Stack.Peek();
        }

        public TType Pop()
        {
            return Stack.Pop();
        }

        public void Push(TType item)
        {
            Stack.Push(item);
        }

        public IEnumerator<TType> GetEnumerator()
        {
            return Stack.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // explicit
            return GetEnumerator();
        }
    }
}