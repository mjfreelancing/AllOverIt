﻿namespace AllOverIt.Serialization.Json.Abstractions
{
    /// <summary>Represents a JSON object or array element.</summary>
    public interface IElementDictionary
    {
        /// <summary>Try to get the value of a specified property.</summary>
        /// <param name="propertyName">The property to get the value from.</param>
        /// <param name="value">The property value, as an <see cref="object"/>.</param>
        /// <returns><see langword="true" /> if the property exists, otherwise <see langword="false" />.</returns>
        bool TryGetValue(string propertyName, out object value);

        /// <summary>Get the value of a specified property.</summary>
        /// <param name="propertyName">The property to get the value from.</param>
        /// <returns>The property value, as an <see cref="object"/>.</returns>
        object GetValue(string propertyName);
    }
}