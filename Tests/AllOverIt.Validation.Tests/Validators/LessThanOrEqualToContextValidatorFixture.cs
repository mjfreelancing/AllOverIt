using AllOverIt.Fixture;
using AllOverIt.Validation.Validators;
using FluentAssertions;
using System;

namespace AllOverIt.Validation.Tests.Validators
{
    public class LessThanOrEqualToContextValidatorFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Expected_Name()
        {
            var validator = new LessThanOrEqualToContextValidator<int, int, int>(_ => _);

            var typeName = typeof(LessThanOrEqualToContextValidator<,,>).Name;
            var tickIndex = typeName.IndexOf("`", StringComparison.Ordinal);

            tickIndex.Should().BeGreaterThan(-1);

            validator.Name
                .Should()
                .Be(typeName[..tickIndex]);
        }
    }
}
