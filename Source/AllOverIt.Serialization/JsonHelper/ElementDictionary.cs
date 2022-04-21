using System;
using AllOverIt.Assertion;
using System.Collections.Generic;
using AllOverIt.Serialization.JsonHelper.Exceptions;

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
            try
            {
                return _element[propertyName];
            }
            catch (KeyNotFoundException)
            {
                throw new JsonHelperException($"The property {propertyName} was not found.");
            }
        }
    }
}
