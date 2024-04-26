using AllOverIt.Async;
using AllOverIt.Pipes.Anonymous;
using AllOverIt.Process;
using AllOverIt.Process.Extensions;
using System.Diagnostics;
using System.Reflection;

namespace AnonymousPipeServerDemo
{
    internal class Program
    {
        static async Task Main()
        {
            LogMessage("Server Started");

            Process clientProcess;
            Task backgroundTask;

            using var cts = new CancellationTokenSource();

            using (var pipeServer = new AnonymousPipeServer())
            {
                // Start the pipe server and get the client handle (the client app needs this to connect back to the server). 
                var clientHandle = pipeServer.Start(inheritability: HandleInheritability.Inheritable);

                // Determine the location of the client demo to launch.
                var clientPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("Server", "Client");
                var clientExePath = Path.Combine(clientPath, "AnonymousPipeClientDemo.exe");

                LogMessage("Server launching client app...");

                // Launch the client application, giving it the client handle provided by the pipe server.
                clientProcess = ProcessBuilder
                    .For(clientExePath)
                    .WithArguments(clientHandle)
                    .Start();

                LogMessage("Server started Client, sending handshake...");

                // Send a handshake message.
                pipeServer.Writer.WriteLine("Handshake");

                // And wait for the client to completely read it.
                pipeServer.WaitForPipeDrain();

                LogMessage("Server completed handshake with the Client.");

                // Give some time for the client demo to log to the console
                await Task.Delay(500);

                string message;

                backgroundTask = StartBackgroundCommunication(pipeServer, cts.Token);

                do
                {
                    LogMessage("Type a message to send to the client (type 'quit' to exit): ");

                    message = Console.ReadLine();

                    pipeServer.Writer.WriteLine(message);

                } while (!message.Equals("quit", StringComparison.InvariantCultureIgnoreCase));

                cts.Cancel();

                await backgroundTask;
            }

            clientProcess.WaitForExit();
            clientProcess.Close();

            LogMessage("Server terminated");

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static Task StartBackgroundCommunication(AnonymousPipeServer pipeServer, CancellationToken cancellationToken)
        {
            var options = new RepeatingTaskOptions
            {
                InitialDelay = TimeSpan.FromSeconds(10),
                RepeatDelay = TimeSpan.FromSeconds(10)
            };

            return RepeatingTask.StartAsync(() =>
            {
                pipeServer.Writer.WriteLine($"Server sent a message at {DateTime.Now:T}");
            }, options, cancellationToken);
        }

        private static void LogMessage(string message)
        {
            Console.WriteLine($"{DateTime.Now:o} {message}");
        }
    }
}
