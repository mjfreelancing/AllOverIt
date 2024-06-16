using AllOverIt.Collections;
using AllOverIt.Collections.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Threading;
using FluentAssertions;
using NSubstitute;
using System.Collections;

namespace AllOverIt.Tests.Collections.Extensions
{
    public class LockableListExtensionsFixture : FixtureBase
    {
        private readonly IReadWriteLock _lockFake = Substitute.For<IReadWriteLock>();
        private readonly IList<string> _list;
        private readonly LockableList<string> _lockableList;

        public LockableListExtensionsFixture()
        {
            _list = CreateMany<string>(3).ToList();
            _lockableList = new LockableList<string>(_list, _lockFake);
        }

        public class AddRange : LockableListExtensionsFixture
        {
            private sealed class DummyList : IList<string>
            {
                private readonly List<string> _list = [];

                public string this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

                public int Count => _list.Count;

                public bool IsReadOnly => throw new NotImplementedException();

                public void Add(string item)
                {
                    _list.Add(item);
                }

                public void Clear()
                {
                    throw new NotImplementedException();
                }

                public bool Contains(string item)
                {
                    throw new NotImplementedException();
                }

                public void CopyTo(string[] array, int arrayIndex)
                {
                    throw new NotImplementedException();
                }

                public IEnumerator<string> GetEnumerator()
                {
                    return _list.GetEnumerator();
                }

                public int IndexOf(string item)
                {
                    throw new NotImplementedException();
                }

                public void Insert(int index, string item)
                {
                    throw new NotImplementedException();
                }

                public bool Remove(string item)
                {
                    throw new NotImplementedException();
                }

                public void RemoveAt(int index)
                {
                    throw new NotImplementedException();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }
            }

            [Fact]
            public void Should_Throw_When_List_Null()
            {
                Invoking(() =>
                {
                    LockableListExtensions.AddRange((ILockableList<string>) null, []);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("lockableList");
            }

            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    LockableListExtensions.AddRange(_lockableList, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Lock_And_Release_When_List()
            {
                var list = CreateMany<string>(3).ToList();

                _lockableList.AddRange(list);

                // Only 1 outer lock required
                AssertEnterExitWriteLock(1);
            }

            [Fact]
            public void Should_Add_Items()
            {
                var list = CreateMany<string>(3).ToList();

                var expected = _list.Concat(list).ToList();

                _lockableList.AddRange(list);

                _lockableList.Count.Should().Be(6);
                _lockableList.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
            }

            [Fact]
            public void Should_Lock_And_Release_When_Other_List()
            {
                var lockableList = new LockableList<string>(new DummyList(), _lockFake);

                var list = CreateMany<string>(3).ToList();

                lockableList.AddRange(list);

                // Once for the outer lock, plus 3 individual locks
                AssertEnterExitWriteLock(4);
            }

            [Fact]
            public void Should_Add_Other_Items()
            {
                var lockableList = new LockableList<string>(new DummyList(), _lockFake);

                var list = CreateMany<string>(3).ToList();

                lockableList.AddRange(list);

                lockableList.Count.Should().Be(3);
                lockableList.Should().BeEquivalentTo(list, options => options.WithStrictOrdering());
            }
        }

        private void AssertEnterExitWriteLock(int count)
        {
            _lockFake.Received(count).EnterWriteLock();
            _lockFake.Received(count).ExitWriteLock();

            _lockFake.DidNotReceive().EnterReadLock(Arg.Any<bool>());
            _lockFake.DidNotReceive().ExitReadLock();
        }
    }
}
