using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Pipeline;
using AllOverIt.Patterns.Pipeline.Extensions;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Tests.Patterns.Pipeline.Extensions
{
    public class PipelineBuilderExtensionsFixture : FixtureBase
    {
        internal class PipelineBuilderDummy : IPipelineBuilder<int, double>
        {
            public Func<int, double> Build()
            {
                throw new NotImplementedException();
            }
        }

        internal class PipelineBuilderNoInputDummy : IPipelineBuilder<double>
        {
            public Func<double> Build()
            {
                throw new NotImplementedException();
            }
        }

        internal class StepDummy : IPipelineStep<double, string>
        {
            public string Execute(double input)
            {
                throw new NotImplementedException();
            }
        }

        internal class PipelineBuilderAsyncDummy : IPipelineBuilderAsync<int, double>
        {
            public Func<int, CancellationToken, Task<double>> Build()
            {
                throw new NotImplementedException();
            }
        }

        internal class PipelineBuilderNoInputAsyncDummy : IPipelineBuilderAsync<double>
        {
            public Func<CancellationToken, Task<double>> Build()
            {
                throw new NotImplementedException();
            }
        }

        internal class StepAsyncDummy : IPipelineStepAsync<double, string>
        {
            public Task<string> ExecuteAsync(double input, CancellationToken vancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        private readonly PipelineBuilderDummy _builderDummy = new();
        private readonly PipelineBuilderNoInputDummy _builderNoInputDummy = new();
        private readonly Func<double, string> _funcStep = value => value.ToString();
        private readonly StepDummy _stepDummy = new();

        private readonly PipelineBuilderAsyncDummy _builderAsyncDummy = new();
        private readonly PipelineBuilderNoInputAsyncDummy _builderNoInputAsyncDummy = new();
        private readonly Func<double, CancellationToken, Task<string>> _funcStepAsync = (value, cancellationToken) => Task.FromResult(value.ToString());
        private readonly StepAsyncDummy _stepAsyncDummy = new();

        #region Pipe - with input, synchronous, return IPipelineBuilder<TIn, TNextOut>

        public class Pipe_Func_Builder : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = ((IPipelineBuilder<int, double>) null).Pipe(_funcStep);
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
                    _ = _builderDummy.Pipe((Func<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = _builderDummy.Pipe(_funcStep);

                actual.Should().BeOfType<PipelineBuilder<int, double, string>>();
            }
        }

        public class Pipe_Step_Instance_Builder : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = ((IPipelineBuilder<int, double>) null).Pipe(_stepDummy);
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
                    _ = _builderDummy.Pipe((IPipelineStep<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = _builderDummy.Pipe(_stepDummy);

                actual.Should().BeOfType<PipelineBuilder<int, double, string>>();
            }
        }

        public class Pipe_Step_Type_Builder : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = ((IPipelineBuilder<int, double>) null).Pipe<StepDummy, int, double, string>();
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("prevStep");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = _builderDummy.Pipe<StepDummy, int, double, string>();

                actual.Should().BeOfType<PipelineBuilder<int, double, string>>();
            }
        }

        #endregion

        #region Pipe - with input, asynchronous, return IPipelineBuilderAsync<TIn, TNextOut>

        public class PipeAsync_FuncAsync_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = ((IPipelineBuilderAsync<int, double>) null).PipeAsync(_funcStepAsync);
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
                    _ = _builderAsyncDummy.PipeAsync((Func<double, CancellationToken, Task<string>>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = _builderAsyncDummy.PipeAsync(_funcStepAsync);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }
        }

        public class PipeAsync_StepAsync_Instance_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = ((IPipelineBuilderAsync<int, double>) null).PipeAsync(_stepAsyncDummy);
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
                    _ = _builderAsyncDummy.PipeAsync((IPipelineStepAsync<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = _builderAsyncDummy.PipeAsync(_stepAsyncDummy);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }
        }

        public class PipeAsync_StepAsync_Type_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.PipeAsync<StepAsyncDummy, int, double, string>(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("prevStep");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = _builderAsyncDummy.PipeAsync<StepAsyncDummy, int, double, string>();

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }
        }

        #endregion

        #region Pipe - with input, synchronous, return IPipelineBuilderAsync<TIn, TNextOut>

        public class Pipe_Func_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = ((IPipelineBuilderAsync<int, double>) null).Pipe(_funcStep);
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
                    _ = _builderAsyncDummy.Pipe((Func<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = _builderAsyncDummy.Pipe(_funcStep);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }

            [Fact]
            public async Task Should_Invoke_Pipeline()
            {
                var value = Create<int>();

                var pipe = PipelineBuilder
                   .PipeAsync<int, int>((value, cancellationToken) => Task.FromResult(value + 1))
                   .Pipe(v => v * 3)
                   .Build();

                var expected = (value + 1) * 3;

                var actual = await pipe.Invoke(value, CancellationToken.None);

                expected.Should().Be(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    var pipe = PipelineBuilder
                       .PipeAsync<int, int>((value, cancellationToken) => Task.FromResult(Create<int>()))
                       .Pipe(v => v)
                       .Build();

                    _ = await pipe.Invoke(Create<int>(), cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }
        }

        public class Pipe_Step_Instance_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = ((IPipelineBuilderAsync<int, double>) null).Pipe(_stepDummy);
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
                    _ = _builderAsyncDummy.Pipe((IPipelineStep<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = _builderAsyncDummy.Pipe(_stepDummy);

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }
        }

        public class Pipe_Step_Type_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = ((IPipelineBuilderAsync<int, double>) null).Pipe<StepDummy, int, double, string>();
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("prevStep");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = _builderAsyncDummy.Pipe<StepDummy, int, double, string>();

                actual.Should().BeOfType<PipelineBuilderAsync<int, double, string>>();
            }
        }

        #endregion

        #region Pipe - with no input, synchronous, return IPipelineBuilder<TIn, TNextOut>

        public class Pipe_NoInput_Func_Builder : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = ((IPipelineBuilder<double>) null).Pipe(_funcStep);
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
                    _ = _builderNoInputDummy.Pipe((Func<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = _builderNoInputDummy.Pipe(_funcStep);

                actual.Should().BeOfType<PipelineNoInputBuilder<double, string>>();
            }
        }

        public class Pipe_NoInput_Step_Instance_Builder : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = ((IPipelineBuilder<double>) null).Pipe(_stepDummy);
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
                    _ = _builderNoInputDummy.Pipe((IPipelineStep<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = _builderNoInputDummy.Pipe(_stepDummy);

                actual.Should().BeOfType<PipelineNoInputBuilder<double, string>>();
            }
        }

        public class Pipe_NoInput_Step_Type_Builder : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = ((IPipelineBuilder<double>) null).Pipe<StepDummy, double, string>();
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("prevStep");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = _builderNoInputDummy.Pipe<StepDummy, double, string>();

                actual.Should().BeOfType<PipelineNoInputBuilder<double, string>>();
            }
        }

        #endregion

        #region Pipe - with no input, asynchronous, return IPipelineBuilderAsync<TIn, TNextOut>

        public class PipeAsync_NoInput_FuncAsync_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.PipeAsync(null, _funcStepAsync);
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
                    _ = _builderNoInputAsyncDummy.PipeAsync((Func<double, CancellationToken, Task<string>>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = _builderNoInputAsyncDummy.PipeAsync(_funcStepAsync);

                actual.Should().BeOfType<PipelineNoInputBuilderAsync<double, string>>();
            }
        }

        public class PipeAsync_NoInput_StepAsync_Instance_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.PipeAsync(null, _stepAsyncDummy);
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
                    _ = _builderNoInputAsyncDummy.PipeAsync((IPipelineStepAsync<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = _builderNoInputAsyncDummy.PipeAsync(_stepAsyncDummy);

                actual.Should().BeOfType<PipelineNoInputBuilderAsync<double, string>>();
            }
        }

        public class PipeAsync_NoInput_StepAsync_Type_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = PipelineBuilderExtensions.PipeAsync<StepAsyncDummy, double, string>(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("prevStep");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = _builderNoInputAsyncDummy.PipeAsync<StepAsyncDummy, double, string>();

                actual.Should().BeOfType<PipelineNoInputBuilderAsync<double, string>>();
            }
        }

        #endregion

        #region Pipe - with no input, synchronous, return IPipelineBuilderAsync<TIn, TNextOut>

        public class Pipe_NoInput_Func_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = ((IPipelineBuilderAsync<double>) null).Pipe(_funcStep);
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
                    _ = _builderNoInputAsyncDummy.Pipe((Func<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = _builderNoInputAsyncDummy.Pipe(_funcStep);

                actual.Should().BeOfType<PipelineNoInputBuilderAsync<double, string>>();
            }

            [Fact]
            public async Task Should_Invoke_Pipeline()
            {
                var value = Create<int>();

                var pipe = PipelineBuilder
                   .PipeAsync(cancellationToken => Task.FromResult(value + 1))
                   .Pipe(v => v * 3)
                   .Build();

                var expected = (value + 1) * 3;

                var actual = await pipe.Invoke(CancellationToken.None);

                expected.Should().Be(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    var pipe = PipelineBuilder
                       .PipeAsync(cancellationToken => Task.FromResult(Create<int>()))
                       .Pipe(v => v)
                       .Build();

                    var actual = await pipe.Invoke(cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }
        }

        public class Pipe_NoInput_Step_Instance_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = ((IPipelineBuilderAsync<double>) null).Pipe(_stepDummy);
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
                    _ = _builderNoInputAsyncDummy.Pipe((IPipelineStep<double, string>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("step");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = _builderNoInputAsyncDummy.Pipe(_stepDummy);

                actual.Should().BeOfType<PipelineNoInputBuilderAsync<double, string>>();
            }
        }

        public class Pipe_NoInput_Step_Type_BuilderAsync : PipelineBuilderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prev_Step_Null()
            {
                Invoking(() =>
                {
                    _ = ((IPipelineBuilderAsync<double>) null).Pipe<StepDummy, double, string>();
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("prevStep");
            }

            [Fact]
            public void Should_Create_Step_Builder()
            {
                var actual = _builderNoInputAsyncDummy.Pipe<StepDummy, double, string>();

                actual.Should().BeOfType<PipelineNoInputBuilderAsync<double, string>>();
            }
        }

        #endregion
    }
}