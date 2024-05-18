using System.Text.Json;

namespace AllOverIt.Serialization.Json.SystemText.Converters
{
    /// <summary>Provides <see cref="NestedDictionaryConverter"/>  options.</summary>
    public sealed class NestedDictionaryConverterOptions
    {
        /// <summary>The System.Text Json serializer uses the <see cref="JsonTokenType.Number"/> token when serializing
        /// integer, float, double, and decimal values. This option indicates if deserialization should treat the value
        /// as a decimal (when <see langword="True"/>), otherwise a double (when <see langword="False"/>).</summary>
        /// <remarks>The default is <see langword="False"/> as it is more common and performant.</remarks>
        public bool ReadFloatingAsDecimals { get; init; }
    }
}