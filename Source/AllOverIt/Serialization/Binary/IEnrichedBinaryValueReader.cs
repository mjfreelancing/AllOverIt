using System;

namespace AllOverIt.Serialization.Binary
{
    public interface IEnrichedBinaryValueReader
    {
        Type Type { get; }
        object ReadValue(EnrichedBinaryReader reader);
        TValue ReadValue<TValue>(EnrichedBinaryReader reader);
    }
}
