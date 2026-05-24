using AllOverIt.Fixture;
using AllOverIt.Validation.Validators;

namespace AllOverIt.Validation.Tests.Validators
{
    public class GreaterThanContextValidatorFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Expected_Name()
        {
            var validator = new GreaterThanContextValidator<int, int, int>(_ => _);

            var typeName = typeof(GreaterThanContextValidator<,,>).Name;
            var tickIndex = typeName.IndexOf("`", StringComparison.Ordinal);

            tickIndex.ShouldBeGreaterThan(-1);

            validator.Name
                 .ShouldBe(typeName[..tickIndex]);
        }
    }
}






