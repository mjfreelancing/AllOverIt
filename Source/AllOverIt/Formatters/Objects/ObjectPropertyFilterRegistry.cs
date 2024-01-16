﻿using AllOverIt.Assertion;
using System;
using System.Collections.Generic;

namespace AllOverIt.Formatters.Objects
{
    /// <inheritdoc cref="IObjectPropertyFilterRegistry" />
    public sealed class ObjectPropertyFilterRegistry : IObjectPropertyFilterRegistry
    {
        private readonly Lazy<IObjectPropertySerializer> _defaultSerializer;

        // A filter is created for each request due to the serializer managing state.
        private readonly Dictionary<Type, Func<IObjectPropertySerializer>> _filterRegistry = [];

        /// <summary>Constructor.</summary>
        /// <param name="defaultSerializerFactory">An optional factory method to construct an <see cref="IObjectPropertySerializer"/> instance.</param>
        public ObjectPropertyFilterRegistry(Func<IObjectPropertySerializer> defaultSerializerFactory = default)
        {
            var serializerFactory = defaultSerializerFactory is not null
                ? defaultSerializerFactory
                : () => new ObjectPropertySerializer();

            _defaultSerializer = new Lazy<IObjectPropertySerializer>(serializerFactory);
        }

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

            serializer = _defaultSerializer.Value;

            return false;
        }

        private static ObjectPropertySerializer CreateObjectPropertySerializer<TFilter>(ObjectPropertySerializerOptions serializerOptions)
            where TFilter : ObjectPropertyFilter, new()
        {
            _ = serializerOptions.WhenNotNull(nameof(serializerOptions));

            var filter = new TFilter();

            return new ObjectPropertySerializer(serializerOptions, filter);
        }

        private static ObjectPropertySerializer CreateObjectPropertySerializer<TFilter>(Action<ObjectPropertySerializerOptions> serializerOptions)
            where TFilter : ObjectPropertyFilter, new()
        {
            var options = new ObjectPropertySerializerOptions();
            serializerOptions.Invoke(options);

            return CreateObjectPropertySerializer<TFilter>(options);
        }
    }
}