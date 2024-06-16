using AllOverIt.Extensions;
using AllOverIt.Pipes.Named.Events;
using AllOverIt.Pipes.Named.Extensions;
using AllOverIt.Pipes.Named.Serialization;
using AllOverIt.Pipes.Named.Server;
using AllOverIt.Reactive.Messaging;
using NamedPipeTypes;
using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.AccessControl;
using System.Security.Principal;

namespace NamedPipeServerDemo
{
    internal class PipeServer
    {
        private readonly EventBus _eventBus = new();
        private IDisposable? _eventSubscription;
        private CancellationTokenSource? _runningToken;
        private ConcurrentDictionary<string, bool> _pendingConnectionPings = [];

        private PipeServer()
        {
            _eventSubscription = _eventBus
                .GetEvent<PingMessage>()
                .SelectMany(async (ping, _, token) =>
                {
                    try
                    {
                        var pipeMessage = new PipeMessage
                        {
                            Id = Guid.NewGuid(),
                            Text = $"{DateTime.Now.Ticks}"
                        };

                        PipeLogger.Append(ConsoleColor.Green, $"Server sending : {pipeMessage}");

                        var success = _pendingConnectionPings.TryGetValue(ping.Connection.ConnectionId, out var isConnected);

                        // success should always be true, but there is still a potential race condition between writing to the
                        // connection, and that connection being broken - such as if the client application is terminated while
                        // the ping messages are active. If this happens, the app could crash - not handling that scenario in
                        // this demo application. The issue is not present if the client applications are gracefully quit and
                        // terminated.
                        if (success && isConnected)
                        {
                            await ping.Connection
                                .WriteAsync(pipeMessage, token)
                                .ConfigureAwait(false);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception exception)
                    {
                        // IOException              - A broken pipe
                        // ObjectDisposedException  - Cannot access a closed pipe
                        PipeLogger.Append(ConsoleColor.Gray, exception.Message);
                    }

                    return Unit.Default;
                })
                .Subscribe();
        }

        public static Task RunAsync(string pipeName, bool useCustomReaderWriter = true)
        {
            // If the custom reader/writers are not register then a DynamicBinaryValueReader / DynamicBinaryValueWriter
            // will be created by EnrichedBinaryReader / EnrichedBinaryWriter. The customer reader / writer will be
            // more efficient in time and space as they can be tailored to the exact shape of the model and avoid
            // writing unnecessary type information.
            INamedPipeSerializer<PipeMessage> serializer = useCustomReaderWriter
                ? new PipeMessageSerializer()
                : new NamedPipeSerializer<PipeMessage>();

            var pipeServer = new PipeServer();

            return pipeServer.RunAsync(pipeName, serializer);
        }

        private async Task RunAsync(string pipeName, INamedPipeSerializer<PipeMessage> serializer)
        {
            PipeLogger.Append(ConsoleColor.Gray, $"Running in SERVER mode. PipeName: {pipeName}");
            PipeLogger.Append(ConsoleColor.Gray, "Enter 'quit' to exit");

            try
            {
                var namedPipeServerFactory = new NamedPipeServerFactory<PipeMessage>(serializer);

                using (_runningToken = new CancellationTokenSource())
                {
                    // Could also use this (without the NamedPipeServerFactory)
                    // var server = new NamedPipeServer<PipeMessage>(pipeName, serializer);
                    await using (var server = namedPipeServerFactory.CreateNamedPipeServer(pipeName))
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
                                    var message = await Console.In
                                        .ReadLineAsync(_runningToken.Token)
                                        .ConfigureAwait(false);

                                    if (message.IsNullOrEmpty())
                                    {
                                        continue;
                                    }

                                    if (message == "quit")
                                    {
                                        _runningToken.Cancel();

                                        PipeLogger.Append(ConsoleColor.Gray, "Quitting...");

                                        break;
                                    }

                                    var pipeMessage = new PipeMessage
                                    {
                                        Text = message!,
                                    };

                                    PipeLogger.Append(ConsoleColor.Red, $" >> Sent: {pipeMessage}");

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
                            pipeSecurity.AddIdentityAccessRule(WellKnownSidType.BuiltinUsersSid, PipeAccessRights.ReadWrite, AccessControlType.Allow);
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

            _eventSubscription?.Dispose();
            _eventSubscription = null;

            PipeLogger.Append(ConsoleColor.Gray, "Server Stopped!");
        }

        private async Task WaitForUserQuit()
        {
            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, _runningToken!.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private static async void DoOnClientConnected(object? server, NamedPipeConnectionEventArgs<PipeMessage, INamedPipeServerConnection<PipeMessage>> args)
        {
            var connection = args.Connection;

            try
            {
                PipeLogger.Append(ConsoleColor.Blue, $"Client connection {connection.ConnectionId} is now connected.");

                var pipeMessage = new PipeMessage
                {
                    Text = "Welcome!"
                };

                PipeLogger.Append(ConsoleColor.Green, $"Sending : {pipeMessage}");

                await connection.WriteAsync(pipeMessage).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await connection.DisconnectAsync().ConfigureAwait(false);

                DoOnException(exception);
            }
        }

        private void DoOnClientDisconnected(object? server, NamedPipeConnectionEventArgs<PipeMessage, INamedPipeServerConnection<PipeMessage>> args)
        {
            var connection = args.Connection;

            _pendingConnectionPings.AddOrUpdate(connection.ConnectionId, _ => false, (_, _) => false);

            PipeLogger.Append(ConsoleColor.Magenta, $"Client connection {connection.ConnectionId} disconnected");
        }

        private void DoOnMessageReceived(object? sender, NamedPipeConnectionMessageEventArgs<PipeMessage, INamedPipeServerConnection<PipeMessage>> args)
        {
            var connection = args.Connection;

            PipeLogger.Append(ConsoleColor.Yellow, $"Server received: {args.Message} from {connection.ConnectionId} (as '{connection.GetImpersonationUserName()}')");

            if (!args.Message.PingBack || _runningToken!.Token.IsCancellationRequested)
            {
                return;
            }

            _pendingConnectionPings.AddOrUpdate(connection.ConnectionId, _ => true, (_, _) => true);

            // Ping back
            var message = new PingMessage
            {
                Connection = connection
            };

            _eventBus.Publish(message);
        }

        private static void DoOnException(Exception exception)
        {
            Console.Error.WriteLine($"Exception: {exception}");
        }

        private void DoOnException(object? sender, NamedPipeExceptionEventArgs args)
        {
            DoOnException(args.Exception);
        }
    }
}