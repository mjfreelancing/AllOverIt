using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Mapping.Exceptions;
using AllOverIt.Reflection;
using System;
using System.Collections;
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

                public PropertyMatchInfo(PropertyInfo sourceInfo, PropertyInfo targetInfo)
                {
                    SourceInfo = sourceInfo;
                    TargetInfo = targetInfo;

                    SourceGetter = PropertyHelper.CreatePropertyGetter(SourceInfo);
                    TargetSetter = PropertyHelper.CreatePropertySetter(TargetInfo);
                }
            }

            public ObjectMapperOptions MapperOptions { get; }
            public PropertyMatchInfo[] Matches { get; }

            // 'mapperResolver' is used to get property mapping for nested (object) properties
            internal MatchingPropertyMapper(Type sourceType, Type targetType, ObjectMapperOptions mapperOptions)
            {
                MapperOptions = mapperOptions.WhenNotNull(nameof(mapperOptions));

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

                    var matchInfo = new PropertyMatchInfo(sourcePropInfo, targetPropInfo);
                    matchedProps.Add(matchInfo);
                }

                Matches = matchedProps.ToArray();
            }
        }

        private readonly IDictionary<Type, Func<object>> _listCreators = new Dictionary<Type, Func<object>>();
        private readonly IDictionary<Type, Func<object>> _typeCreators = new Dictionary<Type, Func<object>>();
        private readonly IDictionary<(Type, Type), MatchingPropertyMapper> _mapperCache = new Dictionary<(Type, Type), MatchingPropertyMapper>();

        /// <summary>Defines the default mapper options to apply when explicit options are not setup at the time of mapping configuration.</summary>
        public ObjectMapperOptions DefaultOptions { get; }

        /// <summary>Constructor.</summary>
        public ObjectMapper()
        {
            DefaultOptions = new(this);
        }

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

            return (TTarget) MapSourceToTarget(source, target, false);
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

            return (TTarget) MapSourceToTarget(source, target, false);
        }

        private object MapSourceToTarget(object source, object target, bool isDeepClone)
        {
            var sourceType = source.GetType();
            var targetType = target.GetType();

            var propertyMapper = GetMapper(sourceType, targetType);

            foreach (var match in propertyMapper.Matches)
            {
                var value = match.SourceGetter.Invoke(source);
                var sourceValue = propertyMapper.MapperOptions.GetConvertedValue(match.SourceInfo.Name, value);

                var sourcePropertyType = match.SourceInfo.PropertyType;
                var targetPropertyType = match.TargetInfo.PropertyType;

                var doDeepClone = isDeepClone || propertyMapper.MapperOptions.IsClone(match.SourceInfo.Name);


                // If the target property type is different then a conversion or mapping may be required
                // Enumerable types need to be checked because the source / target types




                if (sourceValue is not null)
                {
                    var sourceValueType = sourceValue.GetType();

                    if (sourceValueType.IsValueType)
                    {
                        sourceValue = ConvertValueIfNotTargetType(sourceValue, sourceValueType, targetPropertyType);
                    }
                    else if (sourceValueType != CommonTypes.StringType)
                    {
                        var isAssignable = targetPropertyType.IsAssignableFrom(sourceValueType);

                        if (sourcePropertyType.IsEnumerableType())
                        {
                            // Expecting the target to also be an enumerable type
                            Throw<ObjectMapperException>.When(
                                !targetPropertyType.IsEnumerableType(),
                                $"Cannot map {sourcePropertyType.GetFriendlyName()} to {targetPropertyType.GetFriendlyName()}. Use a custom conversion.");

                            if (!isAssignable || doDeepClone)
                            {
                                var sourceElementType = sourcePropertyType.GetGenericArguments()[0];
                                var targetElementType = targetPropertyType.GetGenericArguments()[0];

                                var listType = CommonTypes.ListGenericType.MakeGenericType(new[] { targetElementType });

                                if (!_listCreators.TryGetValue(listType, out var listFactory))
                                {
                                    listFactory = listType.GetFactory();
                                    
                                    _listCreators.Add(listType, listFactory);
                                }

                                var typedList = (IList) listFactory.Invoke();

                                var sourceElements = GetSourceElements(sourceValue);

                                foreach (var sourceElement in sourceElements)
                                {
                                    var currentElement = sourceElement;

                                    if (sourceElementType.IsValueType)
                                    {
                                        currentElement = ConvertValueIfNotTargetType(currentElement, sourceElementType, targetElementType);
                                    }
                                    else if (sourceElementType != CommonTypes.StringType)
                                    {
                                        var targetCtor = targetElementType.GetConstructor(Type.EmptyTypes);     // TODO: Cache a compiled factory

                                        Throw<ObjectMapperException>.WhenNull(
                                            targetCtor, 
                                            $"The type '{targetElementType.GetFriendlyName()}' does not have a default constructor. Use a custom conversion.");

                                        var targetInstance = targetCtor.Invoke(null);

                                        currentElement = MapSourceToTarget(currentElement, targetInstance, doDeepClone);
                                    }

                                    typedList.Add(currentElement);
                                }

                                sourceValue = typedList;
                            }
                        }
                        else
                        {
                            if (!isAssignable || doDeepClone)
                            {
                                var targetInstance = CreateType(targetPropertyType);
                                sourceValue = MapSourceToTarget(sourceValue, targetInstance, doDeepClone);
                            }
                        }
                    }                   
                }

                match.TargetSetter.Invoke(target, sourceValue);
            }

            return target;
        }

        private static object ConvertValueIfNotTargetType(object sourceValue, Type sourceValueType, Type targetPropertyType)
        {
            if (sourceValueType != targetPropertyType && sourceValueType.IsDerivedFrom(CommonTypes.IConvertibleType))
            {
                // attempt to convert the source value to the target type
                var convertToType = targetPropertyType.IsNullableType()
                    ? Nullable.GetUnderlyingType(targetPropertyType)
                    : targetPropertyType;

                // If this throws then a custom conversion will be requiered - not attempting to convert between value types here.
                // The custom conversion could use an explicit cast, an appropriate Parse() method, or even use .As<T>().
                sourceValue = Convert.ChangeType(sourceValue, convertToType);
            }

            return sourceValue;
        }

        private static IEnumerable<object> GetSourceElements(object sourceElements)
        {
            var sourceItemsIterator = ((IEnumerable) sourceElements).GetEnumerator();

            while (sourceItemsIterator.MoveNext())
            {
                yield return sourceItemsIterator.Current;
            }
        }

        private object CreateType(Type type)
        {
            if (_typeCreators.TryGetValue(type, out var creator))
            {
                return creator.Invoke();
            }

            creator = type.GetFactory();

            _typeCreators.Add(type, creator);

            return creator.Invoke();
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
                mapperOptions);

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

            var mapperOptions = new TypedObjectMapperOptions<TSource, TTarget>(this);

            configure?.Invoke(mapperOptions);

            return mapperOptions;
        }
    }
}