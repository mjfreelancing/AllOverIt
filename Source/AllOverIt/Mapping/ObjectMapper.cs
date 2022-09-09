using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Mapping.Exceptions;
using AllOverIt.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AllOverIt.Mapping
{
    /// <summary>Implements an object mapper that will copy property values from a source onto a target.</summary>
    public sealed class ObjectMapper : IObjectMapper
    {
        internal readonly ObjectMapperConfiguration _configuration;

        /// <summary>Constructor. A default constructed <see cref="ObjectMapperConfiguration"/> will be used.</summary>
        public ObjectMapper()
            : this(new ObjectMapperConfiguration())
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="configuration">The configuration to be used by the mapper.</param>
        public ObjectMapper(ObjectMapperConfiguration configuration)
        {
            _configuration = configuration.WhenNotNull(nameof(configuration));
        }

        /// <summary>Constructor.</summary>
        /// <param name="configuration">Provides the ability to configure the mapper at the time of construction.</param>
        public ObjectMapper(Action<ObjectMapperConfiguration> configuration)
        {
            _ = configuration.WhenNotNull(nameof(configuration));

            _configuration = new ObjectMapperConfiguration();
            configuration.Invoke(_configuration);
        }

        /// <summary>Constructor.</summary>
        /// <param name="defaultOptions">Provides the ability to specify default options for all mapping operations.</param>
        /// <param name="configuration">Provides the ability to configure the mapper at the time of construction.</param>
        public ObjectMapper(Action<ObjectMapperOptions> defaultOptions, Action<ObjectMapperConfiguration> configuration)
        {
            _ = defaultOptions.WhenNotNull(nameof(defaultOptions));
            _ = configuration.WhenNotNull(nameof(configuration));

            _configuration = new ObjectMapperConfiguration(defaultOptions);
            configuration.Invoke(_configuration);
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

            var propertyMatcher = _configuration._propertyMatcherCache.GetOrCreateMapper(sourceType, targetType);

            foreach (var match in propertyMatcher.Matches)
            {
                // Get the source value
                var sourceValue = match.SourceGetter.Invoke(source);

                // See if we skip this property based on its value
                if (propertyMatcher.MatcherOptions.IsExcludedWhen(match.SourceInfo.Name, sourceValue))
                {
                    continue;
                }

                var sourcePropertyType = match.SourceInfo.PropertyType;
                var targetPropertyType = match.TargetInfo.PropertyType;

                // Is there a null value replacement configured
                sourceValue ??= propertyMatcher.MatcherOptions.GetNullReplacement(match.SourceInfo.Name);

                // Is there a conversion configured ?
                sourceValue = propertyMatcher.MatcherOptions.GetConvertedValue(this, match.SourceInfo.Name, sourceValue);

                var deepCopySource = isDeepCopy || propertyMatcher.MatcherOptions.IsDeepCopy(match.SourceInfo.Name);

                // Handles null source values, including the creation of empty collections if required
                var targetValue = GetMappedSourceValue(sourceValue, sourcePropertyType, targetPropertyType, deepCopySource, propertyMatcher);

                match.TargetSetter.Invoke(target, targetValue);
            }

            return target;
        }

        private object GetMappedSourceValue(object sourceValue, Type sourcePropertyType, Type targetPropertyType, bool deepCopy,
            ObjectPropertyMatcher propertyMatcher)
        {
            if (sourceValue is null)
            {
                if (targetPropertyType.IsEnumerableType() && !_configuration.Options.AllowNullCollections)
                {
                    return CreateEmptyCollection(targetPropertyType);
                }
                
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

            if (!isAssignable || deepCopy)
            {
                return CreateTargetFromSourceValue(sourceValue, sourceValueType, sourcePropertyType, targetPropertyType, deepCopy);
            }

            return sourceValue;
        }

        private object CreateTargetFromSourceValue(object sourceValue, Type sourceValueType, Type sourcePropertyType, Type targetPropertyType, bool deepCopy)
        {
            // Configuration via ConstructUsing() takes precedence over mapping properties
            if (_configuration._typeFactory.TryGet(sourcePropertyType, targetPropertyType, out var factory))
            {
                return factory.Invoke(this, sourceValue);
            }

            if (sourceValueType.IsEnumerableType())
            {
                return sourceValue switch
                {
                    IDictionary _ => MapToDictionary(sourceValue, sourceValueType, targetPropertyType),
                    IEnumerable _ => MapToCollection(sourceValue, sourceValueType, targetPropertyType, deepCopy),
                    _ => throw new ObjectMapperException($"Cannot map type '{sourceValueType.GetFriendlyName()}'."),
                };
            }

            var targetInstance = CreateType(targetPropertyType);
            return MapSourceToTarget(sourceValue, targetInstance, deepCopy);
        }

        private object MapToDictionary(object sourceValue, Type sourceValueType, Type targetPropertyType)
        {
            Throw<ObjectMapperException>.When(
                !sourceValueType.IsGenericType || !targetPropertyType.IsGenericType,
                "Non-generic dictionary mapping is not supported.");

            // get types for the source dictionary
            var sourceTypeArgs = sourceValueType.GenericTypeArguments;
            var sourceDictionaryKeyType = sourceTypeArgs[0];
            var sourceDictionaryValueType = sourceTypeArgs[1];
            var sourceKvpType = CommonTypes.KeyValuePairType.MakeGenericType(new[] { sourceDictionaryKeyType, sourceDictionaryValueType });

            // Create the target dictionary
            var (dictionaryInstance, targetKvpType) = CreateDictionary(targetPropertyType);

            var dictionaryAddMethod = CommonTypes.ICollectionGenericType
                .MakeGenericType(targetKvpType)
                .GetMethod("Add", new[] { targetKvpType });                             // todo: cache this

            var sourceElements = GetSourceElements(sourceValue);

            foreach (var sourceElement in sourceElements)
            {
                // Start by assuming the sourceKvpType and targetKvpType are the same. If they are not, then a casting
                // error will be thrown and the caller should call ConstructUsing() to provide the required factory.
                var targetElement = sourceElement;

                if (_configuration._typeFactory.TryGet(sourceKvpType, targetKvpType, out var factory))
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

        private object MapToCollection(object sourceValue, Type sourceValueType, Type targetPropertyType, bool doDeepCopy)
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

            return GetAsListOrArray(listType, listInstance, targetPropertyType);
        }

        private object CreateEmptyCollection(Type targetPropertyType)
        {
            if (targetPropertyType.IsDerivedFrom(CommonTypes.IDictionaryGenericType))
            {
                var (instance, _) = CreateDictionary(targetPropertyType);

                return instance;
            }

            if (targetPropertyType.IsDerivedFrom(CommonTypes.IEnumerableGenericType))
            {
                var targetElementType = targetPropertyType.IsArray
                    ? targetPropertyType.GetElementType()
                    : targetPropertyType.GetGenericArguments()[0];

                var (listType, listInstance) = CreateTypedList(targetElementType);

                return GetAsListOrArray(listType, listInstance, targetPropertyType);
            }

            return null;
        }

        private object GetAsListOrArray(Type listType, IList listInstance, Type targetPropertyType)
        {
            if (targetPropertyType.IsArray)
            {
                var toArrayMethod = listType.GetMethod("ToArray");                          // TODO: Cache this
                return toArrayMethod.Invoke(listInstance, Type.EmptyTypes);
            }

            return listInstance;
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

        private (object Instance, Type KvpType) CreateDictionary(Type targetPropertyType)
        {
            var targetTypeArgs = targetPropertyType.GenericTypeArguments;
            var targetKeyType = targetTypeArgs[0];
            var targetValueType = targetTypeArgs[1];

            var dictionaryInstance = CreatedTypedDictionary(targetKeyType, targetValueType);
            var targetKvpType = CommonTypes.KeyValuePairType.MakeGenericType(new[] { targetKeyType, targetValueType });

            return (dictionaryInstance, targetKvpType);
        }

        private (Type ListType, IList ListInstance) CreateTypedList(Type targetElementType)
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
            if (!_configuration._typeFactory.TryGet(type, out var factory))
            {
                factory = type.GetFactory();

                _configuration._typeFactory.Add(type, factory);
            }

            return factory.Invoke();
        }
    }
}