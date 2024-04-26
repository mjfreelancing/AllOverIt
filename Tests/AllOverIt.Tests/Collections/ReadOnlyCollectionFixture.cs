using AllOverIt.Collections;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Collections;
using System.Collections.Generic;

namespace AllOverIt.Tests.Collections
{
    public class ReadOnlyCollectionFixture : FixtureBase
    {
        private readonly IList<int> _data;
        private readonly ReadOnlyCollection<int> _collection;

        public ReadOnlyCollectionFixture()
        {
            _data = CreateMany<int>(5).AsList();
            _collection = new ReadOnlyCollection<int>(_data);
        }

        public class Constructor_Default : ReadOnlyCollectionFixture
        {
            [Fact]
            public void Should_Be_Empty()
            {
                var actual = new ReadOnlyCollection<int>();
                actual.Should().BeEmpty();
            }
        }

        public class Count : ReadOnlyCollectionFixture
        {
            [Fact]
            public void Should_Return_Count()
            {
                _collection.Count.Should().Be(_data.Count);
            }
        }

        public class GetEnumerator : ReadOnlyCollectionFixture
        {
            [Fact]
            public void Should_Iterate_Data()
            {
                var results = new List<int>();
                var enumerator = _collection.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    results.Add(enumerator.Current);
                }

                results.Should().BeEquivalentTo(_data);
            }

            [Fact]
            public void Should_Iterate_Data_Using_Explicit_Interface()
            {
                var results = new List<int>();
                var enumerator = ((IEnumerable) _collection).GetEnumerator();

                while (enumerator.MoveNext())
                {
                    results.Add((int) enumerator.Current);
                }

                results.Should().BeEquivalentTo(_data);
            }
        }

        [Fact]
        public void Should_Be_Immutable()
        {
            _collection.Should().NotBeAssignableTo<ICollection<int>>();
        }
    }
}
