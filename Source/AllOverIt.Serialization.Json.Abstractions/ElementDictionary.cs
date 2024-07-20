using AllOverIt.Assertion;
using AllOverIt.Serialization.Json.Abstractions.Exceptions;
using System.Collections;

namespace AllOverIt.Serialization.Json.Abstractions
{
    internal sealed class ElementDictionary : IElementDictionary
    {
        private readonly IDictionary<string, object?> _element;

        public IEnumerable<string> Names => _element.Keys;

        public IEnumerable<object?> Values => _element.Values;

        public ElementDictionary(IDictionary<string, object?> element)
        {
            _element = element.WhenNotNull();
        }

        public bool TryGetValue(string propertyName, out object? value)
        {
            _ = propertyName.WhenNotNullOrEmpty();

            return _element.TryGetValue(propertyName, out value);
        }

        public object? GetValue(string propertyName)
        {
            _ = propertyName.WhenNotNullOrEmpty();

            if (!TryGetValue(propertyName, out var value))
            {
                JsonHelperException.ThrowPropertyNotFound(propertyName);
            }

            return value;
        }

        public bool TrySetValue(string propertyName, object? value)
        {
            _ = propertyName.WhenNotNullOrEmpty();

            if (!_element.ContainsKey(propertyName))
            {
                return false;
            }

            _element[propertyName] = value;

            return true;
        }

        public void SetValue(string propertyName, object? value)
        {
            _ = propertyName.WhenNotNullOrEmpty();

            // Cannot use '_element[propertyName] = value' as this would add a new value if it doesn't exist.
            if (!TrySetValue(propertyName, value))
            {
                JsonHelperException.ThrowPropertyNotFound(propertyName);
            }
        }

        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            return _element.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
