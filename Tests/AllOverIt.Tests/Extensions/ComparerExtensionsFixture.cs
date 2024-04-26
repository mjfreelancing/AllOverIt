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
                return string.Compare(lhs.First, rhs.First);
            }
        }

        private sealed class DummySecondComparer : IComparer<DummyString>
        {
            public static readonly IComparer<DummyString> Instance = new DummySecondComparer();

            public int Compare(DummyString lhs, DummyString rhs)
            {
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

                values.Select(item => item.First).Should().BeInDescendingOrder();
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
                   new DummyString{ First = "a", Second = Create<string>() },
                   new DummyString{ First = "b", Second = Create<string>() },
                   new DummyString{ First = "a", Second = Create<string>() },
                   new DummyString{ First = "b", Second = Create<string>() },
                   new DummyString{ First = "a", Second = Create<string>() },
                };

                var expected = values.OrderBy(item => item.First).ThenBy(item => item.Second).ToList();

                var sorter = DummyFirstComparer.Instance.Then(DummySecondComparer.Instance);

                values.Sort(sorter);

                values.Should().ContainInOrder(expected);
            }
        }
    }
}