using AllOverIt.Assertion;
using System.IO.Pipes;

namespace AllOverIt.Pipes.Anonymous
{
    /// <summary>Provides an anonymous pipe client that can be configured as a reader or a writer.</summary>
    public sealed class AnonymousPipeClient : AnonymousPipeBase
    {
        private AnonymousPipeClientStream? _pipeClientStream;

        /// <summary>Initializes an anonymous readable pipe client using a string representation of the client handle
        /// as provided by an anonymous pipe server.</summary>
        /// <param name="clientHandle">A string representation of the client handle as provided by an anonymous pipe server.</param>
        public void Start(string clientHandle)
        {
            _ = clientHandle.WhenNotNullOrEmpty();

            Start(PipeDirection.In, clientHandle);
        }

        /// <summary>Initializes an anonymous pipe client using a provided data stream direction and a string representation
        /// of the client handle as provided by an anonymous pipe server.</summary>
        /// <param name="direction">The direction of the pipe's data stream. A value of <see cref="PipeDirection.Out"/> makes
        /// the pipe writable and a value of <see cref="PipeDirection.In"/> makes the pipe readable.</param>
        /// <param name="clientHandle">A string representation of the client handle as provided by an anonymous pipe server.</param>
        public void Start(PipeDirection direction, string clientHandle)
        {
            _ = clientHandle.WhenNotNullOrEmpty();

            Throw<InvalidOperationException>.WhenNotNull(_pipeClientStream, "The anonymous pipe client has already been started.");

            _pipeClientStream = new AnonymousPipeClientStream(direction, clientHandle);

            InitializeStart(direction, _pipeClientStream);
        }
    }
}
