using AllOverIt.Collections;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Tests.Collections
{
    public class ListFixture : FixtureBase
    {
        public class EmptyReadOnly : ListFixture
        {
            [Fact]
            public void Should_Return_Empty_List()
            {
                var actual = List.EmptyReadOnly<int>();

                actual.Should().BeEmpty();
            }

            [Fact]
            public void Should_Return_As_ReadOnly()
            {
                var actual = List.EmptyReadOnly<int>();

                actual.Should().BeAssignableTo<IReadOnlyCollection<int>>();
            }

            [Fact]
            public void Should_Be_Immutable()
            {
                var actual = List.EmptyReadOnly<int>();

                actual.Should().NotBeAssignableTo<ICollection<int>>();
            }
        }
    }
}
