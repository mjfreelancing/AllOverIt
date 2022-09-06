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
        private readonly ObjectMapperConfiguration _objectMapperConfigurator;

        /// <summary>Constructor. A default constructed <see cref="ObjectMapperConfiguration"/> will be used.</summary>
        public ObjectMapper()
            : this(new ObjectMapperConfiguration())
        {
        }

        /// <summary>Constructor.</summary>
        public ObjectMapper(ObjectMapperConfiguration objectMapperConfigurator)
        {
            _objectMapperConfigurator = objectMapperConfigurator.WhenNotNull(nameof(objectMapperConfigurator));
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

            var propertyMapper = _objectMapperConfigurator.PropertyMatchers.GetOrCreateMapper(sourceType, targetType);

            foreach (var match in propertyMapper.Matches)
            {
                var value = match.SourceGetter.Invoke(source);
                var sourceValue = propertyMapper.MatcherOptions.GetConvertedValue(this, match.SourceInfo.Name, value);

                var sourcePropertyType = match.SourceInfo.PropertyType;
                var targetPropertyType = match.TargetInfo.PropertyType;

                var deepCopySource = isDeepCopy || propertyMapper.MatcherOptions.IsDeepCopy(match.SourceInfo.Name);

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

            if (_objectMapperConfigurator.TypeFactory.TryGet(sourcePropertyType, targetPropertyType, out var factory))
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

                if (_objectMapperConfigurator.TypeFactory.TryGet(sourceKvpType, targetKvpType, out var factory))
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
            if (!_objectMapperConfigurator.TypeFactory.TryGet(type, out var factory))
            {
                factory = type.GetFactory();

                _objectMapperConfigurator.TypeFactory.Add(type, factory);
            }

            return factory.Invoke();
        }
    }
}