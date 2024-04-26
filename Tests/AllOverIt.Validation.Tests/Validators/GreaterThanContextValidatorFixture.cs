using AllOverIt.Fixture;
using AllOverIt.Validation.Validators;
using FluentAssertions;
using System;

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

            tickIndex.Should().BeGreaterThan(-1);

            validator.Name
                .Should()
                .Be(typeName[..tickIndex]);
        }
    }
}
