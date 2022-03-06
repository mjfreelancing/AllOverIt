using AllOverIt.Assertion;
using AllOverIt.Extensions;
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
            private readonly IReadOnlyCollection<(PropertyInfo, PropertyInfo)> _matches;

            internal MatchingPropertyMapper(Type sourceType, Type targetType, ObjectMapperOptions mapperOptions)
            {
                _ = mapperOptions.WhenNotNull(nameof(mapperOptions));
                
                var matches = ObjectMapperHelper.GetMappableProperties(sourceType, targetType, mapperOptions.Binding, mapperOptions.Aliases);

                if (mapperOptions.Filter != null)
                {
                    matches = matches.Where(mapperOptions.Filter).AsReadOnlyCollection();
                }

                var matchedProps = new List<(PropertyInfo, PropertyInfo)>();

                foreach (var match in matches)
                {
                    var sourcePropInfo = sourceType
                        .GetPropertyInfo(mapperOptions.Binding, false)
                        .SingleOrDefault(item => item.Name == match.Name);

                    var targetName = ObjectMapperHelper.GetTargetAliasName(match.Name, mapperOptions.Aliases);

                    var targetPropInfo = targetType
                        .GetPropertyInfo(mapperOptions.Binding, false)
                        .SingleOrDefault(item => item.Name == targetName);

                    matchedProps.Add((sourcePropInfo, targetPropInfo));
                }

                _matches = matchedProps;
            }

            internal void MapPropertyValues(object source, object target)
            {
                foreach (var (sourcePropInfo, targetPropInfo) in _matches)
                {
                    var value = sourcePropInfo.GetValue(source);
                    targetPropInfo.SetValue(target, value);
                }
            }
        }

        // Not thread safe - if to be used across multiple threads then configure the mappings in advance
        private readonly IDictionary<(Type, Type), MatchingPropertyMapper> _mapperCache = new Dictionary<(Type, Type), MatchingPropertyMapper>();

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

            CreateMapper(sourceType, targetType, mapperOptions);
        }

        /// <inheritdoc />
        public TTarget Map<TTarget>(object source)
            where TTarget : class, new()
        {
            _ = source.WhenNotNull(nameof(source));

            var sourceType = source.GetType();
            var targetType = typeof(TTarget);
            var target = new TTarget();

            return MapSourceToTarget(sourceType, source, targetType, target);
        }

        /// <inheritdoc />
        public TTarget Map<TSource, TTarget>(TSource source, TTarget target)
            where TSource : class
            where TTarget : class
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            return MapSourceToTarget(sourceType, source, targetType, target);
        }

        private TTarget MapSourceToTarget<TTarget>(Type sourceType, object source, Type targetType, TTarget target)
            where TTarget : class
        {
            _ = source.WhenNotNull(nameof(source));
            _ = target.WhenNotNull(nameof(source));

            var mapper = GetMapper(sourceType, targetType);
            mapper.MapPropertyValues(source, target);

            return target;
        }

        private void CreateMapper(Type sourceType, Type targetType, ObjectMapperOptions mapperOptions)
        {
            _ = mapperOptions.WhenNotNull(nameof(mapperOptions));

            var mappingKey = (sourceType, targetType);

            if (_mapperCache.TryGetValue(mappingKey, out var mapper))
            {
                throw new Exception($"Mapping already exists between {sourceType.GetFriendlyName()} and {targetType.GetFriendlyName()}");    // TODO: Needs a custom exception
            }

            mapper = new MatchingPropertyMapper(sourceType, targetType, mapperOptions);
            _mapperCache.Add(mappingKey, mapper);
        }

        private MatchingPropertyMapper GetMapper(Type sourceType, Type targetType)
        {
            var mappingKey = (sourceType, targetType);

            if (!_mapperCache.TryGetValue(mappingKey, out var mapper))
            {
                throw new Exception($"Mapping not defined for {sourceType.GetFriendlyName()} and {targetType.GetFriendlyName()}");    // TODO: Needs a custom exception
            }

            return mapper;
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