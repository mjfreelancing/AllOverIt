using AllOverIt.Collections;
using AllOverIt.Fixture;

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

                actual.ShouldBeEmpty();
            }

            [Fact]
            public void Should_Return_As_ReadOnly()
            {
                var actual = List.EmptyReadOnly<int>();

                actual.ShouldBeAssignableTo<IReadOnlyCollection<int>>();
            }

            [Fact]
            public void Should_Be_Immutable()
            {
                var actual = List.EmptyReadOnly<int>();

                actual.ShouldNotBeAssignableTo<ICollection<int>>();
            }
        }
    }
}




