using AllOverIt.Async;
using AllOverIt.Fixture;
using AllOverIt.Pipes.Exceptions;
using AllOverIt.Pipes.Named.Client;
using AllOverIt.Pipes.Named.Events;
using AllOverIt.Pipes.Named.Serialization;
using AllOverIt.Pipes.Named.Server;
using FluentAssertions;
using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Tests.Named
{
    [Collection("NamedPipes")]
    public class NamedPipeFixture_Mixed_Functional : FixtureBase, IAsyncLifetime
    {
        private class DummyMessage
        {
            public string Value { get; set; }
        }

        private sealed class DummyStream : PipeStream
        {
            public DummyStream()
                : base(PipeDirection.InOut, 1024)
            {
            }
        }

        private sealed class DummyBadSerializer : INamedPipeSerializer<DummyMessage>
        {
            private readonly PipeDirection _errorDirection;

            public DummyBadSerializer(PipeDirection errorDirection)
            {
                _errorDirection = errorDirection;
            }

            DummyMessage INamedPipeSerializer<DummyMessage>.Deserialize(byte[] bytes)
            {
                if (_errorDirection == PipeDirection.In)
                {
                    throw new InvalidOperationException();
                }

                return new DummyMessage();
            }

            byte[] INamedPipeSerializer<DummyMessage>.Serialize(DummyMessage message)
            {
                if (_errorDirection == PipeDirection.Out)
                {
                    throw new InvalidOperationException();
                }

                return new byte[] { 1, 2, 3 };
            }
        }

        private static TimeSpan ConnectTimeout = TimeSpan.FromSeconds(1);
        private CancellationTokenSource _timeoutSource;

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Should_Return_Server_Connected_Status(bool connected)
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.IsStarted.Should().BeFalse();

                if (connected)
                {
                    server.Start();
                }

                server.IsStarted.Should().Be(connected);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Should_Return_Client_Connected_Status(bool connectToServer)
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = Create<DummyMessage>();

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    server.Start();

                    mre1.Set();

                    mre2.Wait();
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    client.IsConnected.Should().BeFalse();

                    mre1.Wait();

                    if (connectToServer)
                    {
                        await client.ConnectAsync(ConnectTimeout);
                    }

                    client.IsConnected.Should().Be(connectToServer);
                }

                mre2.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);
        }

        [Fact]
        public async Task Should_Throw_When_Already_Connected()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = Create<DummyMessage>();

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    server.Start();

                    mre1.Set();

                    mre2.Wait();
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    mre1.Wait();

                    await client.ConnectAsync(ConnectTimeout);

                    await Invoking(async () =>
                    {
                        await client.ConnectAsync(ConnectTimeout);
                    })
                    .Should()
                    .ThrowAsync<PipeException>()
                    .WithMessage("The named pipe client is already connected.");

                    client.IsConnected.Should().BeTrue();
                }

                mre2.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);
        }

        [Fact]
        public async Task Should_Disconnect_Client_From_Server()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = Create<DummyMessage>();

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    server.Start();

                    mre1.Set();

                    mre2.Wait();
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    mre1.Wait();

                    await client.ConnectAsync(ConnectTimeout);

                    client.IsConnected.Should().BeTrue();

                    await client.DisconnectAsync();

                    client.IsConnected.Should().BeFalse();
                }

                mre2.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);
        }

        [Fact]
        public async Task Should_Return_Server_PipeName()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.PipeName.Should().Be(pipeName);
            }
        }

        [Fact]
        public async Task Should_Return_Client_PipeName()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();

            await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
            {
                client.PipeName.Should().Be(pipeName);
            }
        }

        [Fact]
        public async Task Should_Get_Server_Client_Connection_IsConnected()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    void Server_OnClientConnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> e)
                    {
                        mre2.Set();
                    }

                    server.OnClientConnected += Server_OnClientConnected;

                    server.Start();

                    mre1.Set();

                    mre2.Wait();

                    server.Connections.Single().IsConnected.Should().BeTrue();

                    mre3.Wait();
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    mre1.Wait();

                    await client.ConnectAsync(ConnectTimeout);

                    // Wait for the server to update the connection list - triggered via Server_OnClientConnected
                    mre2.Wait();
                }

                mre3.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);
        }

        [Fact]
        public async Task Should_Get_No_Server_Client_Connections_After_Client_Disconnects()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();
            var mre4 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    void Server_OnClientConnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> e)
                    {
                        mre2.Set();
                    }

                    server.OnClientConnected += Server_OnClientConnected;

                    server.Start();

                    mre1.Set();

                    mre2.Wait();

                    server.Connections.Single().IsConnected.Should().BeTrue();

                    mre3.Set();

                    mre4.Wait();    // wait for the client to disconnect

                    server.Connections.Should().BeEmpty();
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    mre1.Wait();

                    await client.ConnectAsync(ConnectTimeout);

                    // Wait for the server to update the connection list
                    mre3.Wait();
                }

                mre4.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);
        }

        [Fact]
        public async Task Should_Catch_Exception_When_Serializing()
        {
            var pipeName = Create<string>();
            var serializer = new DummyBadSerializer(PipeDirection.Out);
            Exception actual = null;

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();
            var mre4 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    void Server_OnException(object sender, NamedPipeExceptionEventArgs eventArgs)
                    {
                        actual = eventArgs.Exception;
                        mre3.Set();
                    }

                    server.OnException += Server_OnException;

                    try
                    {
                        server.Start();

                        mre1.Set();

                        mre2.Wait();

                        await server.WriteAsync(Create<DummyMessage>(), CancellationToken.None);

                        mre4.Wait();
                    }
                    finally
                    {
                        server.OnException -= Server_OnException;
                    }
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    mre1.Wait();

                    await client.ConnectAsync(ConnectTimeout);

                    mre2.Set();

                    mre3.Wait();
                }

                mre4.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);

            actual.Should().BeAssignableTo<Exception>();
        }

        [Fact]
        public async Task Should_Catch_Exception_When_Deserializing()
        {
            var pipeName = Create<string>();
            var serializer = new DummyBadSerializer(PipeDirection.In);
            Exception actual = null;

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    async void Server_OnClientConnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
                    {
                        await server.WriteAsync(Create<DummyMessage>(), CancellationToken.None);
                    }

                    server.OnClientConnected += Server_OnClientConnected;

                    try
                    {
                        server.Start();

                        mre1.Set();

                        mre3.Wait();
                    }
                    finally
                    {
                        server.OnClientConnected -= Server_OnClientConnected;
                    }
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    void Client_OnException(object sender, NamedPipeExceptionEventArgs eventArgs)
                    {
                        actual = eventArgs.Exception;
                        mre2.Set();
                    }

                    client.OnException += Client_OnException;

                    try
                    {
                        mre1.Wait();

                        await client.ConnectAsync(TimeSpan.FromSeconds(10));        // ConnectTimeout

                        mre2.Wait();
                    }
                    finally
                    {
                        client.OnException -= Client_OnException;
                    }
                }

                mre3.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);

            // DummyBadSerializer throws InvalidOperationException
            actual.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public async Task Should_Throw_When_Cannot_Get_Impersonation_User_Name()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();
            var mre4 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    void Server_OnClientConnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> e)
                    {
                        mre2.Set();
                    }

                    server.OnClientConnected += Server_OnClientConnected;

                    server.Start();

                    mre1.Set();

                    mre2.Wait();     // wait for the client to connect

                    Invoking(() =>
                    {
                        _ = server.Connections.Single().GetImpersonationUserName();
                    })
                    .Should()
                    .Throw<IOException>()
                    .WithMessage("Unable to impersonate using a named pipe until data has been read from that pipe.");

                    mre3.Set();
                }

                mre4.Wait();
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    mre1.Wait();

                    await client.ConnectAsync(ConnectTimeout);

                    // wait for the server-side assertion
                    mre3.Wait();
                }

                mre4.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);
        }

        [Fact]
        public async Task Should_Get_Impersonation_User_Name()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();
            var mre4 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    void Server_OnMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
                    {
                        mre2.Set();
                    }

                    server.OnMessageReceived += Server_OnMessageReceived;

                    try
                    {
                        server.Start();

                        mre1.Set();

                        mre2.Wait();

                        var username = server.Connections.Single().GetImpersonationUserName();

                        username.Should().Be(Environment.UserName);

                        mre3.Set();

                        mre4.Wait();
                    }
                    finally
                    {
                        server.OnMessageReceived -= Server_OnMessageReceived;
                    }
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    mre1.Wait();

                    await client.ConnectAsync(ConnectTimeout);

                    await client.WriteAsync(Create<DummyMessage>(), CancellationToken.None);

                    mre3.Wait();
                }

                mre4.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);
        }

        [Fact]
        public async Task Should_Send_Message_From_Server_To_Client()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = Create<DummyMessage>();
            DummyMessage actual = null;

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();
            var mre4 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    server.Start();

                    mre1.Set();

                    mre2.Wait();

                    await server.WriteAsync(expected, CancellationToken.None);

                    mre4.Wait();
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    void Client_OnMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
                    {
                        actual = eventArgs.Message;
                        mre3.Set();
                    }

                    client.OnMessageReceived += Client_OnMessageReceived;

                    try
                    {
                        mre1.Wait();

                        await client.ConnectAsync(ConnectTimeout);

                        mre2.Set();

                        mre3.Wait();
                    }
                    finally
                    {
                        client.OnMessageReceived -= Client_OnMessageReceived;
                    }
                }

                mre4.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);

            expected.Should().BeEquivalentTo(actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task Should_Send_Message_From_Server_To_Filtered_Client(int connectionIndex)
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = Create<DummyMessage>();
            string actual = default;

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();
            var mre4 = new ManualResetEventSlim();
            var mre5 = new ManualResetEventSlim();

            void Client_OnMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
            {
                actual = eventArgs.Message.Value;
                mre2.Set();
            }

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    server.Start();

                    mre1.Set();

                    mre3.Wait();

                    if (connectionIndex == 2)
                    {
                        var filteredIds = (server as NamedPipeServer<DummyMessage>)
                            .Connections
                            .Select(item => item.ConnectionId)
                            .ToArray();

                        await server.WriteAsync(
                            expected,
                            connection => connection.ConnectionId == filteredIds[0] ||
                                          connection.ConnectionId == filteredIds[1],
                            CancellationToken.None);
                    }
                    else
                    {
                        var filteredId = (server as NamedPipeServer<DummyMessage>)
                            .Connections
                            .ElementAt(connectionIndex)
                            .ConnectionId;

                        expected.Value = filteredId;

                        await server.WriteAsync(
                            expected,
                            connection => connection.ConnectionId == filteredId,
                            CancellationToken.None);
                    }

                    mre2.Wait();

                    if (connectionIndex != 2)
                    {
                        actual.Should().Be(expected.Value);
                    }

                    mre4.Set();

                    mre5.Wait();

                    await server.StopAsync();
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                var composites = new CompositeAsyncDisposable();

                async Task<NamedPipeClient<DummyMessage>> CreateClientAsync()
                {
                    var client = new NamedPipeClient<DummyMessage>(pipeName, serializer);

                    composites.Add(client);

                    client.OnMessageReceived += Client_OnMessageReceived;

                    await client.ConnectAsync(ConnectTimeout);

                    return client;
                }

                mre1.Wait();

                NamedPipeClient<DummyMessage> client1 = null;
                NamedPipeClient<DummyMessage> client2 = null;

                try
                {
                    client1 = await CreateClientAsync();

                    client2 = await CreateClientAsync();

                    // Give time for the clients to connect
                    await Task.Delay(100);

                    mre3.Set();

                    mre4.Wait();
                }
                finally
                {
                    foreach (var client in composites.Disposables.Cast<NamedPipeClient<DummyMessage>>())
                    {
                        client.OnMessageReceived -= Client_OnMessageReceived;
                    }

                    await composites.DisposeAsync();
                }

            }, _timeoutSource.Token).Unwrap();

            await clientTask;

            mre5.Set();

            await serverTask;

            actual.Should().Be(expected.Value);
        }

        [Fact]
        public async Task Should_Send_Message_From_Client_To_Server()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = Create<DummyMessage>();
            DummyMessage actual = null;

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();
            var mre4 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    void Server_OnMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
                    {
                        actual = eventArgs.Message;
                        mre2.Set();
                    }

                    server.OnMessageReceived += Server_OnMessageReceived;

                    try
                    {
                        server.Start();

                        mre1.Set();

                        mre3.Wait();
                    }
                    finally
                    {
                        server.OnMessageReceived -= Server_OnMessageReceived;
                    }
                }

                mre4.Wait();
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    mre1.Wait();

                    await client.ConnectAsync(ConnectTimeout);

                    await client.WriteAsync(expected, CancellationToken.None);

                    mre2.Wait();

                    mre3.Set();
                }

                mre4.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);

            expected.Should().BeEquivalentTo(actual);
        }

        [Fact]
        public async Task Should_Not_Throw_When_Write_While_Disconnected()
        {
            await Invoking(async () =>
            {
                var serializer = new NamedPipeSerializer<DummyMessage>();

                await using (var client = new NamedPipeClient<DummyMessage>(Create<string>(), serializer))
                {
                    await client.WriteAsync(Create<DummyMessage>(), CancellationToken.None);
                }
            })
            .Should()
            .NotThrowAsync();
        }

        [Fact]
        public async Task Should_Throw_TimeoutException_When_Cannot_Find_Server()
        {
            await Invoking(async () =>
            {
                var serializer = new NamedPipeSerializer<DummyMessage>();

                await using (var client = new NamedPipeClient<DummyMessage>(Create<string>(), serializer))
                {
                    await client.ConnectAsync(TimeSpan.FromMilliseconds(100));
                }
            })
            .Should()
            .ThrowAsync<TimeoutException>();
        }

        [Fact]
        public async Task Should_Raise_Client_OnConnected()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var actual = false;

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    server.Start();

                    mre1.Set();

                    mre3.Wait();
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    void Client_OnConnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
                    {
                        actual = true;
                        mre2.Set();
                    }

                    client.OnConnected += Client_OnConnected;

                    mre1.Wait();

                    await client.ConnectAsync(ConnectTimeout);

                    mre2.Wait();
                }

                mre3.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);

            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Raise_Client_OnDisconnected()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var actual = false;

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();
            var mre4 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    server.Start();

                    mre1.Set();

                    mre2.Wait();

                    await server.StopAsync();

                    mre4.Wait();
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    void Client_OnDisconnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
                    {
                        actual = true;
                        mre3.Set();
                    }

                    client.OnDisconnected += Client_OnDisconnected;

                    mre1.Wait();

                    await client.ConnectAsync(ConnectTimeout);

                    mre2.Set();

                    mre3.Wait();
                }

                mre4.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);

            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Raise_Client_OnMessageReceived()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var actual = false;

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();
            var mre4 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    server.Start();

                    mre1.Set();

                    mre2.Wait();

                    await server.WriteAsync(Create<DummyMessage>(), CancellationToken.None);

                    mre4.Wait();
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    void Client_OnMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
                    {
                        actual = true;
                        mre3.Set();
                    }

                    client.OnMessageReceived += Client_OnMessageReceived;

                    try
                    {
                        mre1.Wait();

                        await client.ConnectAsync(ConnectTimeout);

                        mre2.Set();

                        mre3.Wait();
                    }
                    finally
                    {
                        client.OnMessageReceived -= Client_OnMessageReceived;
                    }
                }

                mre4.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);

            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Raise_Client_OnException()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = new Exception();
            Exception actual = null;

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();
            var mre4 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    server.Start();

                    mre1.Set();

                    mre3.Wait();

                    await server.WriteAsync(Create<DummyMessage>(), CancellationToken.None);

                    mre4.Wait();
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    void Client_OnMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
                    {
                        throw expected;
                    }

                    void Client_OnException(object sender, NamedPipeExceptionEventArgs eventArgs)
                    {
                        actual = eventArgs.Exception;
                        mre2.Set();
                    }

                    client.OnMessageReceived += Client_OnMessageReceived;
                    client.OnException += Client_OnException;

                    try
                    {
                        mre1.Wait();

                        await client.ConnectAsync(ConnectTimeout);

                        mre3.Set();

                        mre2.Wait();
                    }
                    finally
                    {
                        client.OnMessageReceived -= Client_OnMessageReceived;
                        client.OnException -= Client_OnException;
                    }
                }

                mre4.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public async Task Should_Raise_Client_OnException_When_OnDisconnected_Throws()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = new Exception();
            Exception actual = null;

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();
            var mre4 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    server.Start();

                    mre1.Set();

                    mre3.Wait();

                    await server.StopAsync();

                    mre4.Wait();
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    void Client_OnDisconnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
                    {
                        throw expected;
                    }

                    void Client_OnException(object sender, NamedPipeExceptionEventArgs eventArgs)
                    {
                        actual = eventArgs.Exception;
                        mre2.Set();
                    }

                    client.OnDisconnected += Client_OnDisconnected;
                    client.OnException += Client_OnException;

                    mre1.Wait();

                    await client.ConnectAsync(ConnectTimeout);

                    mre3.Set();

                    mre2.Wait();

                    client.IsConnected.Should().BeFalse();
                }

                mre4.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public async Task Should_Raise_Server_OnClientConnected()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var actual = false;

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    void Server_OnClientConnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
                    {
                        actual = true;
                        mre2.Set();
                    }

                    server.OnClientConnected += Server_OnClientConnected;

                    server.Start();

                    mre1.Set();

                    mre3.Wait();
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    mre1.Wait();

                    await client.ConnectAsync(ConnectTimeout);

                    mre2.Wait();
                }

                mre3.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);

            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Disconnect_When_OnClientConnected_Throws()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = new Exception();
            Exception actual = null;

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    void Server_OnClientConnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
                    {
                        throw expected;
                    }

                    void Server_OnException(object sender, NamedPipeExceptionEventArgs eventArgs)
                    {
                        actual = eventArgs.Exception;
                        mre2.Set();
                    }

                    server.OnClientConnected += Server_OnClientConnected;
                    server.OnException += Server_OnException;

                    server.Start();

                    mre1.Set();

                    mre3.Wait();
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    mre1.Wait();

                    await client.ConnectAsync(ConnectTimeout);

                    mre2.Wait();

                    await Task.Delay(10);

                    client.IsConnected.Should().BeFalse();
                }
            }, _timeoutSource.Token).Unwrap();

            await clientTask;

            mre3.Set();

            await serverTask;

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public async Task Should_Raise_Server_OnClientDisconnected()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var actual = false;

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    void Server_OnClientDisconnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
                    {
                        actual = true;
                        mre2.Set();
                    }

                    server.OnClientDisconnected += Server_OnClientDisconnected;

                    server.Start();

                    mre1.Set();

                    mre3.Wait();
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    mre1.Wait();

                    await client.ConnectAsync(ConnectTimeout);

                    await client.DisconnectAsync();

                    mre2.Wait();
                }

                mre3.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);

            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Raise_Server_OnMessageReceived()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var actual = false;

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    void Server_OnMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
                    {
                        actual = true;
                        mre2.Set();
                    }

                    server.OnMessageReceived += Server_OnMessageReceived;

                    try
                    {

                        server.Start();

                        mre1.Set();

                        mre3.Wait();
                    }
                    finally
                    {
                        server.OnMessageReceived -= Server_OnMessageReceived;
                    }
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    mre1.Wait();

                    await client.ConnectAsync(ConnectTimeout);

                    await client.WriteAsync(Create<DummyMessage>(), CancellationToken.None);

                    mre2.Wait();
                }

                mre3.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);

            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Not_Throw_When_Server_OnMessageReceived_Not_Assigned()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = Create<DummyMessage>();
            DummyMessage actual = null;
            var counter = 0;

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();
            var mre4 = new ManualResetEventSlim();
            var mre5 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    void Server_OnMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
                    {
                        actual = eventArgs.Message;
                        counter++;
                        mre2.Set();
                    }

                    server.Start();

                    mre1.Set();

                    mre3.Wait();

                    // Now assign the handler
                    server.OnMessageReceived += Server_OnMessageReceived;

                    try
                    {
                        mre4.Set();

                        mre5.Wait();
                    }
                    finally
                    {
                        server.OnMessageReceived -= Server_OnMessageReceived;
                    }
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    mre1.Wait();

                    await client.ConnectAsync(ConnectTimeout);

                    // Code path testing when OnMessageReceived is not assigned
                    await client.WriteAsync(Create<DummyMessage>(), CancellationToken.None);

                    // Give the server some time to receive the message (cannot use OnMessageReceived here since we are testing when it is not assigned)
                    await Task.Delay(10);

                    mre3.Set();

                    mre4.Wait();

                    await client.WriteAsync(expected, CancellationToken.None);

                    mre2.Wait();
                }

                mre5.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);

            counter.Should().Be(1);
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Should_Raise_Server_OnException_And_Disconnect()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = new Exception();
            Exception actual = null;

            var mre1 = new ManualResetEventSlim();
            var mre2 = new ManualResetEventSlim();
            var mre3 = new ManualResetEventSlim();

            var serverTask = Task.Factory.StartNew(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    void Server_OnMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
                    {
                        throw expected;
                    }

                    void Server_OnException(object sender, NamedPipeExceptionEventArgs eventArgs)
                    {
                        actual = eventArgs.Exception;
                        mre2.Set();
                    }

                    server.OnMessageReceived += Server_OnMessageReceived;
                    server.OnException += Server_OnException;

                    try
                    {
                        server.Start();

                        mre1.Set();

                        mre3.Wait();
                    }
                    finally
                    {
                        server.OnMessageReceived -= Server_OnMessageReceived;
                        server.OnException -= Server_OnException;
                    }
                }
            }, _timeoutSource.Token).Unwrap();

            var clientTask = Task.Factory.StartNew(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    mre1.Wait();

                    await client.ConnectAsync(ConnectTimeout);

                    await client.WriteAsync(Create<DummyMessage>(), CancellationToken.None);

                    mre2.Wait();
                }

                mre3.Set();
            }, _timeoutSource.Token).Unwrap();

            await Task.WhenAll(serverTask, clientTask);

            actual.Should().BeSameAs(expected);
        }

        public Task InitializeAsync()
        {
            _timeoutSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _timeoutSource.Dispose();

            return Task.CompletedTask;
        }
    }
}