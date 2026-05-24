using AllOverIt.Aspects;
using AllOverIt.Fixture;

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

            state.GetResult().ShouldBeNull();

            state.SetResult(expected);

            state.GetResult().ShouldBe(expected);
        }

        [Fact]
        public void Should_Set_Get_Typed_Result_Method()
        {
            var expected = Create<string>();
            var state = new DummyStringState();

            state.GetResult().ShouldBeNull();

            state.SetResult(expected);

            state.GetResult<string>().ShouldBe(expected);
        }

        [Fact]
        public void Should_Set_Get_Typed_Result_Property()
        {
            var expected = Create<string>();
            var state = new DummyStringState();

            state.GetResult().ShouldBeNull();

            state.Result = expected;

            state.Result.ShouldBe(expected);
        }
    }
}


