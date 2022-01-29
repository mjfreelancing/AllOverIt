using AllOverIt.Fixture;
using AllOverIt.Helpers;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

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

                var expected = GetCumulativeHash(values);

                var actual = HashCodeHelper.CalculateHashCode(values);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Return_Expected_Hash_Code()
            {
                var intValues = CreateMany<int>().Cast<object>();
                var stringValues = CreateMany<string>().Cast<object>();
                var allValues = intValues.Concat(stringValues).ToArray();

                var expected = GetCumulativeHash(allValues);

                var actual = HashCodeHelper.CalculateHashCode(allValues);

                actual.Should().Be(expected);
            }
        }

        public class CalculateHashCode_Seed_Params_Object : HashCodeHelperFixture
        {
            [Fact]
            public void Should_Return_Expected_Hash_Code_With_Custom_Seed()
            {
                var seed = Create<int>();

                var intValues = CreateMany<int>().Cast<object>().ToArray();
                var stringValues = CreateMany<string>().Cast<object>().ToArray();

                var expected = GetCumulativeHash(intValues.Concat(stringValues), seed);

                var actualStart = HashCodeHelper.CalculateHashCode(seed, intValues);
                var actualEnd = HashCodeHelper.CalculateHashCode(actualStart, stringValues);

                actualEnd.Should().Be(expected);
            }

            [Fact]
            public void Should_Return_Expected_Hash_Code_From_Two_Calls()
            {
                var intValues = CreateMany<int>().Cast<object>().ToArray();
                var stringValues = CreateMany<string>().Cast<object>().ToArray();

                var expected = GetCumulativeHash(intValues.Concat(stringValues));

                var actualStart = HashCodeHelper.CalculateHashCode(intValues);
                var actualEnd = HashCodeHelper.CalculateHashCode(actualStart, stringValues);

                actualEnd.Should().Be(expected);
            }
        }

        //public class CalculateHashCode_Type : HashCodeHelperFixture
        //{
        //    [Fact]
        //    public void Should_Return_Expected_Null_Seed()
        //    {
        //        var values = new int?[] { null };

        //        var expected = GetCumulativeHash(values.Cast<object>());

        //        var actual = HashCodeHelper.CalculateHashCode((int?) null);

        //        actual.Should().Be(expected);
        //    }

        //    [Fact]
        //    public void Should_Return_Expected_Hash_Code()
        //    {
        //        var value = Create<int?>();

        //        var expected = GetCumulativeHash(new object[] {value});

        //        var actual = HashCodeHelper.CalculateHashCode(value);

        //        actual.Should().Be(expected);
        //    }
        //}

        public class CalculateHashCode_Enumerable_Type : HashCodeHelperFixture
        {
            [Fact]
            public void Should_Return_Expected_Null_Seed()
            {
                var values = new int?[] { null };

                var expected = GetCumulativeHash(values.Cast<object>());

                var actual = HashCodeHelper.CalculateHashCode(values);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Return_Expected_Hash_Code()
            {
                var intValues = CreateMany<int>();

                var expected = GetCumulativeHash(intValues.Cast<object>());

                var actual = HashCodeHelper.CalculateHashCode(intValues);

                actual.Should().Be(expected);
            }
        }

        public class CalculateHashCode_Seed_Enumerable_Type : HashCodeHelperFixture
        {
            [Fact]
            public void Should_Return_Expected_Hash_Code_With_Custom_Seed()
            {
                var seed = Create<int>();

                var intValues1 = CreateMany<int>().ToList();
                var intValues2 = CreateMany<int>().ToList();

                var expected = GetCumulativeHash(intValues1.Concat(intValues2).Cast<object>(), seed);

                var actualStart = HashCodeHelper.CalculateHashCode(seed, intValues1);
                var actualEnd = HashCodeHelper.CalculateHashCode(actualStart, intValues2);

                actualEnd.Should().Be(expected);
            }

            [Fact]
            public void Should_Return_Expected_Hash_Code_From_Two_Calls()
            {
                var intValues1 = CreateMany<int>().ToList();
                var intValues2 = CreateMany<int>().ToList();

                var expected = GetCumulativeHash(intValues1.Concat(intValues2).Cast<object>());

                var actualStart = HashCodeHelper.CalculateHashCode(intValues1);
                var actualEnd = HashCodeHelper.CalculateHashCode(actualStart, intValues2);

                actualEnd.Should().Be(expected);
            }
        }

        private static int GetCumulativeHash(IEnumerable<object> values, int seed = 17)
        {
            return values.Aggregate(seed, (current, value) => current * 23 + (value?.GetHashCode() ?? 0));
        }
    }
}