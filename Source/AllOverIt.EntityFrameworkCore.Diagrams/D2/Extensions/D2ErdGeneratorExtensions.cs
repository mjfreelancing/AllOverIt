using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.IO;
using AllOverIt.Process;
using AllOverIt.Process.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="D2ErdGenerator"/>.</summary>
    [ExcludeFromCodeCoverage]
    public static class D2ErdGeneratorExtensions
    {
        /// <summary>Exports the D2 diagram to a file along with any additional configured formats.</summary>
        /// <param name="generator">The entity relationship diagram generator.</param>
        /// <param name="dbContext">The source <see cref="DbContext"/> to generate an entity relationship diagram.</param>
        /// <param name="options">The D2 diagram export options.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A task that completes when the diagram export is completed.</returns>
        public static async Task ExportAsync(this D2ErdGenerator generator, DbContext dbContext, D2ErdExportOptions options,
            CancellationToken cancellationToken = default)
        {
            _ = dbContext.WhenNotNull(nameof(dbContext));
            _ = options.WhenNotNull(nameof(options));

            var content = generator.Generate(dbContext);

            await CreateDiagramFileAsync(content, options, cancellationToken).ConfigureAwait(false);
            await FormatDiagramFileAsync(options, cancellationToken).ConfigureAwait(false);
            await CreateAdditionalFormatsAsync(options, cancellationToken).ConfigureAwait(false);
        }

        private static Task CreateDiagramFileAsync(string content, D2ErdExportOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return FileUtils.CreateFileWithContentAsync(content, options.DiagramFileName, cancellationToken);
        }

        private static Task<ProcessExecutorResult> FormatDiagramFileAsync(D2ErdExportOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var formatFile = ProcessBuilder
               .For("d2.exe")
               .WithArguments("fmt", options.DiagramFileName)
               .WithStandardOutputHandler(options.StandardOutputHandler)        // null is OK
               .WithErrorOutputHandler(options.ErrorOutputHandler)              // null is OK
               .BuildProcessExecutor();

            return formatFile.ExecuteAsync(cancellationToken);
        }

        private static async Task CreateAdditionalFormatsAsync(D2ErdExportOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (options.Formats.IsNullOrEmpty())
            {
                return;
            }

            var layoutEngine = options.LayoutEngine.ToLowerInvariant();

            foreach (var format in options.Formats)
            {
                var extension = $"{format}".ToLowerInvariant();
                var exportFileName = Path.ChangeExtension(options.DiagramFileName, extension);

                var export = ProcessBuilder
                   .For("d2.exe")
                   .WithArguments("-l", layoutEngine, "-t", $"{(int) options.Theme}", options.DiagramFileName, exportFileName)
                   .WithStandardOutputHandler(options.StandardOutputHandler)    // null is OK
                   .WithErrorOutputHandler(options.ErrorOutputHandler)          // null is OK
                   .BuildProcessExecutor();

                await export.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}