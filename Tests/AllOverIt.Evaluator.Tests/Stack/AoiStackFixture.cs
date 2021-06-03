using AllOverIt.Evaluator.Stack;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections;
using System.Linq;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Stack
{
    public class AoiStackFixture : AoiFixtureBase
    {
        private AoiStack<string> _stack;

        public AoiStackFixture()
        {
            _stack = new AoiStack<string>();
        }

        public class Constructor_Default : AoiStackFixture
        {
            [Fact]
            public void Should_Create_Empty_Stack()
            {
                var isEmpty = !_stack.Any();

                isEmpty.Should().BeTrue();
            }
        }

        public class Constructor_Enumerable : AoiStackFixture
        {
            [Fact]
            public void Should_Throw_When_Enumerable_Null()
            {
                Invoking(() => _stack = new AoiStack<string>(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("collection");
            }

            [Fact]
            public void Should_Contain_Provided_Elements()
            {
                var items = CreateMany<string>();

                _stack = new AoiStack<string>(items);

                var actual = _stack.AsEnumerable();

                actual.Should().BeEquivalentTo(items);
            }
        }

        public class Count : AoiStackFixture
        {
            [Fact]
            public void Should_Return_Current_Count()
            {
                _stack.Push(Create<string>());

                var count = _stack.Count;

                count.Should().Be(1);
            }
        }

        public class Contains : AoiStackFixture
        {
            private readonly string _item;

            public Contains()
            {
                _item = Create<string>();

                _stack.Push(Create<string>());
                _stack.Push(_item);
                _stack.Push(Create<string>());
            }

            [Fact]
            public void Should_Return_False_When_Not_In_Stack()
            {
                var result = _stack.Contains(Create<string>());

                result.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_True_When_In_Stack()
            {
                var result = _stack.Contains(_item);

                result.Should().BeTrue();
            }
        }

        public class Clear : AoiStackFixture
        {
            [Fact]
            public void Should_Clear_Stack()
            {
                _stack.Push(Create<string>());
                _stack.Push(Create<string>());

                _stack.Clear();

                var count = _stack.Count;

                count.Should().Be(0);
            }
        }

        public class Any : AoiStackFixture
        {
            [Fact]
            public void Should_Return_True_When_Stack_Populated()
            {
                _stack.Push(Create<string>());
                _stack.Push(Create<string>());

                var any = _stack.Any();

                any.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_False_When_Stack_Empty()
            {
                var any = _stack.Any();

                any.Should().BeFalse();
            }
        }

        public class Peek : AoiStackFixture
        {
            [Fact]
            public void Should_Throw_When_Stack_Empty()
            {
                Invoking(() => _stack.Peek())
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage("Stack empty.");
            }

            [Fact]
            public void Should_Return_Top_Item()
            {
                var expected = Create<string>();

                _stack.Push(Create<string>());
                _stack.Push(expected);

                var actual = _stack.Peek();

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Not_Remove_Item()
            {
                _stack.Push(Create<string>());
                _stack.Push(Create<string>());

                _stack.Peek();

                _stack.Count.Should().Be(2);
            }
        }

        public class Pop : AoiStackFixture
        {
            [Fact]
            public void Should_Throw_When_Stack_Empty()
            {
                Invoking(() => _stack.Pop())
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage("Stack empty.");
            }

            [Fact]
            public void Should_Return_Top_Item()
            {
                var expected = Create<string>();

                _stack.Push(Create<string>());
                _stack.Push(expected);

                var actual = _stack.Pop();

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Remove_Item()
            {
                _stack.Push(Create<string>());
                _stack.Push(Create<string>());

                _stack.Pop();

                _stack.Count.Should().Be(1);
            }
        }

        public class Push : AoiStackFixture
        {
            [Fact]
            public void Should_Push_Item()
            {
                var count = Create<int>();

                for (var i = 0; i < count; i++)
                {
                    _stack.Push(Create<string>());
                }

                _stack.Count.Should().Be(count);
            }

            [Fact]
            public void Should_Push_Item_At_Top()
            {
                var expected = Create<string>();

                _stack.Push(Create<string>());
                _stack.Push(expected);

                _stack.Peek().Should().Be(expected);
            }
        }

        public class ToArray : AoiStackFixture
        {
            [Fact]
            public void Should_Create_An_Array_Copy()
            {
                var items = CreateMany<string>();

                _stack = new AoiStack<string>(items);

                var actual = _stack.ToArray();

                ReferenceEquals(actual, items).Should().BeFalse();
                actual.Should().BeEquivalentTo(items);
            }
        }

        public class GetEnumerator : AoiStackFixture
        {
            [Fact]
            public void Should_Enumerate_All_Elements()
            {
                var items = CreateMany<string>();

                _stack = new AoiStack<string>(items);

                IEnumerator enumerator = _stack.GetEnumerator();

                var count = 0;

                while (enumerator.MoveNext())
                {
                    count++;
                }

                count.Should().Be(items.Count);
            }

            [Fact]
            public void Should_Enumerate_Elements_As_LIFO()
            {
                var items = CreateMany<string>();

                _stack = new AoiStack<string>(items);

                var enumerator = _stack.GetEnumerator();

                // need to compare in reverse order (LIFO)
                var index = items.Count - 1;

                while (enumerator.MoveNext())
                {
                    enumerator.Current.Should().Be(items.ElementAt(index--));
                }
            }
        }
    }
}
