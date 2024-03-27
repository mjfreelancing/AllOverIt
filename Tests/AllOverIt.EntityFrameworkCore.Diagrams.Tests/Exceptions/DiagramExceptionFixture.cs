using AllOverIt.EntityFrameworkCore.Diagrams.Exceptions;
using AllOverIt.Fixture;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests.Exceptions
{
    public class DiagramExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<DiagramException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<DiagramException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<DiagramException>();
        }
    }
}