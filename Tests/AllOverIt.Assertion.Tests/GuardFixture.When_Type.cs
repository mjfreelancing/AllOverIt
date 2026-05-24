using AllOverIt.Assertion;
using AllOverIt.Fixture.Extensions;
using Shouldly;
using System.Collections;

namespace AllOverIt.Tests.Assertion
{
    public partial class GuardFixture
    {
        private class DummyReadOnlyCollection : IReadOnlyCollection<int>
        {
            private readonly List<int> _items = new();

            public int Count => _items.Count;

            public DummyReadOnlyCollection(IEnumerable<int> items)
            {
                _items.AddRange(items);
            }

            public IEnumerator<int> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class DummyCollection : ICollection<int>
        {
            private readonly List<int> _items = new();

            public int Count => _items.Count;

            public DummyCollection(IEnumerable<int> items)
            {
                _items.AddRange(items);
            }

            public bool IsReadOnly => throw new NotImplementedException();

            public void Add(int item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(int item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(int[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<int> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            public bool Remove(int item)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class DummyEnumerable : IEnumerable<int>
        {
            private readonly List<int> _items;

            public DummyEnumerable(IEnumerable<int> items)
            {
                _items = new List<int>(items);
            }

            public IEnumerator<int> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class WhenNotNull_Type : GuardFixture
        {
            [Fact]
            public void Should_Throw_With_Expected_Name()
            {
                DummyClass dummy = null;

                Should.Throw<ArgumentNullException>(() =>
                    {
                        Guard.WhenNotNull(dummy);
                    })
                    .WithNamedMessageWhenNull(nameof(dummy));
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                var name = Create<string>();

                Should.Throw<ArgumentNullException>(() =>
                    {
                        DummyClass dummy = null;

                        Guard.WhenNotNull(dummy, name);
                    })
                    .WithNamedMessageWhenNull(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Null()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();

                Should.Throw<ArgumentNullException>(() =>
                    {
                        DummyClass dummy = null;

                        Guard.WhenNotNull(dummy, name, errorMessage);
                    })
                    .WithNamedMessageWhenNull(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Should.NotThrow(() =>
                    {
                        var dummy = new DummyClass();

                        _ = Guard.WhenNotNull(dummy, Create<string>());
                    });
            }

            [Fact]
            public void Should_Not_Throw_Message()
            {
                var errorMessage = Create<string>();

                Should.NotThrow(() =>
                    {
                        var dummy = new DummyClass();

                        _ = Guard.WhenNotNull(dummy, Create<string>(), errorMessage);
                    });
            }

            [Fact]
            public void Should_Return_Object()
            {
                var expected = new DummyClass();

                var actual = Guard.WhenNotNull(expected, Create<string>());

                actual.ShouldBeSameAs(expected);
            }
        }

        public class WhenNotNullOrEmpty_Type : GuardFixture
        {
            [Fact]
            public void Should_Throw_With_Expected_Name()
            {
                IEnumerable<DummyClass> dummy = null;

                Should.Throw<ArgumentNullException>(() =>
                    {
                        Guard.WhenNotNullOrEmpty(dummy);
                    })
                    .WithNamedMessageWhenNull(nameof(dummy));
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                var name = Create<string>();

                Should.Throw<ArgumentNullException>(() =>
                    {
                        IEnumerable<DummyClass> dummy = null;

                        Guard.WhenNotNullOrEmpty(dummy, name);
                    })
                    .WithNamedMessageWhenNull(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Null()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();

                Should.Throw<ArgumentNullException>(() =>
                    {
                        IEnumerable<DummyClass> dummy = null;

                        Guard.WhenNotNullOrEmpty(dummy, name, errorMessage);
                    })
                    .WithNamedMessageWhenNull(name, errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var name = Create<string>();
                var expected = new List<DummyClass>();

                Should.Throw<ArgumentException>(() =>
                    {
                        Guard.WhenNotNullOrEmpty(expected, name);
                    })
                    .WithNamedMessageWhenEmpty(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();
                var expected = new List<DummyClass>();

                Should.Throw<ArgumentException>(() =>
                    {
                        Guard.WhenNotNullOrEmpty(expected, name, errorMessage);
                    })
                    .WithNamedMessageWhenEmpty(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Should.NotThrow(() =>
                    {
                        var dummy = new List<DummyClass> { new DummyClass() };

                        _ = Guard.WhenNotNullOrEmpty(dummy, Create<string>());
                    });
            }

            [Fact]
            public void Should_Not_Throw_Message()
            {
                var errorMessage = Create<string>();

                Should.NotThrow(() =>
                    {
                        var dummy = new List<DummyClass> { new DummyClass() };

                        _ = Guard.WhenNotNullOrEmpty(dummy, Create<string>(), errorMessage);
                    });
            }

            [Fact]
            public void Should_Return_Object()
            {
                var expected = new List<DummyClass> { new DummyClass() };

                var actual = Guard.WhenNotNullOrEmpty(expected, Create<string>());

                actual.ShouldBeSameAs(expected);
            }
        }

        public class WhenNotEmpty_Type : GuardFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Null()
            {
                var name = Create<string>();

                Should.NotThrow(() =>
                    {
                        IEnumerable<DummyClass> dummy = null;

                        _ = Guard.WhenNotEmpty(dummy, name);
                    });
            }

            [Fact]
            public void Should_Throw_With_Expected_Name()
            {
                var expected = new List<DummyClass>();

                Should.Throw<ArgumentException>(() =>
                    {
                        Guard.WhenNotEmpty(expected);
                    })
                    .WithNamedMessageWhenEmpty(nameof(expected));
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var name = Create<string>();
                var expected = new List<DummyClass>();

                Should.Throw<ArgumentException>(() =>
                    {
                        Guard.WhenNotEmpty(expected, name);
                    })
                    .WithNamedMessageWhenEmpty(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();
                var expected = new List<DummyClass>();

                Should.Throw<ArgumentException>(() =>
                    {
                        Guard.WhenNotEmpty(expected, name, errorMessage);
                    })
                    .WithNamedMessageWhenEmpty(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Should.NotThrow(() =>
                    {
                        var dummy = new List<DummyClass> { new DummyClass() };

                        _ = Guard.WhenNotEmpty(dummy, Create<string>());
                    });
            }

            [Fact]
            public void Should_Not_Throw_Message()
            {
                var errorMessage = Create<string>();

                Should.NotThrow(() =>
                    {
                        var dummy = new List<DummyClass> { new DummyClass() };

                        _ = Guard.WhenNotEmpty(dummy, Create<string>(), errorMessage);
                    });
            }

            [Fact]
            public void Should_Return_Object()
            {
                var expected = new List<DummyClass> { new DummyClass() };

                var actual = Guard.WhenNotEmpty(expected, Create<string>());

                actual.ShouldBeSameAs(expected);
            }

            [Fact]
            public void Should_Not_Throw_When_List_Not_Empty()
            {
                Should.NotThrow(() =>
                {
                    var actual = new List<int>(CreateMany<int>());

                    _ = actual.WhenNotEmpty();
                });
            }

            [Fact]
            public void Should_Not_Throw_When_ReadOnlyCollection_Not_Empty()
            {
                Should.NotThrow(() =>
                {
                    var actual = new DummyReadOnlyCollection(CreateMany<int>());

                    _ = actual.WhenNotEmpty();
                });
            }

            [Fact]
            public void Should_Not_Throw_When_Collection_Not_Empty()
            {
                Should.NotThrow(() =>
                {
                    var actual = new DummyCollection(CreateMany<int>());

                    _ = actual.WhenNotEmpty();
                });
            }

            [Fact]
            public void Should_Not_Throw_When_Enumerable_Not_Empty()
            {
                Should.NotThrow(() =>
                {
                    var actual = new DummyEnumerable(CreateMany<int>());

                    _ = actual.WhenNotEmpty();
                });
            }
        }
    }
}
