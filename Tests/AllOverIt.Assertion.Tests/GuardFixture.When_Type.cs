using AllOverIt.Assertion;
using AllOverIt.Fixture.Extensions;
using AutoFixture;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;
using Xunit.Sdk;

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

                Invoking(() =>
                    {
                        Guard.WhenNotNull(dummy);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull(nameof(dummy));
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                var name = Create<string>();

                Invoking(() =>
                    {
                        DummyClass dummy = null;

                        Guard.WhenNotNull(dummy, name);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Null()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();

                Invoking(() =>
                    {
                        DummyClass dummy = null;

                        Guard.WhenNotNull(dummy, name, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                    {
                        var dummy = new DummyClass();

                        _ = Guard.WhenNotNull(dummy, Create<string>());
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_Message()
            {
                var errorMessage = Create<string>();

                Invoking(() =>
                    {
                        var dummy = new DummyClass();

                        _ = Guard.WhenNotNull(dummy, Create<string>(), errorMessage);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Return_Object()
            {
                var expected = new DummyClass();

                var actual = Guard.WhenNotNull(expected, Create<string>());

                actual.Should().BeSameAs(expected);
            }
        }

        public class WhenNotNullOrEmpty_Type : GuardFixture
        {
            [Fact]
            public void Should_Throw_With_Expected_Name()
            {
                IEnumerable<DummyClass> dummy = null;

                Invoking(() =>
                    {
                        Guard.WhenNotNullOrEmpty(dummy);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull(nameof(dummy));
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                var name = Create<string>();

                Invoking(() =>
                    {
                        IEnumerable<DummyClass> dummy = null;

                        Guard.WhenNotNullOrEmpty(dummy, name);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Null()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();

                Invoking(() =>
                    {
                        IEnumerable<DummyClass> dummy = null;

                        Guard.WhenNotNullOrEmpty(dummy, name, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull(name, errorMessage);
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var name = Create<string>();
                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        Guard.WhenNotNullOrEmpty(expected, name);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();
                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        Guard.WhenNotNullOrEmpty(expected, name, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                    {
                        var dummy = new List<DummyClass> { new DummyClass() };

                        _ = Guard.WhenNotNullOrEmpty(dummy, Create<string>());
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_Message()
            {
                var errorMessage = Create<string>();

                Invoking(() =>
                    {
                        var dummy = new List<DummyClass> { new DummyClass() };

                        _ = Guard.WhenNotNullOrEmpty(dummy, Create<string>(), errorMessage);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Return_Object()
            {
                var expected = new List<DummyClass> { new DummyClass() };

                var actual = Guard.WhenNotNullOrEmpty(expected, Create<string>());

                actual.Should().BeSameAs(expected);
            }
        }

        public class WhenNotEmpty_Type : GuardFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Null()
            {
                var name = Create<string>();

                Invoking(() =>
                    {
                        IEnumerable<DummyClass> dummy = null;

                        _ = Guard.WhenNotEmpty(dummy, name);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_With_Expected_Name()
            {
                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        Guard.WhenNotEmpty(expected);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty(nameof(expected));
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var name = Create<string>();
                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        Guard.WhenNotEmpty(expected, name);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty(name);
            }

            [Fact]
            public void Should_Throw_Message_When_Empty()
            {
                var errorMessage = Create<string>();

                var name = Create<string>();
                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        Guard.WhenNotEmpty(expected, name, errorMessage);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty(name, errorMessage);
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                    {
                        var dummy = new List<DummyClass> { new DummyClass() };

                        _ = Guard.WhenNotEmpty(dummy, Create<string>());
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_Message()
            {
                var errorMessage = Create<string>();

                Invoking(() =>
                    {
                        var dummy = new List<DummyClass> { new DummyClass() };

                        _ = Guard.WhenNotEmpty(dummy, Create<string>(), errorMessage);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Return_Object()
            {
                var expected = new List<DummyClass> { new DummyClass() };

                var actual = Guard.WhenNotEmpty(expected, Create<string>());

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Not_Throw_When_List_Not_Empty()
            {
                Invoking(() =>
                {
                    var actual = new List<int>(CreateMany<int>());

                    _ = actual.WhenNotEmpty();
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_ReadOnlyCollection_Not_Empty()
            {
                Invoking(() =>
                {
                    var actual = new DummyReadOnlyCollection(CreateMany<int>());

                    _ = actual.WhenNotEmpty();
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Collection_Not_Empty()
            {
                Invoking(() =>
                {
                    var actual = new DummyCollection(CreateMany<int>());

                    _ = actual.WhenNotEmpty();
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Enumerable_Not_Empty()
            {
                Invoking(() =>
                {
                    var actual = new DummyEnumerable(CreateMany<int>());

                    _ = actual.WhenNotEmpty();
                })
                    .Should()
                    .NotThrow();
            }
        }
    }
}