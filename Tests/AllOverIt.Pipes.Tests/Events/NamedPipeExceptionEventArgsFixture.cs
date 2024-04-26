using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pipes.Named.Events;
using FluentAssertions;

namespace AllOverIt.Pipes.Tests.Events
{
    public class NamedPipeExceptionEventArgsFixture : FixtureBase
    {
        public class Constructor : NamedPipeExceptionEventArgsFixture
        {
            [Fact]
            public void Should_Throw_When_Exception_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeExceptionEventArgs(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("exception");
            }

            [Fact]
            public void Should_Return_Exception()
            {
                var expected = new Exception();

                var eventArgs = new NamedPipeExceptionEventArgs(expected);

                eventArgs.Exception.Should().BeSameAs(expected);
            }
        }
    }
}