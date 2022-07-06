using System;

namespace AllOverIt.Serialization.Binary
{
    public interface IEnrichedBinaryValueWriter
    {
        Type Type { get; }
        void WriteValue(EnrichedBinaryWriter writer, object value);
    }
}
