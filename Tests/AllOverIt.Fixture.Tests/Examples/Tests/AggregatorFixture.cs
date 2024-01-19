using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Fixture.Tests.Examples.SUT;
using AutoFixture.AutoFakeItEasy;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AllOverIt.Fixture.Tests.Examples.Tests
{
    public class AggregatorFixture : FixtureBase
    {
        public AggregatorFixture()
          : base(new AutoFakeItEasyCustomization { GenerateDelegates = true })
        {
        }

        public class Constructor : AggregatorFixture
        {
            [Fact]
            public void Should_Throw_When_Calculator_Null()
            {
                Invoking(() => new Aggregator(null))
                  .Should()
                  .Throw<ArgumentNullException>();
            }

            [Fact]
            public void Should_Use_Injected_Calculator()
            {
                // Arrange
                var values = CreateMany<double>();
                var calculatorFake = this.CreateStub<ICalculator>();

                // register the calculator so it will be auto-injected when we 'create' an aggregator
                Register(() => calculatorFake);

                // creates the aggregator, injecting the fake calculator
                var aggregator = Create<Aggregator>();

                // Act
                // call a method that will invoke the calculator
                aggregator.Summate(values.ToArray());

                // Assert
                A.CallTo(() => calculatorFake.Add(A<double>.Ignored, A<double>.Ignored))
                  .MustHaveHappenedANumberOfTimesMatching(count => count == values.Count);
            }
        }

        public class Summate : AggregatorFixture
        {
            [Fact]
            public void Should_Aggregate_Values()
            {
                // Note: this test is configured to use a real calculator (not a recommended testing approach, but shown for example purposes)

                // Arrange
                var values = CreateMany<double>();
                var calculator = new Calculator();

                Inject<ICalculator>(calculator);
                var aggregator = Create<Aggregator>();

                // the above calls to Register() and Create() is effectively the same as:
                // var aggregator = new Aggregator(calculator);

                // Act
                var actual = aggregator.Summate(values.ToArray());

                // Assert
                var expected = values.Sum();
                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Aggregate_Using_The_Calculator()
            {
                // this test is configured to use a fake calculator - random values are used for all input/output

                // Arrange

                // values to be aggregated
                var values = CreateMany<double>();

                // The aggregator calls Add() on the fake calculator. The return value from these calls are used as
                // the next input to the aggregator. The final call to Add() will be the 'aggregated' result.
                // This test is configured to completely ignore what the calculator does when requested to add two
                // numbers; instead we are only concerned with the result from the last call made by the aggregator.
                var addResults = CreateMany<double>();

                // the fake calculator
                var calculatorFake = this.CreateStub<ICalculator>();

                // register the calculator so it will be auto-injected when we 'create' an aggregator
                Inject(calculatorFake);

                // configure the fake calculator to sequentially return values from addResults irrespective of the input values
                A.CallTo(() => calculatorFake.Add(A<double>.Ignored, A<double>.Ignored))
                  .ReturnsNextFromSequence(addResults.ToArray());

                // Create the aggregator - the fake calculator will be injected
                var aggregator = Create<Aggregator>();

                // Act
                var actual = aggregator.Summate(values.ToArray());

                // Assert
                var expected = addResults[values.Count - 1];
                actual.Should().Be(expected);
            }
        }
    }
}