using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
    public class ListExtensionsFixture : FixtureBase
    {
        public class FirstElement_IReadOnlyList : ListExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    ListExtensions.FirstElement((IReadOnlyList<int>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Items_Empty()
            {
                Invoking(() =>
                {
                    ListExtensions.FirstElement(Array.Empty<int>().AsReadOnlyList());
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("items");
            }

            [Fact]
            public void Should_Return_First_Element()
            {
                var items = CreateMany<int>();
                
                var expected = items.First();

                var actual = items.FirstElement();

                expected.Should().Be(actual);
            }
        }

        public class LastElement_IReadOnlyList : ListExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    ListExtensions.LastElement((IReadOnlyList<int>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Items_Empty()
            {
                Invoking(() =>
                {
                    ListExtensions.LastElement(Array.Empty<int>().AsReadOnlyList());
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("items");
            }

            [Fact]
            public void Should_Return_Last_Element()
            {
                var items = CreateMany<int>().AsReadOnlyList();

                var expected = items.Last();

                var actual = items.LastElement();

                expected.Should().Be(actual);
            }
        }

        public class FirstElement_IList : ListExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    ListExtensions.FirstElement((IList<int>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Items_Empty()
            {
                Invoking(() =>
                {
                    ListExtensions.FirstElement(Array.Empty<int>().AsList());
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("items");
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
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    ListExtensions.LastElement((IList<int>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Items_Empty()
            {
                Invoking(() =>
                {
                    ListExtensions.LastElement(Array.Empty<int>().AsList());
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("items");
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
    }
}