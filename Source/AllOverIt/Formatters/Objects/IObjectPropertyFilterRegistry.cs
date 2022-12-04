using System;

namespace AllOverIt.Formatters.Objects
{
    /// <summary>Represents a registry of <see cref="ObjectPropertyFilter"/> types that can later be retrieved to filter
    /// the properties of a given object during its serialization via an <see cref="IObjectPropertySerializer"/> instance.</summary>
    public interface IObjectPropertyFilterRegistry
    {
        /// <summary>Registers an object type to a filter type that will be used by an <see cref="IObjectPropertySerializer"/> to
        /// process the properties of an object instance.</summary>
        /// <typeparam name="TType">The object type to register against a filter type.</typeparam>
        /// <typeparam name="TFilter">The filter type.</typeparam>
        /// <param name="serializerOptions">Options to be used by each constructed serializer. If no options are provided then
        /// a default <see cref="ObjectPropertySerializerOptions"/> instance will be used.</param>
        /// <remarks>This method should not be used if multiple threads construct the same filter type because each filter instance
        /// will be assigned to the same <see cref="ObjectPropertySerializerOptions.Filter"/> property.</remarks>
        public void Register<TType, TFilter>(ObjectPropertySerializerOptions serializerOptions = default)
            where TFilter : ObjectPropertyFilter, new();

        /// <summary>Registers an object type to a filter type that will be used by an <see cref="IObjectPropertySerializer"/> to
        /// process the properties of an object instance.</summary>
        /// <typeparam name="TType">The object type to register against a filter type.</typeparam>
        /// <typeparam name="TFilter">The filter type.</typeparam>
        /// <param name="serializerOptions">Provides the ability to configure the serializer options when the filter is created. The
        /// options must not include a Filter as the registered filter will be used.</param>
        /// <remarks>This method is suitable for cases where a new filter instance is created across different threads at the same time.
        /// This methods constructs, and configures, and new <see cref="ObjectPropertySerializerOptions"/> instance each time a new
        /// filter is created.</remarks>
        public void Register<TType, TFilter>(Action<ObjectPropertySerializerOptions> serializerOptions)
            where TFilter : ObjectPropertyFilter, new();

        /// <summary>Finds a registered filter that can process the properties of the provided object and returns a suitably configured serializer.</summary>
        /// <param name="object">The object to find an <see cref="ObjectPropertyFilter"/> that can process its properties.</param>
        /// <param name="serializer">If the registry has a registered filter that can process the object's properties then a new serializer configured with
        /// the filter is returned. If there is no suitable filter then a default serializer is returned.</param>
        /// <returns>True if a registered filter is found, otherwise False. If there is no registered serializer then <paramref name="serializer"/>
        /// will be assigned a default serializer.</returns>
        /// <remarks>Serializers and their associated filter are constructed on demand, and they are each treated as a Singleton.</remarks>
        /// 
        bool GetObjectPropertySerializer(object @object, out IObjectPropertySerializer serializer);
        /// <summary>Finds a registered filter that can process the properties of the provided object type and returns a suitably configured serializer.</summary>
        /// <typeparam name="TType">The object type to find an <see cref="ObjectPropertyFilter"/> that can process its properties.</typeparam>
        /// <param name="serializer">If the registry has a registered filter that can process the object's properties then a new serializer configured with
        /// the filter is returned. If there is no suitable filter then a default serializer is returned.</param>
        /// <returns>True if a registered filter is found, otherwise False. If there is no registered serializer then <paramref name="serializer"/>
        /// will be assigned a default serializer.</returns>
        /// <remarks>Serializers and their associated filter are constructed on demand, and they are each treated as a Singleton.</remarks>
        bool GetObjectPropertySerializer<TType>(out IObjectPropertySerializer serializer);

        /// <summary>Finds a registered filter that can process the properties of the provided object type and returns a suitably configured serializer.</summary>
        /// <param name="type">The object type to find an <see cref="ObjectPropertyFilter"/> that can process its properties.</param>
        /// <param name="serializer">If the registry has a registered filter that can process the object's properties then a new serializer configured with
        /// the filter is returned. If there is no suitable filter then a default serializer is returned.</param>
        /// <returns>True if a registered filter is found, otherwise False. If there is no registered serializer then <paramref name="serializer"/>
        /// will be assigned a default serializer.</returns>
        /// <remarks>Serializers and their associated filter are constructed on demand, and they are each treated as a Singleton.</remarks>
        bool GetObjectPropertySerializer(Type type, out IObjectPropertySerializer serializer);
    }
}