using System;

namespace AllOverIt.Serialization.Binary
{
    public interface IEnrichedBinaryValueWriter
    {
        Type Type { get; }
        void WriteValue(IEnrichedBinaryWriter writer, object value);
    }
}
