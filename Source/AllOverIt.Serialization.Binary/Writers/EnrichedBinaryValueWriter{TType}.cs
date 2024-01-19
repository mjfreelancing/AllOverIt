using System;

namespace AllOverIt.Serialization.Binary.Writers
{
    /// <inheritdoc cref="IEnrichedBinaryValueWriter"/>
    /// <typeparam name="TType">The object type written by this value writer.</typeparam>
    public abstract class EnrichedBinaryValueWriter<TType> : IEnrichedBinaryValueWriter
    {
        /// <inheritdoc />
        public Type Type => typeof(TType);

        /// <inheritdoc />
        public abstract void WriteValue(IEnrichedBinaryWriter writer, object value);
    }
}
