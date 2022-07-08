using System;

namespace AllOverIt.Serialization.Binary
{
    public abstract class EnrichedBinaryValueWriter<TType> : IEnrichedBinaryValueWriter
    {
        public Type Type => typeof(TType);

        public abstract void WriteValue(IEnrichedBinaryWriter writer, object value);
    }
}
