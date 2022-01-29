﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using AllOverIt.Extensions;
using AllOverIt.Patterns.Enumeration;

namespace AllOverIt.Serialization.SystemTextJson.Converters
{
    /// <summary>Supports creating JsonConverter instances for <see cref="EnrichedEnum{TEnum}"/> types.</summary>
    public class EnrichedEnumJsonConverterFactory : JsonConverterFactory
    {
        /// <summary>Returns true if the object to be converted is a <see cref="EnrichedEnum{TEnum}"/>.</summary>
        /// <param name="typeToConvert">The object type.</param>
        /// <returns>True if the object to be converted is a <see cref="EnrichedEnum{TEnum}"/>.</returns>
        public override bool CanConvert(Type typeToConvert)
        {
            // The typeToConvert is derived from EnrichedEnum<TEnum>, so need to get the generic from the base class.
            return typeToConvert.IsDerivedFrom(typeof(EnrichedEnum<>)) &&
                   typeToConvert == typeToConvert.BaseType!.GenericTypeArguments[0];      // must be MyEnum : EnrichedEnum<MyEnum>
        }

        /// <summary>Creates a JsonConverter for a specific <see cref="EnrichedEnum{TEnum}"/> type.</summary>
        /// <param name="typeToConvert">The <see cref="EnrichedEnum{TEnum}"/> type to convert.</param>
        /// <param name="options">The serializer options.</param>
        /// <returns>A JsonConverter for a specific <see cref="EnrichedEnum{TEnum}"/> type.</returns>
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var genericArg = typeToConvert.BaseType!.GenericTypeArguments[0];
            var genericType = typeof(EnrichedEnumJsonConverter<>).MakeGenericType(genericArg);

            return (JsonConverter) Activator.CreateInstance(genericType);
        }
    }
}