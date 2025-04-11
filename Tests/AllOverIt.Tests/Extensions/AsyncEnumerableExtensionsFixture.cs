using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System.Collections.ObjectModel;

namespace AllOverIt.Tests.Extensions
{
    public class AsyncEnumerableExtensionsFixture : FixtureBase
    {
        private sealed class DummyParent
        {
            public DummyChild[] Children { get; set; }
        }

        private sealed class DummyChild
        {
            public string ChildName { get; set; }
        }

        private readonly DummyParent[] _parentsArray;
        private readonly IAsyncEnumerable<DummyParent> _parents;

        public AsyncEnumerableExtensionsFixture()
        {
            _parentsArray = CreateMany<DummyParent>().ToArray();
            _parents = ToCancellableAsyncEnumerable(_parentsArray);
        }

        public class ToReadOnlyCollectionAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IAsyncEnumerable<bool> items = null;

                        await items.ToReadOnlyCollectionAsync();
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Convert_To_ReadOnlyCollection()
            {
                var expected = CreateMany<string>();

                var actual = await ToCancellableAsyncEnumerable(expected).ToReadOnlyCollectionAsync();

                actual.Should().BeOfType<ReadOnlyCollection<string>>();

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                await Invoking(async () =>
                {
                    await ToCancellableAsyncEnumerable(CreateMany<string>()).ToReadOnlyCollectionAsync(cancellationTokenSource.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class SelectAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(
                        async () =>
                        {
                            IAsyncEnumerable<bool> items = null;

                            await items.SelectAsync((item, token) => Task.FromResult(item)).ToListAsync();
                        })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                        async () =>
                        {
                            await _parents
                                .SelectAsync<DummyParent, DummyParent>((Func<DummyParent, CancellationToken, Task<DummyParent>>)null)
                                .ToListAsync();
                        })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public async Task Should_Cancel_Iteration()
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(
                        async () =>
                        {
                            await _parents
                                .SelectAsync((parent, token) => Task.FromResult(parent), cts.Token)
                                .ToListAsync();
                        })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public async Task Should_Iterate_Collection(bool useCancellationToken)
            {
                List<DummyChild> actual;

                if (useCancellationToken)
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        actual = await _parents
                            .SelectAsync((parent, token) => Task.FromResult(parent.Children.First()), cts.Token)
                            .ToListAsync();
                    }
                }
                else
                {
                    actual = await _parents
                        .SelectAsync((parent, token) => Task.FromResult(parent.Children.First()))
                        .ToListAsync();
                }

                var expected = _parentsArray.Select(parent => parent.Children.First());

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class SelectManyAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                        async () =>
                        {
                            IAsyncEnumerable<DummyParent> parents = null;

                            await parents
                                .SelectManyAsync(parent => parent.Children)
                                .ToListAsync();
                        })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    async () =>
                    {
                        await _parents
                            .SelectManyAsync((Func<DummyParent, IEnumerable<DummyChild>>)null)
                            .ToListAsync();
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public async Task Should_Cancel_Iteration()
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(
                        async () =>
                        {
                            await _parents
                                .SelectManyAsync(item => item.Children, cts.Token)
                                .ToListAsync();
                        })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public async Task Should_Iterate_Collection(bool useCancellationToken)
            {
                List<DummyChild> actual;

                if (useCancellationToken)
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        actual = await _parents
                            .SelectManyAsync(item => item.Children, cts.Token)
                            .ToListAsync();
                    }
                }
                else
                {
                    actual = await _parents
                        .SelectManyAsync(item => item.Children)
                        .ToListAsync();
                }

                var expected = _parentsArray.SelectMany(parent => parent.Children);

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class SelectManyToArrayAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IAsyncEnumerable<DummyParent> parents = null;

                        await parents.SelectManyToArrayAsync(parent => parent.Children);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToArrayAsync((Func<DummyParent, IEnumerable<DummyChild>>)null);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public async Task Should_Cancel_Iteration()
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToArrayAsync(parent => parent.Children, cts.Token);
                    })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Project_To_Array()
            {
                var actual = await _parents.SelectManyToArrayAsync(parent => parent.Children);

                actual.Should().BeOfType<DummyChild[]>();
            }

