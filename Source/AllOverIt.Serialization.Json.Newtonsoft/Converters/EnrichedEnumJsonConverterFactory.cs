using AllOverIt.Extensions;
using AllOverIt.Patterns.Enumeration;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace AllOverIt.Serialization.Json.Newtonsoft.Converters
{
    /// <summary>Supports creating JsonConverter instances for <see cref="EnrichedEnum{TEnum}"/> types.</summary>
    public class EnrichedEnumJsonConverterFactory : JsonConverterFactory
    {
        private static readonly ConcurrentDictionary<Type, JsonConverter> Converters = new();

        /// <summary>When enabled, caches converters created for concrete EnrichedEnum types.</summary>
        /// <remarks>Caching is enabled by default and can only be disabled at the time of construction.</remarks>
        public bool EnableCaching { get; init; }

        /// <summary>Constructor.</summary>
        public EnrichedEnumJsonConverterFactory()
        {
            EnableCaching = true;
        }

        /// <summary>Returns true if the object to be converted is a <see cref="EnrichedEnum{TEnum}"/>.</summary>
        /// <param name="objectType">The object type.</param>
        /// <returns><see langword="True" /> if the object to be converted is a <see cref="EnrichedEnum{TEnum}"/>.</returns>
        public override bool CanConvert(Type objectType)
        {
            // The objectType is derived from EnrichedEnum<TEnum>, so need to get the generic from the base class.
            return objectType.IsEnrichedEnum() &&
                   objectType == objectType.BaseType!.GenericTypeArguments[0];      // must be MyEnum : EnrichedEnum<MyEnum>
        }

        /// <summary>Creates a JsonConverter for a specific <see cref="EnrichedEnum{TEnum}"/> type.</summary>
        /// <param name="objectType">The <see cref="EnrichedEnum{TEnum}"/> type to convert.</param>
        /// <returns>A JsonConverter for a specific <see cref="EnrichedEnum{TEnum}"/> type.</returns>
        public override JsonConverter CreateConverter(Type objectType)
        {
            return EnableCaching
                ? Converters.GetOrAdd(objectType, CreateEnrichedEnumJsonConverter)
                : CreateEnrichedEnumJsonConverter(objectType);
        }

        private static JsonConverter CreateEnrichedEnumJsonConverter(Type objectType)
        {
            var genericArg = objectType.BaseType!.GenericTypeArguments[0];
            var genericType = typeof(EnrichedEnumJsonConverter<>).MakeGenericType(genericArg);

            return (JsonConverter) Activator.CreateInstance(genericType)!;
        }
    }
}