using AllOverIt.Collections;

namespace AllOverIt.Serilog.Sinks.CircularBuffer
{
    /// <summary>Represents a circular buffer of <see cref="CircularBufferSinkMessage"/> messages.</summary>
    public interface ICircularBufferSinkMessages : ICircularBuffer<CircularBufferSinkMessage>
    {
    }
}
