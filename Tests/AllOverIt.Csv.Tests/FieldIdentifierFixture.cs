using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using Shouldly;

namespace AllOverIt.Csv.Tests
{
    public class FieldIdentifierFixture : FixtureBase
    {
        private FieldIdentifier<string> CreateFieldIdentifier(string id = default)
        {
            return new FieldIdentifier<string>()
            {
                Id = id ?? Create<string>(),
                Names = [.. CreateMany<string>()]
            };
        }
        public class FieldId_Comparer : FieldIdentifierFixture
        {
            [Fact]
            public void Should_Throw_When_Left_Null()
            {
                var fieldIdentifier = CreateFieldIdentifier();

                Should.Throw<InvalidOperationException>(() =>
                {
                    FieldIdentifier<string>.Comparer.Equals(null, fieldIdentifier);
                })
                .WithMessage("Field identifiers must not be null.");
            }

            [Fact]
            public void Should_Throw_When_Right_Null()
            {
                var fieldIdentifier = CreateFieldIdentifier();

                Should.Throw<InvalidOperationException>(() =>
                {
                    FieldIdentifier<string>.Comparer.Equals(fieldIdentifier, null);
                })
                .WithMessage("Field identifiers must not be null.");
            }

            [Fact]
            public void Should_Return_True_When_Same_Reference()
            {
                var fieldIdentifier = CreateFieldIdentifier();

                var actual = FieldIdentifier<string>.Comparer.Equals(fieldIdentifier, fieldIdentifier);

                actual.ShouldBeTrue();
            }

            [Fact]
            public void Should_Return_True_When_Same_Id()
            {
                var fieldIdentifier1 = CreateFieldIdentifier();
                var fieldIdentifier2 = CreateFieldIdentifier(fieldIdentifier1.Id);

                var actual = FieldIdentifier<string>.Comparer.Equals(fieldIdentifier1, fieldIdentifier2);

                actual.ShouldBeTrue();
            }

            [Fact]
            public void Should_Return_True_When_Different_Id()
            {
                var fieldIdentifier1 = CreateFieldIdentifier();
                var fieldIdentifier2 = CreateFieldIdentifier();

                var actual = FieldIdentifier<string>.Comparer.Equals(fieldIdentifier1, fieldIdentifier2);

                actual.ShouldBeFalse();
            }
        }
    }
}



