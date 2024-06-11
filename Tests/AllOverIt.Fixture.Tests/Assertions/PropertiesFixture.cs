#nullable enable

using AllOverIt.Fixture.Assertions;
using FluentAssertions;

namespace AllOverIt.Fixture.Tests
{
    public class PropertiesFixture : FixtureBase
    {
        private sealed class DummyClass;

        public class For : PropertiesFixture
        {
            [Fact]
            public void Should_Return_ClassProperties()
            {
                // Need to use object rather than ClassProperties<DummyClass>
                object actual = Properties.For<DummyClass>(Create<bool>());

                actual.Should().BeOfType<ClassProperties<DummyClass>>();
            }
        }
    }
}