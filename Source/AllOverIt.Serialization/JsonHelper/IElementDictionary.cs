using System;

namespace AllOverIt.Serialization.JsonHelper
{
    /// <summary>Represents a JSON object or array element.</summary>
    public interface IElementDictionary
    {
        /// <summary>Try to get the value of a specified property.</summary>
        /// <param name="propertyName">The property to get the value from.</param>
        /// <param name="value">The property value, as an <see cref="Object"/>.</param>
        /// <returns>True if the property exists, otherwise false.</returns>
        bool TryGetValue(string propertyName, out object value);

        /// <summary>Get the value of a specified property.</summary>
        /// <param name="propertyName">The property to get the value from.</param>
        /// <returns>The property value, as an <see cref="Object"/>.</returns>
        object GetValue(string propertyName);
    }
}
