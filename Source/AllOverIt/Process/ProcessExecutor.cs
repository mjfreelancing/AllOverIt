using AllOverIt.Assertion;
using AllOverIt.Process.Exceptions;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using SystemProcess = System.Diagnostics.Process;

namespace AllOverIt.Process
{
    /// <summary>Implements a process executor that supports argument passing, setting a working directory, using process
    /// specific environments and can capture standard and error output as a buffered result or asynchronously.</summary>
    public sealed class ProcessExecutor : IProcessExecutor
    {
        [ExcludeFromCodeCoverage]
        private sealed class DataOutputBuffer
        {
            private readonly StringBuilder _dataOutput = new();

            public void OnDataReceived(object sender, DataReceivedEventArgs eventArgs)
            {
                if (eventArgs.Data is not null)
                {
                    _dataOutput.AppendLine(eventArgs.Data);
                }
            }

            public override string ToString()
            {
                return _dataOutput.ToString();
            }
        }

        private bool _disposed;
        internal readonly SystemProcess _process;
        internal readonly ProcessExecutorOptions _options;

        /// <summary>Constructor.</summary>
        /// <param name="options">The options used to configure the executor.</param>
        public ProcessExecutor(ProcessExecutorOptions options)
        {
            _options = options.WhenNotNull();

            _process = ProcessFactory.CreateProcess(_options);
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public async Task<ProcessExecutorResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await DoExecuteAsync(_options.StandardOutputHandler, _options.ErrorOutputHandler, cancellationToken).ConfigureAwait(false);

            return new ProcessExecutorResult(_process);
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public Task<ProcessExecutorBufferedResult> ExecuteBufferedAsync(CancellationToken cancellationToken = default)
        {
            return DoExecuteBufferedAsync(cancellationToken);
        }

        /// <summary>Disposes of process resources.</summary>
        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            if (!_disposed)
            {
                _process.Close();
                _process.Dispose();

                _disposed = true;
            }
        }

        [ExcludeFromCodeCoverage]
        private async Task DoExecuteAsync(DataReceivedEventHandler? standardOutputHandler, DataReceivedEventHandler? errorOutputHandler,
            CancellationToken cancellationToken)
        {
            if (standardOutputHandler is not null)
            {
                _process.StartInfo.RedirectStandardOutput = true;
                _process.OutputDataReceived += standardOutputHandler;
            }

            if (errorOutputHandler is not null)
            {
                _process.StartInfo.RedirectStandardError = true;
                _process.ErrorDataReceived += errorOutputHandler;
            }

            _process.Start();

            if (standardOutputHandler is not null)
            {
                _process.BeginOutputReadLine();
            }

            if (errorOutputHandler is not null)
            {
                _process.BeginErrorReadLine();
            }

            Exception? processException = null;

            try
            {
                var milliseconds = (int) _options.Timeout.TotalMilliseconds;

                // Cater for an explicit timeout for the scenario where the provided cancellationToken does not have an associated timeout (via a CancellationTokenSource)
                if (milliseconds != 0)        // -1 means indefinite
                {
                    using var cts = new CancellationTokenSource(milliseconds);
                    using var linked = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);

                    await WaitForProcessAsync(linked.Token).ConfigureAwait(false);
                }
                else
                {
                    await WaitForProcessAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                KillProcess();

                throw new TimeoutException("Process wait timeout expired.");
            }
            catch (Exception exception)
            {
                processException = exception;
            }
            finally
            {
                if (standardOutputHandler is not null)
                {
                    _process.CancelOutputRead();
                    _process.OutputDataReceived -= standardOutputHandler;
                }

                if (errorOutputHandler is not null)
                {
                    _process.CancelErrorRead();
                    _process.ErrorDataReceived -= errorOutputHandler;
                }
            }

            if (processException is not null)
            {
                throw new ProcessException("Process execution failed.", processException);
            }
        }

        [ExcludeFromCodeCoverage]
        private async Task<ProcessExecutorBufferedResult> DoExecuteBufferedAsync(CancellationToken cancellationToken)
        {
            var standardOutput = new DataOutputBuffer();
            var errorOutput = new DataOutputBuffer();

            await DoExecuteAsync(standardOutput.OnDataReceived, errorOutput.OnDataReceived, cancellationToken);

            return new ProcessExecutorBufferedResult(_process, standardOutput.ToString(), errorOutput.ToString());
        }

        [ExcludeFromCodeCoverage]
        private Task WaitForProcessAsync(CancellationToken cancellationToken)
        {
            return _process.WaitForExitAsync(cancellationToken);
        }

        [ExcludeFromCodeCoverage]
        private void KillProcess()
        {
            _process.Kill(true);
        }
    }
}
