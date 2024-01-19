using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Pipeline;
using AllOverIt.ReactiveUI.CommandPipeline;
using FluentAssertions;
using ReactiveUI;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.ReactiveUI.Tests.CommandPipeline
{
    public class ReactiveCommandPipelineBuilderFixture : FixtureBase
    {
        public class Pipe_ReactiveCommand_TIn_TPrevOut : ReactiveCommandPipelineBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Command_Null()
            {
                Invoking(() =>
                {
                    _ = ReactiveCommandPipelineBuilder.Pipe<double, string>(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("command");
            }

            [Fact]
            public void Should_Return_PipelineBuilderAsync()
            {
                var command = ReactiveCommand.Create<double, string>(_ => string.Empty);

                var actual = ReactiveCommandPipelineBuilder.Pipe<double, string>(command);

                actual.Should().BeOfType<PipelineBuilderAsync<double, string>>();
            }
        }
    }
}