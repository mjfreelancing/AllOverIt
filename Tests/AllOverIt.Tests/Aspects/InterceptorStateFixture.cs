using AllOverIt.Aspects;
using AllOverIt.Fixture;
using FluentAssertions;

namespace AllOverIt.Tests.Aspects
{
    public class InterceptorStateFixture : FixtureBase
    {
        private class DummyState : InterceptorState
        {
        }

        private class DummyStringState : InterceptorState<string>
        {
        }

        [Fact]
        public void Should_Set_Get_Result()
        {
            var expected = Create<int>();
            var state = new DummyState();

            state.GetResult().Should().BeNull();

            state.SetResult(expected);

            state.GetResult().Should().Be(expected);
        }

        [Fact]
        public void Should_Set_Get_Typed_Result_Method()
        {
            var expected = Create<string>();
            var state = new DummyStringState();

            state.GetResult().Should().BeNull();

            state.SetResult(expected);

            state.GetResult<string>().Should().Be(expected);
        }

        [Fact]
        public void Should_Set_Get_Typed_Result_Property()
        {
            var expected = Create<string>();
            var state = new DummyStringState();

            state.GetResult().Should().BeNull();

            state.Result = expected;

            state.Result.Should().Be(expected);
        }
    }
}