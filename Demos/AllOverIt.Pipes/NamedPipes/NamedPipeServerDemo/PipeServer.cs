﻿using AllOverIt.Pipes.Named.Events;
using AllOverIt.Pipes.Named.Extensions;
using AllOverIt.Pipes.Serialization;
using AllOverIt.Pipes.Serialization.Binary;
using AllOverIt.Pipes.Named.Server;
using AllOverIt.Reactive;
using NamedPipeTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace NamedPipeServerDemo
{
    internal class PipeServer
    {
        private readonly ConcurrentDictionary<IPipeServerConnection<PipeMessage>, IDisposable> _pingSubscriptions = new();
        private CancellationTokenSource _runningToken;

        private PipeServer()
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

            var pipeServer = new PipeServer();

            return pipeServer.RunAsync(pipeName, serializer);
        }

        private async Task RunAsync(string pipeName, IMessageSerializer<PipeMessage> serializer)
        {
            PipeLogger.Append(ConsoleColor.Gray, $"Running in SERVER mode. PipeName: {pipeName}");
            PipeLogger.Append(ConsoleColor.Gray, "Enter 'quit' to exit");

            try
            {
                using (_runningToken = new CancellationTokenSource())
                {
                    await using (var server = new PipeServer<PipeMessage>(pipeName, serializer))
                    {
                        PipeLogger.Append(ConsoleColor.Gray, "Server starting...");

                        server.OnClientConnected += DoOnClientConnected;
                        server.OnClientDisconnected += DoOnClientDisconnected;
                        server.OnMessageReceived += DoOnMessageReceived;
                        server.OnException += DoOnException;

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

                                        PipeLogger.Append(ConsoleColor.Gray, "Quiting...");

                                        break;
                                    }

                                    // PipeLogger.Append(ConsoleColor.Gray, $"Sent to {server.ConnectedClients.Count} clients");

                                    var pipeMessage = new PipeMessage
                                    {
                                        Text = message,
                                    };

                                    PipeLogger.Append(ConsoleColor.Red, $" >> Sent with Id {pipeMessage}");

                                    await server.WriteAsync(pipeMessage, _runningToken.Token).ConfigureAwait(false);
                                }
                                catch (Exception exception)
                                {
                                    DoOnException(exception);
                                }
                            }
                        }, _runningToken.Token);

                        server.Start(pipeSecurity =>
                        {
#pragma warning disable CA1416 // Validate platform compatibility
                            pipeSecurity.AddIdentityAccessRule(WellKnownSidType.BuiltinUsersSid, PipeAccessRights.ReadWrite, AccessControlType.Allow);
#pragma warning restore CA1416 // Validate platform compatibility
                        });

                        PipeLogger.Append(ConsoleColor.Gray, "Server is started!");

                        // Wait until the user quits
                        await WaitForUserQuit().ConfigureAwait(false);

                        await runningTask.ConfigureAwait(false);

                        // When the server is disposed it will shut down or we can explicitly disconnect all clients first
                        await server.StopAsync().ConfigureAwait(false);

                        server.OnClientConnected -= DoOnClientConnected;
                        server.OnClientDisconnected -= DoOnClientDisconnected;
                        server.OnMessageReceived -= DoOnMessageReceived;
                        server.OnException -= DoOnException;

                        PipeLogger.Append(ConsoleColor.Gray, "Disposing Server...");
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                DoOnException(exception);
            }

            PipeLogger.Append(ConsoleColor.Gray, "Server Stopped!");
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

        private static void DoOnClientConnected(object server, ConnectionEventArgs<PipeMessage, IPipeServerConnection<PipeMessage>> args)
        {
            var connection = args.Connection;

            TaskHelper.ExecuteAsyncAndWait(async () =>
            {
                try
                {
                    PipeLogger.Append(ConsoleColor.Blue, $"Client {connection.PipeName} is now connected.");

                    var pipeMessage = new PipeMessage
                    {
                        Text = "Welcome!"
                    };

                    await connection.WriteAsync(pipeMessage).ConfigureAwait(false);

                    PipeLogger.Append(ConsoleColor.Green, $"Sending : {pipeMessage}");
                }
                catch (Exception exception)
                {
                    await connection.DisconnectAsync().ConfigureAwait(false);

                    DoOnException(exception);
                }
            });
        }

        private void DoOnClientDisconnected(object server, ConnectionEventArgs<PipeMessage, IPipeServerConnection<PipeMessage>> args)
        {
            var connection = args.Connection;

            _pingSubscriptions.Remove(connection, out _);

            PipeLogger.Append(ConsoleColor.Magenta, $"Client {args.Connection.PipeName} disconnected");
        }

        private void DoOnMessageReceived(object sender, ConnectionMessageEventArgs<PipeMessage, IPipeServerConnection<PipeMessage>> args)
        {
            var connection = args.Connection;

            PipeLogger.Append(ConsoleColor.Yellow, $"Received: {args.Message} from {connection.PipeName} (as '{connection.GetImpersonationUserName()}')");

            if (_runningToken.Token.IsCancellationRequested)
            {
                _pingSubscriptions.Remove(connection, out _);
                return;
            }

            // Ping back
            var subscription = Observable
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

                        PipeLogger.Append(ConsoleColor.Green, $"Sending : {pipeMessage}");

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

            _pingSubscriptions.AddOrUpdate(connection, c => subscription, (c, d) => subscription);
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