using System.IO;
using System.IO.Pipes;

namespace AllOverIt.Pipes.Anonymous
{
    /// <summary>Provides an anonymous pipe server that can be configured as a reader or a writer.</summary>
    public sealed class AnonymousPipeServer : AnonymousPipeBase
    {
        private AnonymousPipeServerStream _pipeServerStream;
        private string _clientHandleString;
        private HandleInheritability _inheritability;

        /// <summary>Initializes an anonymous pipe server using a provided data stream direction and inheritability mode.</summary>
        /// <param name="direction">The direction of the pipe's data stream. A value of <see cref="PipeDirection.Out"/> makes
        /// the pipe writable and a value of <see cref="PipeDirection.In"/> makes the pipe readable. If not specified the pipe
        /// will be writable.</param>
        /// <param name="inheritability">Specifies whether the underlying handle is inheritable by child processes. If not specified
        /// child processes will not inherit the underlying handle.</param>
        /// <returns>A string representation of the handle to be used by an <see cref="AnonymousPipeClient"/>.</returns>
        public string Start(PipeDirection direction = PipeDirection.Out, HandleInheritability inheritability = HandleInheritability.None)
        {
            _inheritability = inheritability;

            _pipeServerStream = new AnonymousPipeServerStream(direction, _inheritability);

            InitializeStart(direction, _pipeServerStream);

            _clientHandleString = _pipeServerStream.GetClientHandleAsString();

            return _clientHandleString;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            DisposeClientHandleIfRequired();

            base.Dispose(disposing);
        }

        private void DisposeClientHandleIfRequired()
        {
            if (_clientHandleString is not null)
            {
                _pipeServerStream.DisposeLocalCopyOfClientHandle();
                _clientHandleString = null;
            }
        }
    }
}
