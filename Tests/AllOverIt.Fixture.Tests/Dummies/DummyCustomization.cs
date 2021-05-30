using AutoFixture;

namespace AllOverIt.Fixture.Tests.Dummies
{
    internal class DummyCustomization : ICustomization
    {
        public static IFixture Fixture { get; set; }

        public void Customize(IFixture fixture)
        {
            Fixture = fixture;
        }
    }
}