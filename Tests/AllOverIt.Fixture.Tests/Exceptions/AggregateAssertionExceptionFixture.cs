using AllOverIt.Fixture.Exceptions;
using FluentAssertions;

namespace AllOverIt.Fixture.Tests.Exceptions
{
    public class AggregateAssertionExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<AggregateAssertionException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<AggregateAssertionException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<AggregateAssertionException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException_And_AggregateException()
        {
            var message = Create<string>();
            var innerException = new AggregateException();
            var aggregateException = new AggregateException();

            var exception = new AggregateAssertionException(message, innerException, aggregateException);

            exception.Message.Should().Be(message);
            exception.InnerException.Should().BeSameAs(innerException);
            exception.UnhandledException.Should().BeSameAs(aggregateException);
        }
    }
}