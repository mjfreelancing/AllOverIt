using AllOverIt.Extensions;
using AllOverIt.Patterns.Enumeration;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AllOverIt.Serialization.Json.SystemText.Converters
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
        /// <param name="typeToConvert">The object type.</param>
        /// <returns><see langword="true" /> if the object to be converted is a <see cref="EnrichedEnum{TEnum}"/>.</returns>
        public override bool CanConvert(Type typeToConvert)
        {
            // The typeToConvert is derived from EnrichedEnum<TEnum>, so need to get the generic from the base class.
            return typeToConvert.IsEnrichedEnum() &&
                   typeToConvert == typeToConvert.BaseType!.GenericTypeArguments[0];      // must be MyEnum : EnrichedEnum<MyEnum>
        }

        /// <summary>Creates a JsonConverter for a specific <see cref="EnrichedEnum{TEnum}"/> type.</summary>
        /// <param name="typeToConvert">The <see cref="EnrichedEnum{TEnum}"/> type to convert.</param>
        /// <param name="options">The serializer options.</param>
        /// <returns>A JsonConverter for a specific <see cref="EnrichedEnum{TEnum}"/> type.</returns>
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return EnableCaching
                ? Converters.GetOrAdd(typeToConvert, CreateEnrichedEnumJsonConverter)
                : CreateEnrichedEnumJsonConverter(typeToConvert);
        }

        private static JsonConverter CreateEnrichedEnumJsonConverter(Type objectType)
        {
            var genericArg = objectType.BaseType!.GenericTypeArguments[0];
            var genericType = typeof(EnrichedEnumJsonConverter<>).MakeGenericType(genericArg);

            return (JsonConverter) Activator.CreateInstance(genericType);
        }
    }
}