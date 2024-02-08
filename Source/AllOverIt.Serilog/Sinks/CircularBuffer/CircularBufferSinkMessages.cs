using AllOverIt.Collections;

namespace AllOverIt.Serilog.Sinks.CircularBuffer
{
    internal class CircularBufferSinkMessages : CircularBuffer<CircularBufferSinkMessage>, ICircularBufferSinkMessages
    {
        public CircularBufferSinkMessages(int capacity)
            : base(capacity)
        {
        }
    }
}
