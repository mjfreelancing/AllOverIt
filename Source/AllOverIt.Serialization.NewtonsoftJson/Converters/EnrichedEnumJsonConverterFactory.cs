using System;
using AllOverIt.Extensions;
using AllOverIt.Patterns.Enumeration;
using Newtonsoft.Json;

namespace AllOverIt.Serialization.NewtonsoftJson.Converters
{
    /// <summary>Supports creating JsonConverter instances for <see cref="EnrichedEnum{TEnum}"/> types.</summary>
    public class EnrichedEnumJsonConverterFactory : JsonConverterFactory
    {
        /// <summary>Returns true if the object to be converted is a <see cref="EnrichedEnum{TEnum}"/>.</summary>
        /// <param name="objectType">The object type.</param>
        /// <returns>True if the object to be converted is a <see cref="EnrichedEnum{TEnum}"/>.</returns>
        public override bool CanConvert(Type objectType)
        {
            // The objectType is derived from EnrichedEnum<TEnum>, so need to get the generic from the base class.
            return objectType.IsDerivedFrom(typeof(EnrichedEnum<>)) &&
                   objectType == objectType.BaseType!.GenericTypeArguments[0];      // must be MyEnum : EnrichedEnum<MyEnum>
        }

        /// <summary>Creates a JsonConverter for a specific <see cref="EnrichedEnum{TEnum}"/> type.</summary>
        /// <param name="objectType">The <see cref="EnrichedEnum{TEnum}"/> type to convert.</param>
        /// <returns>A JsonConverter for a specific <see cref="EnrichedEnum{TEnum}"/> type.</returns>
        public override JsonConverter CreateConverter(Type objectType)
        {
            var genericArg = objectType.BaseType!.GenericTypeArguments[0];
            var genericType = typeof(EnrichedEnumJsonConverter<>).MakeGenericType(genericArg);

            return (JsonConverter) Activator.CreateInstance(genericType);
        }
    }
}