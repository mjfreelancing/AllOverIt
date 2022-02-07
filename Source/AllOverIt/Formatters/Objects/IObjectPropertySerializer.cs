using System.Collections.Generic;

namespace AllOverIt.Formatters.Objects
{
    /// <summary>Represents a serializer that can convert an object to an IDictionary&lt;string, string&gt; using a dot notation for nested members.</summary>
    public interface IObjectPropertySerializer
    {
        /// <summary>Provides options that determine how serialization of properties and their values are handled.</summary>
        ObjectPropertySerializerOptions Options { get; }

        /// <summary>Serializes an object to an IDictionary&lt;string, string&gt;.</summary>
        /// <param name="instance">The object to be serialized.</param>
        /// <returns>A flat IDictionary&lt;string, string&gt; of all properties using a dot notation for nested members.</returns>
        IDictionary<string, string> SerializeToDictionary(object instance);
    }
}