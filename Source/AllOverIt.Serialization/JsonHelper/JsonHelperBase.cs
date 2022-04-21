using AllOverIt.Assertion;
using AllOverIt.Serialization.Abstractions;
using AllOverIt.Serialization.JsonHelper.Extensions;
using System.Collections.Generic;

namespace AllOverIt.Serialization.JsonHelper
{
    /// <summary>Provides functionality to extract values from a JSON string or anonymous object.</summary>
    public abstract class JsonHelperBase
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IElementDictionary _element;

        /// <summary>Constructor.</summary>
        /// <param name="value">An object, typically anonymous, that is to be queried for property values.</param>
        /// <param name="jsonSerializer">The serializer to use for processing.</param>
        public JsonHelperBase(object value, IJsonSerializer jsonSerializer)
            : this(jsonSerializer)
        {
            _element = CreateElementDictionary(value);
        }

        /// <summary>Constructor.</summary>
        /// <param name="value">A JSON string that is to be queried for property values.</param>
        /// <param name="jsonSerializer">The serializer to use for processing.</param>
        public JsonHelperBase(string value, IJsonSerializer jsonSerializer)
            : this(jsonSerializer)
        {
            _element = CreateElementDictionary(value);
        }

        private JsonHelperBase(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer.WhenNotNull(nameof(jsonSerializer));
        }

        /// <summary>Tries to get the value of a property on the root element.</summary>
        /// <param name="propertyName">The property name to get the value of.</param>
        /// <param name="value">The value of the property, if the property exists.</param>
        /// <returns>True if the property exists, otherwise false.</returns>
        public bool TryGetValue(string propertyName, out object value)
        {
            return _element.TryGetValue(propertyName, out value);
        }

        /// <summary>Gets the value of a specified property.</summary>
        /// <param name="propertyName">The property name to get the value of.</param>
        /// <returns>The value of a specified property.</returns>
        public object GetValue(string propertyName)
        {
            return _element.GetValue(propertyName);
        }

        /// <summary>Tries to get the value of a specified property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="propertyName">The property name to get the value of.</param>
        /// <param name="value">The value of the property, if the property exists.</param>
        /// <returns>True if the property exists, otherwise false.</returns>
        public bool TryGetValue<TValue>(string propertyName, out TValue value)
        {
            return _element.TryGetValue(propertyName, out value);
        }

        /// <summary>Gets the value of a specified property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="propertyName">The property name to get the value of.</param>
        /// <returns>The value of a specified property.</returns>
        public TValue GetValue<TValue>(string propertyName)
        {
            return _element.GetValue<TValue>(propertyName);
        }

        /// <summary>Tries to get an array of elements for a specified property.</summary>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <param name="array">The array of elements for the specified property.</param>
        /// <returns>True if the property exists, otherwise false.</returns>
        public bool TryGetArray(string arrayPropertyName, out IEnumerable<IElementDictionary> array)
        {
            return _element.TryGetArray(arrayPropertyName, out array);
        }

        /// <summary>Gets an array of elements for a specified property.</summary>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <returns>The array of elements for the specified property.</returns>
        public IEnumerable<IElementDictionary> GetArray(string arrayPropertyName)
        {
            return _element.GetArray(arrayPropertyName);
        }

        /// <summary>Tries to get the value of a property from each element of a specified array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <param name="propertyName">The property name to get the value of.</param>
        /// <param name="arrayValues">The value of each element in the specified array property.</param>
        /// <returns>True if the array and property exists, otherwise false.</returns>
        public bool TryGetArrayValues<TValue>(string arrayPropertyName, string propertyName, out IEnumerable<TValue> arrayValues)
        {
            return _element.TryGetArrayValues(arrayPropertyName, propertyName, out arrayValues);
        }

        /// <summary>Gets the value of a property from each element of a specified array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <param name="propertyName">The property name of the child element to get the value of.</param>
        /// <returns>The value of each element in the specified array property.</returns>
        public IEnumerable<TValue> GetArrayValues<TValue>(string arrayPropertyName, string propertyName)
        {
            return _element.GetArrayValues<TValue>(arrayPropertyName, propertyName);
        }

        /// <summary>Try to get a child array of elements for a specified property.</summary>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childArray">The deepest child array of elements.</param>
        /// <returns>A child array of elements for a specified property.</returns>
        public bool TryGetChildArray(IEnumerable<string> arrayPropertyNames, out IEnumerable<IElementDictionary> childArray)
        {
            return _element.TryGetChildArray(arrayPropertyNames, out childArray);
        }

        /// <summary>Get a child array of elements for a specified property.</summary>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <returns>The deepest child array of elements.</returns>
        public IEnumerable<IElementDictionary> GetChildArray(IEnumerable<string> arrayPropertyNames)
        {
            return _element.GetChildArray(arrayPropertyNames);
        }

        /// <summary>Tries to get the value of a property from each element of a specified child array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childPropertyName">The property name of the child element to get the value of.</param>
        /// <param name="childArrayValues">The value of each element in the specified child array property.</param>
        /// <returns>True if the arrays and property exists, otherwise false.</returns>
        public bool TryGetChildArrayValues<TValue>(IEnumerable<string> arrayPropertyNames, string childPropertyName, out IEnumerable<TValue> childArrayValues)
        {
            return _element.TryGetChildArrayValues(arrayPropertyNames, childPropertyName, out childArrayValues);
        }

        /// <summary>Get the value of a property from each element of a specified child array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childPropertyName">The property name of the child element to get the value of.</param>
        /// <returns>The value of each element in the specified child array property.</returns>
        public IEnumerable<TValue> GetChildArrayValues<TValue>(IEnumerable<string> arrayPropertyNames, string childPropertyName)
        {
            return _element.GetChildArrayValues<TValue>(arrayPropertyNames, childPropertyName);
        }

        private IElementDictionary CreateElementDictionary(object value)
        {
            var serialized = _jsonSerializer.SerializeObject(value);
            return CreateElementDictionary(serialized);
        }

        private IElementDictionary CreateElementDictionary(string value)
        {
            var dictionary = _jsonSerializer.DeserializeObject<Dictionary<string, object>>(value);
            return new ElementDictionary(dictionary);
        }
    }
}
