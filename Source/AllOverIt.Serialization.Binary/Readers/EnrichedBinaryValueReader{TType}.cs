namespace AllOverIt.Serialization.Binary.Readers
{
    /// <inheritdoc cref="IEnrichedBinaryValueReader"/>
    /// <typeparam name="TType">The object type read by this value reader.</typeparam>
    public abstract class EnrichedBinaryValueReader<TType> : IEnrichedBinaryValueReader
    {
        /// <inheritdoc />
        public Type Type { get; } = typeof(TType);

        /// <inheritdoc />
        public abstract object ReadValue(IEnrichedBinaryReader reader);
    }
}
