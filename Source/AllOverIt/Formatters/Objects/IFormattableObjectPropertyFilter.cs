﻿namespace AllOverIt.Formatters.Objects
{
    /// <summary>Indicates the ability to format values serialized by an <see cref="IObjectPropertySerializer"/> instance.</summary>
    public interface IFormattableObjectPropertyFilter
    {
        /// <summary>Provides the ability to format a value generated by a <see cref="IObjectPropertySerializer"/> instance.</summary>
        /// <param name="value">The value that can be formatted.</param>
        /// <returns>The replacement, formatted, value to be serialized.</returns>
        string OnFormatValue(string value);
    }
}