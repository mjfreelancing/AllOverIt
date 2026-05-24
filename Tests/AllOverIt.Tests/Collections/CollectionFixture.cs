using AllOverIt.Collections;
using AllOverIt.Fixture;

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

                actual.ShouldBeEmpty();
            }

            [Fact]
            public void Should_Return_As_ReadOnly()
            {
                var actual = Collection.EmptyReadOnly<int>();

                actual.ShouldBeAssignableTo<IReadOnlyCollection<int>>();
            }

            [Fact]
            public void Should_Be_Immutable()
            {
                var actual = Collection.EmptyReadOnly<int>();

                actual.ShouldNotBeAssignableTo<ICollection<int>>();
            }
        }
    }
}




