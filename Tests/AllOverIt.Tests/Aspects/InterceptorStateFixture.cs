using AllOverIt.Aspects;
using AllOverIt.Fixture;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Aspects
{
    public class InterceptorStateFixture : FixtureBase
    {
        private class DummyState : InterceptorState
        {
        }

        [Fact]
        public void Should_Have_None_State()
        {
            InterceptorState.None.Should().BeAssignableTo<InterceptorState>();
        }
    }
}