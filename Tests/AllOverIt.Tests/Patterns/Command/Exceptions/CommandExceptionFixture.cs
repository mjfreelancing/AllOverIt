using AllOverIt.Fixture;
using AllOverIt.Patterns.Command.Exceptions;
using Xunit;

namespace AllOverIt.Tests.Patterns.Command.Exceptions
{
    public class CommandExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<CommandException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<CommandException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<CommandException>();
        }
    }
}