            [Fact]
            public async Task Should_Iterate_Collection()
            {
                var actual = await _parents.SelectManyToArrayAsync(parent => parent.Children);

                var expected = _parentsArray.SelectMany(parent => parent.Children);

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class SelectManyToListAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IAsyncEnumerable<DummyParent> parents = null;

                        await parents.SelectManyToListAsync(parent => parent.Children);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToListAsync((Func<DummyParent, IEnumerable<DummyChild>>)null);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public async Task Should_Cancel_Iteration()
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToListAsync(parent => parent.Children, cts.Token);
                    })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Project_To_List()
            {
                var actual = await _parents.SelectManyToListAsync(parent => parent.Children);

                actual.Should().BeOfType<List<DummyChild>>();
            }

            [Fact]
            public async Task Should_Iterate_Collection()
            {
                var actual = await _parents.SelectManyToListAsync(parent => parent.Children);

                var expected = _parentsArray.SelectMany(parent => parent.Children);

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class SelectManyToReadOnlyCollectionAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IAsyncEnumerable<DummyParent> parents = null;

                        await parents.SelectManyToReadOnlyCollectionAsync(parent => parent.Children);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToReadOnlyCollectionAsync((Func<DummyParent, IEnumerable<DummyChild>>)null);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public async Task Should_Cancel_Iteration()
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToReadOnlyCollectionAsync(parent => parent.Children, cts.Token);
                    })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Project_To_Collection()
            {
                var actual = await _parents.SelectManyToReadOnlyCollectionAsync(parent => parent.Children);

                actual.Should().BeOfType<ReadOnlyCollection<DummyChild>>();
            }

            [Fact]
            public async Task Should_Iterate_Collection()
            {
                var actual = await _parents.SelectManyToReadOnlyCollectionAsync(parent => parent.Children);

                var expected = _parentsArray.SelectMany(parent => parent.Children);

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class SelectManyAsync_Async : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                        async () =>
                        {
                            IAsyncEnumerable<DummyParent> parents = null;

                            await parents
                                .SelectManyAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()))
                                .ToListAsync();
                        })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    async () =>
                    {
                        await _parents
                            .SelectManyAsync((Func<DummyParent, CancellationToken, Task<IEnumerable<DummyChild>>>)null)
                            .ToListAsync();
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public async Task Should_Cancel_Iteration()
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(
                        async () =>
                        {
                            await _parents
                                .SelectManyAsync((item, token) => Task.FromResult(item.Children.AsEnumerable()), cts.Token)
                                .ToListAsync();
                        })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public async Task Should_Iterate_Collection(bool useCancellationToken)
            {
                List<DummyChild> actual;

                if (useCancellationToken)
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        actual = await _parents
                            .SelectManyAsync((item, token) => Task.FromResult(item.Children.AsEnumerable()), cts.Token)
                            .ToListAsync();
                    }
                }
                else
                {
                    actual = await _parents
                        .SelectManyAsync((item, token) => Task.FromResult(item.Children.AsEnumerable()))
                        .ToListAsync();
                }

                var expected = _parentsArray.SelectMany(parent => parent.Children);

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class SelectManyToArrayAsync_Async : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IAsyncEnumerable<DummyParent> parents = null;

                        await parents.SelectManyToArrayAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToArrayAsync((Func<DummyParent, CancellationToken, Task<IEnumerable<DummyChild>>>)null);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public async Task Should_Cancel_Iteration()
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToArrayAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()), cts.Token);
                    })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Project_To_Array()
            {
                var actual = await _parents.SelectManyToArrayAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));

                actual.Should().BeOfType<DummyChild[]>();
            }

            [Fact]
            public async Task Should_Iterate_Collection()
            {
                var actual = await _parents.SelectManyToArrayAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));

                var expected = _parentsArray.SelectMany(parent => parent.Children);

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class SelectManyToListAsync_Async : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IAsyncEnumerable<DummyParent> parents = null;

                        await parents.SelectManyToListAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToListAsync((Func<DummyParent, CancellationToken, Task<IEnumerable<DummyChild>>>)null);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public async Task Should_Cancel_Iteration()
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToListAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()), cts.Token);
                    })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Project_To_List()
            {
                var actual = await _parents.SelectManyToListAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));

                actual.Should().BeOfType<List<DummyChild>>();
            }

            [Fact]
            public async Task Should_Iterate_Collection()
            {
                var actual = await _parents.SelectManyToListAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));

                var expected = _parentsArray.SelectMany(parent => parent.Children);

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class SelectManyToReadOnlyCollectionAsync_Async : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IAsyncEnumerable<DummyParent> parents = null;

                        await parents.SelectManyToReadOnlyCollectionAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToReadOnlyCollectionAsync((Func<DummyParent, CancellationToken, Task<IEnumerable<DummyChild>>>)null);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public async Task Should_Cancel_Iteration()
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToReadOnlyCollectionAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()), cts.Token);
                    })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Project_To_Collection()
            {
                var actual = await _parents.SelectManyToReadOnlyCollectionAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));

                actual.Should().BeOfType<ReadOnlyCollection<DummyChild>>();
            }

            [Fact]
            public async Task Should_Iterate_Collection()
            {
                var actual = await _parents.SelectManyToReadOnlyCollectionAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));

                var expected = _parentsArray.SelectMany(parent => parent.Children);

                expected.Should().BeEquivalentTo(actual);
            }
        }

