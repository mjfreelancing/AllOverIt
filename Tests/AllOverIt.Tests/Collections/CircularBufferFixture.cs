using AllOverIt.Collections;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using System.Collections;
using AllOverIt.Shouldly.Extensions;

namespace AllOverIt.Tests.Collections
{
    public class CircularBufferFixture : FixtureBase
    {
        public class Constructor_Capacity : CircularBufferFixture
        {
            [Fact]
            public void Should_Throw_When_Invalid_Capacity()
            {
                var capacity = GetWithinRange(-1, 0);

                Invoking(() =>
                {
                    _ = new CircularBuffer<int>(capacity);
                })
                .ShouldThrow<ArgumentOutOfRangeException>()
                .WithMessage("The circular buffer requires a capacity of at least 1. (Parameter 'capacity')");
            }

            [Fact]
            public void Should_Initialize()
            {
                var capacity = GetWithinRange(1, 10);

                var buffer = new CircularBuffer<int>(capacity);

                buffer.Length.ShouldBe(0);
                buffer.Capacity.ShouldBe(capacity);
                buffer.IsEmpty.ShouldBeTrue();
                buffer.IsFull.ShouldBeFalse();
            }
        }

        public class Constructor_Capacity_Items : CircularBufferFixture
        {
            [Fact]
            public void Should_Throw_When_Invalid_Capacity()
            {
                var capacity = GetWithinRange(-1, 0);

                Invoking(() =>
                {
                    _ = new CircularBuffer<int>(capacity, CreateMany<int>().ToArray());
                })
                .ShouldThrow<ArgumentOutOfRangeException>()
                .WithMessage("The circular buffer requires a capacity of at least 1. (Parameter 'capacity')");
            }

            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    _ = new CircularBuffer<int>(Create<int>(), null);
                })
                .ShouldThrow<ArgumentNullException>()
                .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Items_Too_Long()
            {
                Invoking(() =>
                {
                    var capacity = Create<int>();

                    _ = new CircularBuffer<int>(capacity, CreateMany<int>(capacity + 1).ToArray());
                })
                .ShouldThrow<ArgumentException>()
                .WithMessage("The item count exceeds the circular buffer capacity.");
            }

            [Fact]
            public void Should_Initialize_Default()
            {
                var capacity = GetWithinRange(1, 10);

                var buffer = new CircularBuffer<int>(capacity);

                buffer.Length.ShouldBe(0);
                buffer.Capacity.ShouldBe(capacity);
                buffer.IsEmpty.ShouldBeTrue();
                buffer.IsFull.ShouldBeFalse();
            }

            [Fact]
            public void Should_Initialize_Some_Items()
            {
                var capacity = GetWithinRange(5, 10);
                var size = capacity - 3;
                var items = CreateMany<int>(size).ToArray();

                var buffer = new CircularBuffer<int>(capacity, items);

                buffer.Length.ShouldBe(size);
                buffer.Capacity.ShouldBe(capacity);
                buffer.IsEmpty.ShouldBeFalse();
                buffer.IsFull.ShouldBeFalse();
            }

