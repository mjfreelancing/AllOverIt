using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
    public class DoubleExtensionsFixture : AoiFixtureBase
    {
        public class IsZero : DoubleExtensionsFixture
        {
            [Fact]
            public void Should_Compare_To_Zero()
            {
                var actual = DoubleExtensions.IsZero(0.0d);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Compare_To_Internal_Epsilon()
            {
                var actual = DoubleExtensions.IsZero(1E-7);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Compare_To_Less_Than_Internal_Epsilon()
            {
                var value = 1E-8;
                var actual = DoubleExtensions.IsZero(value);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Compare_To_Greater_Than_Internal_Epsilon()
            {
                var value = 1E-6;
                var actual = DoubleExtensions.IsZero(value);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Compare_Negative_To_Zero()
            {
                var value = -Create<double>() - 0.1d;
                var actual = DoubleExtensions.IsZero(value);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Compare_Positive_To_Zero()
            {
                var value = -Create<double>() + 0.1d;
                var actual = DoubleExtensions.IsZero(value);

                actual.Should().BeFalse();
            }
        }
        public class IsZero_Tolerance : DoubleExtensionsFixture
        {
            private const double Tolerance = 0.01;

            [Fact]
            public void Should_Compare_To_Zero()
            {
                var actual = DoubleExtensions.IsZero(0.0d, Tolerance);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Compare_To_Internal_Epsilon()
            {
                var actual = DoubleExtensions.IsZero(Tolerance, Tolerance);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Compare_To_Less_Than_Internal_Epsilon()
            {
                var value = Tolerance / 10;
                var actual = DoubleExtensions.IsZero(value, Tolerance);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Compare_To_Greater_Than_Internal_Epsilon()
            {
                var value = Tolerance * 10;
                var actual = DoubleExtensions.IsZero(value, Tolerance);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Compare_Negative_To_Zero()
            {
                var value = -Create<double>() - 0.1d;
                var actual = DoubleExtensions.IsZero(value, Tolerance);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Compare_Positive_To_Zero()
            {
                var value = -Create<double>() + 0.1d;
                var actual = DoubleExtensions.IsZero(value, Tolerance);

                actual.Should().BeFalse();
            }
        }

        public class IsEqualTo : DoubleExtensionsFixture
        {
            [Theory]
            [InlineData(1E-7, 0.0d)]
            [InlineData(0.0d, 1E-7)]
            public void Should_Not_Compare_To_Epsilon_Difference(double val1, double val2)
            {
                var actual = DoubleExtensions.IsEqualTo(val1, val2);

                actual.Should().BeFalse();
            }

            [Theory]
            [InlineData(1E-8, 0.0d)]
            [InlineData(0.0d, 1E-8)]
            public void Should_Compare_To_Less_Than_Epsilon(double val1, double val2)
            {
                var actual = DoubleExtensions.IsEqualTo(val1, val2);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(1E-6, 0.0d)]
            [InlineData(0.0d, 1E-6)]
            public void Should_Not_Compare_To_Greater_Than_Epsilon(double val1, double val2)
            {
                var actual = DoubleExtensions.IsEqualTo(val1, val2);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Compare_Same_Value()
            {
                var val = Create<double>();
                var actual = DoubleExtensions.IsEqualTo(val, val);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Compare_Different_Value()
            {
                var val1 = Create<double>();
                var val2 = CreateExcluding(val1);

                var actual = DoubleExtensions.IsEqualTo(val1, val2);

                actual.Should().BeFalse();
            }
        }

        public class IsEqualTo_Tolerance : DoubleExtensionsFixture
        {
            private const double Tolerance = 0.01;
            private const double ToleranceSmaller = 0.001;
            private const double ToleranceBigger = 0.1;

            [Theory]
            [InlineData(Tolerance, 0.0d)]
            [InlineData(0.0d, Tolerance)]
            public void Should_Not_Compare_To_Epsilon_Difference(double val1, double val2)
            {
                var actual = DoubleExtensions.IsEqualTo(val1, val2, Tolerance);

                actual.Should().BeFalse();
            }

            [Theory]
            [InlineData(ToleranceSmaller, 0.0d)]
            [InlineData(0.0d, ToleranceSmaller)]
            public void Should_Compare_To_Less_Than_Epsilon(double val1, double val2)
            {
                var actual = DoubleExtensions.IsEqualTo(val1, val2, Tolerance);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(ToleranceBigger, 0.0d)]
            [InlineData(0.0d, ToleranceBigger)]
            public void Should_Not_Compare_To_Greater_Than_Epsilon(double val1, double val2)
            {
                var actual = DoubleExtensions.IsEqualTo(val1, val2, Tolerance);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Compare_Same_Value()
            {
                var val = Create<double>();
                var actual = DoubleExtensions.IsEqualTo(val, val, Tolerance);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Compare_Different_Value()
            {
                var val1 = Create<double>();
                var val2 = CreateExcluding(val1);

                var actual = DoubleExtensions.IsEqualTo(val1, val2, Tolerance);

                actual.Should().BeFalse();
            }
        }
    }
}