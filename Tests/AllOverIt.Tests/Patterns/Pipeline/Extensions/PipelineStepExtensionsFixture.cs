using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Pipeline;
using AllOverIt.Patterns.Pipeline.Extensions;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Patterns.Pipeline.Extensions
{
    public class PipelineStepExtensionsFixture : FixtureBase
    {
        public class Func_IPipelineStep : PipelineStepExtensionsFixture
        {
            private sealed class PipelineStepDummy : IPipelineStep<int, double>
            {
                private readonly double _factor;

                public PipelineStepDummy(double factor)
                {
                    _factor = factor;
                }

                public double Execute(int input)
                {
                    return input + _factor;
                }
            }

            [Fact]
            public void Should_Throw_When_Step_Null()
            {
                Invoking(() =>
                {
                    IPipelineStep<int, double> step = null;

                    _ = step.AsFunc();
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }


            [Fact]
            public void Should_Return_Func()
            {
                var factor = Create<double>();
                var step = new PipelineStepDummy(factor);

                var func = step.AsFunc();

                var input = Create<int>();
                var expected = input + factor;

                var actual = func.Invoke(input);
                expected.Should().Be(actual);
            }
        }

        public class Func_IPipelineStepAsync : PipelineStepExtensionsFixture
        {
            private sealed class PipelineStepAsyncDummy : IPipelineStepAsync<int, double>
            {
                private readonly double _factor;

                public PipelineStepAsyncDummy(double factor)
                {
                    _factor = factor;
                }

                public Task<double> ExecuteAsync(int input, CancellationToken cancellationToken)
                {
                    return Task.FromResult(input + _factor);
                }
            }

            [Fact]
            public void Should_Throw_When_Step_Null()
            {
                Invoking(() =>
                {
                    IPipelineStepAsync<int, double> step = null;

                    _ = step.AsFunc();
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public async Task Should_Return_Func()
            {
                var factor = Create<double>();
                var step = new PipelineStepAsyncDummy(factor);

                var func = step.AsFunc();

                var input = Create<int>();
                var expected = input + factor;

                var actual = await func.Invoke(input, CancellationToken.None);
                expected.Should().Be(actual);
            }

            // Cannot test 'Throw_When_Cancelled' since it is up to the 'step' implementation to throw
        }
    }
}