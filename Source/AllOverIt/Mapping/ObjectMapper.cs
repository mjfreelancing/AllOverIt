using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AllOverIt.Mapping
{
    /// <summary>Implements an object mapper that will copy property values from a source onto a target.</summary>
    public sealed class ObjectMapper : IObjectMapper
    {
        private class MatchingPropertyMapper
        {
            private readonly Type _sourceType;
            private readonly Type _targetType;
            private readonly BindingOptions _bindingOptions;
            private readonly IReadOnlyCollection<PropertyInfo> _matches;

            internal MatchingPropertyMapper(Type sourceType, Type targetType, BindingOptions options)
            {
                _sourceType = sourceType;
                _targetType = targetType;
                _bindingOptions = options;
                _matches = ObjectMapperHelper.GetMappableProperties(_sourceType, _targetType, options);
            }

            internal void MapPropertyValues(object source, object target, ObjectMapperOptions options)
            {
                // see if any properties need filtering out
                var matches = options?.Filter != null
                    ? _matches.Where(options.Filter).AsReadOnlyCollection()
                    : _matches;

                foreach (var match in matches)
                {
                    var value = source.GetPropertyValue(_sourceType, match.Name, _bindingOptions);
                    target.SetPropertyValue(_targetType, match.Name, value, _bindingOptions);
                }
            }
        }

        // Not thread safe - if to be used across multiple threads then configure the mappings in advance
        private readonly IDictionary<(Type, Type, BindingOptions), MatchingPropertyMapper> _mapperCache = new Dictionary<(Type, Type, BindingOptions), MatchingPropertyMapper>();

        /// <summary>Provides options that control how source properties are copied onto a target instance.</summary>
        public ObjectMapperOptions DefaultOptions { get; } = new();

        /// <inheritdoc />
        public void Configure<TSource, TTarget>(Action<ObjectMapperOptions> configure)
            where TSource : class
            where TTarget : class
        {
            _ = configure.WhenNotNull(nameof(configure));

            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            var mapperOptions = GetConfiguredOptions(configure);

            _ = TryCacheMapper(sourceType, targetType, mapperOptions);
        }

        /// <inheritdoc />
        public TTarget Map<TTarget>(object source, Action<ObjectMapperOptions> configure = default)
            where TTarget : class, new()
        {
            _ = source.WhenNotNull(nameof(source));

            var sourceType = source.GetType();
            var targetType = typeof(TTarget);
            var target = new TTarget();

            return MapSourceToTarget(sourceType, source, targetType, target, configure);
        }

        /// <inheritdoc />
        public TTarget Map<TSource, TTarget>(TSource source, TTarget target, Action<ObjectMapperOptions> configure = default)
            where TSource : class
            where TTarget : class
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            return MapSourceToTarget(sourceType, source, targetType, target, configure);
        }

        private TTarget MapSourceToTarget<TTarget>(Type sourceType, object source, Type targetType, TTarget target, Action<ObjectMapperOptions> configure)
            where TTarget : class
        {
            _ = source.WhenNotNull(nameof(source));
            _ = target.WhenNotNull(nameof(source));

            var mapperOptions = GetConfiguredOptions(configure);
            var mapper = TryCacheMapper(sourceType, targetType, mapperOptions);
            mapper.MapPropertyValues(source, target, mapperOptions);

            return target;
        }

        private MatchingPropertyMapper TryCacheMapper(Type sourceType, Type targetType, ObjectMapperOptions mapperOptions)
        {
            _ = mapperOptions.WhenNotNull(nameof(mapperOptions));

            var mappingKey = (sourceType, targetType, mapperOptions.Binding);

            if (!_mapperCache.TryGetValue(mappingKey, out var mapper))
            {
                mapper = new MatchingPropertyMapper(sourceType, targetType, mapperOptions.Binding);
                _mapperCache.Add(mappingKey, mapper);
            }

            return mapper;
        }

        private ObjectMapperOptions GetConfiguredOptions(Action<ObjectMapperOptions> configure)
        {
            if (configure == null)
            {
                return DefaultOptions;
            }

            var mapperOptions = new ObjectMapperOptions();
            configure.Invoke(mapperOptions);

            return mapperOptions;
        }
    }
}