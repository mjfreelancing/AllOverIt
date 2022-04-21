using AllOverIt.Assertion;
using System.Collections.Generic;

namespace AllOverIt.Serialization.JsonHelper
{
    internal sealed class ElementDictionary : IElementDictionary
    {
        private readonly IReadOnlyDictionary<string, object> _element;

        public ElementDictionary(IReadOnlyDictionary<string, object> value)
        {
            _element = value.WhenNotNull(nameof(value));
        }

        public bool TryGetValue(string propertyName, out object value)
        {
            if (_element.TryGetValue(propertyName, out value))
            {
                return true;
            }

            return false;
        }

        public object GetValue(string propertyName)
        {
            return _element[propertyName];
        }
    }
}