#if !NET10_0_OR_GREATER

        public class ToArrayAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                        async () =>
                        {
                            IAsyncEnumerable<bool> items = null;

                            await items.ToArrayAsync();
                        })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Convert_To_Array()
            {
                var expected = CreateMany<string>();

                var actual = await ToCancellableAsyncEnumerable(expected).ToArrayAsync();

                actual.Should().BeOfType<string[]>();

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                await Invoking(async () =>
                    {
                        await ToCancellableAsyncEnumerable(CreateMany<string>()).ToArrayAsync(cancellationTokenSource.Token);
                    })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class ToListAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                        async () =>
                        {
                            IAsyncEnumerable<bool> items = null;

                            await items.ToListAsync();
                        })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Convert_To_List()
            {
                var expected = CreateMany<string>();

                var actual = await ToCancellableAsyncEnumerable(expected).ToListAsync();

                actual.Should().BeOfType<List<string>>();

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                await Invoking(async () =>
                {
                    await ToCancellableAsyncEnumerable(CreateMany<string>()).ToListAsync(cancellationTokenSource.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

#endif

        public class SelectToArrayAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(async () =>
                {
                    _ = await AsyncEnumerableExtensions.SelectToArrayAsync<int, int>((IAsyncEnumerable<int>)null, item => Create<int>());
                })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Convert_To_Array()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await ToCancellableAsyncEnumerable(items).SelectToArrayAsync(item => expected[item]);

                actual.Should().BeOfType<string[]>();

                expected.Values.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                await Invoking(async () =>
                {
                    await ToCancellableAsyncEnumerable(CreateMany<string>()).SelectToArrayAsync(item => item, cancellationTokenSource.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class SelectToListAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(async () =>
                {
                    _ = await AsyncEnumerableExtensions.SelectToListAsync<int, int>((IAsyncEnumerable<int>)null, item => Create<int>());
                })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Convert_To_List()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await ToCancellableAsyncEnumerable(items).SelectToListAsync(item => expected[item]);

                actual.Should().BeOfType<List<string>>();

                expected.Values.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                await Invoking(async () =>
                {
                    await ToCancellableAsyncEnumerable(CreateMany<string>()).SelectToListAsync(item => item, cancellationTokenSource.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class SelectToReadOnlyCollectionAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(async () =>
                {
                    _ = await AsyncEnumerableExtensions.SelectToReadOnlyCollectionAsync<int, int>((IAsyncEnumerable<int>)null, item => Create<int>());
                })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Convert_To_ReadOnlyCollection()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await ToCancellableAsyncEnumerable(items).SelectToReadOnlyCollectionAsync(item => expected[item]);

                actual.Should().BeOfType<ReadOnlyCollection<string>>();

                expected.Values.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                await Invoking(async () =>
                {
                    await ToCancellableAsyncEnumerable(CreateMany<string>()).SelectToReadOnlyCollectionAsync(item => item, cancellationTokenSource.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class SelectToArrayAsync_Async : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(async () =>
                {
                    _ = await AsyncEnumerableExtensions.SelectToArrayAsync<int, int>((IAsyncEnumerable<int>)null, (item, token) => Task.FromResult(Create<int>()));
                })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Convert_To_Array()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await ToCancellableAsyncEnumerable(items).SelectToArrayAsync((item, token) => Task.FromResult(expected[item]));

                actual.Should().BeOfType<string[]>();

                expected.Values.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                await Invoking(async () =>
                {
                    await ToCancellableAsyncEnumerable(CreateMany<string>()).SelectToArrayAsync((item, token) => Task.FromResult(item), cancellationTokenSource.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class SelectToListAsyncAsync_Async : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(async () =>
                {
                    _ = await AsyncEnumerableExtensions.SelectToListAsync<int, int>((IAsyncEnumerable<int>)null, (item, token) => Task.FromResult(Create<int>()));
                })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Convert_To_List()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await ToCancellableAsyncEnumerable(items).SelectToListAsync((item, token) => Task.FromResult(expected[item]));

                actual.Should().BeOfType<List<string>>();

                expected.Values.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                await Invoking(async () =>
                {
                    await ToCancellableAsyncEnumerable(CreateMany<string>()).SelectToListAsync((item, token) => Task.FromResult(item), cancellationTokenSource.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class SelectToReadOnlyCollectionAsync_Async : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(async () =>
                {
                    _ = await AsyncEnumerableExtensions.SelectToReadOnlyCollectionAsync<int, int>((IAsyncEnumerable<int>)null, (item, token) => Task.FromResult(Create<int>()));
                })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Convert_To_ReadOnlyCollection()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await ToCancellableAsyncEnumerable(items).SelectToReadOnlyCollectionAsync((item, token) => Task.FromResult(expected[item]));

                actual.Should().BeOfType<ReadOnlyCollection<string>>();

                expected.Values.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                await Invoking(async () =>
                {
                    await ToCancellableAsyncEnumerable(CreateMany<string>()).SelectToReadOnlyCollectionAsync((item, token) => Task.FromResult(item), cancellationTokenSource.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class ForEachAsync_Action : EnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(async () =>
                {
                    IAsyncEnumerable<object> items = null;

                    await items.ForEachAsync((_, _) => { });
                })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancelledAtIndex = 0;

                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();

                    var values = Create<string>();

                    await ToCancellableAsyncEnumerable(values).ForEachAsync((item, index) =>
                    {
                        if (index > 1)
                        {
                            cancelledAtIndex = index;
                            cts.Cancel();
                        }
                    }, cts.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();

                cancelledAtIndex.Should().Be(2);
            }

            [Fact]
            public async Task Should_Iterate_Items_With_Index()
            {
                var values = Create<string>();
                var count = 0;

                await ToCancellableAsyncEnumerable(values).ForEachAsync((item, index) =>
                {
                    item.Should().Be(values.ElementAt(index));
                    count++;
                });

                count.Should().Be(values.Length);
            }
        }

        public class ForEachAsync_Func : EnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(async () =>
                {
                    IAsyncEnumerable<object> items = null;

                    await items.ForEachAsync((_, _) => Task.CompletedTask);
                })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancelledAtIndex = 0;

                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();

                    var values = Create<string>();

                    await ToCancellableAsyncEnumerable(values).ForEachAsync((item, index) =>
                    {
                        if (index > 1)
                        {
                            cancelledAtIndex = index;
                            cts.Cancel();
                        }

                        return Task.CompletedTask;
                    }, cts.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();

                cancelledAtIndex.Should().Be(2);
            }

            [Fact]
            public async Task Should_Iterate_Items_With_Index()
            {
                var values = Create<string>();
                var count = 0;

                await ToCancellableAsyncEnumerable(values).ForEachAsync((item, index) =>
                {
                    item.Should().Be(values.ElementAt(index));
                    count++;

                    return Task.CompletedTask;
                });

                count.Should().Be(values.Length);
            }
        }

        public class WithIndex : EnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                        async () =>
                        {
                            IAsyncEnumerable<object> items = null;

                            // AsListAsync() is required to invoke the method
                            _ = await items.WithIndexAsync().ToListAsync();
                        })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var index = 0;

                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();

                    var values = Create<string>();
                    var expectedValues = values.Select((item, index) => (item, index)).AsReadOnlyCollection();

                    await foreach (var (value, idx) in ToCancellableAsyncEnumerable(values).WithIndexAsync(cts.Token))
                    {
                        index++;

                        if (index > 1)
                        {
                            cts.Cancel();
                        }
                    }
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();

                index.Should().Be(2);
            }

            [Fact]
            public async Task Should_Provide_Item_Index()
            {
                var values = Create<string>();
                var expectedValues = values.Select((item, index) => (item, index)).AsReadOnlyCollection();

                var index = 0;

                await foreach (var (value, idx) in ToCancellableAsyncEnumerable(values).WithIndexAsync())
                {
                    var expected = expectedValues.ElementAt(index++);

                    expected
                        .Should()
                        .BeEquivalentTo((value, idx));
                }
            }
        }




        // TODO: Make this a public extension method
        private static IAsyncEnumerable<TType> ToCancellableAsyncEnumerable<TType>(IEnumerable<TType> items)
        {
            return new CancellableAsyncEnumerableSource<TType>(items);
        }




        // TODO: Make this a publicly available class
        private sealed class CancellableAsyncEnumerableSource<TType> : IAsyncEnumerable<TType>
        {
            private readonly IEnumerable<TType> _items;

            public CancellableAsyncEnumerableSource(IEnumerable<TType> items)
            {
                _items = items;
            }

            public IAsyncEnumerator<TType> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new CancellableAsyncEnumerator(_items, cancellationToken);
            }

            private sealed class CancellableAsyncEnumerator : IAsyncEnumerator<TType>
            {
                private readonly IEnumerator<TType> _enumerator;
                private readonly CancellationToken _cancellationToken;

                public CancellableAsyncEnumerator(IEnumerable<TType> items, CancellationToken cancellationToken)
                {
                    _enumerator = items.GetEnumerator();
                    _cancellationToken = cancellationToken;
                }

                public TType Current => _enumerator.Current;

                public async ValueTask<bool> MoveNextAsync()
                {
                    _cancellationToken.ThrowIfCancellationRequested();

                    // Ensure asynchronous execution
                    await Task.Yield();

                    _cancellationToken.ThrowIfCancellationRequested();

                    return _enumerator.MoveNext();
                }

                public ValueTask DisposeAsync()
                {
                    _enumerator.Dispose();
                    return ValueTask.CompletedTask;
                }
            }
        }





    }
}
