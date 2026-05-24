using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Command;
using AllOverIt.Patterns.Command.Exceptions;

namespace AllOverIt.Tests.Patterns.Command
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
                .ShouldNotThrow();
            }

            [Fact]
            public void Should_Throw_When_Commands_Null()
            {
                Invoking(() =>
                {
                    _ = new CommandPipeline<int, int>(null);
                })
                .ShouldThrow<ArgumentNullException>()
                .WithNamedMessageWhenNull("commands");
            }

            [Fact]
            public void Should_Throw_When_Commands_Empty()
            {
                Invoking(() =>
                {
                    _ = new CommandPipeline<int, int>(Array.Empty<ICommand<int, int>>());
                })
                .ShouldThrow<ArgumentException>()
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

                actual.ShouldBe(expected);
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
                .ShouldThrow<ArgumentNullException>()
                .WithNamedMessageWhenNull("commands");
            }

            [Fact]
            public void Should_Throw_When_Commands_Empty()
            {
                Invoking(() =>
                {
                    var pipeline = new CommandPipeline<int, int>();
                    pipeline.Append(Array.Empty<ICommand<int, int>>());
                })
                .ShouldThrow<ArgumentException>()
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

                actual.ShouldBe(expected);
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

                actual.ShouldBe(expected);
            }

            [Fact]
            public void Should_Return_Self()
            {
                var pipeline = new CommandPipeline<int, int>();

                var actual = pipeline.Append(new DummyCommand());

                actual.ShouldBeSameAs(pipeline);
            }
        }

        public class Execute_Method : CommandPipelineFixture
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
               .ShouldThrow<CommandException>()
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

                commands[0].Sequence.ShouldBe(1);
                commands[1].Sequence.ShouldBe(2);
                commands[2].Sequence.ShouldBe(3);
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

                actual.ShouldBe(expected);
            }
        }
    }
}



