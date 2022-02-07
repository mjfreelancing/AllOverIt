using AllOverIt.Fixture;
using AllOverIt.Formatters.Objects;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Formatters.Objects
{
    public class ObjectPropertyFilterFixture : FixtureBase
    {
        private sealed class ObjectPropertyFilterDummy : ObjectPropertyFilter
        {
        }

        public class OnIncludeProperty : ObjectPropertyFilterFixture
        {
            [Fact]
            public void Should_Default_Return_True()
            {
                var sut = new ObjectPropertyFilterDummy();

                sut.OnIncludeProperty().Should().BeTrue();
            }
        }

        public class OnIncludeValue : ObjectPropertyFilterFixture
        {
            [Fact]
            public void Should_Default_Return_True()
            {
                var sut = new ObjectPropertyFilterDummy();

                sut.OnIncludeValue().Should().BeTrue();
            }
        }
    }
}