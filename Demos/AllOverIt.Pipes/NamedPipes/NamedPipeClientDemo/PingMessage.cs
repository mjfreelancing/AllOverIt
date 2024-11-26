using AllOverIt.Pipes.Named.Client;
using NamedPipeTypes;

namespace NamedPipeClientDemo
{
    internal sealed class PingMessage
    {
        public required INamedPipeClientConnection<PipeMessage> Connection { get; init; }
    }
}