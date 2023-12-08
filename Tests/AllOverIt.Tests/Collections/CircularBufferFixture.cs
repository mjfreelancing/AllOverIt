using AllOverIt.Collections;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

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
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("The circular buffer requires a capacity of at least 1.");
            }

            [Fact]
            public void Should_Initialize()
            {
                var capacity = GetWithinRange(1, 10);

                var buffer = new CircularBuffer<int>(capacity);

                buffer.Size.Should().Be(0);
                buffer.Capacity.Should().Be(capacity);
                buffer.IsEmpty.Should().BeTrue();
                buffer.IsFull.Should().BeFalse();
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
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("The circular buffer requires a capacity of at least 1.");
            }

            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    _ = new CircularBuffer<int>(Create<int>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
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
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("The item count exceeds the circular buffer capacity.");
            }

            [Fact]
            public void Should_Initialize_Default()
            {
                var capacity = GetWithinRange(1, 10);

                var buffer = new CircularBuffer<int>(capacity);

                buffer.Size.Should().Be(0);
                buffer.Capacity.Should().Be(capacity);
                buffer.IsEmpty.Should().BeTrue();
                buffer.IsFull.Should().BeFalse();
            }

            [Fact]
            public void Should_Initialize_Some_Items()
            {
                var capacity = GetWithinRange(5, 10);
                var size = capacity - 3;
                var items = CreateMany<int>(size).ToArray();

                var buffer = new CircularBuffer<int>(capacity, items);

                buffer.Size.Should().Be(size);
                buffer.Capacity.Should().Be(capacity);
                buffer.IsEmpty.Should().BeFalse();
                buffer.IsFull.Should().BeFalse();
            }

            [Fact]
            public void Should_Initialize_Full_Items()
            {
                var capacity = GetWithinRange(5, 10);
                var items = CreateMany<int>(capacity).ToArray();

                var buffer = new CircularBuffer<int>(capacity, items);

                buffer.Size.Should().Be(capacity);
                buffer.Capacity.Should().Be(capacity);
                buffer.IsEmpty.Should().BeFalse();
                buffer.IsFull.Should().BeTrue();
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
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage("The buffer is empty.");
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

                    actual.Should().Be(expected);
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

                    actual.Should().Be(expected[0]);
                }

                buffer.PushBack(Create<int>());
                buffer.Front().Should().Be(expected[1]);

                buffer.PushBack(Create<int>());
                buffer.Front().Should().Be(expected[2]);
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
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage("The buffer is empty.");
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

                    actual.Should().Be(expected[0]);
                }

                buffer.PushFront(Create<int>());
                buffer.Back().Should().Be(expected[1]);

                buffer.PushFront(Create<int>());
                buffer.Back().Should().Be(expected[2]);
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

                    actual.Should().Be(expected);
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
                .Should()
                .Throw<IndexOutOfRangeException>()
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

                    actual.Should().Be(expected[i]);
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
                .Should()
                .Throw<IndexOutOfRangeException>()
                .WithMessage("The provided index exceeds the current buffer size.");
            }
        }

        public class PushFront : CircularBufferFixture
        {

        }

        public class PushBack : CircularBufferFixture
        {

        }

        public class PopFront : CircularBufferFixture
        {

        }

        public class PopBack : CircularBufferFixture
        {

        }

        public class Clear : CircularBufferFixture
        {

        }

        public class ToArray : CircularBufferFixture
        {

        }

        public class GetEnumerator : CircularBufferFixture
        {

        }
    }
}
