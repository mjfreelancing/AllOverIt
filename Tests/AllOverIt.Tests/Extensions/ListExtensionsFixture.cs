using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System.ComponentModel;

namespace AllOverIt.Tests.Extensions
{
    public class ListExtensionsFixture : FixtureBase
    {
        public class FirstElement_IReadOnlyList : ListExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_List_Null()
            {
                Invoking(() =>
                {
                    ListExtensions.FirstElement((IReadOnlyList<int>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("list");
            }

            [Fact]
            public void Should_Throw_When_List_Empty()
            {
                Invoking(() =>
                {
                    ListExtensions.FirstElement(Array.Empty<int>().AsReadOnlyList());
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("list");
            }

            [Fact]
            public void Should_Return_First_Element()
            {
                var items = CreateMany<int>();

                var expected = items[0];

                var actual = items.FirstElement();

                expected.Should().Be(actual);
            }
        }

        public class LastElement_IReadOnlyList : ListExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_List_Null()
            {
                Invoking(() =>
                {
                    ListExtensions.LastElement((IReadOnlyList<int>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("list");
            }

            [Fact]
            public void Should_Throw_When_List_Empty()
            {
                Invoking(() =>
                {
                    ListExtensions.LastElement(Array.Empty<int>().AsReadOnlyList());
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("list");
            }

            [Fact]
            public void Should_Return_Last_Element()
            {
                var items = CreateMany<int>();

                var expected = items[items.Count - 1];

                var actual = ListExtensions.LastElement(items);

                expected.Should().Be(actual);
            }
        }

        public class FirstElement_IList : ListExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_List_Null()
            {
                Invoking(() =>
                {
                    ListExtensions.FirstElement((IList<int>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("list");
            }

            [Fact]
            public void Should_Throw_When_List_Empty()
            {
                Invoking(() =>
                {
                    ListExtensions.FirstElement((IList<int>) new List<int>());
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("list");
            }

            [Fact]
            public void Should_Return_First_Element()
            {
                var items = CreateMany<int>().AsList();

                var expected = items.First();

                var actual = items.FirstElement();

                expected.Should().Be(actual);
            }
        }

        public class LastElement_IList : ListExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_List_Null()
            {
                Invoking(() =>
                {
                    ListExtensions.LastElement((IList<int>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("list");
            }

            [Fact]
            public void Should_Throw_When_List_Empty()
            {
                Invoking(() =>
                {
                    ListExtensions.LastElement((IList<int>) new List<int>());
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("list");
            }

            [Fact]
            public void Should_Return_Last_Element()
            {
                var items = CreateMany<int>().AsList();

                var expected = items.Last();

                var actual = items.LastElement();

                expected.Should().Be(actual);
            }
        }

        public class AddMany : ListExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_List_Null()
            {
                Invoking(() =>
                {
                    ListExtensions.AddMany((IList<int>) null, CreateMany<int>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("list");
            }

            [Fact]
            public void Should_Not_Throw_When_List_Empty()
            {
                Invoking(() =>
                {
                    ListExtensions.AddMany(new List<int>(), CreateMany<int>());
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    ListExtensions.AddMany(new List<int>(), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Not_Not_Throw_When_Items_Empty()
            {
                Invoking(() =>
                {
                    ListExtensions.AddMany(CreateMany<int>().AsList(), Array.Empty<int>());
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Add_To_List()
            {
                IList<int> list = new List<int>(CreateMany<int>());
                var items = CreateMany<int>();

                var expected = list.Concat(items).ToList();

                list.AddMany(items);

                list.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Add_To_Non_List()
            {
                IList<int> list = new BindingList<int>();

                var items = CreateMany<int>();

                var expected = list.Concat(items).ToList();

                list.AddMany(items);

                list.Should().BeEquivalentTo(expected);
            }
        }
    }
}