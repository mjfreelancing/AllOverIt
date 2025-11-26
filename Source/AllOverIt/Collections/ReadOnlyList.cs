using System.Collections;

namespace AllOverIt.Collections
{
    /// <summary>Provides a truly immutable list.</summary>
    /// <typeparam name="TType">The type stored by the list.</typeparam>
    public class ReadOnlyList<TType> : IReadOnlyList<TType>
    {
        private readonly List<TType> _list;

        /// <inheritdoc />
        public TType this[int index] => _list[index];

        /// <inheritdoc />
        public int Count => _list.Count;

        /// <summary>Constructor.</summary>

        public ReadOnlyList()
            : this([])
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="data">The data to add to the readonly list.</param>
        public ReadOnlyList(IEnumerable<TType> data)
        {
            _list = [.. data];
        }

        /// <inheritdoc />
        public IEnumerator<TType> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
