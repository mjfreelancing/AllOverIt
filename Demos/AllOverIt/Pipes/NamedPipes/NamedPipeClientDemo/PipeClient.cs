﻿using AllOverIt.Pipes.Client;
using AllOverIt.Pipes.Connection;
using AllOverIt.Pipes.Events;
using AllOverIt.Pipes.Serialization;
using AllOverIt.Pipes.Serialization.Binary;
using NamedPipeTypes;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NamedPipeDemo
{
    internal static class PipeClient
    {
        public static async Task RunAsync(string pipeName, bool useCustomReaderWriter = true)
        {
            // If the custom reader/writers are not register then a DynamicBinaryValueReader / DynamicBinaryValueWriter
            // will be created by EnrichedBinaryReader / EnrichedBinaryWriter. The customer reader / writer will be
            // more efficient in time and space as they can be tailored to the exact shape of the model and avoid
            // writing unnecessary type information.
            IMessageSerializer<PipeMessage> serializer = useCustomReaderWriter
                ? new PipeMessageSerializer()
                : new BinaryMessageSerializer<PipeMessage>();

            try
            {
                using (var source = new CancellationTokenSource())
                {
                    Console.WriteLine($"Running in CLIENT mode. PipeName: {pipeName}");
                    Console.WriteLine("Enter 'quit' to exit");

                    await using var client = new PipeClient<PipeMessage>(pipeName, serializer);

                    client.OnConnected += DoOnClientConnected;
                    client.OnDisconnected += DoOnClientDisconnected;
                    
                    client.OnMessageReceived += (o, args) => PipeLogger.Append(ConsoleColor.Red, $"Received: {args.Message}");

                    client.OnException += (o, args) => DoOnException(args.Exception);

                    _ = Task.Run(async () =>
                    {
                        while (!source.Token.IsCancellationRequested)
                        {
                            try
                            {
                                var message = await Console.In.ReadLineAsync().ConfigureAwait(false);

                                if (message == "quit")
                                {
                                    source.Cancel();
                                    break;
                                }

                                var pipeMessage = new PipeMessage
                                {
                                    Text = message
                                };

                                PipeLogger.Append(ConsoleColor.Yellow, $"Sending : {pipeMessage}");

                                await client
                                    .WriteAsync(pipeMessage, source.Token)
                                    .ConfigureAwait(false);
                            }
                            catch (Exception exception)
                            {
                                DoOnException(exception);
                                source.Cancel();
                            }
                        }
                    }, source.Token);

                    Console.WriteLine("Client connecting...");

                    await client.ConnectAsync(source.Token).ConfigureAwait(false);

                    Console.WriteLine("Client connected");

                    // Wait until the user quites with 'q'
                    await WaitForCancellationAsync(source.Token).ConfigureAwait(false);

                    // When the client is disposed it will shut down
                    Console.WriteLine("Disposing Client...");
                }

                Console.WriteLine("Client Stopped!");
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                DoOnException(exception);
            }
        }

        private static async Task WaitForCancellationAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private static void DoOnClientConnected(object sender, ConnectionEventArgs<PipeMessage, IPipeClientConnection<PipeMessage>> args)
        {
            Console.WriteLine("Connected to server");

            SendConnectionRegularMessages(args.Connection);
        }

        private static void DoOnClientDisconnected(object sender, ConnectionEventArgs<PipeMessage, IPipeClientConnection<PipeMessage>> args)
        {
            Console.WriteLine("Disconnected from server");
        }

        private static void SendConnectionRegularMessages(IPipeConnection<PipeMessage> connection)
        {
            var subscription = Observable
                .Interval(TimeSpan.FromMilliseconds(100))
                .TakeWhile(_ => connection.IsConnected)
                .Take(5)
                .SelectMany(async async =>
                {
                    try
                    {
                        var pipeMessage = new PipeMessage
                        {
                            Id = Guid.NewGuid(),
                            Text = $"{DateTime.Now.Ticks}"
                        };

                        PipeLogger.Append(ConsoleColor.Yellow, $"Sending : {pipeMessage}");

                        await connection
                            .WriteAsync(pipeMessage)
                            .ConfigureAwait(false);
                    }
                    catch (IOException)
                    {
                        // Most likely a broken pipe
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception exception)
                    {
                        // It's possible an internal semaphore in PipeStreamWriter may throw an ObjectDisposedException
                        // if the server is terminated and the pipe is broken.
                        Console.WriteLine(exception.Message);
                    }

                    return Unit.Default;
                })
                .Subscribe();
        }


        private static void DoOnException(Exception exception)
        {
            Console.Error.WriteLine($"Exception: {exception}");
        }
    }   
}