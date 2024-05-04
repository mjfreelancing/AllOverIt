using AllOverIt.Fixture;
using AllOverIt.Validation.Validators;
using FluentAssertions;

namespace AllOverIt.Validation.Tests.Validators
{
    public class ExclusiveBetweenContextValidatorFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Expected_Name()
        {
            var validator = new ExclusiveBetweenContextValidator<int, int, int>(_ => _, _ => _);

            var typeName = typeof(ExclusiveBetweenContextValidator<,,>).Name;
            var tickIndex = typeName.IndexOf("`", StringComparison.Ordinal);

            tickIndex.Should().BeGreaterThan(-1);

            validator.Name
                .Should()
                .Be(typeName[..tickIndex]);
        }
    }
}
