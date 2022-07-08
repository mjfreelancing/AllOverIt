using System;

namespace AllOverIt.Serialization.Binary
{
    public interface IEnrichedBinaryValueReader
    {
        Type Type { get; }
        object ReadValue(IEnrichedBinaryReader reader);
        TValue ReadValue<TValue>(IEnrichedBinaryReader reader);
    }
}
