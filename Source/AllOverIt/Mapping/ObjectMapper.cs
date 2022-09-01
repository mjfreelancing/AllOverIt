using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Mapping.Exceptions;
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
        internal class MatchingPropertyMapper
        {
            internal sealed class PropertyMatchInfo
            {
                public PropertyInfo SourceInfo { get; }
                public PropertyInfo TargetInfo { get; }
                public Func<object, object> SourceGetter { get; }
                public Action<object, object> TargetSetter { get; }

                public PropertyMatchInfo(PropertyInfo sourceInfo, PropertyInfo targetInfo, Func<Type, Type, MatchingPropertyMapper> mapperResolver,
                    Func<Type, object> typeCreator)
                {
                    SourceInfo = sourceInfo;
                    TargetInfo = targetInfo;

                    SourceGetter = PropertyHelper.CreatePropertyGetter(SourceInfo);
                    TargetSetter = PropertyHelper.CreatePropertySetter(TargetInfo);

                    // Look to see if nested object mapping needs to be handled - will only get here if the property has not been excluded
                    if (!SourceInfo.PropertyType.IsValueType && SourceInfo.PropertyType != CommonTypes.StringType)
                    {
                        if (SourceInfo.PropertyType.IsEnumerableType())
                        {
                            // TODO: Currently only supporting shallow copy of things like IEnumerable<T> to IEnumerable<T>, unless a
                            //       converter is configured. To be updated to look at the enumerable types and process their mapping.
                            return;
                        }

                        TargetSetter = MapNestedObject(mapperResolver, typeCreator);
                    }
                }

                private Action<object, object> MapNestedObject(Func<Type, Type, MatchingPropertyMapper> mapperResolver, Func<Type, object> typeCreator)
                {
                    // Get (or create) a suitable mapper for this property
                    var propertyMapper = mapperResolver.Invoke(SourceInfo.PropertyType, TargetInfo.PropertyType);

                    Throw<ObjectMapperException>.WhenNull(propertyMapper, $"No mapper found for type '{SourceInfo.PropertyType.GetFriendlyName()}'.");

                    var parentTargetSetter = TargetSetter;

                    // Replace the target setter so it handles the mapping of the nested object properties
                    return (parentTarget, parentPropertyValue) =>
                    {
                        // Create the target type
                        var childProperty = typeCreator.Invoke(TargetInfo.PropertyType);

                        // Map the child properties
                        propertyMapper.MapPropertyValues(parentPropertyValue, childProperty);

                        // Assign the child property to the parent target
                        parentTargetSetter.Invoke(parentTarget, childProperty);
                    };
                }
            }

            internal readonly ObjectMapperOptions MapperOptions;
            internal readonly PropertyMatchInfo[] Matches;

            // 'mapperResolver' is used to get property mapping for nested (object) properties
            internal MatchingPropertyMapper(Type sourceType, Type targetType, ObjectMapperOptions mapperOptions,
                Func<Type, Type, MatchingPropertyMapper> mapperResolver, Func<Type, object> typeCreator)
            {
                MapperOptions = mapperOptions.WhenNotNull(nameof(mapperOptions));
                _ = mapperResolver.WhenNotNull(nameof(mapperResolver));

                // Find properties that match between the source and target (or have an alias) and meet any filter criteria.
                var matches = ObjectMapperHelper.GetMappableProperties(sourceType, targetType, mapperOptions);

                var sourcePropertyInfo = ReflectionCache
                    .GetPropertyInfo(sourceType, mapperOptions.Binding)
                    .ToDictionary(prop => prop.Name);

                var targetPropertyInfo = ReflectionCache
                    .GetPropertyInfo(targetType, mapperOptions.Binding)
                    .ToDictionary(prop => prop.Name);

                var matchedProps = new List<PropertyMatchInfo>(matches.Count);

                foreach (var match in matches)
                {
                    var sourcePropInfo = sourcePropertyInfo[match.Name];
                    var targetName = ObjectMapperHelper.GetTargetAliasName(match.Name, mapperOptions);
                    var targetPropInfo = targetPropertyInfo[targetName];

                    var matchInfo = new PropertyMatchInfo(sourcePropInfo, targetPropInfo, mapperResolver, typeCreator);
                    matchedProps.Add(matchInfo);
                }

                Matches = matchedProps.ToArray();
            }

            internal void MapPropertyValues(object source, object target)
            {
                foreach (var match in Matches)
                {
                    var value = match.SourceGetter.Invoke(source);
                    var targetValue = MapperOptions.GetConvertedValue(match.SourceInfo.Name, value);

                    match.TargetSetter.Invoke(target, targetValue);
                }
            }
        }

        private readonly IDictionary<Type, Func<object>> _TypeCreators = new Dictionary<Type, Func<object>>();
        private readonly IDictionary<(Type, Type), MatchingPropertyMapper> _mapperCache = new Dictionary<(Type, Type), MatchingPropertyMapper>();

        /// <summary>Defines the default mapper options to apply when explicit options are not setup at the time of mapping configuration.</summary>
        public ObjectMapperOptions DefaultOptions { get; } = new();

        /// <inheritdoc />
        public void Configure<TSource, TTarget>(Action<TypedObjectMapperOptions<TSource, TTarget>> configure = default)
            where TSource : class
            where TTarget : class
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            var mapperOptions = GetConfiguredOptionsOrDefault(configure);

            _ = CreateMapper(sourceType, targetType, mapperOptions);
        }

        /// <inheritdoc />
        /// <remarks>If mapping configuration is not performed in advance then default configuration will be applied. The configuration
        /// cannot be changed later.</remarks>
        public TTarget Map<TTarget>(object source)
            where TTarget : class, new()
        {
            _ = source.WhenNotNull(nameof(source));

            var target = new TTarget();

            return MapSourceToTarget(source, target);
        }

        /// <inheritdoc />
        /// <remarks>If mapping configuration is not performed in advance then default configuration will be applied. The configuration
        /// cannot be changed later.</remarks>
        public TTarget Map<TSource, TTarget>(TSource source, TTarget target)
            where TSource : class
            where TTarget : class
        {
            _ = source.WhenNotNull(nameof(source));
            _ = target.WhenNotNull(nameof(target));

            return MapSourceToTarget(source, target);
        }

        private TTarget MapSourceToTarget<TTarget>(object source, TTarget target)
            where TTarget : class
        {
            var sourceType = source.GetType();
            var targetType = typeof(TTarget);

            var mapper = GetMapper(sourceType, targetType);
            mapper.MapPropertyValues(source, target);

            return target;
        }

        private MatchingPropertyMapper CreateMapper(Type sourceType, Type targetType, ObjectMapperOptions mapperOptions)
        {
            _ = mapperOptions.WhenNotNull(nameof(mapperOptions));

            var mappingKey = (sourceType, targetType);

            if (_mapperCache.TryGetValue(mappingKey, out _))
            {
                throw new ObjectMapperException($"Mapping already exists between {sourceType.GetFriendlyName()} and {targetType.GetFriendlyName()}.");
            }

            var mapper = new MatchingPropertyMapper(
                sourceType,
                targetType,
                mapperOptions,

                // Gets a source => target propery mapper
                (srcType, trgType) =>
                {
                    // If nested (object) properties need to be mapped, get the registered mapper
                    var nestedMappingKey = (srcType, trgType);

                    // If CreateMapper is called, this results in the _mapperCache being updated
                    return _mapperCache.TryGetValue(nestedMappingKey, out var mapper)
                        ? mapper
                        : CreateMapper(srcType, trgType, mapperOptions);
                },

                // Creates an instance of a given type used a factory cache
                type =>
                {
                    if (_TypeCreators.TryGetValue(type, out var creator))
                    {
                        return creator.Invoke();
                    }

                    creator = type.GetFactory();

                    _TypeCreators.Add(type, creator);

                    return creator.Invoke();
                });

            _mapperCache.Add(mappingKey, mapper);

            return mapper;
        }

        internal MatchingPropertyMapper GetMapper(Type sourceType, Type targetType)
        {
            var mappingKey = (sourceType, targetType);
            
            return _mapperCache.TryGetValue(mappingKey, out var mapper)
                ? mapper
                : CreateMapper(sourceType, targetType, DefaultOptions);
        }

        private ObjectMapperOptions GetConfiguredOptionsOrDefault<TSource, TTarget>(Action<TypedObjectMapperOptions<TSource, TTarget>> configure)
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