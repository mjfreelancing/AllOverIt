using AllOverIt.Fixture;
using AllOverIt.Validation.Validators;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Validation.Tests.Validators
{
    public class LessThanOrEqualToContextValidatorFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Expected_Name()
        {
            var validator = new LessThanOrEqualToContextValidator<int, int, int>(_ => _);

            var typeName = typeof(LessThanOrEqualToContextValidator<,,>).Name;
            var tickIndex = typeName.IndexOf("`");

            tickIndex.Should().BeGreaterThan(-1);

            validator.Name
                .Should()
                .Be(typeName[0..tickIndex]);
        }
    }
}
