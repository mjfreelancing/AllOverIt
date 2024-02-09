using AllOverIt.Collections;

namespace AllOverIt.Serilog.Sinks.CircularBuffer
{
    /// <summary>A circular buffer of <see cref="CircularBufferSinkMessage"/> messages used for capturing log events.</summary>
    public class CircularBufferSinkMessages : CircularBuffer<CircularBufferSinkMessage>, ICircularBufferSinkMessages
    {
        /// <summary>Constructor.</summary>
        /// <param name="capacity">The maximum number of items that can be held by the buffer.</param>
        public CircularBufferSinkMessages(int capacity)
            : base(capacity)
        {
        }
    }
}
