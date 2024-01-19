using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Csv.Tests
{
    public class FieldIdentifierFixture : FixtureBase
    {
        public class FieldId_Comparer : FieldIdentifierFixture
        {
            [Fact]
            public void Should_Throw_When_Left_Null()
            {
                var fieldIdentifier = new FieldIdentifier<string>();

                Invoking(() =>
                {
                    FieldIdentifier<string>.Comparer.Equals(null, fieldIdentifier);
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Field identifiers must not be null.");
            }

            [Fact]
            public void Should_Throw_When_Right_Null()
            {
                var fieldIdentifier = new FieldIdentifier<string>();

                Invoking(() =>
                {
                    FieldIdentifier<string>.Comparer.Equals(fieldIdentifier, null);
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Field identifiers must not be null.");
            }

            [Fact]
            public void Should_Return_True_When_Same_Reference()
            {
                var fieldIdentifier = new FieldIdentifier<string>();

                var actual = FieldIdentifier<string>.Comparer.Equals(fieldIdentifier, fieldIdentifier);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_True_When_Same_Id()
            {
                var fieldIdentifier1 = new FieldIdentifier<string>
                {
                    Id = Create<string>()
                };

                var fieldIdentifier2 = new FieldIdentifier<string>
                {
                    Id = fieldIdentifier1.Id
                };

                var actual = FieldIdentifier<string>.Comparer.Equals(fieldIdentifier1, fieldIdentifier2);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_True_When_Different_Id()
            {
                var fieldIdentifier1 = new FieldIdentifier<string>
                {
                    Id = Create<string>()
                };

                var fieldIdentifier2 = new FieldIdentifier<string>
                {
                    Id = Create<string>()
                };

                var actual = FieldIdentifier<string>.Comparer.Equals(fieldIdentifier1, fieldIdentifier2);

                actual.Should().BeFalse();
            }
        }
    }
}
