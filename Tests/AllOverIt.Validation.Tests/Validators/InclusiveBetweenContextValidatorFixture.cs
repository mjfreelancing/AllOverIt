using AllOverIt.Fixture;
using AllOverIt.Validation.Validators;

namespace AllOverIt.Validation.Tests.Validators
{
    public class InclusiveBetweenContextValidatorFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Expected_Name()
        {
            var validator = new InclusiveBetweenContextValidator<int, int, int>(_ => _, _ => _);

            var typeName = typeof(InclusiveBetweenContextValidator<,,>).Name;
            var tickIndex = typeName.IndexOf("`", StringComparison.Ordinal);

            tickIndex.ShouldBeGreaterThan(-1);

            validator.Name
                 .ShouldBe(typeName[..tickIndex]);
        }
    }
}






