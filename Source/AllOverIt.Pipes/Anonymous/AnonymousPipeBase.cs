using AllOverIt.Assertion;
using System.IO.Pipes;

namespace AllOverIt.Pipes.Anonymous
{
    /// <summary>Provides base anonymous client or server pipe functionality catering for read or write operations.</summary>
    public abstract class AnonymousPipeBase : IDisposable
    {
        private PipeDirection _direction;
        private PipeStream _pipeStream;
        private StreamReader _streamReader;
        private StreamWriter _streamWriter;

        /// <summary>Gets the stream reader for a readable anonymous pipe.</summary>
        public StreamReader Reader
        {
            get
            {
                AssertCanRead();

                _streamReader ??= new StreamReader(_pipeStream);

                return _streamReader;
            }
        }

        /// <summary>Gets the stream writer for a writable anonymous pipe.</summary>
        public StreamWriter Writer
        {
            get
            {
                AssertCanWrite();

                _streamWriter ??= new StreamWriter(_pipeStream);

                return _streamWriter;
            }
        }

        /// <summary>Waits for the other end of the pipe to read all sent bytes. This is only applicable
        /// to writable anonymous pipes.</summary>
        public void WaitForPipeDrain()
        {
            AssertCanWrite();

            _pipeStream.WaitForPipeDrain();
        }

        /// <summary>Disposes of the internal streams.</summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>Initializes the anonymous pipe as writable when <paramref name="direction"/> is <see cref="PipeDirection.Out"/>
        /// or readable when <paramref name="direction"/> is <see cref="PipeDirection.In"/>.</summary>
        /// <param name="direction">The direction of the pipe's data stream. A value of <see cref="PipeDirection.Out"/> makes
        /// the pipe writable and a value of <see cref="PipeDirection.In"/> makes the pipe readable.</param>
        /// <param name="pipeStream">The pipe's stream.</param>
        protected void InitializeStart(PipeDirection direction, PipeStream pipeStream)
        {
            _ = pipeStream.WhenNotNull(nameof(pipeStream));

            Throw<InvalidOperationException>.WhenNotNull(_pipeStream, "The anonymous pipe has already been initialized.");

            _direction = direction;
            _pipeStream = pipeStream;

            if (_direction == PipeDirection.Out)
            {
                _streamWriter = new StreamWriter(_pipeStream)
                {
                    AutoFlush = true
                };
            }
            else
            {
                _streamReader = new StreamReader(_pipeStream);
            }
        }

        /// <summary>Disposes of the internal resources.</summary>
        /// <param name="disposing">Indicates if the internal resources are to be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _streamReader?.Dispose();
                _streamReader = null;

                _streamWriter?.Dispose();
                _streamWriter = null;

                _pipeStream?.Dispose();
                _pipeStream = null;
            }
        }

        private void AssertCanRead()
        {
            Throw<InvalidOperationException>.WhenNull(_pipeStream, $"The anonymous pipe has not been initialized. Call the {nameof(InitializeStart)}() method.");
            Throw<InvalidOperationException>.When(_direction == PipeDirection.Out, "The anonymous pipe is write-only.");
        }

        private void AssertCanWrite()
        {
            Throw<InvalidOperationException>.WhenNull(_pipeStream, $"The anonymous pipe has not been initialized. Call the {nameof(InitializeStart)}() method.");
            Throw<InvalidOperationException>.When(_direction == PipeDirection.In, "The anonymous pipe is read-only.");
        }
    }
}
