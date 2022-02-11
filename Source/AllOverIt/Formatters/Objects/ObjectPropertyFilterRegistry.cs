using System;
using System.Collections.Generic;
using AllOverIt.Assertion;

namespace AllOverIt.Formatters.Objects
{
    /// <summary>Provides a registry of <see cref="ObjectPropertyFilter"/> types that can later be retrieved to filter
    /// the properties of a given object during its serialization via an <see cref="IObjectPropertySerializer"/> instance.</summary>
    public sealed class ObjectPropertyFilterRegistry : IObjectPropertyFilterRegistry
    {
        private sealed record FilterSerializer
        {
            public ObjectPropertyFilter Filter { get; init; }
            public Lazy<IObjectPropertySerializer> Serializer { get; init; }
        }

        private static readonly IObjectPropertySerializer DefaultSerializer = new ObjectPropertySerializer();

        // The filters are only created once and re-used across all requests.
        private readonly IList<FilterSerializer> _filters = new List<FilterSerializer>();

        /// <inheritdoc />
        public void Register<TFilter>(ObjectPropertySerializerOptions serializerOptions = null)
            where TFilter : ObjectPropertyFilter, IRegisteredObjectPropertyFilter, new()
        {
            var filter = new TFilter();
            Register(filter, serializerOptions);
        }

        /// <inheritdoc />
        public void Register<TFilter>(TFilter filter, ObjectPropertySerializerOptions serializerOptions = null)
            where TFilter : ObjectPropertyFilter, IRegisteredObjectPropertyFilter
        {
            serializerOptions
                ?.Filter
                .CheckIsNull(nameof(serializerOptions.Filter), $"The {nameof(ObjectPropertyFilterRegistry)} expects the provided options to not include a filter.");

            var lazySerializer = new Lazy<IObjectPropertySerializer>(() =>
            {
                var options = serializerOptions ?? new ObjectPropertySerializerOptions();
                options.Filter = filter;

                return new ObjectPropertySerializer(options);
            });

            var filterSerializer = new FilterSerializer
            {
                Filter = filter,
                Serializer = lazySerializer
            };

            _filters.Add(filterSerializer);
        }

        /// <inheritdoc />
        public void Register<TFilter>(Action<ObjectPropertySerializerOptions> serializerOptions)
            where TFilter : ObjectPropertyFilter, IRegisteredObjectPropertyFilter, new()
        {
            var options = new ObjectPropertySerializerOptions();
            serializerOptions.Invoke(options);

            Register<TFilter>(options);
        }

        /// <inheritdoc />
        public void Register<TFilter>(TFilter filter, Action<ObjectPropertySerializerOptions> serializerOptions)
            where TFilter : ObjectPropertyFilter, IRegisteredObjectPropertyFilter
        {
            var options = new ObjectPropertySerializerOptions();
            serializerOptions.Invoke(options);

            Register(filter, options);
        }

        /// <inheritdoc />
        public bool GetObjectPropertySerializer(object @object, out IObjectPropertySerializer serializer)
        {
            foreach (var filterSerializer in _filters)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (filterSerializer.Filter is IRegisteredObjectPropertyFilter filterable &&
                    filterable.CanFilter(@object))
                {
                    serializer = filterSerializer.Serializer.Value;
                    return true;
                }
            }

            serializer = DefaultSerializer;
            return false;
        }
    }
}