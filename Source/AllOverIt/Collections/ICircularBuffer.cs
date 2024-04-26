namespace AllOverIt.Collections
{
    /// <summary>Represents a fixed-size circular buffer providing insert, pop, and push operations.</summary>
    public interface ICircularBuffer<TType> : IEnumerable<TType>
    {
        /// <summary>The number of items in the buffer. The length can be less than the <see cref="Capacity"/>.</summary>
        int Length { get; }

        /// <summary>The maximum number of items that can be held by the buffer.</summary>
        int Capacity { get; }

        /// <summary>Indicates if the number of items in the buffer matches the <see cref="Capacity"/>.</summary>
        bool IsFull { get; }

        /// <summary>Indicates if there are no items in the buffer.</summary>
        bool IsEmpty { get; }

        /// <summary>Gets the element at the specified index.</summary>
        /// <param name="index">The index of the element to get from the buffer.</param>
        /// <returns>The element at the specified index.</returns>
        TType this[int index] { get; set; }

        /// <summary>Gets the element at the front of the buffer.</summary>
        /// <returns>The element at the front of the buffer.</returns>
        TType Front();

        /// <summary>Gets the element at the end of the buffer.</summary>
        /// <returns>The element at the end of the buffer.</returns>
        TType Back();

        /// <summary>Inserts a new element at the front of the buffer. If the buffer is full then the last
        /// element in the buffer will be removed to make room for the element being inserted.</summary>
        /// <param name="item">The new element to be inserted at the front of the buffer.</param>
        public void PushFront(TType item);

        /// <summary>Inserts a new element at the back of the buffer. If the buffer is full then the first
        /// element in the buffer will be removed to make room for the element being inserted.</summary>
        /// <param name="item">The new element to be inserted at the back of the buffer.</param>
        void PushBack(TType item);

        /// <summary>Removes and returns the element at the front of the buffer.</summary>
        /// <returns>The element removed from the front of the buffer.</returns>
        TType PopFront();

        /// <summary>Removes and returns the element at the back of the buffer.</summary>
        /// <returns>The element removed from the back of the buffer.</returns>
        TType PopBack();

        /// <summary>Clears the buffer.</summary>
        void Clear();

        /// <summary>Copies the <see cref="CircularBuffer{TType}"/> to a new array.</summary>
        /// <returns>A new array containing copies of the elements of the <see cref="CircularBuffer{TType}"/>.</returns>
        TType[] ToArray();
    }
}