using System.IO;
using System.Text;

namespace AllOverIt.Io
{
    /// <summary>Implements a text streamer for character output in a particular Encoding, backed by a memory stream.</summary>
    public class TextStreamer : MemoryStream
    {
        private StreamWriter _writer;

        /// <summary>Constructor.</summary>
        /// <param name="encoding">The character encoding to use. When not specified this will default to a UTF8 encoding with no
        /// byte order mark (BOM).</param>
        /// <param name="capacity">The initial capacity (usable buffer) applied to the underlying memory stream.</param>
        /// <param name="bufferSize">The buffer size to use. When not specified this will default to the same default used by
        /// <see cref="StreamWriter"/> (1024 bytes). The same minimum of 128 bytes is also enforced.</param>
        public TextStreamer(Encoding encoding = null, int capacity = 0, int bufferSize = -1)
            : this(null, encoding, capacity, bufferSize)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="value">Initial text to populate the stream with.</param>
        /// <param name="encoding">The character encoding to use. When not specified this will default to a UTF8 encoding with no
        /// byte order mark (BOM).</param>
        /// <param name="capacity">The initial capacity (usable buffer) applied to the underlying memory stream.</param>
        /// <param name="bufferSize">The buffer size to use. When not specified this will default to the same default used by
        /// <see cref="StreamWriter"/> (1024 bytes). The same minimum of 128 bytes is also enforced.</param>
        public TextStreamer(string value, Encoding encoding = null, int capacity = 0, int bufferSize = -1)
            : base(capacity)
        {
            _writer = new StreamWriter(this, encoding, bufferSize, false);

            if (value != null)
            {
                _writer.Write(value);
            }
        }

        /// <summary>Gets the underlying <see cref="StreamWriter"/> that can be used to write to the current position in the memory stream.</summary>
        public StreamWriter GetWriter()
        {
            return _writer;
        }

        /// <summary>Flushes the underlying writer to the memory stream and returns the content as a string.</summary>
        /// <returns>The text content contained by the internal stream.</returns>
        public override string ToString()
        {
            var stream = _writer.BaseStream;

            _writer.Flush();

            stream.Position = 0;

            using (var reader = new StreamReader(stream, _writer.Encoding, true, -1, true))
            {
                return reader.ReadToEnd();
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            _writer?.Dispose();
            _writer = null;
        }
    }
}