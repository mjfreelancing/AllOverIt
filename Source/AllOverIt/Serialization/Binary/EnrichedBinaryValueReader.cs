using System;

namespace AllOverIt.Serialization.Binary
{
    public abstract class EnrichedBinaryValueReader<TType> : IEnrichedBinaryValueReader
    {
        public Type Type => typeof(TType);

        public abstract object ReadValue(IEnrichedBinaryReader reader);

        public TValue ReadValue<TValue>(IEnrichedBinaryReader reader)
        {
            return (TValue)ReadValue(reader);
        }
    }
}
