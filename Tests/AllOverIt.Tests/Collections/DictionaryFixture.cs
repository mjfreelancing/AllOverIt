using AllOverIt.Collections;
using AllOverIt.Fixture;

namespace AllOverIt.Tests.Collections
{
    public class DictionaryFixture : FixtureBase
    {
        public class EmptyReadOnly : DictionaryFixture
        {
            [Fact]
            public void Should_Return_Empty_List()
            {
                var actual = Dictionary.EmptyReadOnly<int, string>();

                actual.ShouldBeEmpty();
            }

            [Fact]
            public void Should_Return_As_ReadOnly()
            {
                var actual = Dictionary.EmptyReadOnly<int, string>();

                actual.ShouldBeAssignableTo<IReadOnlyDictionary<int, string>>();
            }

            [Fact]
            public void Should_Be_Immutable()
            {
                var actual = Dictionary.EmptyReadOnly<int, string>();

                actual.ShouldNotBeAssignableTo<ICollection<KeyValuePair<int, string>>>();
            }
        }
    }
}




