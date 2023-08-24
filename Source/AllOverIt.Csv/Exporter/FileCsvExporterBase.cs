using AllOverIt.Assertion;
using System.IO;

namespace AllOverIt.Csv.Exporter
{
    public abstract class FileCsvExporterBase<TModel> : BufferedCsvExporterBase<TModel>
    {
        private readonly string _filePath;
        private readonly FileMode _fileMode;

        public FileCsvExporterBase(string filePath, FileMode fileMode = FileMode.Create)
        {
            _filePath = filePath.WhenNotNullOrEmpty(nameof(filePath));
            _fileMode = fileMode;
        }

        protected override Stream CreateStream()
        {
            return new FileStream(_filePath, _fileMode);
        }
    }
}
