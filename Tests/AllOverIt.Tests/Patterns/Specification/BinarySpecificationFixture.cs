using AllOverIt.Fixture.Extensions;
using AllOverIt.Tests.Patterns.Specification.Dummies;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class BinarySpecificationFixture : SpecificationFixtureBase
    {
        private readonly bool _expectedResult;
        private readonly BinarySpecificationDummy _specification;

        public BinarySpecificationFixture()
        {
            _expectedResult = Create<bool>();
            _specification = new BinarySpecificationDummy(IsEven, IsPositive, _expectedResult);
        }

        [Fact]
        public void Should_Throw_When_Null_Left_Specification()
        {
            Invoking(() =>
                {
                    _ = new BinarySpecificationDummy(null, IsPositive, _expectedResult);
                })
                .ShouldThrow<ArgumentNullException>()
                .WithNamedMessageWhenNull("leftSpecification");
        }

        [Fact]
        public void Should_Throw_When_Null_Right_Specification()
        {
            Invoking(() =>
                {
                    _ = new BinarySpecificationDummy(IsEven, null, _expectedResult);
                })
                .ShouldThrow<ArgumentNullException>()
                .WithNamedMessageWhenNull("rightSpecification");
        }

        [Fact]
        public void Should_Set_Specification_Members()
        {
            _specification.Left.ShouldBeSameAs(IsEven);
            _specification.Right.ShouldBeSameAs(IsPositive);
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



