using System.Collections;

namespace AllOverIt.Collections
{
    /// <summary>Provides a truly immutable collection.</summary>
    /// <typeparam name="TType">The type stored by the collection.</typeparam>
    public class ReadOnlyCollection<TType> : IReadOnlyCollection<TType>
    {
        private readonly List<TType> _list;

        /// <inheritdoc />
        public int Count => _list.Count;

        /// <summary>Constructor.</summary>
        public ReadOnlyCollection()
        {
            _list = [];
        }

        /// <summary>Constructor.</summary>
        /// <param name="data">The data to add to the readonly list.</param>
        public ReadOnlyCollection(IEnumerable<TType> data = default)
        {
            _list = new(data);
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
