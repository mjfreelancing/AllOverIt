namespace AllOverIt.Zip
{
    /// <summary>Represents a ZIP package that allows files to be added and compressed into a stream.
    /// The package must be completed before the content stream can be accessed.</summary>
    public interface IZipPackage : IDisposable
    {
        /// <summary>Gets the ZIP archive content as a stream.</summary>
        /// <value>A <see cref="Stream"/> containing the compressed ZIP archive data.</value>
        /// <exception cref="InvalidOperationException">The archive stream has been disposed.</exception>
        /// <exception cref="InvalidOperationException">The archive must be completed to access the stream.</exception>
        /// <remarks>This property can only be accessed after <see cref="Complete"/> has been called.
        /// The returned stream is positioned at the beginning of the archive data.</remarks>
        Stream Content { get; }

        /// <summary>Asynchronously adds a new entry to the ZIP archive with the specified content.</summary>
        /// <param name="entryName">The name of the entry within the ZIP archive. Cannot be null or empty.</param>
        /// <param name="content">The binary content to be added to the entry.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        /// <exception cref="ArgumentException">entryName is null or empty.</exception>
        /// <exception cref="InvalidOperationException">The archive has already been disposed.</exception>
        /// <exception cref="OperationCanceledException">The operation was cancelled.</exception>
        /// <remarks>Entries are compressed using optimal compression level. The archive must not be completed or disposed when adding entries.</remarks>
        Task AddEntryAsync(string entryName, ReadOnlyMemory<byte> content, CancellationToken cancellationToken);

        /// <summary>Completes the ZIP archive and prepares the content stream for reading.</summary>
        /// <exception cref="InvalidOperationException">The archive has already been completed.</exception>
        /// <exception cref="InvalidOperationException">The archive stream has been disposed.</exception>
        /// <remarks>This method must be called before accessing the <see cref="Content"/> property.
        /// Once completed, no additional entries can be added to the archive.
        /// The content stream will be positioned at the beginning after completion.</remarks>
        void Complete();
    }

}
