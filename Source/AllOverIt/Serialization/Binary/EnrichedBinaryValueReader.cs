using System;

namespace AllOverIt.Serialization.Binary
{
    /// <inheritdoc cref="IEnrichedBinaryValueReader"/>
    /// <typeparam name="TType">The object type read by this value reader.</typeparam>
    public abstract class EnrichedBinaryValueReader<TType> : IEnrichedBinaryValueReader
    {
        /// <inheritdoc />
        public Type Type => typeof(TType);

        /// <inheritdoc />
        public abstract object ReadValue(IEnrichedBinaryReader reader);

        /// <inheritdoc />
        public TValue ReadValue<TValue>(IEnrichedBinaryReader reader)
        {
            // TODO: add a guard to check typeof(TValue) == Type

            return (TValue)ReadValue(reader);
        }
    }
}
