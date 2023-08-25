using AllOverIt.Assertion;
using System.IO;

namespace AllOverIt.Csv.Exporter
{
    /// <summary>Implements a buffered CSV exporter that writes the content to a file.</summary>
    /// <typeparam name="TModel">The model type representing the columns of each row to be exported.</typeparam>
    public abstract class FileCsvExporterBase<TModel> : BufferedCsvExporterBase<TModel>
    {
        private readonly string _filePath;
        private readonly FileMode _fileMode;

        /// <summary>Constructor.</summary>
        /// <param name="filePath">The path to the file to be created and written to.</param>
        /// <param name="fileMode">Controls whether the file should be created new, replaced, or appended. The default is <see cref="FileMode.Create"/>.</param>
        public FileCsvExporterBase(string filePath, FileMode fileMode = FileMode.Create)
            : this(filePath, fileMode, null)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="filePath">The path to the file to be created and written to.</param>
        /// <param name="fileMode">Controls whether the file should be created new, replaced, or appended. The default is <see cref="FileMode.Create"/>.</param>
        /// <param name="configuration">The configuration to use.</param>
        public FileCsvExporterBase(string filePath, FileMode fileMode, BufferedCsvExporterConfiguration configuration)
            : base(configuration)
        {
            _filePath = filePath.WhenNotNullOrEmpty(nameof(filePath));
            _fileMode = fileMode;
        }

        /// <inheritdoc />
        protected override Stream CreateStream()
        {
            return new FileStream(_filePath, _fileMode);
        }
    }
}
