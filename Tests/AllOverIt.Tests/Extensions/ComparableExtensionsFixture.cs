using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System;

namespace AllOverIt.Tests.Extensions
{
    public class ComparableExtensionsFixture : FixtureBase
    {
        private class DummyCompare : IComparable<DummyCompare>
        {
            private readonly double _value;

            public DummyCompare(double value)
            {
                _value = value;
            }

            public int CompareTo(DummyCompare other)
            {
                return _value.CompareTo(other._value);
            }
        }

        private DummyCompare Val1 { get; }
        private DummyCompare Val2 { get; }
        private DummyCompare Sum { get; }

        protected ComparableExtensionsFixture()
        {
            var val1 = Create<double>();
            var val2 = val1 + Create<double>();

            Val1 = new DummyCompare(val1);
            Val2 = new DummyCompare(val2);
            Sum = new DummyCompare(val1 + val2);
        }

        public class LessThan : ComparableExtensionsFixture
        {
            [Fact]
            public void Should_Be_LessThan()
            {
                var actual = Val1.LessThan(Sum);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_LessThan()
            {
                var actual = Sum.LessThan(Val1);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_LessThan_When_Equal()
            {
                var actual = Val1.LessThan(Val1);

                actual.Should().BeFalse();
            }
        }

        public class LessThanOrEqual : ComparableExtensionsFixture
        {
            [Fact]
            public void Should_Be_LessThanOrEqual()
            {
                var actual = Val1.LessThanOrEqual(Sum);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_LessThanOrEqual()
            {
                var actual = Sum.LessThanOrEqual(Val1);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Be_LessThanOrEqual_When_Equal()
            {
                var actual = Val1.LessThanOrEqual(Val1);

                actual.Should().BeTrue();
            }
        }

        public class GreaterThan : ComparableExtensionsFixture
        {
            [Fact]
            public void Should_Be_GreaterThan()
            {
                var actual = Sum.GreaterThan(Val1);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_GreaterThan()
            {
                var actual = Val1.GreaterThan(Sum);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_GreaterThan_When_Equal()
            {
                var actual = Val1.GreaterThan(Val1);

                actual.Should().BeFalse();
            }
        }

        public class GreaterThanOrEqual : ComparableExtensionsFixture
        {
            [Fact]
            public void Should_Be_GreaterThanOrEqual()
            {
                var actual = Sum.GreaterThanOrEqual(Val1);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_GreaterThanOrEqual()
            {
                var actual = Val1.GreaterThanOrEqual(Sum);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Be_GreaterThanOrEqual_When_Equal()
            {
                var actual = Val1.GreaterThanOrEqual(Val1);

                actual.Should().BeTrue();
            }
        }

        public class EqualTo : ComparableExtensionsFixture
        {
            [Fact]
            public void Should_Be_EqualTo()
            {
                var actual = Val1.EqualTo(Val1);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_EqualTo()
            {
                var actual = Val2.EqualTo(Val1);

                actual.Should().BeFalse();
            }
        }

        public class NotEqualTo : ComparableExtensionsFixture
        {
            [Fact]
            public void Should_Be_NotEqualTo()
            {
                var actual = Val1.NotEqualTo(Sum);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_NotEqualTo()
            {
                var actual = Val1.NotEqualTo(Val1);

                actual.Should().BeFalse();
            }
        }
    }
}