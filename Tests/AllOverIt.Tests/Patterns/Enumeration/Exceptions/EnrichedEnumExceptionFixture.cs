using AllOverIt.Fixture;
using AllOverIt.Patterns.Enumeration.Exceptions;

namespace AllOverIt.Tests.Patterns.Enumeration.Exceptions
{
    public class EnrichedEnumExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<EnrichedEnumException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<EnrichedEnumException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<EnrichedEnumException>();
        }
    }
}