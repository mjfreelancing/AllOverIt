using AllOverIt.Exceptions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Command;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Patterns.Enumeration
{
    public class AsyncCommandPipelineFixture : FixtureBase
    {
        private class DummyCommand : IAsyncCommand<int, int>
        {
            public Task<int> ExecuteAsync(int input)
            {
                return Task.FromResult(input + 1);
            }
        }

        public class Constructor : AsyncCommandPipelineFixture
        {
            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                {
                    _ = new AsyncCommandPipeline<int, int>();
                })
                .Should()
                .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_Commands_Null()
            {
                Invoking(() =>
                {
                    _ = new AsyncCommandPipeline<int, int>(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("commands");
            }

            [Fact]
            public void Should_Throw_When_Commands_Empty()
            {
                Invoking(() =>
                {
                    _ = new AsyncCommandPipeline<int, int>(new IAsyncCommand<int, int>[] { });
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("commands");
            }

            [Fact]
            public async Task Should_Append_Commands()
            {
                var commands = new[]
                {
                    new DummyCommand(),
                    new DummyCommand(),
                    new DummyCommand()
                };

                var pipeline = new AsyncCommandPipeline<int, int>(commands);

                var expected = Create<int>();

                var actual = await pipeline.ExecuteAsync(expected - 3);

                actual.Should().Be(expected);
            }
        }

        public class Append : AsyncCommandPipelineFixture
        {
            [Fact]
            public void Should_Throw_When_Commands_Null()
            {
                Invoking(() =>
                {
                    var pipeline = new AsyncCommandPipeline<int, int>();
                    pipeline.Append(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("commands");
            }

            [Fact]
            public void Should_Throw_When_Commands_Empty()
            {
                Invoking(() =>
                {
                    var pipeline = new AsyncCommandPipeline<int, int>();
                    pipeline.Append(new IAsyncCommand<int, int>[] { });
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("commands");
            }

            [Fact]
            public async Task Should_Append_Commands()
            {
                var commands = new[]
                {
                    new DummyCommand(),
                    new DummyCommand(),
                    new DummyCommand()
                };

                var pipeline = new AsyncCommandPipeline<int, int>();

                var expected = Create<int>();

                var actual = await pipeline
                    .Append(commands)
                    .ExecuteAsync(expected - 3);

                actual.Should().Be(expected);
            }

            [Fact]
            public async Task Should_Append_Individual_Commands()
            {
                var command1 = new DummyCommand();
                var command2 = new DummyCommand();
                var command3 = new DummyCommand();

                var pipeline = new AsyncCommandPipeline<int, int>();

                var expected = Create<int>();

                var actual = await pipeline
                    .Append(command1)
                    .Append(command2)
                    .Append(command3)
                    .ExecuteAsync(expected - 3);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Return_Self()
            {
                var pipeline = new AsyncCommandPipeline<int, int>();

                var actual = pipeline.Append(new DummyCommand());

                actual.Should().BeSameAs(pipeline);
            }
        }

        public class Execute : AsyncCommandPipelineFixture
        {
            private class SequenceCommand : IAsyncCommand<int, int>
            {
                public int Sequence { get; private set; }
                
                public Task<int> ExecuteAsync(int input)
                {
                    Sequence = input;

                    return Task.FromResult(input + 1);
                }
            }

            [Fact]
            public void Should_Throw_When_No_Commands()
            {
                Invoking(async () =>
                {
                    var pipeline = new AsyncCommandPipeline<int, int>();
                    await pipeline.ExecuteAsync(Create<int>());
                })
               .Should()
               .ThrowAsync<CommandException>()
               .WithMessage("There are no commands to execute.");
            }

            [Fact]
            public async Task Should_Execute_Commands_In_Order()
            {
                var commands = new[]
{
                    new SequenceCommand(),
                    new SequenceCommand(),
                    new SequenceCommand()
                };

                var pipeline = new AsyncCommandPipeline<int, int>(commands);

                await pipeline.ExecuteAsync(1);

                commands[0].Sequence.Should().Be(1);
                commands[1].Sequence.Should().Be(2);
                commands[2].Sequence.Should().Be(3);
            }

            [Fact]
            public async Task Should_Return_Final_Result()
            {
                var commands = new[]
                {
                    new DummyCommand(),
                    new DummyCommand(),
                    new DummyCommand()
                };

                var pipeline = new AsyncCommandPipeline<int, int>(commands);

                var expected = Create<int>();

                var actual = await pipeline.ExecuteAsync(expected - 3);

                actual.Should().Be(expected);
            }
        }
    }
}