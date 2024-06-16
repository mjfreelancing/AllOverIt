using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pipes.Named.Server;
using AllOverIt.Threading;
using FluentAssertions;
using NSubstitute;

namespace AllOverIt.Tests.Collections
{
    public class LockableListFixture : FixtureBase
    {
        private readonly IReadWriteLock _lockFake = Substitute.For<IReadWriteLock>();
        private readonly IList<string> _list;
        private readonly LockableList<string> _lockableList;

        public LockableListFixture()
        {
            _list = CreateMany<string>(3).ToList();
            _lockableList = new LockableList<string>(_list, _lockFake);
        }

        public class Constructor : LockableListFixture
        {
            [Fact]
            public void Should_Throw_When_List_Null()
            {
                Invoking(() =>
                {
                    _ = new LockableList<string>(null, Create<bool>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("list");
            }
        }

        public class Item : LockableListFixture
        {
            public class Get : Item
            {
                [Fact]
                public void Should_Lock_And_Release()
                {
                    _ = _lockableList[0];

                    AssertEnterExitReadLock(false);
                }

                [Fact]
                public void Should_Get_Value()
                {
                    var actual = _lockableList[1];

                    actual.Should().Be(_list[1]);
                }
            }

            public class Set : Item
            {
                [Fact]
                public void Should_Lock_And_Release()
                {
                    _lockableList[0] = Create<string>();

                    AssertEnterExitWriteLock();
                }

                [Fact]
                public void Should_Set_Value()
                {
                    var expected = Create<string>();

                    _lockableList[1] = expected;

                    var actual = _lockableList[1];

                    actual.Should().Be(expected);
                }
            }
        }

        public class Count : LockableListFixture
        {
            [Fact]
            public void Should_Lock_And_Release()
            {
                _ = _lockableList.Count;

                AssertEnterExitReadLock(false);
            }

            [Fact]
            public void Should_Get_Count()
            {
                var actual = _lockableList.Count;

                actual.Should().Be(_list.Count);
            }
        }

        public class IsReadOnly : LockableListFixture
        {
            [Fact]
            public void Should_Lock_And_Release()
            {
                _ = _lockableList.IsReadOnly;

                AssertEnterExitReadLock(false);
            }

            [Fact]
            public void Should_Get_IsReadOnly()
            {
                var actual = _lockableList.IsReadOnly;

                actual.Should().Be(_list.IsReadOnly);
            }
        }

        public class Add : LockableListFixture
        {
            [Fact]
            public void Should_Lock_And_Release()
            {
                _lockableList.Add(Create<string>());

                AssertEnterExitWriteLock();
            }

            [Fact]
            public void Should_Add()
            {
                var item = Create<string>();

                var expected = _list.ToList();
                expected.Add(item);

                _lockableList.Add(item);

                _lockableList.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
            }
        }

        public class Clear : LockableListFixture
        {
            [Fact]
            public void Should_Lock_And_Release()
            {
                _lockableList.Clear();

                AssertEnterExitWriteLock();
            }

            [Fact]
            public void Should_Clear()
            {
                _lockableList.Should().HaveCount(3);

                _lockableList.Clear();

                _lockableList.Should().BeEmpty();
            }
        }

        public class Contains : LockableListFixture
        {
            [Fact]
            public void Should_Lock_And_Release()
            {
                _lockableList.Contains(Create<string>());

                AssertEnterExitReadLock(false);
            }

            [Fact]
            public void Should_Contain()
            {
                _lockableList.Contains(Create<string>()).Should().BeFalse();

                _lockableList.Contains(_list[1]).Should().BeTrue();
            }
        }

        public class CopyTo : LockableListFixture
        {
            [Fact]
            public void Should_Lock_And_Release()
            {
                var array = new string[3];

                _lockableList.CopyTo(array, 0);

                AssertEnterExitReadLock(false);
            }

            [Fact]
            public void Should_CopyTo()
            {
                var array = new string[3];

                _lockableList.CopyTo(array, 0);

                array.Should().BeEquivalentTo(array, options => options.WithStrictOrdering());
            }
        }

        public class GetEnumerator : LockableListFixture
        {
            [Fact]
            public void Should_Lock_And_Release()
            {
                _lockableList.GetEnumerator().Dispose();

                AssertEnterExitReadLock(false);
            }

            [Fact]
            public void Should_Enumerate()
            {
                var actual = _lockableList.ToList();

                actual.Should().BeEquivalentTo(_list, options => options.WithStrictOrdering());
            }
        }

        public class IndexOf : LockableListFixture
        {
            [Fact]
            public void Should_Lock_And_Release()
            {
                _lockableList.IndexOf(Create<string>());

                AssertEnterExitReadLock(false);
            }

            [Fact]
            public void Should_Get_Index()
            {
                var actual = _lockableList.IndexOf(_list[1]);

                actual.Should().Be(1);
            }
        }

        public class Insert : LockableListFixture
        {
            [Fact]
            public void Should_Lock_And_Release()
            {
                _lockableList.Insert(0, Create<string>());

                AssertEnterExitWriteLock();
            }

            [Fact]
            public void Should_Insert()
            {
                var item = Create<string>();

                var expected = _list.ToList();
                expected.Insert(0, item);

                _lockableList.Insert(0, item);

                _lockableList.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
            }
        }

        public class Remove : LockableListFixture
        {
            [Fact]
            public void Should_Lock_And_Release()
            {
                _lockableList.Remove(Create<string>());

                AssertEnterExitWriteLock();
            }

            [Fact]
            public void Should_Remove()
            {
                var item = _list[1];

                var expected = _list.ToList();
                expected.RemoveAt(1);

                _lockableList.Remove(item);

                _lockableList.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
            }
        }

        public class RemoveAt : LockableListFixture
        {
            [Fact]
            public void Should_Lock_And_Release()
            {
                _lockableList.RemoveAt(0);

                AssertEnterExitWriteLock();
            }

            [Fact]
            public void Should_RemoveAt()
            {
                var item = _list[1];

                var expected = _list.ToList();
                expected.RemoveAt(1);

                _lockableList.RemoveAt(1);

                _lockableList.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
            }
        }

        public class GetReadLock : LockableListFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Lock_And_Release(bool upgradeable)
            {
                _lockableList.GetReadLock(upgradeable).Dispose();

                AssertEnterExitReadLock(upgradeable);
            }
        }

        public class GetWriteLock : LockableListFixture
        {
            [Fact]
            public void Should_Lock_And_Release()
            {
                _lockableList.GetWriteLock().Dispose();

                AssertEnterExitWriteLock();
            }
        }

        public class NestedLocks : LockableListFixture
        {
            [Fact]
            public void Should_Upgrade_And_Nest_Lock()
            {
                var item = Create<string>();

                var lockableList = new LockableList<string>(_list, true);

                using (lockableList.GetReadLock(true))
                {
                    // Only performing this write lock to show recursion is supported
                    using (lockableList.GetWriteLock())
                    {
                        // This operation will perform another write lock
                        lockableList[1] = item;
                    }
                }

                _list[1].Should().Be(item);
            }
        }

        private void AssertEnterExitReadLock(bool upgradeable)
        {
            _lockFake.Received().EnterReadLock(upgradeable);
            _lockFake.Received().ExitReadLock();

            _lockFake.DidNotReceive().EnterWriteLock();
            _lockFake.DidNotReceive().ExitWriteLock();
        }

        private void AssertEnterExitWriteLock()
        {
            _lockFake.Received().EnterWriteLock();
            _lockFake.Received().ExitWriteLock();

            _lockFake.DidNotReceive().EnterReadLock(Arg.Any<bool>());
            _lockFake.DidNotReceive().ExitReadLock();
        }
    }
}
