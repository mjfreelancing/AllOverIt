using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Serialization.Json.Abstractions.Extensions;

namespace AllOverIt.Serialization.Json.Abstractions
{
    // Note: Tested indirectly via JsonHelper (SystemTextJson and NewtonsoftJson)

    /// <summary>Provides functionality to extract values from a JSON string or anonymous object.</summary>
    public abstract class JsonHelperBase
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ElementDictionary _element;

        /// <summary>Constructor.</summary>
        /// <param name="value">An object, typically anonymous, that is to be queried for property values.</param>
        /// <param name="jsonSerializer">The serializer to use for processing.</param>
        protected JsonHelperBase(object value, IJsonSerializer jsonSerializer)
            : this(jsonSerializer)
        {
            _ = value.WhenNotNull(nameof(value));

            _element = CreateElementDictionary(value);
        }

        /// <summary>Constructor.</summary>
        /// <param name="value">A JSON string that is to be queried for property values.</param>
        /// <param name="jsonSerializer">The serializer to use for processing.</param>
        protected JsonHelperBase(string value, IJsonSerializer jsonSerializer)
            : this(jsonSerializer)
        {
            _ = value.WhenNotNullOrEmpty(nameof(value));

            _element = CreateElementDictionary(value);
        }

        private JsonHelperBase(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer.WhenNotNull(nameof(jsonSerializer));
        }

        /// <summary>Tries to get the value of a property on the root element.</summary>
        /// <param name="propertyName">The property name to get the value of.</param>
        /// <param name="value">The value of the property, if the property exists.</param>
        /// <returns><see langword="True" /> if the property exists, otherwise <see langword="False" />.</returns>
        public bool TryGetValue(string propertyName, out object value)
        {
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            return _element.TryGetValue(propertyName, out value);
        }

        /// <summary>Gets the value of a specified property.</summary>
        /// <param name="propertyName">The property name to get the value of.</param>
        /// <returns>The value of a specified property.</returns>
        public object GetValue(string propertyName)
        {
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            return _element.GetValue(propertyName);
        }

        /// <summary>Tries to get the value of a specified property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="propertyName">The property name to get the value of.</param>
        /// <param name="value">The value of the property, if the property exists.</param>
        /// <returns><see langword="True" /> if the property exists, otherwise <see langword="False" />.</returns>
        public bool TryGetValue<TValue>(string propertyName, out TValue value)
        {
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            return _element.TryGetValue(propertyName, out value);
        }

        /// <summary>Gets the value of a specified property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="propertyName">The property name to get the value of.</param>
        /// <returns>The value of a specified property.</returns>
        public TValue GetValue<TValue>(string propertyName)
        {
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            return _element.GetValue<TValue>(propertyName);
        }

        /// <summary>Tries to get the value of a specified property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="propertyName">The property name to get the value of.</param>
        /// <param name="values">The values of the property, if the property exists.</param>
        /// <returns><see langword="True" /> if the property exists, otherwise <see langword="False" />.</returns>
        public bool TryGetValues<TValue>(string propertyName, out IReadOnlyCollection<TValue> values)
        {
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            return _element.TryGetValues(propertyName, out values);
        }

        /// <summary>Gets the value of a specified property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="propertyName">The property name to get the values of.</param>
        /// <returns>The value of a specified property.</returns>
        public IReadOnlyCollection<TValue> GetValues<TValue>(string propertyName)
        {
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            return _element.GetValues<TValue>(propertyName);
        }

        /// <summary>Tries to get an array of elements for a specified property.</summary>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <param name="array">The array of elements for the specified property.</param>
        /// <returns><see langword="True" /> if the property exists, otherwise <see langword="False" />.</returns>
        public bool TryGetObjectArray(string arrayPropertyName, out IEnumerable<IElementDictionary> array)
        {
            _ = arrayPropertyName.WhenNotNullOrEmpty(nameof(arrayPropertyName));

            return _element.TryGetObjectArray(arrayPropertyName, out array);
        }

        /// <summary>Gets an array of elements for a specified property.</summary>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <returns>The array of elements for the specified property.</returns>
        public IEnumerable<IElementDictionary> GetObjectArray(string arrayPropertyName)
        {
            _ = arrayPropertyName.WhenNotNullOrEmpty(nameof(arrayPropertyName));

            return _element.GetObjectArray(arrayPropertyName);
        }

