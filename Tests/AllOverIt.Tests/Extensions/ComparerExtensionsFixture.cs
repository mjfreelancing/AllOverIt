using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

namespace AllOverIt.Tests.Extensions
{
    public class ComparerExtensionsFixture : FixtureBase
    {
        private sealed class DummyString
        {
            public string First { get; init; }
            public string Second { get; init; }
        }

        private sealed class DummyFirstComparer : IComparer<DummyString>
        {
            public static readonly IComparer<DummyString> Instance = new DummyFirstComparer();

            public int Compare(DummyString lhs, DummyString rhs)
            {
                if (lhs is null && rhs is null)
                {
                    return 0;
                }

                if (lhs is null)
                {
                    return -1;
                }

                if (rhs is null)
                {
                    return 1;
                }

                return string.Compare(lhs.First, rhs.First);
            }
        }

        private sealed class DummySecondComparer : IComparer<DummyString>
        {
            public static readonly IComparer<DummyString> Instance = new DummySecondComparer();

            public int Compare(DummyString lhs, DummyString rhs)
            {
                if (lhs is null && rhs is null)
                {
                    return 0;
                }

                if (lhs is null)
                {
                    return -1;
                }

                if (rhs is null)
                {
                    return 1;
                }

                return string.Compare(lhs.Second, rhs.Second);
            }
        }

        public class Reverse : ComparerExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Comparer_Null()
            {
                Invoking(() =>
                {
                    _ = ComparerExtensions.Reverse<string>(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("comparer");
            }

            [Fact]
            public void Should_Reverse_Items()
            {
                // Duplicating values so the comparer considers elements that are equal
                var values = CreateMany<DummyString>().ToList();
                values = values.Concat(values).ToList();

                values.Sort(DummyFirstComparer.Instance.Reverse());

                var expected = values.SelectToList(item => item.First);

                expected.Should().BeInDescendingOrder();
            }

            [Theory]
            [InlineData(0, 1)]
            [InlineData(1, 1)]
            [InlineData(2, 1)]
            [InlineData(0, 2)]
            [InlineData(0, 3)]
            [InlineData(1, 1)]
            [InlineData(1, 2)]
            public void Should_Not_Throw_When_Contains_Nulls(int startIndex, int count)
            {
                var values = CreateMany<DummyString>(3).ToList();

                for (var offset = startIndex; offset < startIndex + count; offset++)
                {
                    values[offset] = null;
                }

                var nonNulls = values.Where(item => item is not null).OrderByDescending(item => item.First);

                var expected = nonNulls
                    .Concat(values.Where(item => item is null))
                    .ToList();

                values.Sort(DummyFirstComparer.Instance.Reverse());

                values.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
            }
        }

        public class Then : ComparerExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_First_Comparer_Null()
            {
                Invoking(() =>
                {
                    _ = ComparerExtensions.Then<DummyString>(null, DummySecondComparer.Instance);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("first");
            }

            [Fact]
            public void Should_Throw_When_Next_Comparer_Null()
            {
                Invoking(() =>
                {
                    _ = ComparerExtensions.Then<DummyString>(DummyFirstComparer.Instance, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("next");
            }

            [Fact]
            public void Should_Compose_Comparers()
            {
                var values = new List<DummyString>
                {
                   new DummyString{ First = "a", Second = Create<string>() },
                   new DummyString{ First = "b", Second = Create<string>() },
                   new DummyString{ First = null, Second = Create<string>() },
                   new DummyString{ First = "a", Second = Create<string>() },
                   new DummyString{ First = "b", Second = Create<string>() },
                   new DummyString{ First = "a", Second = Create<string>() },
                   new DummyString{ First = "b", Second = null },
                   new DummyString{ First = "b", Second = Create<string>() },
                   new DummyString{ First = "a", Second = Create<string>() },
                };

                var expected = values
                    .OrderBy(item => item.First)
                    .ThenBy(item => item.Second)
                    .ToList();

                var sorter = DummyFirstComparer.Instance.Then(DummySecondComparer.Instance);

                values.Sort(sorter);

                values.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
            }
        }
    }
}