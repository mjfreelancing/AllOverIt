using AllOverIt.Collections;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Collections;
using System.Collections.Generic;

namespace AllOverIt.Tests.Collections
{
    public class ReadOnlyListFixture : FixtureBase
    {
        private readonly IList<int> _data;
        private readonly ReadOnlyList<int> _list;

        public ReadOnlyListFixture()
        {
            _data = CreateMany<int>(5).AsList();
            _list = new ReadOnlyList<int>(_data);
        }

        public class Constructor_Default : ReadOnlyListFixture
        {
            [Fact]
            public void Should_Be_Empty()
            {
                var actual = new ReadOnlyList<int>();
                actual.Should().BeEmpty();
            }
        }

        public class Constructor_IEnumerable : ReadOnlyListFixture
        {
            [Fact]
            public void Should_Populate()
            {
                var expected = CreateMany<int>();

                var actual = new ReadOnlyList<int>(expected);

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class IndexOperator : ReadOnlyListFixture
        {
            [Fact]
            public void Should_Return_Value_By_Index()
            {
                var index = GetWithinRange(0, 4);

                var expected = _data[index];
                var actual = _list[index];

                actual.Should().Be(expected);
            }
        }

        public class Count : ReadOnlyListFixture
        {
            [Fact]
            public void Should_Return_Count()
            {
                _list.Count.Should().Be(_data.Count);
            }
        }

        public class GetEnumerator : ReadOnlyListFixture
        {
            [Fact]
            public void Should_Iterate_Data()
            {
                var results = new List<int>();
                var enumerator = _list.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    results.Add(enumerator.Current);
                }

                results.Should().BeEquivalentTo(_data);
            }

            [Fact]
            public void Should_Iterate_Data_Using_Explicit_Interface()
            {
                var results = new List<object>();
                var enumerator = ((IEnumerable) _list).GetEnumerator();

                while (enumerator.MoveNext())
                {
                    var value = (int) (enumerator.Current);
                    results.Add(value);
                }

                results.Should().BeEquivalentTo(_data);
            }
        }

        [Fact]
        public void Should_Be_Immutable()
        {
            _list.Should().NotBeAssignableTo<ICollection<int>>();
        }
    }
}
