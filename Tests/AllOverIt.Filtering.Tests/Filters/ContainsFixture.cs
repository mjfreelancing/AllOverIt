using AllOverIt.Filtering.Filters;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllOverIt.Filtering.Tests.Filters
{
    public class ContainsFixture : FixtureBase
    {
        public class Constructor_Default : ContainsFixture
        {
            [Fact]
            public void Should_Set_Default_Value()
            {
                var actual = new Contains();

                actual.Value.Should().Be(default);
            }
        }

        public class Constructor_Value : ContainsFixture
        {
            [Fact]
            public void Should_Set_Value()
            {
                var value = Create<string>();

                var actual = new Contains(value);

                actual.Value.Should().Be(value);
            }
        }

        public class Explicit_Operator : ContainsFixture
        {
            [Fact]
            public void Should_Set_Explicit_Value()
            {
                var value = new Contains(Create<string>());

                var actual = (string) value;

                actual.Should().Be(value.Value);
            }
        }

        public class Implicit_Operator : ContainsFixture
        {
            [Fact]
            public void Should_Set_Implicit_Value()
            {
                var value = Create<string>();

                var actual = (Contains) value;

                actual.Value.Should().Be(value);
            }
        }
    }
}
