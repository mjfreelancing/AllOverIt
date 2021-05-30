using AllOverIt.Fixture.Tests.Dummies;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Fixture.Tests
{
    public class AoiFixtureBaseCustomizationFixture : AoiFixtureBase
    {
        static AoiFixtureBaseCustomizationFixture()
        {
            DummyCustomization.Fixture = null;
        }

        public AoiFixtureBaseCustomizationFixture()
          : base(new DummyCustomization())
        {
        }

        [Fact]
        public void Should_Have_Customization()
        {
            DummyCustomization.Fixture.Should().Be(Fixture);
        }
    }
}