using AllOverIt.Assertion;
using System;
using System.Collections.Generic;

namespace AllOverIt.Formatters.Objects
{
    /// <inheritdoc cref="IObjectPropertyFilterRegistry" />
    public sealed class ObjectPropertyFilterRegistry : IObjectPropertyFilterRegistry
    {
        private static readonly IObjectPropertySerializer DefaultSerializer = new ObjectPropertySerializer();

        // A filter is created for each request due to the serializer managing state.
        private readonly IDictionary<Type, Func<IObjectPropertySerializer>> _filterRegistry = new Dictionary<Type, Func<IObjectPropertySerializer>>();

        /// <inheritdoc />
        public void Register<TType, TFilter>(ObjectPropertySerializerOptions serializerOptions = default)
            where TFilter : ObjectPropertyFilter, new()
        {
            var options = serializerOptions ?? new ObjectPropertySerializerOptions();

            _filterRegistry.Add(typeof(TType), () =>
            {
                return CreateObjectPropertySerializer<TFilter>(options);
            });
        }

        /// <inheritdoc />
        public void Register<TType, TFilter>(Action<ObjectPropertySerializerOptions> serializerOptions)
            where TFilter : ObjectPropertyFilter, new()
        {
            var options = serializerOptions;

            _filterRegistry.Add(typeof(TType), () =>
            {
                return CreateObjectPropertySerializer<TFilter>(options);
            });
        }

        /// <inheritdoc />
        public bool GetObjectPropertySerializer(object @object, out IObjectPropertySerializer serializer)
        {
            _ = @object.WhenNotNull(nameof(@object));

            return GetObjectPropertySerializer(@object.GetType(), out serializer);
        }

        /// <inheritdoc />
        public bool GetObjectPropertySerializer<TType>(out IObjectPropertySerializer serializer)
        {
            return GetObjectPropertySerializer(typeof(TType), out serializer);
        }

        /// <inheritdoc />
        public bool GetObjectPropertySerializer(Type type, out IObjectPropertySerializer serializer)
        {
            _ = type.WhenNotNull(nameof(type));

            if (_filterRegistry.TryGetValue(type, out var serializerFactory))
            {
                serializer = serializerFactory.Invoke();
                return true;
            }

            serializer = DefaultSerializer;
            return false;
        }

        private static IObjectPropertySerializer CreateObjectPropertySerializer<TFilter>(ObjectPropertySerializerOptions serializerOptions)
            where TFilter : ObjectPropertyFilter, new()
        {
            _ = serializerOptions.WhenNotNull(nameof(serializerOptions));

            var filter = new TFilter();

            return new ObjectPropertySerializer(serializerOptions, filter);
        }

        private static IObjectPropertySerializer CreateObjectPropertySerializer<TFilter>(Action<ObjectPropertySerializerOptions> serializerOptions)
            where TFilter : ObjectPropertyFilter, new()
        {
            var options = new ObjectPropertySerializerOptions();
            serializerOptions.Invoke(options);

            return CreateObjectPropertySerializer<TFilter>(options);
        }
    }
}