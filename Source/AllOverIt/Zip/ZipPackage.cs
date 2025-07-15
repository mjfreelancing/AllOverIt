using AllOverIt.Assertion;
using System.IO.Compression;

namespace AllOverIt.Zip
{
    /// <summary>Represents a mockable ZIP package wrapper over <see cref="ZipArchive"/> that allows files
    /// to be added and compressed into a stream.
    /// The package must be completed before the content stream can be accessed.</summary>
    /// <remarks>This class implements the disposable pattern and should be disposed when no longer needed.
    /// The archive must be completed by calling <see cref="Complete"/> before accessing the <see cref="Content"/> stream.
    /// Once completed, no additional entries can be added to the archive.</remarks>
    public sealed class ZipPackage : IZipPackage
    {
        private bool _completed = false;
        private bool _disposed = false;
        private MemoryStream? _memoryStream;
        private ZipArchive? _archive;

        /// <inheritdoc />
        public Stream Content => GetArchiveStream();

        /// <summary>Initializes a new instance of the <see cref="ZipPackage"/> class.</summary>
        /// <remarks>Creates a new ZIP archive in memory with optimal compression settings.</remarks>
        public ZipPackage()
        {
            _memoryStream = new MemoryStream();
            _archive = new ZipArchive(_memoryStream, ZipArchiveMode.Create, true);
        }

        /// <inheritdoc />
        public async Task AddEntryAsync(string entryName, ReadOnlyMemory<byte> content, CancellationToken cancellationToken)
        {
            _ = entryName.WhenNotNullOrEmpty();

            Throw<InvalidOperationException>.WhenNull(_archive, "The archive has already been disposed.");

            cancellationToken.ThrowIfCancellationRequested();

            var zipEntry = _archive.CreateEntry(entryName, CompressionLevel.Optimal);

            using var zipStream = zipEntry.Open();

            await zipStream.WriteAsync(content, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void Complete()
        {
            Throw<InvalidOperationException>.When(_completed, "The archive has already been completed.");
            Throw<InvalidOperationException>.WhenNull(_memoryStream, "The archive stream has been disposed.");

            // The archive must be closed to ensure the stream is completely written to
            DisposeArchive();

            _memoryStream.Position = 0;
            _completed = true;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                DisposeArchive();

                _memoryStream?.Dispose();
                _memoryStream = null;

                _disposed = true;
            }
        }

        private void DisposeArchive()
        {
            _archive?.Dispose();
            _archive = null;
        }

        private MemoryStream GetArchiveStream()
        {
            Throw<InvalidOperationException>.WhenNull(_memoryStream, "The archive stream has been disposed.");
            Throw<InvalidOperationException>.WhenNot(_completed, "The archive must be completed to access the stream.");

            return _memoryStream!;
        }
    }
}
