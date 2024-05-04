using AllOverIt.Patterns.Specification;
using AllOverIt.Tests.Patterns.Specification.Dummies;
using FluentAssertions;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class SpecificationFixture : SpecificationFixtureBase
    {
        // Used for testing operator true and operator false via operator & / && and | / ||
        private static readonly Specification<int> multipleOfTwo = Specification<int>.Create(candidate => candidate % 2 == 0) as Specification<int>;
        private static readonly Specification<int> multipleOfThree = Specification<int>.Create(candidate => candidate % 3 == 0) as Specification<int>;
        private static readonly Specification<int> twoOrThreeSpecification = multipleOfTwo || multipleOfThree;      // Same as: multipleOfTwo.Or(multipleOfThree);
        private static readonly Specification<int> twoAndThreeSpecification = multipleOfTwo && multipleOfThree;     // Same as: multipleOfTwo.And(multipleOfThree);
        private static readonly Specification<int> notMultipleOfTwo = !multipleOfTwo;

        public class IsSatisfiedBy : SpecificationFixture
        {
            private readonly bool _expectedResult;
            private readonly SpecificationDummy _boolSpecification;

            public IsSatisfiedBy()
            {
                _expectedResult = Create<bool>();
                _boolSpecification = new SpecificationDummy(_expectedResult);
            }

            [Fact]
            public void Should_Pass_Candidate()
            {
                var expected = Create<int>();

                _boolSpecification.IsSatisfiedBy(expected);

                _boolSpecification.Candidate.Should().Be(expected);
            }

            [Fact]
            public void Should_Return_Expected_Result()
            {
                var actual = _boolSpecification.IsSatisfiedBy(Create<int>());

                actual.Should().Be(_expectedResult);
            }
        }

        public class Create : SpecificationFixture
        {
            [Fact]
            public void Should_Create_Specification()
            {
                var factor = GetWithinRange(3, 5);

                var specification = Specification<int>.Create(candidate => candidate % factor == 0) as Specification<int>;

                specification.Should().NotBeNull();

                for (var i = 3; i <= 5; i++)
                {
                    var actual = specification.IsSatisfiedBy(i);

                    actual.Should().Be(i == factor);
                }
            }
        }

        public class Or : SpecificationFixture
        {
            [Theory]
            [InlineData(1, false)]
            [InlineData(2, true)]
            [InlineData(3, true)]
            [InlineData(4, true)]
            [InlineData(5, false)]
            [InlineData(6, true)]
            [InlineData(7, false)]
            [InlineData(8, true)]
            [InlineData(9, true)]
            public void Should_Return_Expected_Or_Results(int candidate, bool expected)
            {
                var actual = twoOrThreeSpecification.IsSatisfiedBy(candidate);

                actual.Should().Be(expected);
            }
        }

        public class And : SpecificationFixture
        {
            [Theory]
            [InlineData(1, false)]
            [InlineData(2, false)]
            [InlineData(3, false)]
            [InlineData(4, false)]
            [InlineData(5, false)]
            [InlineData(6, true)]
            [InlineData(7, false)]
            [InlineData(8, false)]
            [InlineData(9, false)]
            public void Should_Return_Expected_And_Results(int candidate, bool expected)
            {
                var actual = twoAndThreeSpecification.IsSatisfiedBy(candidate);

                actual.Should().Be(expected);
            }
        }

        public class Not : SpecificationFixture
        {
            [Theory]
            [InlineData(1, true)]
            [InlineData(2, false)]
            [InlineData(3, true)]
            [InlineData(4, false)]
            [InlineData(5, true)]
            [InlineData(6, false)]
            [InlineData(7, true)]
            [InlineData(8, false)]
            [InlineData(9, true)]
            public void Should_Return_Expected_Not_Results(int candidate, bool expected)
            {
                var actual = notMultipleOfTwo.IsSatisfiedBy(candidate);

                actual.Should().Be(expected);
            }
        }
    }
}