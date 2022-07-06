using System;

namespace AllOverIt.Serialization.Binary
{
    public abstract class EnrichedBinaryValueReader<TType> : IEnrichedBinaryValueReader
    {
        public Type Type => typeof(TType);

        public abstract object ReadValue(EnrichedBinaryReader reader);

        public TValue ReadValue<TValue>(EnrichedBinaryReader reader)
        {
            return (TValue)ReadValue(reader);
        }
    }
}
