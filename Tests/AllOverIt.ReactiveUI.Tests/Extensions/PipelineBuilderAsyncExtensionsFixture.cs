using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Patterns.Pipeline;
using AllOverIt.ReactiveUI.Extensions;
using FakeItEasy;
using FluentAssertions;
using ReactiveUI;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.ReactiveUI.Tests.Extensions
{
    public class PipelineBuilderAsyncExtensionsFixture : FixtureBase
    {
        public class PipelineBuilderAsync_TIn_TPrevOut_TNextOut : PipelineBuilderAsyncExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_PrevStep_Null()
            {
                Invoking(() =>
                {
                    var command = ReactiveCommand.Create<double, string>(_ => string.Empty);

                    _ = PipelineBuilderAsyncExtensions.Pipe<int, double, string>(null, command);
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

                    _ = PipelineBuilderAsyncExtensions.Pipe<int, double, string>(prevStep, null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Return_PipelineBuilderAsync()
            {
                // IPipelineBuilderAsync<int, double>
                var prevStep = PipelineBuilder.PipeAsync<int, double>((_, cancellationToken) => Task.FromResult(Create<double>()));

                var command = ReactiveCommand.Create<double, string>(_ => string.Empty);

                var actual = PipelineBuilderAsyncExtensions.Pipe<int, double, string>(prevStep, command);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }
        }
    }
}