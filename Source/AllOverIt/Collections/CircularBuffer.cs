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
        private int _length;

        /// <summary>Returns the number of items in the buffer. The length can be less than the <see cref="Capacity"/>.</summary>
        public int Length => _length;

        /// <summary>Indicates the maximum number of items that can be held by the buffer.</summary>
        public int Capacity => _buffer.Length;

        /// <summary>Indicates if the number of items in the buffer matches the <see cref="Capacity"/>.</summary>
        public bool IsFull => Length == Capacity;

        /// <summary>Indicates if there are no items in the buffer.</summary>
        public bool IsEmpty => Length == 0;

        /// <summary>Constructs an empty circular buffer with a specified capacity.</summary>
        /// <param name="capacity">The capacity to set. The capacity specifies the maximum number of items that can be held by the buffer.</param>
        public CircularBuffer(int capacity)
            : this(capacity, [])
        {
        }

        /// <summary>Constructs a circular buffer with a specified capacity, and populates it with the specified items.</summary>
        /// <param name="capacity">>The capacity to set. The capacity specifies the maximum number of items that can be held by the buffer.</param>
        /// <param name="items">The initial collection of items to populate the buffer with. A call to <see cref="Front"/> will return
        /// the first item, and a called to <see cref="Back"/> will return the last item.</param>
        public CircularBuffer(int capacity, TType[] items)
        {
            _ = items.WhenNotNull(nameof(items));

            Throw<ArgumentException>.When(capacity < 1, "The circular buffer requires a capacity of at least 1.");
            Throw<ArgumentException>.When(items.Length > capacity, "The item count exceeds the circular buffer capacity.");

            _buffer = new TType[capacity];

            Array.Copy(items, _buffer, items.Length);

            _length = items.Length;
            _start = 0;
            _end = _length == capacity ? 0 : _length;
        }

        public TType Front()
        {
            Throw<InvalidOperationException>.When(IsEmpty, "The buffer is empty.");

            return _buffer[_start];
        }

        public TType Back()
        {
            Throw<InvalidOperationException>.When(IsEmpty, "The buffer is empty.");

            var index = (_end != 0 ? _end : Capacity) - 1;

            return _buffer[index];
        }

        public TType this[int index]
        {
            get
            {
                var internalIndex = InternalIndex(index);

                return _buffer[internalIndex];
            }
            set
            {
                var internalIndex = InternalIndex(index);

                _buffer[internalIndex] = value;
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
                ++_length;
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
                ++_length;
            }
        }

        public TType PopFront()
        {
            Throw<InvalidOperationException>.When(IsEmpty, "The buffer is empty.");

            var value = _buffer[_start];
            _buffer[_start] = default;
            IncrementWithWrap(ref _start);
            --_length;

            return value;
        }

        public TType PopBack()
        {
            Throw<InvalidOperationException>.When(IsEmpty, "The buffer is empty.");

            DecrementWithWrap(ref _end);
            var value = _buffer[_end];
            _buffer[_end] = default;
            --_length;

            return value;
        }

        public void Clear()
        {
            _start = 0;
            _end = 0;
            _length = 0;

            Array.Clear(_buffer, 0, _buffer.Length);
        }

        public TType[] ToArray()
        {
            var array = new TType[Length];
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

            --index;
        }

        private int InternalIndex(int index)
        {
            Throw<IndexOutOfRangeException>.When(index >= _length, "The provided index exceeds the current buffer size.");

            var offset = index < (Capacity - _start)
                ? index
                : index - Capacity;

            return _start + offset;
        }

        // There can be 1 or 2 segments, and one of them may be empty
        private IEnumerable<ArraySegment<TType>> GetArraySegments()
        {
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