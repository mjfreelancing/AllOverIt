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
using Xunit;

using ReadyEvent = System.Threading.Tasks.TaskCompletionSource<bool>;

namespace AllOverIt.Pipes.Tests.Named
{
    [Collection("Pipes")]
    public class NamedPipeFixture_Mixed_Functional : FixtureBase
    {
        private static TimeSpan ConnectTimeout = TimeSpan.FromSeconds(1);

        private class DummyMessage
        {
            public string Value { get; set; }
        }

        public sealed class DummyStream : PipeStream
        {
            public DummyStream()
                : base(PipeDirection.InOut, 1024)
            {
            }
        }

        public sealed class DummyBadSerializer : INamedPipeSerializer<DummyMessage>
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

        [StaTheory]
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

        [StaTheory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Should_Return_Client_Connected_Status(bool connectToServer)
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = Create<DummyMessage>();

            var tcs1 = new ReadyEvent();
            var tcs2 = new ReadyEvent();

            var serverTask = Task.Run(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    server.Start();

                    tcs1.SetResult(true);

                    await tcs2.Task;
                }
            });

            var clientTask = Task.Run(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    client.IsConnected.Should().BeFalse();

                    await tcs1.Task;

                    if (connectToServer)
                    {
                        await client.ConnectAsync(ConnectTimeout);
                    }

                    client.IsConnected.Should().Be(connectToServer);
                }

