using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Tests.Extensions
{
    public partial class EnumerableExtensionsFixture : FixtureBase
    {
        #region ForEachAsTaskAsync

        public class ForEachAsTaskAsync : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsTaskAsync<int>(null, (_, _) => Task.CompletedTask, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsTaskAsync<int>(new[] { Create<int>() }, null, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("func");
            }

            [Theory]
            [InlineData(0)]
            [InlineData(-1)]
            public void Should_Throw_When_DegreeOfParallelism_Less_Than_One(int degreeOfParallelism)
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsTaskAsync<int>(new[] { Create<int>() }, (_, _) => Task.CompletedTask, degreeOfParallelism);
                })
                .Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage("At least one task must be specified. (Parameter 'degreeOfParallelism')");
            }

            [Fact]
            public async Task Should_Process_Collection()
            {
                var expected = Enumerable.Range(0, 100).ToList();

                var actual = new ConcurrentQueue<int>();

                await EnumerableExtensions.ForEachAsTaskAsync<int>(expected, (value, token) =>
                {
                    actual.Enqueue(value);

                    return Task.CompletedTask;
                }, GetWithinRange(2, 10));

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Cancel_Process()
            {
                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    await EnumerableExtensions.ForEachAsTaskAsync<int>(new[] { Create<int>() }, (_, _) => Task.CompletedTask, 1, cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Pass_CancellationToken()
            {
                var cts = new CancellationTokenSource();

                CancellationToken actual = default;

                await EnumerableExtensions.ForEachAsTaskAsync(new[] { Create<int>() }, (_, token) =>
                {
                    actual = token;

                    return Task.CompletedTask;
                }, 1, cts.Token);

                actual.Should().Be(cts.Token);  // Be() and not BeSameAs() as stucts are copied by value
            }
        }

        public class ForEachAsTaskAsync_Input_1 : EnumerableExtensionsFixture
        {
            private readonly double _input1;

            public ForEachAsTaskAsync_Input_1()
            {
                _input1 = Create<double>();
            }

            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsTaskAsync<int, double>(null, (value, input1, token) => Task.CompletedTask, _input1, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsTaskAsync<int, double>(new[] { Create<int>() }, null, _input1, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("func");
            }

            [Theory]
            [InlineData(0)]
            [InlineData(-1)]
            public void Should_Throw_When_DegreeOfParallelism_Less_Than_One(int degreeOfParallelism)
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsTaskAsync<int, double>(new[] { Create<int>() }, (value, input1, token) => Task.CompletedTask, _input1, degreeOfParallelism);
                })
                .Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage("At least one task must be specified. (Parameter 'degreeOfParallelism')");
            }

            [Fact]
            public async Task Should_Process_Collection()
            {
                var expected = Enumerable.Range(0, 100).ToList();

                var actual = new ConcurrentQueue<int>();

                await EnumerableExtensions.ForEachAsTaskAsync<int, double>(expected, (value, input1, token) =>
                {
                    input1.Should().Be(_input1);

                    actual.Enqueue(value);

                    return Task.CompletedTask;
                }, _input1, GetWithinRange(2, 10));

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Cancel_Process()
            {
                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    await EnumerableExtensions.ForEachAsTaskAsync<int, double>(new[] { Create<int>() }, (value, input1, token) => Task.CompletedTask, _input1, 1, cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Pass_CancellationToken()
            {
                var cts = new CancellationTokenSource();

                CancellationToken actual = default;

                await EnumerableExtensions.ForEachAsTaskAsync<int, double>(new[] { Create<int>() }, (value, input1, token) =>
                {
                    actual = token;

                    return Task.CompletedTask;
                }, _input1, 1, cts.Token);

                actual.Should().Be(cts.Token);  // Be() and not BeSameAs() as stucts are copied by value
            }
        }

        public class ForEachAsTaskAsync_Input_2 : EnumerableExtensionsFixture
        {
            private readonly double _input1;
            private readonly double _input2;

            public ForEachAsTaskAsync_Input_2()
            {
                _input1 = Create<double>();
                _input2 = Create<double>();
            }

            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsTaskAsync<int, double, double>(null, (value, input1, input2, token) => Task.CompletedTask, _input1, _input2, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsTaskAsync<int, double, double>(new[] { Create<int>() }, null, _input1, _input2, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("func");
            }

            [Theory]
            [InlineData(0)]
            [InlineData(-1)]
            public void Should_Throw_When_DegreeOfParallelism_Less_Than_One(int degreeOfParallelism)
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsTaskAsync<int, double, double>(new[] { Create<int>() }, (value, input1, input2, token) => Task.CompletedTask, _input1, _input2, degreeOfParallelism);
                })
                .Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage("At least one task must be specified. (Parameter 'degreeOfParallelism')");
            }

            [Fact]
            public async Task Should_Process_Collection()
            {
                var expected = Enumerable.Range(0, 100).ToList();

                var actual = new ConcurrentQueue<int>();

                await EnumerableExtensions.ForEachAsTaskAsync<int, double, double>(expected, (value, input1, input2, token) =>
                {
                    input1.Should().Be(_input1);
                    input2.Should().Be(_input2);

                    actual.Enqueue(value);

                    return Task.CompletedTask;
                }, _input1, _input2, GetWithinRange(2, 10));

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Cancel_Process()
            {
                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    await EnumerableExtensions.ForEachAsTaskAsync<int, double, double>(new[] { Create<int>() }, (value, input1, input2, token) => Task.CompletedTask, _input1, _input2, 1, cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Pass_CancellationToken()
            {
                var cts = new CancellationTokenSource();

                CancellationToken actual = default;

                await EnumerableExtensions.ForEachAsTaskAsync<int, double, double>(new[] { Create<int>() }, (value, input1, input2, token) =>
                {
                    actual = token;

                    return Task.CompletedTask;
                }, _input1, _input2, 1, cts.Token);

                actual.Should().Be(cts.Token);  // Be() and not BeSameAs() as stucts are copied by value
            }
        }

        public class ForEachAsTaskAsync_Input_3 : EnumerableExtensionsFixture
        {
            private readonly double _input1;
            private readonly double _input2;
            private readonly double _input3;

            public ForEachAsTaskAsync_Input_3()
            {
                _input1 = Create<double>();
                _input2 = Create<double>();
                _input3 = Create<double>();
            }

            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsTaskAsync<int, double, double, double>(null,
                        (value, input1, input2, input3, token) => Task.CompletedTask, _input1, _input2, _input3, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsTaskAsync<int, double, double, double>(new[] { Create<int>() }, null, _input1, _input2, _input3, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("func");
            }

            [Theory]
            [InlineData(0)]
            [InlineData(-1)]
            public void Should_Throw_When_DegreeOfParallelism_Less_Than_One(int degreeOfParallelism)
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsTaskAsync<int, double, double, double>(new[] { Create<int>() },
                        (value, input1, input2, input3, token) => Task.CompletedTask, _input1, _input2, _input3, degreeOfParallelism);
                })
                .Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage("At least one task must be specified. (Parameter 'degreeOfParallelism')");
            }

            [Fact]
            public async Task Should_Process_Collection()
            {
                var expected = Enumerable.Range(0, 100).ToList();

                var actual = new ConcurrentQueue<int>();

                await EnumerableExtensions.ForEachAsTaskAsync<int, double, double, double>(expected, (value, input1, input2, input3, token) =>
                {
                    input1.Should().Be(_input1);
                    input2.Should().Be(_input2);
                    input3.Should().Be(_input3);

                    actual.Enqueue(value);

                    return Task.CompletedTask;
                }, _input1, _input2, _input3, GetWithinRange(2, 10));

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Cancel_Process()
            {
                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    await EnumerableExtensions.ForEachAsTaskAsync<int, double, double, double>(new[] { Create<int>() },
                        (value, input1, input2, input3, token) => Task.CompletedTask, _input1, _input2, _input3, 1, cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Pass_CancellationToken()
            {
                var cts = new CancellationTokenSource();

                CancellationToken actual = default;

                await EnumerableExtensions.ForEachAsTaskAsync<int, double, double, double>(new[] { Create<int>() }, (value, input1, input2, input3, token) =>
                {
                    actual = token;

                    return Task.CompletedTask;
                }, _input1, _input2, _input3, 1, cts.Token);

                actual.Should().Be(cts.Token);  // Be() and not BeSameAs() as stucts are copied by value
            }
        }

        public class ForEachAsTaskAsync_Input_4 : EnumerableExtensionsFixture
        {
            private readonly double _input1;
            private readonly double _input2;
            private readonly double _input3;
            private readonly double _input4;

            public ForEachAsTaskAsync_Input_4()
            {
                _input1 = Create<double>();
                _input2 = Create<double>();
                _input3 = Create<double>();
                _input4 = Create<double>();
            }

            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsTaskAsync<int, double, double, double, double>(null,
                        (value, input1, input2, input3, input4, token) => Task.CompletedTask, _input1, _input2, _input3, _input4, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsTaskAsync<int, double, double, double, double>(new[] { Create<int>() }, null, _input1, _input2, _input3, _input4, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("func");
            }

            [Theory]
            [InlineData(0)]
            [InlineData(-1)]
            public void Should_Throw_When_DegreeOfParallelism_Less_Than_One(int degreeOfParallelism)
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsTaskAsync<int, double, double, double, double>(new[] { Create<int>() },
                        (value, input1, input2, input3, input4, token) => Task.CompletedTask, _input1, _input2, _input3, _input4, degreeOfParallelism);
                })
                .Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage("At least one task must be specified. (Parameter 'degreeOfParallelism')");
            }

            [Fact]
            public async Task Should_Process_Collection()
            {
                var expected = Enumerable.Range(0, 100).ToList();

                var actual = new ConcurrentQueue<int>();

                await EnumerableExtensions.ForEachAsTaskAsync<int, double, double, double, double>(expected, (value, input1, input2, input3, input4, token) =>
                {
                    input1.Should().Be(_input1);
                    input2.Should().Be(_input2);
                    input3.Should().Be(_input3);
                    input4.Should().Be(_input4);

                    actual.Enqueue(value);

                    return Task.CompletedTask;
                }, _input1, _input2, _input3, _input4, GetWithinRange(2, 10));

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Cancel_Process()
            {
                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    await EnumerableExtensions.ForEachAsTaskAsync<int, double, double, double, double>(new[] { Create<int>() },
                        (value, input1, input2, input3, input4, token) => Task.CompletedTask, _input1, _input2, _input3, _input4, 1, cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Pass_CancellationToken()
            {
                var cts = new CancellationTokenSource();

                CancellationToken actual = default;

                await EnumerableExtensions.ForEachAsTaskAsync<int, double, double, double, double>(new[] { Create<int>() }, (value, input1, input2, input3, input4, token) =>
                {
                    actual = token;

                    return Task.CompletedTask;
                }, _input1, _input2, _input3, _input4, 1, cts.Token);

                actual.Should().Be(cts.Token);  // Be() and not BeSameAs() as stucts are copied by value
            }
        }

        #endregion

        #region ForEachAsParallelAsync

        public class ForEachAsParallelAsync : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsParallelAsync<int>(null, (_, _) => Task.CompletedTask, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsParallelAsync<int>(new[] { Create<int>() }, null, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("func");
            }

            [Theory]
            [InlineData(0)]
            [InlineData(-1)]
            public void Should_Throw_When_DegreeOfParallelism_Less_Than_One(int degreeOfParallelism)
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsParallelAsync<int>(new[] { Create<int>() }, (_, _) => Task.CompletedTask, degreeOfParallelism);
                })
                .Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage("At least one task must be specified. (Parameter 'degreeOfParallelism')");
            }

            [Fact]
            public async Task Should_Process_Collection()
            {
                var expected = Enumerable.Range(0, 100).ToList();

                var actual = new ConcurrentQueue<int>();

                await EnumerableExtensions.ForEachAsParallelAsync<int>(expected, (value, token) =>
                {
                    actual.Enqueue(value);

                    return Task.CompletedTask;
                }, GetWithinRange(2, 10));

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Cancel_Process()
            {
                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    await EnumerableExtensions.ForEachAsParallelAsync<int>(new[] { Create<int>() }, (_, _) => Task.CompletedTask, 1, cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Pass_CancellationToken()
            {
                var cts = new CancellationTokenSource();

                CancellationToken actual = default;

                await EnumerableExtensions.ForEachAsParallelAsync<int>(new[] { Create<int>() }, (value, token) =>
                {
                    actual = token;

                    return Task.CompletedTask;
                }, 1, cts.Token);

                actual.Should().Be(cts.Token);  // Be() and not BeSameAs() as stucts are copied by value
            }
        }
        public class ForEachAsParallelAsync_Input_1 : EnumerableExtensionsFixture
        {
            private readonly double _input1;

            public ForEachAsParallelAsync_Input_1()
            {
                _input1 = Create<double>();
            }

            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsParallelAsync<int, double>(null, (value, input1, token) => Task.CompletedTask, _input1, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsParallelAsync<int, double>(new[] { Create<int>() }, null, _input1, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("func");
            }

            [Theory]
            [InlineData(0)]
            [InlineData(-1)]
            public void Should_Throw_When_DegreeOfParallelism_Less_Than_One(int degreeOfParallelism)
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsParallelAsync<int, double>(new[] { Create<int>() }, (value, input1, token) => Task.CompletedTask, _input1, degreeOfParallelism);
                })
                .Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage("At least one task must be specified. (Parameter 'degreeOfParallelism')");
            }

            [Fact]
            public async Task Should_Process_Collection()
            {
                var expected = Enumerable.Range(0, 100).ToList();

                var actual = new ConcurrentQueue<int>();

                await EnumerableExtensions.ForEachAsParallelAsync<int, double>(expected, (value, input1, token) =>
                {
                    input1.Should().Be(_input1);

                    actual.Enqueue(value);

                    return Task.CompletedTask;
                }, _input1, GetWithinRange(2, 10));

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Cancel_Process()
            {
                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    await EnumerableExtensions.ForEachAsParallelAsync<int, double>(new[] { Create<int>() }, (value, input1, token) => Task.CompletedTask, _input1, 1, cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Pass_CancellationToken()
            {
                var cts = new CancellationTokenSource();

                CancellationToken actual = default;

                await EnumerableExtensions.ForEachAsParallelAsync<int, double>(new[] { Create<int>() }, (value, input1, token) =>
                {
                    actual = token;

                    return Task.CompletedTask;
                }, _input1, 1, cts.Token);

                actual.Should().Be(cts.Token);  // Be() and not BeSameAs() as stucts are copied by value
            }
        }

        public class ForEachAsParallelAsync_Input_2 : EnumerableExtensionsFixture
        {
            private readonly double _input1;
            private readonly double _input2;

            public ForEachAsParallelAsync_Input_2()
            {
                _input1 = Create<double>();
                _input2 = Create<double>();
            }

            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsParallelAsync<int, double, double>(null, (value, input1, input2, token) => Task.CompletedTask, _input1, _input2, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsParallelAsync<int, double, double>(new[] { Create<int>() }, null, _input1, _input2, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("func");
            }

            [Theory]
            [InlineData(0)]
            [InlineData(-1)]
            public void Should_Throw_When_DegreeOfParallelism_Less_Than_One(int degreeOfParallelism)
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsParallelAsync<int, double, double>(new[] { Create<int>() },
                        (value, input1, input2, token) => Task.CompletedTask, _input1, _input2, degreeOfParallelism);
                })
                .Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage("At least one task must be specified. (Parameter 'degreeOfParallelism')");
            }

            [Fact]
            public async Task Should_Process_Collection()
            {
                var expected = Enumerable.Range(0, 100).ToList();

                var actual = new ConcurrentQueue<int>();

                await EnumerableExtensions.ForEachAsParallelAsync<int, double, double>(expected, (value, input1, input2, token) =>
                {
                    input1.Should().Be(_input1);
                    input2.Should().Be(_input2);

                    actual.Enqueue(value);

                    return Task.CompletedTask;
                }, _input1, _input2, GetWithinRange(2, 10));

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Cancel_Process()
            {
                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    await EnumerableExtensions.ForEachAsParallelAsync<int, double, double>(new[] { Create<int>() },
                        (value, input1, input2, token) => Task.CompletedTask, _input1, _input2, 1, cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Pass_CancellationToken()
            {
                var cts = new CancellationTokenSource();

                CancellationToken actual = default;

                await EnumerableExtensions.ForEachAsParallelAsync<int, double, double>(new[] { Create<int>() }, (value, input1, input2, token) =>
                {
                    actual = token;

                    return Task.CompletedTask;
                }, _input1, _input2, 1, cts.Token);

                actual.Should().Be(cts.Token);  // Be() and not BeSameAs() as stucts are copied by value
            }
        }

        public class ForEachAsParallelAsync_Input_3 : EnumerableExtensionsFixture
        {
            private readonly double _input1;
            private readonly double _input2;
            private readonly double _input3;

            public ForEachAsParallelAsync_Input_3()
            {
                _input1 = Create<double>();
                _input2 = Create<double>();
                _input3 = Create<double>();
            }

            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsParallelAsync<int, double, double, double>(null,
                        (value, input1, input2, input3, token) => Task.CompletedTask, _input1, _input2, _input3, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsParallelAsync<int, double, double, double>(new[] { Create<int>() }, null, _input1, _input2, _input3, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("func");
            }

            [Theory]
            [InlineData(0)]
            [InlineData(-1)]
            public void Should_Throw_When_DegreeOfParallelism_Less_Than_One(int degreeOfParallelism)
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsParallelAsync<int, double, double, double>(new[] { Create<int>() },
                        (value, input1, input2, input3, token) => Task.CompletedTask, _input1, _input2, _input3, degreeOfParallelism);
                })
                .Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage("At least one task must be specified. (Parameter 'degreeOfParallelism')");
            }

            [Fact]
            public async Task Should_Process_Collection()
            {
                var expected = Enumerable.Range(0, 100).ToList();

                var actual = new ConcurrentQueue<int>();

                await EnumerableExtensions.ForEachAsParallelAsync<int, double, double, double>(expected, (value, input1, input2, input3, token) =>
                {
                    input1.Should().Be(_input1);
                    input2.Should().Be(_input2);
                    input3.Should().Be(_input3);

                    actual.Enqueue(value);

                    return Task.CompletedTask;
                }, _input1, _input2, _input3, GetWithinRange(2, 10));

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Cancel_Process()
            {
                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    await EnumerableExtensions.ForEachAsParallelAsync<int, double, double, double>(new[] { Create<int>() },
                        (value, input1, input2, input3, token) => Task.CompletedTask, _input1, _input2, _input3, 1, cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Pass_CancellationToken()
            {
                var cts = new CancellationTokenSource();

                CancellationToken actual = default;

                await EnumerableExtensions.ForEachAsParallelAsync<int, double, double, double>(new[] { Create<int>() }, (value, input1, input2, input3, token) =>
                {
                    actual = token;

                    return Task.CompletedTask;
                }, _input1, _input2, _input3, 1, cts.Token);

                actual.Should().Be(cts.Token);  // Be() and not BeSameAs() as stucts are copied by value
            }
        }

        public class ForEachAsParallelAsync_Input_4 : EnumerableExtensionsFixture
        {
            private readonly double _input1;
            private readonly double _input2;
            private readonly double _input3;
            private readonly double _input4;

            public ForEachAsParallelAsync_Input_4()
            {
                _input1 = Create<double>();
                _input2 = Create<double>();
                _input3 = Create<double>();
                _input4 = Create<double>();
            }

            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsParallelAsync<int, double, double, double, double>(null,
                        (value, input1, input2, input3, input4, token) => Task.CompletedTask, _input1, _input2, _input3, _input4, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsParallelAsync<int, double, double, double, double>(new[] { Create<int>() }, null, _input1, _input2, _input3, _input4, Create<int>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("func");
            }

            [Theory]
            [InlineData(0)]
            [InlineData(-1)]
            public void Should_Throw_When_DegreeOfParallelism_Less_Than_One(int degreeOfParallelism)
            {
                Invoking(() =>
                {
                    EnumerableExtensions.ForEachAsParallelAsync<int, double, double, double, double>(new[] { Create<int>() },
                        (value, input1, input2, input3, input4, token) => Task.CompletedTask, _input1, _input2, _input3, _input4, degreeOfParallelism);
                })
                .Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage("At least one task must be specified. (Parameter 'degreeOfParallelism')");
            }

            [Fact]
            public async Task Should_Process_Collection()
            {
                var expected = Enumerable.Range(0, 100).ToList();

                var actual = new ConcurrentQueue<int>();

                await EnumerableExtensions.ForEachAsParallelAsync<int, double, double, double, double>(expected, (value, input1, input2, input3, input4, token) =>
                {
                    input1.Should().Be(_input1);
                    input2.Should().Be(_input2);
                    input3.Should().Be(_input3);
                    input4.Should().Be(_input4);

                    actual.Enqueue(value);

                    return Task.CompletedTask;
                }, _input1, _input2, _input3, _input4, GetWithinRange(2, 10));

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Cancel_Process()
            {
                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    await EnumerableExtensions.ForEachAsParallelAsync<int, double, double, double, double>(new[] { Create<int>() },
                        (value, input1, input2, input3, input4, token) => Task.CompletedTask, _input1, _input2, _input3, _input4, 1, cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Pass_CancellationToken()
            {
                var cts = new CancellationTokenSource();

                CancellationToken actual = default;

                await EnumerableExtensions.ForEachAsParallelAsync<int, double, double, double, double>(new[] { Create<int>() }, (value, input1, input2, input3, input4, token) =>
                {
                    actual = token;

                    return Task.CompletedTask;
                }, _input1, _input2, _input3, _input4, 1, cts.Token);

                actual.Should().Be(cts.Token);  // Be() and not BeSameAs() as stucts are copied by value
            }
        }

        #endregion
    }
}