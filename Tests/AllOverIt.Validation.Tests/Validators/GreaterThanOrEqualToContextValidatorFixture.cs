using AllOverIt.Fixture;
using AllOverIt.Validation.Validators;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Validation.Tests.Validators
{
    public class GreaterThanOrEqualToContextValidatorFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Expected_Name()
        {
            var validator = new GreaterThanOrEqualToContextValidator<int, int, int>(_ => _);

            var typeName = typeof(GreaterThanOrEqualToContextValidator<,,>).Name;
            var tickIndex = typeName.IndexOf("`", StringComparison.Ordinal);

            tickIndex.Should().BeGreaterThan(-1);

            validator.Name
                .Should()
                .Be(typeName[..tickIndex]);
        }
    }
}