        /// <summary>Tries to get the value of a property from each element of a specified array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <param name="propertyName">The property name to get the value of.</param>
        /// <param name="arrayValues">The value of each element in the specified array property.</param>
        /// <returns><see langword="True" /> if the array and property exists, otherwise <see langword="False" />.</returns>
        public bool TryGetObjectArrayValues<TValue>(string arrayPropertyName, string propertyName, out IEnumerable<TValue> arrayValues)
        {
            _ = arrayPropertyName.WhenNotNullOrEmpty(nameof(arrayPropertyName));
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            return _element.TryGetObjectArrayValues(arrayPropertyName, propertyName, out arrayValues);
        }

        /// <summary>Gets the value of a property from each element of a specified array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <param name="propertyName">The property name of the child element to get the value of.</param>
        /// <returns>The value of each element in the specified array property.</returns>
        public IEnumerable<TValue> GetObjectArrayValues<TValue>(string arrayPropertyName, string propertyName)
        {
            _ = arrayPropertyName.WhenNotNullOrEmpty(nameof(arrayPropertyName));
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            return _element.GetObjectArrayValues<TValue>(arrayPropertyName, propertyName);
        }

        /// <summary>Try to get a child array of elements for a specified property.</summary>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childArray">The deepest child array of elements.</param>
        /// <returns>A child array of elements for a specified property.</returns>
        public bool TryGetDescendantObjectArray(IEnumerable<string> arrayPropertyNames, out IEnumerable<IElementDictionary> childArray)
        {
            var allPropertyNames = arrayPropertyNames.WhenNotNullOrEmpty(nameof(arrayPropertyNames)).AsReadOnlyCollection();

            return _element.TryGetDescendantObjectArray(allPropertyNames, out childArray);
        }

        /// <summary>Get a child array of elements for a specified property.</summary>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <returns>The deepest child array of elements.</returns>
        public IEnumerable<IElementDictionary> GetDescendantObjectArray(IEnumerable<string> arrayPropertyNames)
        {
            var allPropertyNames = arrayPropertyNames.WhenNotNullOrEmpty(nameof(arrayPropertyNames)).AsReadOnlyCollection();

            return _element.GetDescendantObjectArray(allPropertyNames);
        }

        /// <summary>Tries to get the value of a property from each element of a specified child array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childPropertyName">The property name of the child element to get the value of.</param>
        /// <param name="childArrayValues">The value of each element in the specified child array property.</param>
        /// <returns><see langword="True" /> if the arrays and property exists, otherwise <see langword="False" />.</returns>
        public bool TryGetDescendantObjectArrayValues<TValue>(IEnumerable<string> arrayPropertyNames, string childPropertyName, out IEnumerable<TValue> childArrayValues)
        {
            var allPropertyNames = arrayPropertyNames.WhenNotNullOrEmpty(nameof(arrayPropertyNames)).AsReadOnlyCollection();
            _ = childPropertyName.WhenNotNullOrEmpty(nameof(childPropertyName));

            return _element.TryGetDescendantObjectArrayValues<TValue>(allPropertyNames, childPropertyName, out childArrayValues);
        }

        /// <summary>Get the value of a property from each element of a specified child array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childPropertyName">The property name of the child element to get the value of.</param>
        /// <returns>The value of each element in the specified child array property.</returns>
        public IEnumerable<TValue> GetDescendantObjectArrayValues<TValue>(IEnumerable<string> arrayPropertyNames, string childPropertyName)
        {
            var allPropertyNames = arrayPropertyNames.WhenNotNullOrEmpty(nameof(arrayPropertyNames)).AsReadOnlyCollection();
            _ = childPropertyName.WhenNotNullOrEmpty(nameof(childPropertyName));

            return _element.GetDescendantObjectArrayValues<TValue>(allPropertyNames, childPropertyName);
        }

        private ElementDictionary CreateElementDictionary(object value)
        {
            var serialized = _jsonSerializer.SerializeObject(value);

            return CreateElementDictionary(serialized);
        }

        private ElementDictionary CreateElementDictionary(string value)
        {
            var dictionary = _jsonSerializer.DeserializeObject<Dictionary<string, object>>(value);

            return new ElementDictionary(dictionary);
        }
    }
}
