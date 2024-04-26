using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;

namespace AllOverIt.Tests.Extensions
{
    public class DoubleExtensionsFixture : FixtureBase
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
        public class IsZero_Epsilon : DoubleExtensionsFixture
        {
            private readonly double _epsilon = 0.01;

            [Fact]
            public void Should_Compare_To_Zero()
            {
                var actual = DoubleExtensions.IsZero(0.0d, _epsilon);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Compare_To_Internal_Epsilon()
            {
                var actual = DoubleExtensions.IsZero(_epsilon, _epsilon);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Compare_To_Less_Than_Internal_Epsilon()
            {
                var value = _epsilon / 10;
                var actual = DoubleExtensions.IsZero(value, _epsilon);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Compare_To_Greater_Than_Internal_Epsilon()
            {
                var value = _epsilon * 10;
                var actual = DoubleExtensions.IsZero(value, _epsilon);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Compare_Negative_To_Zero()
            {
                var value = -Create<double>() - 0.1d;
                var actual = DoubleExtensions.IsZero(value, _epsilon);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Compare_Positive_To_Zero()
            {
                var value = -Create<double>() + 0.1d;
                var actual = DoubleExtensions.IsZero(value, _epsilon);

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

        public class IsEqualTo_Epsilon : DoubleExtensionsFixture
        {
            private const double Epsilon = 0.01;
            private const double EpsilonSmaller = 0.001;
            private const double EpsilonBigger = 0.1;

            [Theory]
            [InlineData(Epsilon, 0.0d)]
            [InlineData(0.0d, Epsilon)]
            public void Should_Not_Compare_To_Epsilon_Difference(double val1, double val2)
            {
                var actual = DoubleExtensions.IsEqualTo(val1, val2, Epsilon);

                actual.Should().BeFalse();
            }

            [Theory]
            [InlineData(EpsilonSmaller, 0.0d)]
            [InlineData(0.0d, EpsilonSmaller)]
            public void Should_Compare_To_Less_Than_Epsilon(double val1, double val2)
            {
                var actual = DoubleExtensions.IsEqualTo(val1, val2, Epsilon);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(EpsilonBigger, 0.0d)]
            [InlineData(0.0d, EpsilonBigger)]
            public void Should_Not_Compare_To_Greater_Than_Epsilon(double val1, double val2)
            {
                var actual = DoubleExtensions.IsEqualTo(val1, val2, Epsilon);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Compare_Same_Value()
            {
                var val = Create<double>();
                var actual = DoubleExtensions.IsEqualTo(val, val, Epsilon);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Compare_Different_Value()
            {
                var val1 = Create<double>();
                var val2 = CreateExcluding(val1);

                var actual = DoubleExtensions.IsEqualTo(val1, val2, Epsilon);

                actual.Should().BeFalse();
            }
        }
    }
}