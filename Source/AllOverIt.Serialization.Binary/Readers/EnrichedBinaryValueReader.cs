using System;

namespace AllOverIt.Serialization.Binary.Readers
{
    /// <inheritdoc cref="IEnrichedBinaryValueReader"/>
    public abstract class EnrichedBinaryValueReader : IEnrichedBinaryValueReader
    {
        /// <inheritdoc />
        public Type Type { get; }

        /// <summary>Constructor.</summary>
        /// <param name="type">The value type read by this value reader.</param>
        public EnrichedBinaryValueReader(Type type)
        {
            Type = type;
        }

        /// <inheritdoc />
        public abstract object ReadValue(IEnrichedBinaryReader reader);
    }
}
