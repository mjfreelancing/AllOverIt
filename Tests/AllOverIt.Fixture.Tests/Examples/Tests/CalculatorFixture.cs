using AllOverIt.Fixture.Tests.Examples.SUT;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Fixture.Tests.Examples.Tests
{
    public class CalculatorFixture : AoiFixtureBase
    {
        private ICalculator Calculator { get; } = new Calculator();

        public class Add : CalculatorFixture
        {
            [Fact]
            public void Should_Add_Values()
            {
                // Arrange
                var lhs = Create<double>();
                var rhs = Create<double>();

                // Act
                var actual = Calculator.Add(lhs, rhs);

                // Assert
                var expected = lhs + rhs;
                actual.Should().Be(expected);
            }
        }

        public class Divide : CalculatorFixture
        {
            [Fact]
            public void Should_Divide_Values()
            {
                // Arrange
                var numerator = Create<double>();
                var denominator = CreateExcluding(0.0d);

                // Act
                var actual = Calculator.Divide(numerator, denominator);

                // Assert
                var expected = numerator / denominator;
                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Return_Zero_When_Numerator_Zero()
            {
                // Arrange
                var denominator = Create<double>();

                // Act
                var actual = Calculator.Divide(0.0d, denominator);

                // Assert
                actual.Should().Be(0);
            }

            [Fact]
            public void Should_Throw_When_Denominator_Zero()
            {
                // Arrange
                var numerator = Create<double>();

                // Act / Assert
                Invoking(() => Calculator.Divide(numerator, 0.0d))
                  .Should()
                  .Throw<DivideByZeroException>();
            }
        }
    }
}