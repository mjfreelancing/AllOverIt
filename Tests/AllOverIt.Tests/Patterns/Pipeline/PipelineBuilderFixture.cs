using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Patterns.Pipeline;
using AllOverIt.Patterns.Pipeline.Extensions;
using FluentAssertions;

namespace AllOverIt.Tests.Patterns.Pipeline
{
    public class PipelineBuilderFixture : FixtureBase
    {
        private sealed class PipelineStep1Dummy : IPipelineStep<int, double>
        {
            public const double DefaultFactor = 1.23;

            private readonly double _factor;

            public PipelineStep1Dummy()
            {
                _factor = DefaultFactor;
            }

            public PipelineStep1Dummy(double factor)
            {
                _factor = factor;
            }

            public double Execute(int input)
            {
                return input + _factor;
            }
        }

        private sealed class PipelineStep2Dummy : IPipelineStep<double, string>
        {
            public const double DefaultFactor = 1.23;

            private readonly double _factor;

            public PipelineStep2Dummy()
            {
                _factor = DefaultFactor;
            }

            public PipelineStep2Dummy(double factor)
            {
                _factor = factor;
            }

            public string Execute(double input)
            {
                return (input * _factor).ToString();
            }
        }

        private sealed class PipelineStep1AsyncDummy : IPipelineStepAsync<int, double>
        {
            public const double DefaultFactor = 1.23;

            private readonly double _factor;

            public PipelineStep1AsyncDummy()
            {
                _factor = DefaultFactor;
            }

            public PipelineStep1AsyncDummy(double factor)
            {
                _factor = factor;
            }

            public Task<double> ExecuteAsync(int input, CancellationToken cancellationToken)
            {
                return Task.FromResult(input + _factor);
            }
        }

        private sealed class PipelineStep2AsyncDummy : IPipelineStepAsync<double, string>
        {
            public const double DefaultFactor = 1.23;

            private readonly double _factor;

            public PipelineStep2AsyncDummy()
            {
                _factor = DefaultFactor;
            }

            public PipelineStep2AsyncDummy(double factor)
            {
                _factor = factor;
            }

            public Task<string> ExecuteAsync(double input, CancellationToken cancellationToken)
            {
                return Task.FromResult((input * _factor).ToString());
            }
        }

        public class Pipe_NoInput_Func : PipelineBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                Func<double> step = null;

