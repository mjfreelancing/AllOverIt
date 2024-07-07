using AllOverIt.Assertion;

namespace AllOverIt.Serialization.Binary.Writers
{
    /// <inheritdoc cref="IEnrichedBinaryValueWriter"/>
    public abstract class EnrichedBinaryValueWriter : IEnrichedBinaryValueWriter
    {
        /// <inheritdoc />
        public Type Type { get; }

        /// <summary>Constructor.</summary>
        /// <param name="type">The value type this writer write to a stream.</param>
        public EnrichedBinaryValueWriter(Type type)
        {
            Type = type.WhenNotNull(nameof(type));
        }

        /// <inheritdoc />
        public abstract void WriteValue(IEnrichedBinaryWriter writer, object? value);
    }
}
