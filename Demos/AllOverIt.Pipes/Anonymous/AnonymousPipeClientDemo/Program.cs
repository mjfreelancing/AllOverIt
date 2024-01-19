using AllOverIt.Pipes.Anonymous;
using System;

namespace AnonymousPipeClientDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LogMessage("Client Started");

            // This client app is launched by the anonymous pipe server demo, where it sends a client handle.
            if (args.Length > 0)
            {
                using (var pipeClient = new AnonymousPipeClient())
                {
                    pipeClient.Start(args[0]);

                    LogMessage("Client waiting for handshake...");

                    var message = pipeClient.Reader.ReadLine();

                    if (!message.Equals("Handshake", StringComparison.InvariantCultureIgnoreCase))
                    {
                        LogMessage("Client received an invalid handshake message.");
                    }
                    else
                    {
                        LogMessage("Client ready to receive messages...");

                        // Read the server message and echo to the console - until 'quit' is received.
                        do
                        {
                            message = pipeClient.Reader.ReadLine();

                            LogMessage($"Client received: {message}");
                        } while (!message.Equals("quit", StringComparison.InvariantCultureIgnoreCase));
                    }
                }

                LogMessage("Client terminated");
            }
        }

        private static void LogMessage(string message)
        {
            Console.WriteLine($"{DateTime.Now:o} {message}");
        }
    }
}
