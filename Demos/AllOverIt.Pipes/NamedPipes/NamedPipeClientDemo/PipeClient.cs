using AllOverIt.Extensions;
using AllOverIt.Pipes.Named.Client;
using AllOverIt.Pipes.Named.Events;
using AllOverIt.Pipes.Named.Serialization;
using AllOverIt.Reactive.Messaging;
using NamedPipeTypes;
using System.Reactive;
using System.Reactive.Linq;

namespace NamedPipeClientDemo
{
    internal class PipeClient
    {
        private static readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(5);

        private readonly EventBus _eventBus = new();
        private IDisposable? _eventSubscription;
        private CancellationTokenSource? _runningToken;
        private bool _paused;

        private PipeClient()
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
                            Text = $"{DateTime.Now.Ticks}",
                            PingBack = !_paused
                        };

                        PipeLogger.Append(ConsoleColor.Yellow, $"Client sending : {pipeMessage}");

                        await ping.Connection
                            .WriteAsync(pipeMessage, token)
                            .ConfigureAwait(false);
                    }
                    catch (IOException)
                    {
                        // A broken pipe
                    }
                    catch (ObjectDisposedException)
                    {
                        // Cannot access a closed pipe
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

        public static Task RunAsync(string pipeName, bool startPaused, bool useCustomReaderWriter)
        {
            // If the custom reader/writers are not register then a DynamicBinaryValueReader / DynamicBinaryValueWriter
            // will be created by EnrichedBinaryReader / EnrichedBinaryWriter. The customer reader / writer will be
            // more efficient in time and space as they can be tailored to the exact shape of the model and avoid
            // writing unnecessary type information.
            INamedPipeSerializer<PipeMessage> serializer = useCustomReaderWriter
                ? new PipeMessageSerializer()
                : new NamedPipeSerializer<PipeMessage>();

            var pipeClient = new PipeClient();

            return pipeClient.RunAsync(pipeName, startPaused, serializer);
        }

        private async Task RunAsync(string pipeName, bool startPaused, INamedPipeSerializer<PipeMessage> serializer)
        {
            PipeLogger.Append(ConsoleColor.Gray, $"Running in CLIENT mode. PipeName: {pipeName}");
            PipeLogger.Append(ConsoleColor.Gray, "Enter 'run' to auto send messages");
            PipeLogger.Append(ConsoleColor.Gray, "Enter 'pause' to manually send messages");
            PipeLogger.Append(ConsoleColor.Gray, "Enter 'quit' to exit");

            _paused = startPaused;

            try
            {
                var namedPipeClientFactory = new NamedPipeClientFactory<PipeMessage>(serializer);

                using (_runningToken = new CancellationTokenSource())
                {
                    // Could also use this (without the NamedPipeClientFactory)
                    // var client = new NamedPipeClient<PipeMessage>(pipeName, serializer);
                    await using (var client = namedPipeClientFactory.CreateNamedPipeClient(pipeName))
                    {
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
                                    var message = await Console.In
                                        .ReadLineAsync(_runningToken.Token)
                                        .ConfigureAwait(false);

                                    if (message.IsNullOrEmpty())
                                    {
                                        continue;
                                    }

                                    if (message == "pause")
                                    {
                                        _paused = true;
                                    }

                                    if (message == "run")
                                    {
                                        _paused = false;
                                    }

                                    if (message == "quit")
                                    {
                                        _runningToken.Cancel();
                                        break;
                                    }

                                    var pipeMessage = new PipeMessage
                                    {
                                        Text = message!,
                                        PingBack = !_paused
                                    };

                                    PipeLogger.Append(ConsoleColor.Yellow, $"Client sending : {pipeMessage}");

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

                        await client.ConnectAsync(ConnectTimeout, _runningToken.Token).ConfigureAwait(false);

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

            _eventSubscription!.Dispose();
            _eventSubscription = null;
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

        private static void DoOnConnected(object? sender, NamedPipeConnectionEventArgs<PipeMessage, INamedPipeClientConnection<PipeMessage>> args)
        {
            PipeLogger.Append(ConsoleColor.Blue, "Connected to server");
        }

        private void DoOnDisconnected(object? sender, NamedPipeConnectionEventArgs<PipeMessage, INamedPipeClientConnection<PipeMessage>> args)
        {
            PipeLogger.Append(ConsoleColor.Magenta, "Disconnected from server");
        }

        private void DoOnMessageReceived(object? sender, NamedPipeConnectionMessageEventArgs<PipeMessage, INamedPipeClientConnection<PipeMessage>> args)
        {
            PipeLogger.Append(ConsoleColor.Green, $"Client received: {args.Message}");

            if (_paused || _runningToken!.Token.IsCancellationRequested)
            {
                return;
            }

            var connection = args.Connection;

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