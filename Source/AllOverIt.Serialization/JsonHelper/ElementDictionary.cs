using AllOverIt.Assertion;
using AllOverIt.Serialization.JsonHelper.Exceptions;
using System.Collections.Generic;

namespace AllOverIt.Serialization.JsonHelper
{
    internal sealed class ElementDictionary : IElementDictionary
    {
        private readonly IDictionary<string, object> _element;

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

            try
            {
                return _element[propertyName];
            }
            catch (KeyNotFoundException exception)
            {
                throw new JsonHelperException($"The property {propertyName} was not found.", exception);
            }
        }
    }
}
