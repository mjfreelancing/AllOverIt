using AllOverIt.Fixture.Tests.Dummies;
using FluentAssertions;

namespace AllOverIt.Fixture.Tests
{
    public class FixtureBaseCustomizationFixture : FixtureBase
    {
        static FixtureBaseCustomizationFixture()
        {
            DummyCustomization.Fixture = null;
        }

        public FixtureBaseCustomizationFixture()
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