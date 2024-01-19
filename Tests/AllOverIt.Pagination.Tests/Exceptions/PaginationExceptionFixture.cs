using AllOverIt.Fixture;
using AllOverIt.Pagination.Exceptions;
using Xunit;

namespace AllOverIt.Pagination.Tests.Exceptions
{
    public class PaginationExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<PaginationException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<PaginationException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<PaginationException>();
        }
    }
}