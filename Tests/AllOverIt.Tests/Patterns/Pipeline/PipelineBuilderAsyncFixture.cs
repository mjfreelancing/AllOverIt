using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Patterns.Pipeline;
using FluentAssertions;

namespace AllOverIt.Tests.Patterns.Pipeline
{
    public class PipelineBuilderAsync_TIn_TOut : FixtureBase
    {
        public class Constructor : PipelineBuilderAsync_TIn_TOut
        {
            [Fact]
            public void Should_Throw_When_Step_Null()
            {
                Invoking(() =>
                {
                    Func<int, CancellationToken, Task<double>> step = null;

                    _ = new PipelineBuilderAsync<int, double>(step);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }
        }

        public class Build : PipelineBuilderAsync_TIn_TOut
        {
            [Fact]
            public void Should_Return_Func()
            {
                Func<int, CancellationToken, Task<double>> step = (value, cancellationToken) => Task.FromResult((double) value);

                var builder = new PipelineBuilderAsync<int, double>(step);

                var actual = builder.Build();

                actual.Should().BeSameAs(step);
            }
        }
    }

    public class PipelineBuilderAsync_TIn_TPrevOut_TNextOut : FixtureBase
    {
        public class Constructor : PipelineBuilderAsync_TIn_TPrevOut_TNextOut
        {
            [Fact]
            public void Should_Throw_When_PrevStep_Null()
            {
                Invoking(() =>
                {
                    Func<double, CancellationToken, Task<string>> step = (value, cancellationToken) => Task.FromResult(value.ToString());

                    _ = new PipelineBuilderAsync<int, double, string>(null, step);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("prevStep");
            }

            [Fact]
            public void Should_Throw_When_Step_Null()
            {
                Invoking(() =>
                {
                    var prevStep = this.CreateStub<IPipelineBuilderAsync<int, double>>();

                    _ = new PipelineBuilderAsync<int, double, string>(prevStep, null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }
        }

        public class Build : PipelineBuilderAsync_TIn_TPrevOut_TNextOut
        {
            [Fact]
            public async Task Should_Return_Composed_Func()
            {
                var factor = Create<double>();

                // IPipelineBuilderAsync<int, double>
                var builder1 = PipelineBuilder.PipeAsync<int, double>((value, cancellationToken) => Task.FromResult(value * factor));

                var builder2 = new PipelineBuilderAsync<int, double, string>(builder1, (value, cancellationToken) => Task.FromResult($"{value}"));

                var func = builder2.Build();

                var input = Create<int>();
                var expected = $"{input * factor}";

                var actual = await func.Invoke(input, CancellationToken.None);

                expected.Should().Be(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    // IPipelineBuilderAsync<int, double>
                    var builder1 = PipelineBuilder.PipeAsync<int, double>((value, cancellationToken) => Task.FromResult(Create<double>()));

                    var builder2 = new PipelineBuilderAsync<int, double, string>(builder1, (value, cancellationToken) => Task.FromResult($"{value}"));

                    var func = builder2.Build();

                    _ = await func.Invoke(Create<int>(), cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }
        }
    }
}