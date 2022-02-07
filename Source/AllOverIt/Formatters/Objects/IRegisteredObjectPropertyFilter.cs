namespace AllOverIt.Formatters.Objects
{
    /// <summary>Represents an object property filter that can be registered with an <see cref="IObjectPropertyFilterRegistry"/> instance.</summary>
    public interface IRegisteredObjectPropertyFilter
    {
        /// <summary>Determines if the filter can process the properties of the provided object (normally based on its type).</summary>
        /// <param name="object">The object to be tested if it can be processed by the current filter.</param>
        /// <returns>True if the filter can process the properties of the provided object (normally based on its type), otherwise false.</returns>
        /// <remarks>This method is called by <see cref="IObjectPropertyFilterRegistry.GetObjectPropertySerializer"/> to find a suitable
        /// filter for a given object.</remarks>
        bool CanFilter(object @object);
    }
}