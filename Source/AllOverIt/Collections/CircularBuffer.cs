using AllOverIt.Assertion;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AllOverIt.Collections
{
    /// <summary>Implements a circular buffer of a fixed size, providing O(1) performance for access,
    /// insert, pop, and push operations.</summary>
    public class CircularBuffer<TType> : IEnumerable<TType>
    {
        private readonly TType[] _buffer;

        private int _start;
        private int _end;
        private int _size;

        public int Size => _size;

        public int Capacity => _buffer.Length;

        public bool IsFull => Size == Capacity;

        public bool IsEmpty => Size == 0;

        public CircularBuffer(int capacity)
            : this(capacity, [])
        {
        }

        public CircularBuffer(int capacity, TType[] items)
        {
            _ = items.WhenNotNull(nameof(items));

            Throw<ArgumentException>.When(capacity < 1, "The circular buffer requires a capacity of at least 1.");
            Throw<ArgumentException>.When(items.Length > capacity, "The item count exceeds the circular buffer capacity.");

            _buffer = new TType[capacity];

            Array.Copy(items, _buffer, items.Length);

            _size = items.Length;
            _start = 0;
            _end = _size == capacity ? 0 : _size;
        }

        public TType Front()
        {
            Throw<InvalidOperationException>.When(IsEmpty, "The buffer is empty.");

            return _buffer[_start];
        }

        public TType Back()
        {
            Throw<InvalidOperationException>.When(IsEmpty, "The buffer is empty.");

            return _buffer[(_end != 0 ? _end : Capacity) - 1];
        }

        public TType this[int index]
        {
            get
            {
                var actualIndex = InternalIndex(index);

                return _buffer[actualIndex];
            }
            set
            {
                var actualIndex = InternalIndex(index);

                _buffer[actualIndex] = value;
            }
        }

        public void PushFront(TType item)
        {
            if (IsFull)
            {
                DecrementWithWrap(ref _start);
                _end = _start;
                _buffer[_start] = item;
            }
            else
            {
                DecrementWithWrap(ref _start);
                _buffer[_start] = item;
                ++_size;
            }
        }

        public void PushBack(TType item)
        {
            if (IsFull)
            {
                _buffer[_end] = item;
                IncrementWithWrap(ref _end);
                _start = _end;
            }
            else
            {
                _buffer[_end] = item;
                IncrementWithWrap(ref _end);
                ++_size;
            }
        }

        public void PopFront()
        {
            Throw<InvalidOperationException>.When(IsEmpty, "The buffer is empty");

            _buffer[_start] = default;
            IncrementWithWrap(ref _start);
            --_size;
        }

        public void PopBack()
        {
            Throw<InvalidOperationException>.When(IsEmpty, "The buffer is empty");

            DecrementWithWrap(ref _end);
            _buffer[_end] = default;
            --_size;
        }

        public void Clear()
        {
            _start = 0;
            _end = 0;
            _size = 0;

            Array.Clear(_buffer, 0, _buffer.Length);
        }

        public TType[] ToArray()
        {
            var array = new TType[Size];
            var offset = 0;
            var segments = GetArraySegments();

            foreach (var segment in segments)
            {
                Array.Copy(segment.Array, segment.Offset, array, offset, segment.Count);
                offset += segment.Count;
            }

            return array;
        }

        public IEnumerator<TType> GetEnumerator()
        {
            var segments = GetArraySegments();

            foreach (var segment in segments)
            {
                foreach (var element in segment)
                {
                    yield return element;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void IncrementWithWrap(ref int index)
        {
            if (++index == Capacity)
            {
                index = 0;
            }
        }

        private void DecrementWithWrap(ref int index)
        {
            if (index == 0)
            {
                index = Capacity;
            }

            index--;
        }

        private int InternalIndex(int index)
        {
            Throw<IndexOutOfRangeException>.When(index >= _size, "The provided index exceeds the current buffer size.");

            var offset = index < (Capacity - _start)
                ? index
                : index - Capacity;

            return _start + offset;
        }




        /// <summary>
        /// Get the contents of the buffer as 2 ArraySegments.
        /// Respects the logical contents of the buffer, where
        /// each segment and items in each segment are ordered
        /// according to insertion.
        ///
        /// Fast: does not copy the array elements.
        /// Useful for methods like <c>Send(IList&lt;ArraySegment&lt;Byte&gt;&gt;)</c>.
        /// 
        /// <remarks>Segments may be empty.</remarks>
        /// </summary>
        /// <returns>An IList with 2 segments corresponding to the buffer content.</returns>
        private IEnumerable<ArraySegment<TType>> GetArraySegments()
        {
            // The array is composed by at most two non-contiguous segments, 

            if (IsEmpty)
            {
                yield return new ArraySegment<TType>([]);
            }
            else
            {
                yield return FirstSegment();
                yield return SecondSegment();
            }
        }

        private ArraySegment<TType> FirstSegment()
        {
            var count = _start < _end
                ? _end - _start
                : _buffer.Length - _start;

            return new ArraySegment<TType>(_buffer, _start, count);
        }

        private ArraySegment<TType> SecondSegment()
        {
            return _start < _end
                ? new ArraySegment<TType>([])
                : new ArraySegment<TType>(_buffer, 0, _end);
        }
    }
}