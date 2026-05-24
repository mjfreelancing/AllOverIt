using AllOverIt.Fixture.Extensions;
using AllOverIt.Tests.Patterns.Specification.Dummies;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class UnarySpecificationFixture : SpecificationFixtureBase
    {
        private readonly bool _expectedResult;
        private readonly UnarySpecificationDummy _specification;

        public UnarySpecificationFixture()
        {
            _expectedResult = Create<bool>();
            _specification = new UnarySpecificationDummy(IsEven, _expectedResult);
        }

        [Fact]
        public void Should_Throw_When_Null_Specification()
        {
            Invoking(() =>
                {
                    _ = new UnarySpecificationDummy(null, _expectedResult);
                })
                .ShouldThrow<ArgumentNullException>()
                .WithNamedMessageWhenNull("specification");
        }

        [Fact]
        public void Should_Set_Specification_Member()
        {
            _specification.Spec.ShouldBeSameAs(IsEven);
        }

        [Fact]
        public void Should_Pass_Candidate()
        {
            var expected = Create<int>();

            _specification.IsSatisfiedBy(expected);

            _specification.Candidate.ShouldBe(expected);
        }

        [Fact]
        public void Should_Return_Expected_Result()
        {
            var actual = _specification.IsSatisfiedBy(Create<int>());

            actual.ShouldBe(_expectedResult);
        }
    }
}