            [Fact]
            public void Should_Initialize_Full_Items()
            {
                var capacity = GetWithinRange(5, 10);
                var items = CreateMany<int>(capacity).ToArray();

                var buffer = new CircularBuffer<int>(capacity, items);

                buffer.Length.ShouldBe(capacity);
                buffer.Capacity.ShouldBe(capacity);
                buffer.IsEmpty.ShouldBeFalse();
                buffer.IsFull.ShouldBeTrue();
            }
        }

        public class Front : CircularBufferFixture
        {
            [Fact]
            public void Should_Throw_When_Empty()
            {
                Invoking(() =>
                {
                    var buffer = new CircularBuffer<int>(Create<int>());
                    buffer.Front();
                })
                .ShouldThrow<InvalidOperationException>()
                .WithMessage("The circular buffer contains no elements.");
            }

            [Fact]
            public void Should_Get_Front_After_Push_Front()
            {
                var buffer = new CircularBuffer<int>(3);

                for (var i = 0; i < 5; i++)
                {
                    var expected = Create<int>();

                    buffer.PushFront(expected);

                    var actual = buffer.Front();

                    actual.ShouldBe(expected);
                }
            }

            [Fact]
            public void Should_Get_Front_After_Push_Back()
            {
                var expected = CreateMany<int>(3);
                var buffer = new CircularBuffer<int>(3);

                for (var i = 0; i < 3; i++)
                {
                    buffer.PushBack(expected[i]);

                    var actual = buffer.Front();

                    actual.ShouldBe(expected[0]);
                }

                buffer.PushBack(Create<int>());
                buffer.Front().ShouldBe(expected[1]);

                buffer.PushBack(Create<int>());
                buffer.Front().ShouldBe(expected[2]);
            }
        }

        public class Back : CircularBufferFixture
        {
            [Fact]
            public void Should_Throw_When_Empty()
            {
                Invoking(() =>
                {
                    var buffer = new CircularBuffer<int>(Create<int>());
                    buffer.Back();
                })
                .ShouldThrow<InvalidOperationException>()
                .WithMessage("The circular buffer contains no elements.");
            }

            [Fact]
            public void Should_Get_Back_After_Push_Front()
            {
                var expected = CreateMany<int>(3);
                var buffer = new CircularBuffer<int>(3);

                for (var i = 0; i < 3; i++)
                {
                    buffer.PushFront(expected[i]);

                    var actual = buffer.Back();

                    actual.ShouldBe(expected[0]);
                }

                buffer.PushFront(Create<int>());
                buffer.Back().ShouldBe(expected[1]);

                buffer.PushFront(Create<int>());
                buffer.Back().ShouldBe(expected[2]);
            }

            [Fact]
            public void Should_Get_Back_After_Push_Back()
            {
                var buffer = new CircularBuffer<int>(3);

                for (var i = 0; i < 5; i++)
                {
                    var expected = Create<int>();

                    buffer.PushBack(expected);

                    var actual = buffer.Back();

                    actual.ShouldBe(expected);
                }
            }
        }

        public class Items : CircularBufferFixture
        {
            [Fact]
            public void Should_Throw_When_Empty()
            {
                Invoking(() =>
                {
                    var buffer = new CircularBuffer<int>(Create<int>());

                    _ = buffer[1];
                })
                .ShouldThrow<IndexOutOfRangeException>()
                .WithMessage("The provided index exceeds the current buffer size.");
            }

            [Fact]
            public void Should_Get_At_Index()
            {
                var expected = CreateMany<int>(3).ToArray();
                var buffer = new CircularBuffer<int>(4, expected);

                for (var i = 0; i < 3; i++)
                {
                    var actual = buffer[i];

                    actual.ShouldBe(expected[i]);
                }
            }

            [Fact]
            public void Should_Set_At_Index()
            {
                var expected = CreateMany<int>(3).ToArray();
                var buffer = new CircularBuffer<int>(4, expected);

                for (var i = 0; i < 3; i++)
                {
                    buffer[i] = expected[i] + 1;

                    var actual = buffer[i];

                    actual.ShouldBe(expected[i] + 1);
                }
            }

            [Fact]
            public void Should_Throw_When_Index_Out_Of_Range()
            {
                Invoking(() =>
                {
                    var expected = CreateMany<int>(3).ToArray();
                    var buffer = new CircularBuffer<int>(4, expected);

                    _ = buffer[3];
                })
                .ShouldThrow<IndexOutOfRangeException>()
                .WithMessage("The provided index exceeds the current buffer size.");
            }
        }

        public class PushFront : CircularBufferFixture
        {
            [Fact]
            public void Should_Push_Front()
            {
                var source = CreateMany<int>(3);
                var buffer = new CircularBuffer<int>(3);

                foreach (var item in source)
                {
                    buffer.PushFront(item);
                }

                IEnumerable<int> expected = source.ToArray();
                expected = expected.Reverse();

                buffer.ShouldBeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Push_Front_Exceed_Capacity()
            {
                var source = CreateMany<int>(7).ToArray();
                var buffer = new CircularBuffer<int>(3);

                foreach (var item in source)
                {
                    buffer.PushFront(item);
                }

                IEnumerable<int> expected = source[4..];
                expected = expected.Reverse();

                buffer.ShouldBeEquivalentTo(expected);
            }
        }

        public class PushBack : CircularBufferFixture
        {
            [Fact]
            public void Should_Push_Back()
            {
                var expected = CreateMany<int>(3).ToArray();
                var buffer = new CircularBuffer<int>(3);

                foreach (var item in expected)
                {
                    buffer.PushBack(item);
                }

                var actual = buffer.ToArray();

                actual.ShouldBeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Push_Front_Exceed_Capacity()
            {
                var expected = CreateMany<int>(7).ToArray();
                var buffer = new CircularBuffer<int>(3);

                foreach (var item in expected)
                {
                    buffer.PushBack(item);
                }

                var actual = buffer.ToArray();

                actual.ShouldBeEquivalentTo(expected[4..]);
            }
        }

        public class PopFront : CircularBufferFixture
        {
            [Fact]
            public void Should_Pop_Front()
            {
                var expected = CreateMany<int>(3).ToArray();
                var buffer = new CircularBuffer<int>(3, expected);

                var actual = new[] { buffer.PopFront(), buffer.PopFront(), buffer.PopFront() };

                buffer.IsEmpty.ShouldBeTrue();
                actual.ShouldBeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var buffer = new CircularBuffer<int>(3);

                Invoking(() =>
                {
                    _ = buffer.PopFront();
                })
                    .ShouldThrow<InvalidOperationException>()
                    .WithMessage("The circular buffer contains no elements.");
            }
        }

        public class PopBack : CircularBufferFixture
        {
            [Fact]
            public void Should_Pop_Back()
            {
                var source = CreateMany<int>(3).ToArray();
                var buffer = new CircularBuffer<int>(3, source);

                var actual = new[] { buffer.PopBack(), buffer.PopBack(), buffer.PopBack() };

                IEnumerable<int> expected = source.ToArray();
                expected = expected.Reverse();

                buffer.IsEmpty.ShouldBeTrue();
                actual.ShouldBeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Throw_When_Empty()
            {
                var buffer = new CircularBuffer<int>(3);

                Invoking(() =>
                {
                    _ = buffer.PopBack();
                })
                    .ShouldThrow<InvalidOperationException>()
                    .WithMessage("The circular buffer contains no elements.");
            }
        }

        public class Clear : CircularBufferFixture
        {
            [Fact]
            public void Should_Clear()
            {
                var values = CreateMany<int>(3).ToArray();
                var buffer = new CircularBuffer<int>(3, values);

                buffer.IsEmpty.ShouldBeFalse();

                buffer.Clear();

                buffer.IsEmpty.ShouldBeTrue();
            }
        }

        public class ToArray : CircularBufferFixture
        {
            [Fact]
            public void Should_Get_Empty_Array()
            {
                var buffer = new CircularBuffer<string>(3);

                buffer.ToArray().ShouldBeEmpty();
            }

            [Fact]
            public void Should_Get_Array()
            {
                var expected = CreateMany<int?>(3).ToArray();
                var buffer = new CircularBuffer<int?>(3, expected);

                buffer.ToArray().ShouldBeEquivalentTo(expected);
            }
        }

        public class GetEnumerator : CircularBufferFixture
        {
            [Fact]
            public void Should_Enumerate()
            {
                var expected = CreateMany<int>(3).ToArray();
                var buffer = new CircularBuffer<int>(3, expected);

                foreach (var value in buffer.WithIndex())
                {
                    value.Item.ShouldBe(expected[value.Index]);
                }
            }

            [Fact]
            public void Should_Explicit_Enumerate()
            {
                var expected = CreateMany<int?>(3).ToArray();
                var buffer = new CircularBuffer<int?>(3, expected);

                var enumerator = ((IEnumerable)buffer).GetEnumerator();

                var index = 0;

                while (enumerator.MoveNext())
                {
                    var value = (int)enumerator.Current;

                    expected[index++].ShouldBe(value);
                }
            }
        }
    }
}








