using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pipes.Named.Client;
using AllOverIt.Pipes.Named.Serialization;
using FakeItEasy;
using FluentAssertions;

namespace AllOverIt.Pipes.Tests.Named.Client
{
    public class NamedPipeClientFixture : FixtureBase
    {
        public sealed class DummyMessage
        {
            public int Id { get; set; }
        }

        [Collection("NamedPipes")]
        public class Constructor_PipeName_Serializer : NamedPipeClientFixture
        {
            [Fact]
            public void Should_Throw_When_PipeName_Null_Empty_Whitespace()
            {

                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = new NamedPipeClient<DummyMessage>(stringValue, A.Fake<INamedPipeSerializer<DummyMessage>>());
                    }, "pipeName");
            }

            [Fact]
            public void Should_Throw_When_Serializer_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeClient<DummyMessage>(Create<string>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("serializer");
            }
        }

        [Collection("NamedPipes")]
        public class Constructor_PipeName_DomainName_Serializer : NamedPipeClientFixture
        {
            [Fact]
            public void Should_Throw_When_PipeName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = new NamedPipeClient<DummyMessage>(stringValue, Create<string>(), A.Fake<INamedPipeSerializer<DummyMessage>>());
                    }, "pipeName");
            }

            [Fact]
            public void Should_Throw_When_DomainName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = new NamedPipeClient<DummyMessage>(Create<string>(), stringValue, A.Fake<INamedPipeSerializer<DummyMessage>>());
                    }, "serverName");
            }

            [Fact]
            public void Should_Throw_When_Serializer_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeClient<DummyMessage>(Create<string>(), Create<string>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("serializer");
            }
        }

        [Collection("NamedPipes")]
        public class WriteAsync : NamedPipeClientFixture
        {
            [Fact]
            public async Task Should_Throw_When_Message_Null()
            {
                var client = new NamedPipeClient<DummyMessage>(Create<string>(), Create<string>(), A.Fake<INamedPipeSerializer<DummyMessage>>());

                await Invoking(async () =>
                {
                    await client.WriteAsync(null, CancellationToken.None);
                })
                .Should()
                .ThrowAsync<ArgumentNullException>()
                .WithNamedMessageWhenNull("message");
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var client = new NamedPipeClient<DummyMessage>(Create<string>(), Create<string>(), A.Fake<INamedPipeSerializer<DummyMessage>>());

                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(async () =>
                {
                    await client.WriteAsync(Create<DummyMessage>(), cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }
        }

        // Remaining tests in NamedPipeFixture_Mixed_Functional
    }
}