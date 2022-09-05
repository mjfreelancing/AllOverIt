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

        private readonly IDictionary<Type, Func<object>> _internalTypeFactories = new Dictionary<Type, Func<object>>();
        private readonly IDictionary<(Type, Type), MatchingPropertyMapper> _mapperCache = new Dictionary<(Type, Type), MatchingPropertyMapper>();

        // Source => Target factories provided via configuration
        private readonly IDictionary<(Type, Type), Func<IObjectMapper, object, object>> _sourceTargetFactories
            = new Dictionary<(Type, Type), Func<IObjectMapper, object, object>>();      

        /// <summary>Defines the default mapper options to apply when explicit options are not setup at the time of mapping configuration.</summary>
        public ObjectMapperOptions DefaultOptions { get; }

        /// <summary>Constructor.</summary>
        public ObjectMapper()
        {
            DefaultOptions = new(this);
        }

        /// <inheritdoc />
        public void Configure<TSource, TTarget>(Action<TypedObjectMapperOptions<TSource, TTarget>> configure = default)
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
        {
            return (TTarget) MapSourceToTarget(source, target, false);
        }

        private object MapSourceToTarget(object source, object target, bool isDeepCopy)
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

                var deepCopySource = isDeepCopy || propertyMapper.MapperOptions.IsDeepCopy(match.SourceInfo.Name);

                var targetValue = GetMappedSourceValue(sourceValue, sourcePropertyType, targetPropertyType, deepCopySource);

                match.TargetSetter.Invoke(target, targetValue);
            }

            return target;
        }

        private object GetMappedSourceValue(object sourceValue, Type sourcePropertyType, Type targetPropertyType, bool deepCopy)
        {
            if (sourceValue is null)
            {
                return null;
            }

            var sourceValueType = sourceValue.GetType();

            if (sourceValueType == CommonTypes.StringType)
            {
                return sourceValue;
            }

            if (sourceValueType.IsValueType)
            {
                return ConvertValueIfNotTargetType(sourceValue, sourceValueType, targetPropertyType);
            }

            var isAssignable = targetPropertyType.IsAssignableFrom(sourceValueType);

            // Assuming the target is also an enumerable type - a cast exception will result if it cannot be assigned
            if (!isAssignable || deepCopy)
            {
                return CreateTargetFromSourceValue(sourceValue, sourceValueType, sourcePropertyType, targetPropertyType, deepCopy);
            }

            return sourceValue;
        }

        private object CreateTargetFromSourceValue(object sourceValue, Type sourceValueType, Type sourcePropertyType, Type targetPropertyType, bool deepCopy)
        {
            if (sourceValueType.IsEnumerableType())
            {
                return sourceValue switch
                {
                    IDictionary _ => CreateTargetDictionary(sourceValue, sourceValueType, targetPropertyType),
                    IEnumerable _ => CreateTargetCollection(sourceValue, sourceValueType, targetPropertyType, deepCopy),
                    _ => throw new ObjectMapperException($"Cannot map type '{sourceValueType.GetFriendlyName()}'."),
                };
            }

            if (_sourceTargetFactories.TryGetValue((sourcePropertyType, targetPropertyType), out var factory))
            {
                return factory.Invoke(this, sourceValue);
            }

            var targetInstance = CreateType(targetPropertyType);
            return MapSourceToTarget(sourceValue, targetInstance, deepCopy);
        }

        private object CreateTargetDictionary(object sourceValue, Type sourceValueType, Type targetPropertyType)
        {
            Throw<ObjectMapperException>.When(
                !sourceValueType.IsGenericType || !targetPropertyType.IsGenericType,
                "Non-generic dictionary mapping is not supported.");

            // TODO: See what factories can be cached

            // get types for the source dictionary
            var sourceTypeArgs = sourceValueType.GenericTypeArguments;
            var sourceDictionaryKeyType = sourceTypeArgs[0];
            var sourceDictionaryValueType = sourceTypeArgs[1];
            var sourceKvpType = CommonTypes.KeyValuePairType.MakeGenericType(new[] { sourceDictionaryKeyType, sourceDictionaryValueType });

            // Create the target dictionary
            var targetTypeArgs = targetPropertyType.GenericTypeArguments;
            var targetKeyType = targetTypeArgs[0];
            var targetValueType = targetTypeArgs[1];

            var dictionaryInstance = CreatedTypedDictionary(targetKeyType, targetValueType);

            var targetKvpType = CommonTypes.KeyValuePairType.MakeGenericType(new[] { targetKeyType, targetValueType });

            var dictionaryAddMethod = CommonTypes.ICollectionGenericType
                .MakeGenericType(targetKvpType)
                .GetMethod("Add", new[] { targetKvpType });

            var sourceElements = GetSourceElements(sourceValue);

            foreach (var sourceElement in sourceElements)
            {
                // Start by assuming the sourceKvpType and targetKvpType are the same. If they are not, then a casting
                // error will be thrown and the caller should call ConstructUsing() to provide the required factory.
                var targetElement = sourceElement;

                if (_sourceTargetFactories.TryGetValue((sourceKvpType, targetKvpType), out var factory))
                {
                    targetElement = factory.Invoke(this, sourceElement);
                }

                var targetElementType = targetElement.GetType();

                Throw<ObjectMapperException>.When(
                    targetKvpType != targetElementType,
                    $"The type '{targetElementType.GetFriendlyName()}' cannot be assigned to type '{targetKvpType.GetFriendlyName()}'.");

                dictionaryAddMethod.Invoke(dictionaryInstance, new[] { targetElement });
            }

            return dictionaryInstance;
        }

        private object CreateTargetCollection(object sourceValue, Type sourceValueType, Type targetPropertyType, bool doDeepCopy)
        {
            var sourceElementType = sourceValueType.IsArray
                                    ? sourceValueType.GetElementType()
                                    : sourceValueType.GetGenericArguments()[0];

            var targetElementType = targetPropertyType.IsArray
                ? targetPropertyType.GetElementType()
                : targetPropertyType.GetGenericArguments()[0];

            var (listType, listInstance) = CreateTypedList(targetElementType);

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

                    currentElement = MapSourceToTarget(currentElement, targetInstance, doDeepCopy);
                }

                listInstance.Add(currentElement);
            }

            if (targetPropertyType.IsArray)
            {
                var toArrayMethod = listType.GetMethod("ToArray");                          // TODO: Cache this
                return toArrayMethod.Invoke(listInstance, Type.EmptyTypes);
            }
            else
            {
                return listInstance;
            }
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

        private (Type, IList) CreateTypedList(Type targetElementType)
        {
            var listType = CommonTypes.ListGenericType.MakeGenericType(new[] { targetElementType });
            var listInstance = (IList) CreateType(listType);

            return (listType, listInstance);
        }

        private object CreatedTypedDictionary(Type keyType, Type valueType)
        {
            var dictionaryType = CommonTypes.DictionaryGenericType.MakeGenericType(new[] { keyType, valueType });

            return CreateType(dictionaryType);
        }

        private object CreateType(Type type)
        {
            if (!_internalTypeFactories.TryGetValue(type, out var factory))
            {
                factory = type.GetFactory();

                _internalTypeFactories.Add(type, factory);
            }

            return factory.Invoke();
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
        {
            if (configure is null)
            {
                return DefaultOptions;
            }

            var mapperOptions = new TypedObjectMapperOptions<TSource, TTarget>(this, (sourceType, targetType, factory) =>
            {
                var key = (sourceType, targetType);
                _sourceTargetFactories.Add(key, factory);
            });

            configure?.Invoke(mapperOptions);

            return mapperOptions;
        }
    }
}