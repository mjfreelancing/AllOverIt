using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.ReactiveUI.CommandPipeline;
using ReactiveUI;

namespace AllOverIt.ReactiveUI.Tests.CommandPipeline
{
    public class ReactiveCommandPipelineStepFixture : FixtureBase
    {
        public class Constructor : ReactiveCommandPipelineStepFixture
        {
            [Fact]
            public void Should_Throw_When_Command_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new ReactiveCommandPipelineStep<double, string>(null);
                })
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

                expected.ShouldBe(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                await Should.ThrowAsync<OperationCanceledException>(async () =>
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    var command = ReactiveCommand.Create<double, string>(_ => Create<string>());

                    var step = new ReactiveCommandPipelineStep<double, string>(command);

                    _ = await step.ExecuteAsync(Create<double>(), cts.Token);
                });
            }
        }
    }
}






