using AllOverIt.Assertion;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Assertion
{
    public partial class GuardFixture
    {
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

                        Guard.WhenNotNull(dummy, Create<string>());
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

                        Guard.WhenNotNull(dummy, Create<string>(), errorMessage);
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

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_When_Empty(bool ensureIsConcrete)
            {
                var name = Create<string>();
                var expected = new List<DummyClass>();

                Invoking(() =>
                    {
                        Guard.WhenNotNullOrEmpty(expected, ensureIsConcrete, name);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty(name);
            }

            [Fact]
            public void Should_Throw_When_Ensure_Collection_And_Items_Is_Not_A_Collection()
            {
                var range = Enumerable.Range(1, 2);

                Invoking(
                    () =>
                    {
                        Guard.WhenNotNullOrEmpty(range, true, nameof(range));
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage($"Expecting an array or ICollection<T> (Parameter '{nameof(range)}')");
            }

            [Fact]
            public void Should_Not_Throw_When_Items_Is_Array()
            {
                Invoking(
                   () =>
                   {
                       var items = new[] { 1, 2, 3 };
                       Guard.WhenNotNullOrEmpty(items, true, nameof(items));
                   })
                   .Should()
                   .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Items_Is_List()
            {
                Invoking(
                   () =>
                   {
                       var items = new List<int>(new[] { 1, 2, 3 });
                       Guard.WhenNotNullOrEmpty(items, true, nameof(items));
                   })
                   .Should()
                   .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Items_Is_ReadOnlyCollection()
            {
                Invoking(
                   () =>
                   {
                       var items = new ReadOnlyCollection<int>(new List<int>(new[] { 1, 2, 3 }));
                       Guard.WhenNotNullOrEmpty(items, true, nameof(items));
                   })
                   .Should()
                   .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Items_Is_HashSet()
            {
                Invoking(
                   () =>
                   {
                       var items = new HashSet<int>(new[] { 1, 2, 3 });
                       Guard.WhenNotNullOrEmpty(items, true, nameof(items));
                   })
                   .Should()
                   .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Items_Is_Dictionary()
            {
                Invoking(
                   () =>
                   {
                       var items = new Dictionary<int, int> { { 1, 1 }, { 2, 1 } };
                       Guard.WhenNotNullOrEmpty(items, true, nameof(items));
                   })
                   .Should()
                   .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Not_Ensure_Collection()
            {
                var count = 0;

                var range = Enumerable
                    .Range(1, 2)
                    .Select(item =>
                    {
                        count++;
                        return item;
                    });

                Invoking(
                    () =>
                    {
                        // never do this - the Enumerable will be re-evaluated (prior to .NET 5)
                        // see the count below
                        Guard.WhenNotNullOrEmpty(range, false, nameof(range));
                    })
                    .Should()
                    .NotThrow();

                _ = range.ToList();

#if NET5_0_OR_GREATER
                // Any() in .NET 5 and above avoids enumeration
                count.Should().Be(2);
#else
                // This is why the check argument should never be false
                // The Select() call has been called 3 times, instead of 2
                // It only exists for backward compatibility
                count.Should().Be(3);
#endif
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

                        Guard.WhenNotNullOrEmpty(dummy, Create<string>());
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

                        Guard.WhenNotNullOrEmpty(dummy, Create<string>(), errorMessage);
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

                        Guard.WhenNotEmpty(dummy, name);
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
            public void Should_Throw_When_Ensure_Collection_And_Items_Is_Not_A_Collection()
            {
                var range = Enumerable.Range(1, 2);

                Invoking(
                    () =>
                    {
                        Guard.WhenNotEmpty(range, true, nameof(range));
                    })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage($"Expecting an array or ICollection<T> (Parameter '{nameof(range)}')");
            }

            [Fact]
            public void Should_Not_Throw_When_Items_Is_Array()
            {
                Invoking(
                   () =>
                   {
                       var items = new[] { 1, 2, 3 };
                       Guard.WhenNotEmpty(items, true, nameof(items));
                   })
                   .Should()
                   .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Items_Is_List()
            {
                Invoking(
                   () =>
                   {
                       var items = new List<int>(new[] { 1, 2, 3 });
                       Guard.WhenNotEmpty(items, true, nameof(items));
                   })
                   .Should()
                   .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Items_Is_ReadOnlyCollection()
            {
                Invoking(
                   () =>
                   {
                       var items = new ReadOnlyCollection<int>(new List<int>(new[] { 1, 2, 3 }));
                       Guard.WhenNotEmpty(items, true, nameof(items));
                   })
                   .Should()
                   .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Items_Is_HashSet()
            {
                Invoking(
                   () =>
                   {
                       var items = new HashSet<int>(new[] { 1, 2, 3 });
                       Guard.WhenNotEmpty(items, true, nameof(items));
                   })
                   .Should()
                   .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Items_Is_Dictionary()
            {
                Invoking(
                   () =>
                   {
                       var items = new Dictionary<int, int> { { 1, 1 }, { 2, 1 } };
                       Guard.WhenNotEmpty(items, true, nameof(items));
                   })
                   .Should()
                   .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Not_Ensure_Collection()
            {
                var count = 0;

                var range = Enumerable
                    .Range(1, 2)
                    .Select(item =>
                    {
                        count++;
                        return item;
                    });

                Invoking(
                    () =>
                    {
                        // never do this - the Enumerable will be re-evaluated (prior to .NET 5)
                        // see the count below
                        Guard.WhenNotEmpty(range, false, nameof(range));
                    })
                    .Should()
                    .NotThrow();

                _ = range.ToList();

#if NET5_0_OR_GREATER
                // Any() in .NET 5 and above avoids enumeration
                count.Should().Be(2);
#else
                // This is why the check argument should never be false
                // The Select() call has been called 3 times, instead of 2
                // It only exists for backward compatibility
                count.Should().Be(3);
#endif
            }

            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                    {
                        var dummy = new List<DummyClass> { new DummyClass() };

                        Guard.WhenNotEmpty(dummy, Create<string>());
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

                        Guard.WhenNotEmpty(dummy, Create<string>(), errorMessage);
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
        }
    }
}