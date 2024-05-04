using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Pipeline;
using AllOverIt.ReactiveUI.Extensions;
using FluentAssertions;
using ReactiveUI;

namespace AllOverIt.ReactiveUI.Tests.Extensions
{
    public class ReactiveCommandExtensionsFixture : FixtureBase
    {
        public class Pipe_ReactiveCommand_TIn_TPrevOut : ReactiveCommandExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Command_Null()
            {
                Invoking(() =>
                {
                    var step = ReactiveCommand.Create<double, string>(_ => string.Empty);

                    _ = ReactiveCommandExtensions.Pipe<int, double, string>(null, step);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("command");
            }

            [Fact]
            public void Should_Throw_When_Step_Null()
            {
                Invoking(() =>
                {
                    var command = ReactiveCommand.Create<int, double>(_ => Create<double>());

                    _ = ReactiveCommandExtensions.Pipe<int, double, string>(command, null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Return_PipelineBuilderAsync()
            {
                var command = ReactiveCommand.Create<int, double>(_ => Create<double>());
                var step = ReactiveCommand.Create<double, string>(_ => string.Empty);

                var actual = ReactiveCommandExtensions.Pipe<int, double, string>(command, step);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }
        }
    }
}