                tcs2.SetResult(true);
            });

            await Task.WhenAll(serverTask, clientTask);
        }

        [StaFact]
        public async Task Should_Throw_When_Already_Connected()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = Create<DummyMessage>();

            var tcs1 = new ReadyEvent();
            var tcs2 = new ReadyEvent();

            var serverTask = Task.Run(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    server.Start();

                    tcs1.SetResult(true);

                    await tcs2.Task;
                }
            });

            var clientTask = Task.Run(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    await tcs1.Task;

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

                tcs2.SetResult(true);
            });

            await Task.WhenAll(serverTask, clientTask);
        }

        [StaFact]
        public async Task Should_Disconnect_Client_From_Server()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = Create<DummyMessage>();

            var tcs1 = new ReadyEvent();
            var tcs2 = new ReadyEvent();

            var serverTask = Task.Run(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    server.Start();

                    tcs1.SetResult(true);

                    await tcs2.Task;
                }
            });

            var clientTask = Task.Run(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    await tcs1.Task;

                    await client.ConnectAsync(ConnectTimeout);

                    client.IsConnected.Should().BeTrue();

                    await client.DisconnectAsync();

                    client.IsConnected.Should().BeFalse();
                }

                tcs2.SetResult(true);
            });

            await Task.WhenAll(serverTask, clientTask);
        }

        [StaFact]
        public async Task Should_Return_Server_PipeName()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.PipeName.Should().Be(pipeName);
            }
        }

        [StaFact]
        public async Task Should_Return_Client_PipeName()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();

            await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
            {
                client.PipeName.Should().Be(pipeName);
            }
        }

        [StaFact]
        public async Task Should_Get_Server_Client_Connection_IsConnected()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();

            var tcs1 = new ReadyEvent();
            var tcs2 = new ReadyEvent();
            var tcs3 = new ReadyEvent();

            var serverTask = Task.Run(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    void Server_OnClientConnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> e)
                    {
                        tcs2.SetResult(true);
                    }

                    server.OnClientConnected += Server_OnClientConnected;

                    server.Start();

                    tcs1.SetResult(true);

                    await tcs2.Task;

                    server.Connections.Single().IsConnected.Should().BeTrue();

                    await tcs3.Task;
                }
            });

            var clientTask = Task.Run(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    await tcs1.Task;

                    await client.ConnectAsync(ConnectTimeout);

                    // Wait for the server to update the connection list - triggered via Server_OnClientConnected
                    await tcs2.Task;
                }

                tcs3.SetResult(true);
            });

            await Task.WhenAll(serverTask, clientTask);
        }

        [StaFact]
        public async Task Should_Get_No_Server_Client_Connections_After_Client_Disconnects()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();

            var tcs1 = new ReadyEvent();
            var tcs2 = new ReadyEvent();
            var tcs3 = new ReadyEvent();
            var tcs4 = new ReadyEvent();

            var serverTask = Task.Run(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    void Server_OnClientConnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> e)
                    {
                        tcs2.SetResult(true);
                    }

                    server.OnClientConnected += Server_OnClientConnected;

                    server.Start();

                    tcs1.SetResult(true);

                    await tcs2.Task;

                    server.Connections.Single().IsConnected.Should().BeTrue();

                    tcs3.SetResult(true);

                    await tcs4.Task;    // wait for the client to disconnect

                    server.Connections.Should().BeEmpty();
                }
            });

            var clientTask = Task.Run(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    await tcs1.Task;

                    await client.ConnectAsync(ConnectTimeout);

                    // Wait for the server to update the connection list
                    await tcs3.Task;
                }

                tcs4.SetResult(true);
            });

            await Task.WhenAll(serverTask, clientTask);
        }

        [StaFact]
        public async Task Should_Catch_Exception_When_Serializing()
        {
            var pipeName = Create<string>();
            var serializer = new DummyBadSerializer(PipeDirection.Out);
            Exception actual = null;

            var tcs1 = new ReadyEvent();
            var tcs2 = new ReadyEvent();
            var tcs3 = new TaskCompletionSource<Exception>();
            var tcs4 = new ReadyEvent();

            var serverTask = Task.Run(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    void Server_OnException(object sender, NamedPipeExceptionEventArgs eventArgs)
                    {
                        tcs3.SetResult(eventArgs.Exception);
                    }

                    server.OnException += Server_OnException;

                    server.Start();

                    tcs1.SetResult(true);

                    await tcs2.Task;

                    await server.WriteAsync(Create<DummyMessage>(), CancellationToken.None);

                    await tcs4.Task;
                }
            });

            var clientTask = Task.Run(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    await tcs1.Task;

                    await client.ConnectAsync(ConnectTimeout);

                    tcs2.SetResult(true);

                    actual = await tcs3.Task;
                }

                tcs4.SetResult(true);
            });

            await Task.WhenAll(serverTask, clientTask);

            actual.Should().BeAssignableTo<Exception>();
        }

        [StaFact]
        public async Task Should_Catch_Exception_When_Deserializing()
        {
            var pipeName = Create<string>();
            var serializer = new DummyBadSerializer(PipeDirection.In);
            Exception actual = null;

            var tcs1 = new ReadyEvent();
            var tcs2 = new TaskCompletionSource<Exception>();
            var tcs3 = new ReadyEvent();

            var serverTask = Task.Run(async () =>
            {
                await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
                {
                    async void Server_OnClientConnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
                    {
                        await server.WriteAsync(Create<DummyMessage>(), CancellationToken.None);
                    }

                    server.OnClientConnected += Server_OnClientConnected;

                    server.Start();

                    tcs1.SetResult(true);

                    await tcs3.Task;
                }
            });

            var clientTask = Task.Run(async () =>
            {
                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    void Client_OnException(object sender, NamedPipeExceptionEventArgs eventArgs)
                    {
                        tcs2.SetResult(eventArgs.Exception);
                    }

                    client.OnException += Client_OnException;

                    await tcs1.Task;

                    await client.ConnectAsync(ConnectTimeout);

                    actual = await tcs2.Task;
                }

                tcs3.SetResult(true);
            });

            await Task.WhenAll(serverTask, clientTask);

            actual.Should().BeAssignableTo<Exception>();
        }

        //[StaFact]
        //public async Task Should_Throw_When_Cannot_Get_Impersonation_User_Name()
        //{
        //    var pipeName = Create<string>();
        //    var serializer = new NamedPipeSerializer<DummyMessage>();

        //    var tcs1 = new ReadyEvent();
        //    var tcs2 = new ReadyEvent();
        //    var tcs3 = new ReadyEvent();

        //    var serverTask = Task.Run(async () =>
        //    {
        //        await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
        //        {
        //            void Server_OnClientConnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> e)
        //            {
        //                tcs1.SetResult(true);
        //            }

        //            server.OnClientConnected += Server_OnClientConnected;

        //            server.Start();

        //            await tcs1.Task;     // wait for the client to connect

        //            Invoking(() =>
        //            {
        //                _ = server.Connections.Single().GetImpersonationUserName();
        //            })
        //            .Should()
        //            .Throw<IOException>()
        //            .WithMessage("Unable to impersonate using a named pipe until data has been read from that pipe.");

        //            tcs2.SetResult(true);
        //        }

        //        await tcs3.Task;
        //    });

        //    var clientTask = Task.Run(async () =>
        //    {
        //        await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
        //        {
        //            await client.ConnectAsync(ConnectTimeout);

        //            // wait for the server-side assertion
        //            await tcs2.Task;
        //        }

        //        tcs3.SetResult(true);
        //    });

        //    await Task.WhenAll(serverTask, clientTask);
        //}

        //[StaFact]
        //public async Task Should_Get_Impersonation_User_Name()
        //{
        //    var pipeName = Create<string>();
        //    var serializer = new NamedPipeSerializer<DummyMessage>();

        //    var tcs1 = new TaskReadyEvent<DummyMessage>();
        //    var tcs2 = new ReadyEvent();
        //    var tcs3 = new ReadyEvent();

        //    var serverTask = Task.Run(async () =>
        //    {
        //        await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
        //        {
        //            void Server_OnMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
        //            {
        //                tcs1.SetResult(eventArgs.Message);
        //            }

        //            server.OnMessageReceived += Server_OnMessageReceived;

        //            server.Start();

        //            await tcs1.Task;

        //            var username = server.Connections.Single().GetImpersonationUserName();

        //            username.Should().Be(Environment.UserName);

        //            tcs2.SetResult(true);

        //            await tcs3.Task;
        //        }
        //    });

        //    var clientTask = Task.Run(async () =>
        //    {
        //        await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
        //        {
        //            await client.ConnectAsync(ConnectTimeout);

        //            await client.WriteAsync(Create<DummyMessage>(), CancellationToken.None);

        //            _ = await tcs2.Task;
        //        }

        //        tcs3.SetResult(true);
        //    });

        //    await Task.WhenAll(serverTask, clientTask);
        //}

        //[StaFact]
        //public async Task Should_Send_Message_From_Server_To_Client()
        //{
        //    var pipeName = Create<string>();
        //    var serializer = new NamedPipeSerializer<DummyMessage>();
        //    var expected = Create<DummyMessage>();
        //    DummyMessage actual = null;

        //    var tcs1 = new ReadyEvent();
        //    var tcs2 = new TaskReadyEvent<DummyMessage>();
        //    var tcs3 = new ReadyEvent();

        //    var serverTask = Task.Run(async () =>
        //    {
        //        await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
        //        {
        //            server.Start();

        //            await tcs1.Task;

        //            await server.WriteAsync(expected, CancellationToken.None);

        //            await tcs3.Task;
        //        }
        //    });

        //    var clientTask = Task.Run(async () =>
        //    {
        //        await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
        //        {
        //            void Client_OnMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
        //            {
        //                tcs2.SetResult(eventArgs.Message);
        //            }

        //            client.OnMessageReceived += Client_OnMessageReceived;

        //            await client.ConnectAsync(ConnectTimeout);

        //            tcs1.SetResult(true);

        //            actual = await tcs2.Task;
        //        }

        //        tcs3.SetResult(true);
        //    });

        //    await Task.WhenAll(serverTask, clientTask);

        //    expected.Should().BeEquivalentTo(actual);
        //}

        //[StaTheory]
        //[InlineData(0)]
        //[InlineData(1)]
        //[InlineData(2)]
        //public async Task Should_Send_Message_From_Server_To_Filtered_Client(int connectionIndex)
        //{
        //    var pipeName = Create<string>();
        //    var serializer = new NamedPipeSerializer<DummyMessage>();
        //    var expected = Create<DummyMessage>();
        //    string actual = default;

        //    var tcs1 = new TaskReadyEvent<string>();
        //    var tcs2 = new ReadyEvent();
        //    var tcs3 = new ReadyEvent();
        //    var tcs4 = new ReadyEvent();

        //    void Client_OnMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
        //    {
        //        tcs1.SetResult(eventArgs.Message.Value);
        //    }

        //    var serverTask = Task.Run(async () =>
        //    {
        //        await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
        //        {
        //            server.Start();

        //            await tcs2.Task;

        //            if (connectionIndex == 2)
        //            {
        //                var filteredIds = (server as NamedPipeServer<DummyMessage>)
        //                    .Connections
        //                    .Select(item => item.ConnectionId)
        //                    .ToArray();

        //                await server.WriteAsync(
        //                    expected,
        //                    connection => connection.ConnectionId == filteredIds[0] ||
        //                                  connection.ConnectionId == filteredIds[1],
        //                    CancellationToken.None);
        //            }
        //            else
        //            {
        //                var filteredId = (server as NamedPipeServer<DummyMessage>)
        //                    .Connections
        //                    .ElementAt(connectionIndex)
        //                    .ConnectionId;

        //                expected.Value = filteredId;

        //                await server.WriteAsync(
        //                    expected,
        //                    connection => connection.ConnectionId == filteredId,
        //                    CancellationToken.None);
        //            }

        //            var result = await tcs1.Task;

        //            actual = result;

        //            if (connectionIndex != 2)
        //            {
        //                result.Should().Be(expected.Value);
        //            }

        //            tcs3.SetResult(true);

        //            await tcs4.Task;

        //            await server.StopAsync();
        //        }
        //    });

        //    var clientTask = Task.Run(async () =>
        //    {
        //        var composites = new CompositeAsyncDisposable();

        //        async Task<NamedPipeClient<DummyMessage>> CreateClientAsync()
        //        {
        //            var client = new NamedPipeClient<DummyMessage>(pipeName, serializer);

        //            composites.Add(client);

        //            client.OnMessageReceived += Client_OnMessageReceived;
        //            await client.ConnectAsync(ConnectTimeout);

        //            return client;
        //        }

        //        var client1 = await CreateClientAsync();
        //        var client2 = await CreateClientAsync();

        //        tcs2.SetResult(true);

        //        await tcs3.Task;

        //        await composites.DisposeAsync();
        //    });

        //    await clientTask;

        //    tcs4.SetResult(true);

        //    await serverTask;

        //    actual.Should().Be(expected.Value);
        //}

        //[StaFact]
        //public async Task Should_Send_Message_From_Client_To_Server()
        //{
        //    var pipeName = Create<string>();
        //    var serializer = new NamedPipeSerializer<DummyMessage>();
        //    var expected = Create<DummyMessage>();
        //    DummyMessage actual = null;

        //    var tcs1 = new TaskReadyEvent<DummyMessage>();
        //    var tcs2 = new ReadyEvent();
        //    var tcs3 = new ReadyEvent();

        //    var serverTask = Task.Run(async () =>
        //    {
        //        await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
        //        {
        //            void Server_OnMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
        //            {
        //                tcs1.SetResult(eventArgs.Message);
        //            }

        //            server.OnMessageReceived += Server_OnMessageReceived;

        //            server.Start();

        //            await tcs2.Task;
        //        }

        //        await tcs3.Task;
        //    });

        //    var clientTask = Task.Run(async () =>
        //    {
        //        await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
        //        {
        //            await client.ConnectAsync(ConnectTimeout);

        //            await client.WriteAsync(expected, CancellationToken.None);

        //            actual = await tcs1.Task;

        //            tcs2.SetResult(true);
        //        }

        //        tcs3.SetResult(true);
        //    });

        //    await Task.WhenAll(serverTask, clientTask);

        //    expected.Should().BeEquivalentTo(actual);
        //}

        //[StaFact]
        //public async Task Should_Not_Throw_When_Write_While_Disconnected()
        //{
        //    await Invoking(async () =>
        //    {
        //        var serializer = new NamedPipeSerializer<DummyMessage>();

        //        await using (var client = new NamedPipeClient<DummyMessage>(Create<string>(), serializer))
        //        {
        //            await client.WriteAsync(Create<DummyMessage>(), CancellationToken.None);
        //        }
        //    })
        //    .Should()
        //    .NotThrowAsync();
        //}

        //[StaFact]
        //public async Task Should_Throw_TimeoutException_When_Cannot_Find_Server()
        //{
        //    await Invoking(async () =>
        //    {
        //        var serializer = new NamedPipeSerializer<DummyMessage>();

        //        await using (var client = new NamedPipeClient<DummyMessage>(Create<string>(), serializer))
        //        {
        //            await client.ConnectAsync(TimeSpan.FromMilliseconds(100));
        //        }
        //    })
        //    .Should()
        //    .ThrowAsync<TimeoutException>();
        //}

        //[StaFact]
        //public async Task Should_Raise_Client_OnConnected()
        //{
        //    var pipeName = Create<string>();
        //    var serializer = new NamedPipeSerializer<DummyMessage>();
        //    var actual = false;

        //    var tcs1 = new ReadyEvent();
        //    var tcs2 = new ReadyEvent();

        //    var serverTask = Task.Run(async () =>
        //    {
        //        await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
        //        {
        //            server.Start();

        //            await tcs2.Task;
        //        }
        //    });

        //    var clientTask = Task.Run(async () =>
        //    {
        //        await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
        //        {
        //            void Client_OnConnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
        //            {
        //                tcs1.SetResult(true);
        //            }

        //            client.OnConnected += Client_OnConnected;

        //            await client.ConnectAsync(ConnectTimeout);

        //            actual = await tcs1.Task;
        //        }

        //        await Task.Delay(10);

        //        tcs2.SetResult(true);
        //    });

        //    await Task.WhenAll(serverTask, clientTask);

        //    actual.Should().BeTrue();
        //}

        //[StaFact]
        //public async Task Should_Raise_Client_OnDisconnected()
        //{
        //    var pipeName = Create<string>();
        //    var serializer = new NamedPipeSerializer<DummyMessage>();
        //    var actual = false;

        //    var tcs1 = new ReadyEvent();
        //    var tcs2 = new ReadyEvent();
        //    var tcs3 = new ReadyEvent();

        //    var serverTask = Task.Run(async () =>
        //    {
        //        await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
        //        {
        //            server.Start();

        //            await tcs1.Task;

        //            await server.StopAsync();

        //            await tcs3.Task;
        //        }
        //    });

        //    var clientTask = Task.Run(async () =>
        //    {
        //        await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
        //        {
        //            void Client_OnDisconnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
        //            {
        //                tcs2.SetResult(true);
        //            }

        //            client.OnDisconnected += Client_OnDisconnected;

        //            await client.ConnectAsync(ConnectTimeout);

        //            tcs1.SetResult(true);

        //            actual = await tcs2.Task;
        //        }

        //        tcs3.SetResult(true);
        //    });

        //    await Task.WhenAll(serverTask, clientTask);

        //    actual.Should().BeTrue();
        //}

        //[StaFact]
        //public async Task Should_Raise_Client_OnMessageReceived()
        //{
        //    var pipeName = Create<string>();
        //    var serializer = new NamedPipeSerializer<DummyMessage>();
        //    var actual = false;

        //    var tcs1 = new ReadyEvent();
        //    var tcs2 = new ReadyEvent();
        //    var tcs3 = new ReadyEvent();

        //    var serverTask = Task.Run(async () =>
        //    {
        //        await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
        //        {
        //            server.Start();

        //            await tcs1.Task;

        //            await server.WriteAsync(Create<DummyMessage>(), CancellationToken.None);

        //            await tcs3.Task;
        //        }
        //    });

        //    var clientTask = Task.Run(async () =>
        //    {
        //        await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
        //        {
        //            void Client_OnMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
        //            {
        //                tcs2.SetResult(true);
        //            }

        //            client.OnMessageReceived += Client_OnMessageReceived;

        //            await client.ConnectAsync(ConnectTimeout);

        //            tcs1.SetResult(true);

        //            actual = await tcs2.Task;
        //        }

        //        tcs3.SetResult(true);
        //    });

        //    await Task.WhenAll(serverTask, clientTask);

        //    actual.Should().BeTrue();
        //}

        //[StaFact]
        //public async Task Should_Raise_Client_OnException()
        //{
        //    var pipeName = Create<string>();
        //    var serializer = new NamedPipeSerializer<DummyMessage>();
        //    var expected = new Exception();
        //    Exception actual = null;

        //    var tcs1 = new TaskReadyEvent<Exception>();
        //    var tcs2 = new ReadyEvent();
        //    var tcs3 = new ReadyEvent();

        //    var serverTask = Task.Run(async () =>
        //    {
        //        await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
        //        {
        //            server.Start();

        //            await tcs2.Task;

        //            await server.WriteAsync(Create<DummyMessage>(), CancellationToken.None);

        //            await tcs3.Task;
        //        }
        //    });

        //    var clientTask = Task.Run(async () =>
        //    {
        //        await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
        //        {
        //            void Client_OnMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
        //            {
        //                throw expected;
        //            }

        //            void Client_OnException(object sender, NamedPipeExceptionEventArgs eventArgs)
        //            {
        //                tcs1.SetResult(eventArgs.Exception);
        //            }

        //            client.OnMessageReceived += Client_OnMessageReceived;
        //            client.OnException += Client_OnException;

        //            await client.ConnectAsync(ConnectTimeout);

        //            tcs2.SetResult(true);

        //            actual = await tcs1.Task;
        //        }

        //        tcs3.SetResult(true);
        //    });

        //    await Task.WhenAll(serverTask, clientTask);

        //    actual.Should().BeSameAs(expected);
        //}

        //[StaFact]
        //public async Task Should_Raise_Client_OnException_When_OnDisconnected_Throws()
        //{
        //    var pipeName = Create<string>();
        //    var serializer = new NamedPipeSerializer<DummyMessage>();
        //    var expected = new Exception();
        //    Exception actual = null;

        //    var tcs1 = new TaskReadyEvent<Exception>();
        //    var tcs2 = new ReadyEvent();
        //    var tcs3 = new ReadyEvent();

        //    var serverTask = Task.Run(async () =>
        //    {
        //        await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
        //        {
        //            server.Start();

        //            await tcs2.Task;

        //            await server.StopAsync();

        //            await tcs3.Task;
        //        }
        //    });

        //    var clientTask = Task.Run(async () =>
        //    {
        //        await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
        //        {
        //            void Client_OnDisconnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
        //            {
        //                throw expected;
        //            }

        //            void Client_OnException(object sender, NamedPipeExceptionEventArgs eventArgs)
        //            {
        //                tcs1.SetResult(eventArgs.Exception);
        //            }

        //            client.OnDisconnected += Client_OnDisconnected;
        //            client.OnException += Client_OnException;

        //            await client.ConnectAsync(ConnectTimeout);

        //            tcs2.SetResult(true);

        //            actual = await tcs1.Task;

        //            client.IsConnected.Should().BeFalse();
        //        }

        //        tcs3.SetResult(true);
        //    });

        //    await Task.WhenAll(serverTask, clientTask);

        //    actual.Should().BeSameAs(expected);
        //}

        //[StaFact]
        //public async Task Should_Raise_Server_OnClientConnected()
        //{
        //    var pipeName = Create<string>();
        //    var serializer = new NamedPipeSerializer<DummyMessage>();
        //    var actual = false;

        //    var tcs1 = new ReadyEvent();
        //    var tcs2 = new ReadyEvent();

        //    var serverTask = Task.Run(async () =>
        //    {
        //        await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
        //        {
        //            void Server_OnClientConnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
        //            {
        //                tcs1.SetResult(true);
        //            }

        //            server.OnClientConnected += Server_OnClientConnected;

        //            server.Start();

        //            await tcs2.Task;
        //        }
        //    });

        //    var clientTask = Task.Run(async () =>
        //    {
        //        await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
        //        {
        //            await client.ConnectAsync(ConnectTimeout);

        //            actual = await tcs1.Task;
        //        }

        //        tcs2.SetResult(true);
        //    });

        //    await Task.WhenAll(serverTask, clientTask);

        //    actual.Should().BeTrue();
        //}

        //[StaFact]
        //public async Task Should_Disconnect_When_OnClientConnected_Throws()
        //{
        //    var pipeName = Create<string>();
        //    var serializer = new NamedPipeSerializer<DummyMessage>();
        //    var expected = new Exception();
        //    Exception actual = null;

        //    var tcs1 = new TaskReadyEvent<Exception>();
        //    var tcs2 = new ReadyEvent();

        //    var serverTask = Task.Run(async () =>
        //    {
        //        await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
        //        {
        //            void Server_OnClientConnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
        //            {
        //                throw expected;
        //            }

        //            void Server_OnException(object sender, NamedPipeExceptionEventArgs eventArgs)
        //            {
        //                tcs1.SetResult(eventArgs.Exception);
        //            }

        //            server.OnClientConnected += Server_OnClientConnected;
        //            server.OnException += Server_OnException;

        //            server.Start();

        //            await tcs2.Task;
        //        }
        //    });

        //    var clientTask = Task.Run(async () =>
        //    {
        //        await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
        //        {
        //            await client.ConnectAsync(ConnectTimeout);

        //            actual = await tcs1.Task;

        //            client.IsConnected.Should().BeFalse();
        //        }
        //    });

        //    await clientTask;

        //    tcs2.SetResult(true);

        //    await serverTask;

        //    actual.Should().BeSameAs(expected);
        //}

        //[StaFact]
        //public async Task Should_Raise_Server_OnClientDisconnected()
        //{
        //    var pipeName = Create<string>();
        //    var serializer = new NamedPipeSerializer<DummyMessage>();
        //    var actual = false;

        //    var tcs1 = new ReadyEvent();
        //    var tcs2 = new ReadyEvent();

        //    var serverTask = Task.Run(async () =>
        //    {
        //        await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
        //        {
        //            void Server_OnClientDisconnected(object sender, NamedPipeConnectionEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
        //            {
        //                tcs1.SetResult(true);
        //            }

        //            server.OnClientDisconnected += Server_OnClientDisconnected;

        //            server.Start();

        //            await tcs2.Task;
        //        }
        //    });

        //    var clientTask = Task.Run(async () =>
        //    {
        //        await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
        //        {
        //            await client.ConnectAsync(ConnectTimeout);

        //            await client.DisconnectAsync();

        //            actual = await tcs1.Task;
        //        }

        //        tcs2.SetResult(true);
        //    });

        //    await Task.WhenAll(serverTask, clientTask);

        //    actual.Should().BeTrue();
        //}

        //[StaFact]
        //public async Task Should_Raise_Server_OnMessageReceived()
        //{
        //    var pipeName = Create<string>();
        //    var serializer = new NamedPipeSerializer<DummyMessage>();
        //    var actual = false;

        //    var tcs1 = new ReadyEvent();
        //    var tcs2 = new ReadyEvent();

        //    var serverTask = Task.Run(async () =>
        //    {
        //        await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
        //        {
        //            void Server_OnMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
        //            {
        //                tcs1.SetResult(true);
        //            }

        //            server.OnMessageReceived += Server_OnMessageReceived;

        //            server.Start();

        //            await tcs2.Task;
        //        }
        //    });

        //    var clientTask = Task.Run(async () =>
        //    {
        //        await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
        //        {
        //            await client.ConnectAsync(ConnectTimeout);

        //            await client.WriteAsync(Create<DummyMessage>(), CancellationToken.None);

        //            actual = await tcs1.Task;
        //        }

        //        tcs2.SetResult(true);
        //    });

        //    await Task.WhenAll(serverTask, clientTask);

        //    actual.Should().BeTrue();
        //}

        //[StaFact]
        //public async Task Should_Not_Throw_When_Server_OnMessageReceived_Not_Assigned()
        //{
        //    var pipeName = Create<string>();
        //    var serializer = new NamedPipeSerializer<DummyMessage>();
        //    var expected = Create<DummyMessage>();
        //    DummyMessage actual = null;
        //    var counter = 0;

        //    var tcs1 = new TaskReadyEvent<DummyMessage>();
        //    var tcs2 = new ReadyEvent();
        //    var tcs3 = new ReadyEvent();
        //    var tcs4 = new ReadyEvent();

        //    var serverTask = Task.Run(async () =>
        //    {
        //        await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
        //        {
        //            void Server_OnMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
        //            {
        //                counter++;
        //                tcs1.SetResult(eventArgs.Message);
        //            }

        //            server.Start();

        //            await tcs2.Task;

        //            // Now assign the handler
        //            server.OnMessageReceived += Server_OnMessageReceived;

        //            tcs3.SetResult(true);

        //            await tcs4.Task;
        //        }
        //    });

        //    var clientTask = Task.Run(async () =>
        //    {
        //        await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
        //        {
        //            await client.ConnectAsync(ConnectTimeout);

        //            // Code path testing when OnMessageReceived is not assigned
        //            await client.WriteAsync(Create<DummyMessage>(), CancellationToken.None);

        //            // Give the server some time to receive the message (cannot use OnMessageReceived here since we are testing when it is not assigned)
        //            await Task.Delay(10);

        //            tcs2.SetResult(true);

        //            await tcs3.Task;

        //            await client.WriteAsync(expected, CancellationToken.None);

        //            actual = await tcs1.Task;
        //        }

        //        tcs4.SetResult(true);
        //    });

        //    await Task.WhenAll(serverTask, clientTask);

        //    counter.Should().Be(1);
        //    actual.Should().BeEquivalentTo(expected);
        //}

        //[StaFact]
        //public async Task Should_Raise_Server_OnException_And_Disconnect()
        //{
        //    var pipeName = Create<string>();
        //    var serializer = new NamedPipeSerializer<DummyMessage>();
        //    var expected = new Exception();
        //    Exception actual = null;

        //    var tcs1 = new TaskReadyEvent<Exception>();
        //    var tcs2 = new ReadyEvent();

        //    var serverTask = Task.Run(async () =>
        //    {
        //        await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
        //        {
        //            void Server_OnMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
        //            {
        //                throw expected;
        //            }

        //            void Server_OnException(object sender, NamedPipeExceptionEventArgs eventArgs)
        //            {
        //                tcs1.SetResult(eventArgs.Exception);
        //            }

        //            server.OnMessageReceived += Server_OnMessageReceived;
        //            server.OnException += Server_OnException;

        //            server.Start();

        //            await tcs2.Task;
        //        }
        //    });

        //    var clientTask = Task.Run(async () =>
        //    {
        //        await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
        //        {
        //            await client.ConnectAsync(ConnectTimeout);

        //            await client.WriteAsync(Create<DummyMessage>(), CancellationToken.None);

        //            actual = await tcs1.Task;
        //        }

        //        tcs2.SetResult(true);
        //    });

        //    await Task.WhenAll(serverTask, clientTask);

        //    actual.Should().BeSameAs(expected);
        //}
    }
}