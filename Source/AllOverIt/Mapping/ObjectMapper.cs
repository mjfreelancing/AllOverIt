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
            private readonly ObjectMapperOptions _mapperOptions;
            private readonly IReadOnlyCollection<PropertyInfo> _matches;

            internal MatchingPropertyMapper(Type sourceType, Type targetType, ObjectMapperOptions mapperOptions)
            {
                _sourceType = sourceType;
                _targetType = targetType;
                _mapperOptions = mapperOptions.WhenNotNull(nameof(mapperOptions));
                _matches = ObjectMapperHelper.GetMappableProperties(_sourceType, _targetType, _mapperOptions.Binding, mapperOptions.Aliases);
            }

            internal void MapPropertyValues(object source, object target, ObjectMapperOptions mapperOptionsOverride)
            {
                ObjectMapperHelper.MapPropertyValues(_sourceType, source, _targetType, target, _matches, mapperOptionsOverride ?? _mapperOptions);
            }
        }

        // Not thread safe - if to be used across multiple threads then configure the mappings in advance
        private readonly IDictionary<(Type, Type, BindingOptions), MatchingPropertyMapper> _mapperCache = new Dictionary<(Type, Type, BindingOptions), MatchingPropertyMapper>();

        /// <summary>Provides options that control how source properties are copied onto a target instance.</summary>
        public ObjectMapperOptions DefaultOptions { get; } = new();

        /// <inheritdoc />
        public void Configure<TSource, TTarget>(Action<TypedObjectMapperOptions<TSource, TTarget>> configure = default)
            where TSource : class
            where TTarget : class
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            var mapperOptions = GetConfiguredOptions(configure);

            _ = GetMapper(sourceType, targetType, mapperOptions);
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
        public TTarget Map<TSource, TTarget>(TSource source, TTarget target, Action<TypedObjectMapperOptions<TSource, TTarget>> configure = default)
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
            var mapper = GetMapper(sourceType, targetType, mapperOptions);

            var optionsOverride = configure != null
                ? mapperOptions
                : null;

            mapper.MapPropertyValues(source, target, optionsOverride);

            return target;
        }

        private TTarget MapSourceToTarget<TSource, TTarget>(Type sourceType, object source, Type targetType, TTarget target, Action<TypedObjectMapperOptions<TSource, TTarget>> configure)
            where TSource : class
            where TTarget : class
        {
            _ = source.WhenNotNull(nameof(source));
            _ = target.WhenNotNull(nameof(source));

            var mapperOptions = GetConfiguredOptions(configure);
            var mapper = GetMapper(sourceType, targetType, mapperOptions);

            var optionsOverride = configure != null
                ? mapperOptions
                : null;

            mapper.MapPropertyValues(source, target, optionsOverride);

            return target;
        }

        // mapperOptions will be null if not used as an override
        //private TTarget MapSourceToTarget<TTarget>(Type sourceType, object source, Type targetType, TTarget target, ObjectMapperOptions optionsOverride)
        //    where TTarget : class
        //{
        //    var mapper = GetMapper(sourceType, targetType, mapperOptions);

        //    //var optionsOverride = configure != null
        //    //    ? mapperOptions
        //    //    : null;

        //    mapper.MapPropertyValues(source, target, optionsOverride);

        //    return target;
        //}

        private MatchingPropertyMapper GetMapper(Type sourceType, Type targetType, ObjectMapperOptions mapperOptions)
        {
            _ = mapperOptions.WhenNotNull(nameof(mapperOptions));

            var mappingKey = (sourceType, targetType, mapperOptions.Binding);

            if (!_mapperCache.TryGetValue(mappingKey, out var mapper))
            {
                mapper = new MatchingPropertyMapper(sourceType, targetType, mapperOptions);
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

        private ObjectMapperOptions GetConfiguredOptions<TSource, TTarget>(Action<TypedObjectMapperOptions<TSource, TTarget>> configure)
            where TSource : class
            where TTarget : class
        {
            if (configure == null)
            {
                return DefaultOptions;
            }

            var mapperOptions = new TypedObjectMapperOptions<TSource, TTarget>();
            configure.Invoke(mapperOptions);

            return mapperOptions;
        }
    }
}