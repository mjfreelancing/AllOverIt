using AllOverIt.Exceptions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Command;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Patterns.Enumeration
{
    public class CommandPipelineFixture : FixtureBase
    {
        private class DummyCommand : ICommand<int, int>
        {
            public int Execute(int input)
            {
                return input + 1;
            }
        }

        public class Constructor : CommandPipelineFixture
        {
            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                {
                    _ = new CommandPipeline<int, int>();
                })
                .Should()
                .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_Commands_Null()
            {
                Invoking(() =>
                {
                    _ = new CommandPipeline<int, int>(null);
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
                    _ = new CommandPipeline<int, int>(new ICommand<int, int>[] { });
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("commands");
            }

            [Fact]
            public void Should_Append_Commands()
            {
                var commands = new[]
                {
                    new DummyCommand(),
                    new DummyCommand(),
                    new DummyCommand()
                };

                var pipeline = new CommandPipeline<int, int>(commands);

                var expected = Create<int>();

                var actual = pipeline.Execute(expected - 3);

                actual.Should().Be(expected);
            }
        }

        public class Append : CommandPipelineFixture
        {
            [Fact]
            public void Should_Throw_When_Commands_Null()
            {
                Invoking(() =>
                {
                    var pipeline = new CommandPipeline<int, int>();
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
                    var pipeline = new CommandPipeline<int, int>();
                    pipeline.Append(new ICommand<int, int>[] { });
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("commands");
            }

            [Fact]
            public void Should_Append_Commands()
            {
                var commands = new[]
                {
                    new DummyCommand(),
                    new DummyCommand(),
                    new DummyCommand()
                };

                var pipeline = new CommandPipeline<int, int>();

                var expected = Create<int>();

                var actual = pipeline
                    .Append(commands)
                    .Execute(expected - 3);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Append_Individual_Commands()
            {
                var command1 = new DummyCommand();
                var command2 = new DummyCommand();
                var command3 = new DummyCommand();

                var pipeline = new CommandPipeline<int, int>();

                var expected = Create<int>();

                var actual = pipeline
                    .Append(command1)
                    .Append(command2)
                    .Append(command3)
                    .Execute(expected - 3);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Return_Self()
            {
                var pipeline = new CommandPipeline<int, int>();

                var actual = pipeline.Append(new DummyCommand());

                actual.Should().BeSameAs(pipeline);
            }
        }

        public class Execute : CommandPipelineFixture
        {
            private class SequenceCommand : ICommand<int, int>
            {
                public int Sequence { get; private set; }
                
                public int Execute(int input)
                {
                    Sequence = input;

                    return input + 1;
                }
            }

            [Fact]
            public void Should_Throw_When_No_Commands()
            {
                Invoking(() =>
                {
                    var pipeline = new CommandPipeline<int, int>();
                    pipeline.Execute(Create<int>());
                })
               .Should()
               .Throw<CommandException>()
               .WithMessage("There are no commands to execute.");
            }

            [Fact]
            public void Should_Execute_Commands_In_Order()
            {
                var commands = new[]
{
                    new SequenceCommand(),
                    new SequenceCommand(),
                    new SequenceCommand()
                };

                var pipeline = new CommandPipeline<int, int>(commands);

                pipeline.Execute(1);

                commands[0].Sequence.Should().Be(1);
                commands[1].Sequence.Should().Be(2);
                commands[2].Sequence.Should().Be(3);
            }

            [Fact]
            public void Should_Return_Final_Result()
            {
                var commands = new[]
                {
                    new DummyCommand(),
                    new DummyCommand(),
                    new DummyCommand()
                };

                var pipeline = new CommandPipeline<int, int>(commands);

                var expected = Create<int>();

                var actual = pipeline.Execute(expected - 3);

                actual.Should().Be(expected);
            }
        }
    }
}