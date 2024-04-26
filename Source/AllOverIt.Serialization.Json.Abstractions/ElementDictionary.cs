using AllOverIt.Assertion;
using AllOverIt.Serialization.Json.Abstractions.Exceptions;
using System.Collections;
using System.Collections.Generic;

namespace AllOverIt.Serialization.Json.Abstractions
{
    internal sealed class ElementDictionary : IElementDictionary
    {
        private readonly IDictionary<string, object> _element;

        public IEnumerable<string> Names => _element.Keys;

        public IEnumerable<object> Values => _element.Values;

        public ElementDictionary(IDictionary<string, object> element)
        {
            _element = element.WhenNotNull(nameof(element));
        }

        public bool TryGetValue(string propertyName, out object value)
        {
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            return _element.TryGetValue(propertyName, out value);
        }

        public object GetValue(string propertyName)
        {
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            if (TryGetValue(propertyName, out var value))
            {
                return value;
            }

            throw new JsonHelperException($"The property {propertyName} was not found.");
        }

        public bool TrySetValue(string propertyName, object value)
        {
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            if (!_element.ContainsKey(propertyName))
            {
                return false;
            }

            _element[propertyName] = value;

            return true;
        }

        public void SetValue(string propertyName, object value)
        {
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            // Cannot use '_element[propertyName] = value' as this would add a new value if it doesn't exist.
            if (!TrySetValue(propertyName, value))
            {
                throw new JsonHelperException($"The property {propertyName} was not found.");
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _element.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