                Invoking(() =>
                {
                    _ = PipelineBuilder.Pipe(step);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Return_Pipeline_Builder()
            {
                Func<double> step = () => Create<double>();

                var actual = PipelineBuilder.Pipe(step);

                actual.Should().BeOfType<PipelineNoInputBuilder<double>>();
            }

            [Fact]
            public void Should_Create_Pipeline_Step()
            {
                var invoked = false;

                Func<double> step = () =>
                {
                    invoked = true;
                    return Create<double>();
                };

                var actual = PipelineBuilder.Pipe(step);

                actual.Build().Invoke();

                invoked.Should().BeTrue();
            }

            [Fact]
            public void Should_Create_Pipeline_Sequence()
            {
                var counter = 0;
                var invoked1 = 0;
                var invoked2 = 0;

                Func<double> step1 = () =>
                {
                    counter++;
                    invoked1 = counter;
                    return Create<double>();
                };

                Func<double, string> step2 = value =>
                {
                    counter++;
                    invoked2 = counter;

                    return value.ToString();
                };

                var actual = PipelineBuilder.Pipe(step1).Pipe(step2);

                actual.Build().Invoke();

                invoked1.Should().Be(1);
                invoked2.Should().Be(2);
            }
        }

        public class Pipe_Func : PipelineBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                Func<int, double> step = null;

                Invoking(() =>
                {
                    _ = PipelineBuilder.Pipe(step);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Return_Pipeline_Builder()
            {
                Func<int, double> step = value => (double) value;

                var actual = PipelineBuilder.Pipe(step);

                actual.Should().BeOfType<PipelineBuilder<int, double>>();
            }

            [Fact]
            public void Should_Create_Pipeline_Step()
            {
                var invoked = false;

                Func<int, double> step = value =>
                {
                    invoked = true;
                    return (double) value;
                };

                var actual = PipelineBuilder.Pipe(step);

                actual.Build().Invoke(Create<int>());

                invoked.Should().BeTrue();
            }

            [Fact]
            public void Should_Create_Pipeline_Sequence()
            {
                var counter = 0;
                var invoked1 = 0;
                var invoked2 = 0;

                Func<int, double> step1 = value =>
                {
                    counter++;
                    invoked1 = counter;
                    return (double) value;
                };

                Func<double, string> step2 = value =>
                {
                    counter++;
                    invoked2 = counter;

                    return value.ToString();
                };

                var actual = PipelineBuilder.Pipe(step1).Pipe(step2);

                actual.Build().Invoke(Create<int>());

                invoked1.Should().Be(1);
                invoked2.Should().Be(2);
            }
        }

        public class Pipe_PipelineStep_Instance : PipelineBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                IPipelineStep<int, double> step = null;

                Invoking(() =>
                {
                    _ = PipelineBuilder.Pipe(step);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Return_Pipeline_Builder()
            {
                var step = new PipelineStep1Dummy(Create<double>());

                var actual = PipelineBuilder.Pipe(step);

                actual.Should().BeOfType<PipelineBuilder<int, double>>();
            }

            [Fact]
            public void Should_Create_Pipeline_Step()
            {
                var factor = Create<double>();
                var step = new PipelineStep1Dummy(factor);

                var actual = PipelineBuilder.Pipe(step);

                var input = Create<int>();
                var result = actual.Build().Invoke(input);

                var expected = input + factor;

                expected.Should().Be(result);
            }

            [Fact]
            public void Should_Create_Pipeline_Sequence()
            {
                var factor1 = Create<double>();
                var step1 = new PipelineStep1Dummy(factor1);

                var factor2 = Create<double>();
                var step2 = new PipelineStep2Dummy(factor2);

                var actual = PipelineBuilder.Pipe(step1).Pipe(step2);

                var input = Create<int>();
                var result = actual.Build().Invoke(input);

                var expected = $"{(input + factor1) * factor2}";

                expected.Should().Be(result);
            }
        }

        public class Pipe_PipelineStep_Type : PipelineBuilderFixture
        {
            [Fact]
            public void Should_Return_Pipeline_Builder()
            {
                var actual = PipelineBuilder.Pipe<PipelineStep1Dummy, int, double>();

                actual.Should().BeOfType<PipelineBuilder<int, double>>();
            }

            [Fact]
            public void Should_Create_Pipeline_Step()
            {
                var factor = PipelineStep1Dummy.DefaultFactor;

                var actual = PipelineBuilder.Pipe<PipelineStep1Dummy, int, double>();

                var input = Create<int>();
                var result = actual.Build().Invoke(input);

                var expected = input + factor;

                expected.Should().Be(result);
            }

            [Fact]
            public void Should_Create_Pipeline_Sequence()
            {
                var factor1 = PipelineStep1Dummy.DefaultFactor;
                var factor2 = PipelineStep2Dummy.DefaultFactor;

                var actual = PipelineBuilder
                    .Pipe<PipelineStep1Dummy, int, double>()
                    .Pipe<PipelineStep2Dummy, int, double, string>();

                var input = Create<int>();
                var result = actual.Build().Invoke(input);

                var expected = $"{(input + factor1) * factor2}";

                expected.Should().Be(result);
            }
        }

        public class PipeAsync_NoInput__Func : PipelineBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                Func<CancellationToken, Task<double>> step = null;

                Invoking(() =>
                {
                    _ = PipelineBuilder.PipeAsync(step);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Return_Pipeline_Builder()
            {
                Func<CancellationToken, Task<double>> step = cancellationToken => Task.FromResult(Create<double>());

                var actual = PipelineBuilder.PipeAsync(step);

                actual.Should().BeOfType<PipelineNoInputBuilderAsync<double>>();
            }

            [Fact]
            public async Task Should_Create_Pipeline_Step()
            {
                var invoked = false;

                Func<CancellationToken, Task<double>> step = cancellationToken =>
                {
                    invoked = true;
                    return Task.FromResult(Create<double>());
                };

                var actual = PipelineBuilder.PipeAsync(step);

                await actual.Build().Invoke(CancellationToken.None);

                invoked.Should().BeTrue();
            }

            // Cannot test 'Throw_When_Cancelled' since the 'step' is responsible for throwing

            [Fact]
            public void Should_Create_Pipeline_Sequence()
            {
                var counter = 0;
                var invoked1 = 0;
                var invoked2 = 0;

                Func<CancellationToken, Task<double>> step1 = cancellationToken =>
                {
                    counter++;
                    invoked1 = counter;
                    return Task.FromResult(Create<double>());
                };

                Func<double, CancellationToken, Task<string>> step2 = (value, cancellationToken) =>
                {
                    counter++;
                    invoked2 = counter;
                    return Task.FromResult(value.ToString());
                };

                var actual = PipelineBuilder.PipeAsync(step1).PipeAsync(step2);

                actual.Build().Invoke(CancellationToken.None);

                invoked1.Should().Be(1);
                invoked2.Should().Be(2);
            }
        }

        public class PipeAsync_Func : PipelineBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                Func<int, CancellationToken, Task<double>> step = null;

                Invoking(() =>
                {
                    _ = PipelineBuilder.PipeAsync(step);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Return_Pipeline_Builder()
            {
                Func<int, CancellationToken, Task<double>> step = (value, cancellationToken) => Task.FromResult((double) value);

                var actual = PipelineBuilder.PipeAsync(step);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double>>();
            }

            // Cannot test 'Throw_When_Cancelled' since the 'step' is responsible for throwing

            [Fact]
            public async Task Should_Create_Pipeline_Step()
            {
                var invoked = false;

                Func<int, CancellationToken, Task<double>> step = (value, cancellationToken) =>
                {
                    invoked = true;
                    return Task.FromResult((double) value);
                };

                var actual = PipelineBuilder.PipeAsync(step);

                await actual.Build().Invoke(Create<int>(), CancellationToken.None);

                invoked.Should().BeTrue();
            }

            [Fact]
            public void Should_Create_Pipeline_Sequence()
            {
                var counter = 0;
                var invoked1 = 0;
                var invoked2 = 0;

                Func<int, CancellationToken, Task<double>> step1 = (value, cancellationToken) =>
                {
                    counter++;
                    invoked1 = counter;
                    return Task.FromResult((double) value);
                };

                Func<double, CancellationToken, Task<string>> step2 = (value, cancellationToken) =>
                {
                    counter++;
                    invoked2 = counter;
                    return Task.FromResult(value.ToString());
                };

                var actual = PipelineBuilder.PipeAsync(step1).PipeAsync(step2);

                actual.Build().Invoke(Create<int>(), CancellationToken.None);

                invoked1.Should().Be(1);
                invoked2.Should().Be(2);
            }
        }

        public class PipeAsync_PipelineStep_Instance : PipelineBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                IPipelineStepAsync<int, double> step = null;

                Invoking(() =>
                {
                    _ = PipelineBuilder.PipeAsync(step);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Return_Pipeline_Builder()
            {
                var step = new PipelineStep1AsyncDummy(Create<double>());

                var actual = PipelineBuilder.PipeAsync(step);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double>>();
            }

            [Fact]
            public async Task Should_Create_Pipeline_Step()
            {
                var factor = Create<double>();
                var step = new PipelineStep1AsyncDummy(factor);

                var actual = PipelineBuilder.PipeAsync(step);

                var input = Create<int>();
                var result = await actual.Build().Invoke(input, CancellationToken.None);

                var expected = input + factor;

                expected.Should().Be(result);
            }

            [Fact]
            public async Task Should_Create_Pipeline_Sequence()
            {
                var factor1 = Create<double>();
                var step1 = new PipelineStep1AsyncDummy(factor1);

                var factor2 = Create<double>();
                var step2 = new PipelineStep2AsyncDummy(factor2);

                var actual = PipelineBuilder.PipeAsync(step1).PipeAsync(step2);

                var input = Create<int>();
                var result = await actual.Build().Invoke(input, CancellationToken.None);

                var expected = $"{(input + factor1) * factor2}";

                expected.Should().Be(result);
            }
        }

        public class PipeAsync_PipelineStep_Type : PipelineBuilderFixture
        {
            [Fact]
            public void Should_Return_Pipeline_Builder()
            {
                var actual = PipelineBuilder.PipeAsync<PipelineStep1AsyncDummy, int, double>();

                actual.Should().BeOfType<PipelineBuilderAsync<int, double>>();
            }

            // Cannot test 'Throw_When_Cancelled' since the 'step' is responsible for throwing

            [Fact]
            public async Task Should_Create_Pipeline_Step()
            {
                var factor = PipelineStep1Dummy.DefaultFactor;

                var actual = PipelineBuilder.PipeAsync<PipelineStep1AsyncDummy, int, double>();

                var input = Create<int>();
                var result = await actual.Build().Invoke(input, CancellationToken.None);

                var expected = input + factor;

                expected.Should().Be(result);
            }

            [Fact]
            public async Task Should_Create_Pipeline_Sequence()
            {
                var factor1 = PipelineStep1AsyncDummy.DefaultFactor;
                var factor2 = PipelineStep2AsyncDummy.DefaultFactor;

                var actual = PipelineBuilder
                    .PipeAsync<PipelineStep1AsyncDummy, int, double>()
                    .PipeAsync<PipelineStep2AsyncDummy, int, double, string>();

                var input = Create<int>();
                var result = await actual.Build().Invoke(input, CancellationToken.None);

                var expected = $"{(input + factor1) * factor2}";

                expected.Should().Be(result);
            }
        }

        public class PipelineBuilder_TIn_TOut : PipelineBuilderFixture
        {
            public class Constructor : PipelineBuilder_TIn_TOut
            {
                [Fact]
                public void Should_Throw_When_Step_Null()
                {
                    Invoking(() =>
                    {
                        Func<int, double> step = null;

                        _ = new PipelineBuilder<int, double>(step);
                    })
                        .Should()
                        .Throw<ArgumentNullException>()
                        .WithNamedMessageWhenNull("step");
                }
            }

            public class Build : PipelineBuilder_TIn_TOut
            {
                [Fact]
                public void Should_Return_Func()
                {
                    Func<int, double> step = value => (double) value;

                    var builder = new PipelineBuilder<int, double>(step);

                    var actual = builder.Build();

                    actual.Should().BeSameAs(step);
                }
            }
        }

        public class PipelineBuilder_TIn_TPrevOut_TNextOut : PipelineBuilderFixture
        {
            public class Constructor : PipelineBuilder_TIn_TPrevOut_TNextOut
            {
                [Fact]
                public void Should_Throw_When_PrevStep_Null()
                {
                    Invoking(() =>
                    {
                        Func<double, string> step = value => value.ToString();

                        _ = new PipelineBuilder<int, double, string>(null, step);
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
                        var prevStep = this.CreateStub<IPipelineBuilder<int, double>>();

                        _ = new PipelineBuilder<int, double, string>(prevStep, null);
                    })
                        .Should()
                        .Throw<ArgumentNullException>()
                        .WithNamedMessageWhenNull("step");
                }
            }

            public class Build : PipelineBuilder_TIn_TPrevOut_TNextOut
            {
                [Fact]
                public void Should_Return_Composed_Func()
                {
                    var factor = Create<double>();

                    // IPipelineBuilder<int, double>
                    var builder1 = PipelineBuilder.Pipe<int, double>(value => value * factor);

                    var builder2 = new PipelineBuilder<int, double, string>(builder1, value => $"{value}");

                    var func = builder2.Build();

                    var input = Create<int>();
                    var expected = $"{input * factor}";

                    var actual = func.Invoke(input);

                    expected.Should().Be(actual);
                }
            }
        }
    }
}