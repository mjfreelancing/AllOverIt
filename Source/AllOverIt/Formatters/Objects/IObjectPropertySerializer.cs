namespace AllOverIt.Formatters.Objects
{
    /// <summary>A serializer that can convert an object to an IDictionary&lt;string, string&gt; using a dot notation for nested members.</summary>
    public interface IObjectPropertySerializer
    {
        /// <summary>Provides options that determine how serialization of properties and their values are handled.</summary>
        ObjectPropertySerializerOptions Options { get; }

        /// <summary>An optional filter that can be implemented to exclude properties by name or value. Values can be
        /// modified, or formatted if the filter implements <see cref="IFormattableObjectPropertyFilter"/>.</summary>
        ObjectPropertyFilter? Filter { get; }

        /// <summary>Serializes an object to an IDictionary&lt;string, string&gt;.</summary>
        /// <param name="instance">The object to be serialized.</param>
        /// <returns>A flat IDictionary&lt;string, string&gt; of all properties using a dot notation for nested members.</returns>
        IDictionary<string, string> SerializeToDictionary(object instance);
    }
}