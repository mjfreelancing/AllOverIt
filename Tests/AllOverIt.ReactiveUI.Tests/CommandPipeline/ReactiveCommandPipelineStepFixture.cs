using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.ReactiveUI.CommandPipeline;
using FluentAssertions;
using ReactiveUI;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.ReactiveUI.Tests.CommandPipeline
{
    public class ReactiveCommandPipelineStepFixture : FixtureBase
    {
        public class Constructor : ReactiveCommandPipelineStepFixture
        {
            [Fact]
            public void Should_Throw_When_Command_Null()
            {
                Invoking(() =>
                {
                    _ = new ReactiveCommandPipelineStep<double, string>(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("command");
            }
        }

        public class ExecuteAsync : ReactiveCommandPipelineStepFixture
        {
            [Fact]
            public async Task Should_Return_Result()
            {
                var input = Create<double>();
                var expected = $"{input}";

                var command = ReactiveCommand.Create<double, string>(_ => expected);

                var step = new ReactiveCommandPipelineStep<double, string>(command);

                var actual = await step.ExecuteAsync(input, CancellationToken.None);

                expected.Should().Be(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    var command = ReactiveCommand.Create<double, string>(_ => Create<string>());

                    var step = new ReactiveCommandPipelineStep<double, string>(command);

                    _ = await step.ExecuteAsync(Create<double>(), cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }
        }
    }
}