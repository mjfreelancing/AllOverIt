using AllOverIt.Fixture;
using AllOverIt.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Tests.Helpers
{
    public class HashCodeHelperFixture : FixtureBase
    {
        public class CalculateHashCode_Params_Object : HashCodeHelperFixture
        {
            [Fact]
            public void Should_Return_Expected_Null_Seed()
            {
                var values = new object[] { null };

                var expected = GetExpectedHashCode(values);

                var actual = HashCodeHelper.CalculateHashCode(values);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Return_Expected_Hash_Code()
            {
                var intValues = CreateMany<int>().Cast<object>();
                var stringValues = CreateMany<string>().Cast<object>();
                var allValues = intValues.Concat(stringValues).ToArray();

                var expected = GetExpectedHashCode(allValues);

                var actual = HashCodeHelper.CalculateHashCode(allValues);

                actual.Should().Be(expected);
            }
        }

        public class CalculateHashCode_Enumerable_Type : HashCodeHelperFixture
        {
            [Fact]
            public void Should_Return_Expected_Null_Seed()
            {
                var values = new int?[] { null };

                var expected = GetExpectedHashCode(values.Cast<object>());

                var actual = HashCodeHelper.CalculateHashCode(values);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Return_Expected_Hash_Code()
            {
                var intValues = CreateMany<int>();

                var expected = GetExpectedHashCode(intValues.Cast<object>());

                var actual = HashCodeHelper.CalculateHashCode(intValues);

                actual.Should().Be(expected);
            }
        }

        private static int GetExpectedHashCode(IEnumerable<object> items)
        {
            // NOTE: These tests are not checking NET STANDARD 2.0
            var hash = new HashCode();

            foreach (var item in items)
            {
                hash.Add(item);
            }

            return hash.ToHashCode();
        }
    }
}