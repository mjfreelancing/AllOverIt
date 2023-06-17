﻿using AllOverIt.Pipes.Named.Client;
using AllOverIt.Pipes.Named.Events;
using AllOverIt.Pipes.Serialization;
using AllOverIt.Pipes.Serialization.Binary;
using NamedPipeTypes;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NamedPipeClientDemo
{
    internal class PipeClient
    {
        private CancellationTokenSource _runningToken;
        private IDisposable _pingSubscription;

        private PipeClient()
        {            
        }

        public static Task RunAsync(string pipeName, bool useCustomReaderWriter = true)
        {
            // If the custom reader/writers are not register then a DynamicBinaryValueReader / DynamicBinaryValueWriter
            // will be created by EnrichedBinaryReader / EnrichedBinaryWriter. The customer reader / writer will be
            // more efficient in time and space as they can be tailored to the exact shape of the model and avoid
            // writing unnecessary type information.
            IMessageSerializer<PipeMessage> serializer = useCustomReaderWriter
                ? new PipeMessageSerializer()
                : new BinaryMessageSerializer<PipeMessage>();

            var pipeClient = new PipeClient();

            return pipeClient.RunAsync(pipeName, serializer);
        }

        private async Task RunAsync(string pipeName, IMessageSerializer<PipeMessage> serializer)
        {
            PipeLogger.Append(ConsoleColor.Gray, $"Running in CLIENT mode. PipeName: {pipeName}");
            PipeLogger.Append(ConsoleColor.Gray, "Enter 'quit' to exit");

            try
            {
                using (_runningToken = new CancellationTokenSource())
                {
                    await using var client = new PipeClient<PipeMessage>(pipeName, serializer);

                    client.OnConnected += DoOnConnected;
                    client.OnDisconnected += DoOnDisconnected;
                    client.OnMessageReceived += DoOnMessageReceived;
                    client.OnException += DoOnException;

                    var runningTask = Task.Run(async () =>
                    {
                        while (!_runningToken.Token.IsCancellationRequested)
                        {
                            try
                            {
                                var message = await Console.In.ReadLineAsync().ConfigureAwait(false);

                                if (message == "quit")
                                {
                                    _runningToken.Cancel();
                                    break;
                                }

                                var pipeMessage = new PipeMessage
                                {
                                    Text = message
                                };

                                PipeLogger.Append(ConsoleColor.Yellow, $"Sending : {pipeMessage}");

                                await client
                                    .WriteAsync(pipeMessage, _runningToken.Token)
                                    .ConfigureAwait(false);
                            }
                            catch (Exception exception)
                            {
                                DoOnException(exception);
                                _runningToken.Cancel();
                            }
                        }
                    }, _runningToken.Token);

                    PipeLogger.Append(ConsoleColor.Gray, "Client connecting...");

                    await client.ConnectAsync(_runningToken.Token).ConfigureAwait(false);

                    PipeLogger.Append(ConsoleColor.Gray, "Client connected");

                    await WaitForUserQuit().ConfigureAwait(false);

                    await runningTask.ConfigureAwait(false);

                    client.OnConnected -= DoOnConnected;
                    client.OnDisconnected -= DoOnDisconnected;
                    client.OnMessageReceived -= DoOnMessageReceived;
                    client.OnException -= DoOnException;

                    // When the client is disposed it will shut down
                    PipeLogger.Append(ConsoleColor.Gray, "Disposing Client...");
                }

                PipeLogger.Append(ConsoleColor.Gray, "Client Stopped!");
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                DoOnException(exception);
            }
        }

        private async Task WaitForUserQuit()
        {
            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, _runningToken.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private static void DoOnConnected(object sender, ConnectionEventArgs<PipeMessage, IPipeClientConnection<PipeMessage>> args)
        {
            PipeLogger.Append(ConsoleColor.Blue, "Connected to server");
        }

        private void DoOnDisconnected(object sender, ConnectionEventArgs<PipeMessage, IPipeClientConnection<PipeMessage>> args)
        {
            _pingSubscription?.Dispose();
            _pingSubscription = null;

            PipeLogger.Append(ConsoleColor.Magenta, "Disconnected from server");
        }

        private void DoOnMessageReceived(object sender, ConnectionMessageEventArgs<PipeMessage, IPipeClientConnection<PipeMessage>> args)
        {
            PipeLogger.Append(ConsoleColor.Green, $"Received: {args.Message}");

            if (_runningToken.Token.IsCancellationRequested)
            {
                return;
            }

            var connection = args.Connection;

            // Ping back
            _pingSubscription = Observable
                .Timer(TimeSpan.FromMilliseconds(10))
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
                        PipeLogger.Append(ConsoleColor.Gray, exception.Message);
                    }

                    return Unit.Default;
                })
                .Subscribe();
        }

        private static void DoOnException(Exception exception)
        {
            Console.Error.WriteLine($"Exception: {exception}");
        }

        private void DoOnException(object sender, ExceptionEventArgs args)
        {
            DoOnException(args.Exception);
        }
    }   
}