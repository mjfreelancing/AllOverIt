using AllOverIt.Pipes.Named.Server;
using NamedPipeTypes;

namespace NamedPipeServerDemo
{
    internal sealed class PingMessage
    {
        public required INamedPipeServerConnection<PipeMessage> Connection { get; init; }
    }
}