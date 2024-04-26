using AllOverIt.Collections;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Collections.Generic;

namespace AllOverIt.Tests.Collections
{
    public class CollectionFixture : FixtureBase
    {
        public class EmptyReadOnly : CollectionFixture
        {
            [Fact]
            public void Should_Return_Empty_List()
            {
                var actual = Collection.EmptyReadOnly<int>();

                actual.Should().BeEmpty();
            }

            [Fact]
            public void Should_Return_As_ReadOnly()
            {
                var actual = Collection.EmptyReadOnly<int>();

                actual.Should().BeAssignableTo<IReadOnlyCollection<int>>();
            }

            [Fact]
            public void Should_Be_Immutable()
            {
                var actual = Collection.EmptyReadOnly<int>();

                actual.Should().NotBeAssignableTo<ICollection<int>>();
            }
        }
    }
}
