﻿using AllOverIt.Filtering.Filters;
using AllOverIt.Fixture;
using FluentAssertions;

namespace AllOverIt.Filtering.Tests.Filters
{
    public class LessThanOrEqualFixture : FixtureBase
    {
        public class Constructor_Default : LessThanOrEqualFixture
        {
            [Fact]
            public void Should_Set_Default_Value()
            {
                var actual = new LessThanOrEqual<int>();

                actual.Value.Should().Be(default);
            }
        }

        public class Constructor_Value : LessThanOrEqualFixture
        {
            [Fact]
            public void Should_Set_Value()
            {
                var value = Create<int>();

                var actual = new LessThanOrEqual<int>(value);

                actual.Value.Should().Be(value);
            }
        }

        public class Explicit_Operator : LessThanOrEqualFixture
        {
            [Fact]
            public void Should_Set_Explicit_Value()
            {
                var value = new LessThanOrEqual<int>(Create<int>());

                var actual = (int) value;

                actual.Should().Be(value.Value);
            }
        }

        public class Implicit_Operator : LessThanOrEqualFixture
        {
            [Fact]
            public void Should_Set_Implicit_Value()
            {
                var value = Create<int>();

                var actual = (LessThanOrEqual<int>) value;

                actual.Value.Should().Be(value);
            }
        }
    }
}
