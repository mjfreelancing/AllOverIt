using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Patterns.Pipeline;
using FakeItEasy;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Patterns.Pipeline
{
    public class PipelineNoInputBuilderFixture : FixtureBase
    {      
        public class PipelineNoInputBuilder_TIn_TOut : PipelineNoInputBuilderFixture
        {
            public class Constructor : PipelineNoInputBuilder_TIn_TOut
            {
                [Fact]
                public void Should_Throw_When_Step_Null()
                {
                    Invoking(() =>
                    {
                        Func<double> step = null;

                        _ = new PipelineNoInputBuilder<double>(step);
                    })
                        .Should()
                        .Throw<ArgumentNullException>()
                        .WithNamedMessageWhenNull("step");
                }
            }

            public class Build : PipelineNoInputBuilder_TIn_TOut
            {
                [Fact]
                public void Should_Return_Func()
                {
                    Func<double> step = () => Create<double>();

                    var builder = new PipelineNoInputBuilder<double>(step);

                    var actual = builder.Build();

                    actual.Should().BeSameAs(step);
                }
            }
        }

        public class PipelineNoInputBuilder_TIn_TPrevOut_TNextOut : PipelineNoInputBuilderFixture
        {
            public class Constructor : PipelineNoInputBuilder_TIn_TPrevOut_TNextOut
            {
                [Fact]
                public void Should_Throw_When_PrevStep_Null()
                {
                    Invoking(() =>
                    {
                        Func<double, string> step = value => value.ToString();

                        _ = new PipelineNoInputBuilder<double, string>(null, step);
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
                        var prevStep = this.CreateStub<IPipelineBuilder<double>>();

                        _ = new PipelineNoInputBuilder<double, string>(prevStep, null);
                    })
                        .Should()
                        .Throw<ArgumentNullException>()
                        .WithNamedMessageWhenNull("step");
                }
            }

            public class Build : PipelineNoInputBuilder_TIn_TPrevOut_TNextOut
            {
                [Fact]
                public void Should_Return_Composed_Func()
                {
                    var value = Create<double>();

                    // IPipelineBuilder<double>
                    var builder1 = PipelineBuilder.Pipe<double>(() => value);

                    var builder2 = new PipelineNoInputBuilder<double, string>(builder1, value => $"{value}");

                    var func = builder2.Build();

                    var expected = $"{value}";
                    var actual = func.Invoke();

                    expected.Should().Be(actual);
                }
            }
        }
    }
}