using System;

namespace AllOverIt.Formatters.Objects
{
    /// <summary>Represents a registry of <see cref="ObjectPropertyFilter"/> types that can later be retrieved to filter
    /// the properties of a given object during its serialization via an <see cref="IObjectPropertySerializer"/> instance.</summary>
    public interface IObjectPropertyFilterRegistry
    {
        /// <summary>Registers a filter than can be later associated with an <see cref="IObjectPropertySerializer"/> to process the
        /// properties of a given object. The filter's <see cref="IRegisteredObjectPropertyFilter.CanFilter"/> method must return True
        /// for it to be used. If multiple filters can potentially process a given object type, then the first registered filter will
        /// be used.</summary>
        /// <typeparam name="TFilter">The filter type.</typeparam>
        /// <param name="serializerOptions">Options to be used by each constructed serializer (one per filter type). If no options are
        /// provided then a default <see cref="ObjectPropertySerializerOptions"/> instance will be used.</param>
        /// <remarks>
        ///   <para>The <paramref name="serializerOptions"/> must not include a filter.</para>
        ///   <para>The serializer options cannot be shared across filters since the created filter is assigned to the options when
        ///         used. If you need to guarantee each filter is assigned to a unique options instance then use the
        ///         <see cref="Register{TFilter}(Action{ObjectPropertySerializerOptions})"/> overload.</para>
        /// </remarks>
        void Register<TFilter>(ObjectPropertySerializerOptions serializerOptions = null)
            where TFilter : ObjectPropertyFilter, IRegisteredObjectPropertyFilter, new();

        /// <summary>Registers a filter than can be later associated with an <see cref="IObjectPropertySerializer"/> to process the
        /// properties of a given object. The filter's <see cref="IRegisteredObjectPropertyFilter.CanFilter"/> method must return True
        /// for it to be used. If multiple filters can potentially process a given object type, then the first registered filter will
        /// be used.</summary>
        /// <typeparam name="TFilter">The filter type.</typeparam>
        /// <param name="serializerOptions">Provides the ability to configure the serializer options when the filter is created. The
        /// options must not include a Filter as the registered filter will be used.</param>
        /// <remarks>Use this overload when you must guarantee that a unique instance of the options is used per filter, such as
        /// in a multi-threaded scenario.</remarks>
        void Register<TFilter>(Action<ObjectPropertySerializerOptions> serializerOptions)
            where TFilter : ObjectPropertyFilter, IRegisteredObjectPropertyFilter, new();

        /// <summary>Finds a registered filter that can process the properties of the provided object and returns a suitably configured serializer.</summary>
        /// <param name="object">The object to find an <see cref="ObjectPropertyFilter"/> that can process its properties.</param>
        /// <param name="serializer">If the registry has a registered filter that can process the object's properties then a new serializer configured with
        /// the filter is returned. If there is no suitable filter then a default serializer is returned.</param>
        /// <returns>True if a registered filter is found, otherwise False.</returns>
        /// <remarks>Serializers and their associated filter are constructed on demand, and they are each treated as a Singleton.</remarks>
        bool GetObjectPropertySerializer(object @object, out IObjectPropertySerializer serializer);
    }
}