namespace AllOverIt.Serialization.Json.Abstractions
{
    /// <summary>Represents a JSON object or array element. This type is enumerable.</summary>
    public interface IElementDictionary : IEnumerable<KeyValuePair<string, object>>
    {
        /// <summary>Gets the property names for this JSON node.</summary>
        public IEnumerable<string> Names { get; }

        /// <summary>Gets the property values for this JSON node.</summary>
        public IEnumerable<object> Values { get; }

        /// <summary>Try to get the value of a specified property.</summary>
        /// <param name="propertyName">The property to get the value from.</param>
        /// <param name="value">The current property value.</param>
        /// <returns><see langword="True" /> if the property exists, otherwise <see langword="False" />.</returns>
        bool TryGetValue(string propertyName, out object value);

        /// <summary>Get the value of a specified property.</summary>
        /// <param name="propertyName">The property to get the value from.</param>
        /// <returns>The current property value.</returns>
        object GetValue(string propertyName);

        /// <summary>Try to set the value of a specified property.</summary>
        /// <param name="propertyName">The property to set the value for.</param>
        /// <param name="value">The new property value.</param>
        /// <returns><see langword="True"/> if the value was updated, otherwise <see langword="False"/>.</returns>
        bool TrySetValue(string propertyName, object value);

        /// <summary>Set the value of a specified property.</summary>
        /// <param name="propertyName">The property to set the value for.</param>
        /// <param name="value">The new property value.</param>
        void SetValue(string propertyName, object value);
    }
}
