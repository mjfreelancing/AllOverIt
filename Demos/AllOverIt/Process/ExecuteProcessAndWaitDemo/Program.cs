using AllOverIt.Process;
using AllOverIt.Process.Extensions;
using System.Diagnostics;

namespace ExecuteProcessAndWaitDemo
{
    class Program
    {
        static async Task Main()
        {
            // Example of launching a process without waiting
            // var process = ProcessBuilder
            //     .For("notepad.exe")
            //     .WithWorkingDirectory(@"C:\Windows")
            //     .Start();

            Console.WriteLine("Close notepad within 10 seconds so this console application can complete");

            var cts = new CancellationTokenSource(10000);

            var notepadTask = OpenNotepadAndWaitAsync(cts.Token);
            var cmdOutputTask = CaptureCmdOutputAsync(cts.Token);

            await Task.WhenAll(notepadTask, cmdOutputTask);

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static async Task OpenNotepadAndWaitAsync(CancellationToken cancellationToken)
        {
            //        var p = ProcessBuilder
            //            .For(processFilename)
            //            .WithWorkingDirectory(workingDirectory)
            //            .WithArguments(arguments)
            //            .WithStandardOutputHandler(Process_OutputDataReceived)
            //            .WithErrorOutputHandler(Process_ErrorDataReceived)
            //            .Build();

            //        var r1 = await p.ExecuteAsync(new CancellationTokenSource(timeoutMilliseconds).Token);

            //        var r2 = await p.ExecuteBufferedAsync(new CancellationTokenSource(timeoutMilliseconds).Token);

            try
            {
                // The commented code below shows an alternative approach to capturing the output.

                var process = ProcessBuilder
                    .For("notepad.exe")
                    .WithWorkingDirectory(@"C:\Windows")
                    //.WithStandardOutputHandler(Process_OutputDataReceived)
                    //.WithErrorOutputHandler(Process_ErrorDataReceived)
                    .BuildProcessExecutor();

                //var result = await p.ExecuteAsync(cancellationToken);

                var result = await process.ExecuteBufferedAsync(cancellationToken);

                //var output = await Process.ExecuteAndWaitAsync(@"C:\Windows", "notepad.exe", string.Empty, 10000);

                Console.WriteLine(result.StandardOutput);
                Console.WriteLine($"Exit Code = {result.ExitCode}");
            }
            catch (TimeoutException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private static async Task CaptureCmdOutputAsync(CancellationToken cancellationToken)
        {
            // The commented code below shows an alternative approach to capturing the output.

            var process = ProcessBuilder
                .For("cmd.exe")
                .WithArguments("/c", "dir", "/s")
                .WithStandardOutputHandler(LogStandardOutput)
                .WithErrorOutputHandler(LogErrorOutput)
                .BuildProcessExecutor();

            var result = await process.ExecuteAsync(cancellationToken);

            //var result = await process.ExecuteBufferedAsync(cancellationToken);
            //Console.WriteLine(result.StandardOutput);

            Console.WriteLine($"Exit Code = {result.ExitCode}");

            //var output = await Process.ExecuteAndWaitAsync(string.Empty, "cmd.exe", @"/c dir /s", -1);

            //Console.WriteLine(output.StandardOutput);
        }

        private static void LogStandardOutput(object sender, DataReceivedEventArgs e)
        {
            if (e.Data is not null)
            {
                Console.WriteLine(e.Data);
            }
        }

        private static void LogErrorOutput(object sender, DataReceivedEventArgs e)
        {
            if (e.Data is not null)
            {
                Console.WriteLine($"Error: {e.Data}");
            }
        }
    }
}
