#nullable enable

using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Result;
using AllOverIt.Shouldly.Extensions;
namespace AllOverIt.Tests.Patterns.Result;

public class AggregateEnrichedErrorFixture : FixtureBase
{
    private readonly EnrichedError[] EmptyErrors = [];

    private readonly EnrichedError[] EnrichedErrors =
    [
        EnrichedResult.Fail().Error!,
        EnrichedResult.Fail<int>().Error!,
        EnrichedResult.Fail<double>().Error!,
    ];

    public class Constructor_Errors : AggregateEnrichedErrorFixture
    {
        [Fact]
        public void Should_Throw_When_Errors_Null()
        {
            Invoking(() =>
            {
                _ = new AggregateEnrichedError((EnrichedError[]) null!);
            })
            .ShouldThrow<ArgumentNullException>()
            .WithNamedMessageWhenNull("errors");
        }

        [Fact]
        public void Should_Throw_When_Errors_Empty()
        {
            Invoking(() =>
            {
                _ = new AggregateEnrichedError();
            })
            .ShouldThrow<ArgumentException>()
            .WithNamedMessageWhenEmpty("errors");

            Invoking(() =>
            {
                _ = new AggregateEnrichedError(EmptyErrors);
            })
            .ShouldThrow<ArgumentException>()
            .WithNamedMessageWhenEmpty("errors");
        }

        [Fact]
        public void Should_Have_Errors()
        {
            var actual = new AggregateEnrichedError(EnrichedErrors);

            actual.Errors.ShouldBeEquivalentTo(EnrichedErrors);
        }
    }

    public class Constructor_Description_Errors : AggregateEnrichedErrorFixture
    {
        [Fact]
        public void Should_Throw_When_Errors_Null()
        {
            Invoking(() =>
            {
                _ = new AggregateEnrichedError(default(string)!, (EnrichedError[]) null!);
            })
            .ShouldThrow<ArgumentNullException>()
            .WithNamedMessageWhenNull("errors");
        }

        [Fact]
        public void Should_Throw_When_Errors_Empty()
        {
            Invoking(() =>
            {
                _ = new AggregateEnrichedError(default(string)!);
            })
            .ShouldThrow<ArgumentException>()
            .WithNamedMessageWhenEmpty("errors");

            Invoking(() =>
            {
                _ = new AggregateEnrichedError(default(string)!, EmptyErrors);
            })
            .ShouldThrow<ArgumentException>()
            .WithNamedMessageWhenEmpty("errors");
        }

        [Fact]
        public void Should_Have_Errors()
        {
            var actual = new AggregateEnrichedError(default(string)!, EnrichedErrors);

            actual.Errors.ShouldBeEquivalentTo(EnrichedErrors);
        }
    }

    public class Constructor_Type_Description_Errors : AggregateEnrichedErrorFixture
    {
        [Fact]
        public void Should_Throw_When_Errors_Null()
        {
            Invoking(() =>
            {
                _ = new AggregateEnrichedError(default(string)!, default(string)!, (EnrichedError[]) null!);
            })
            .ShouldThrow<ArgumentNullException>()
            .WithNamedMessageWhenNull("errors");
        }

        [Fact]
        public void Should_Throw_When_Errors_Empty()
        {
            Invoking(() =>
            {
                _ = new AggregateEnrichedError(default(string)!, default(string)!);
            })
            .ShouldThrow<ArgumentException>()
            .WithNamedMessageWhenEmpty("errors");

            Invoking(() =>
            {
                _ = new AggregateEnrichedError(default(string)!, default(string)!, EmptyErrors);
            })
            .ShouldThrow<ArgumentException>()
            .WithNamedMessageWhenEmpty("errors");
        }

        [Fact]
        public void Should_Have_Errors()
        {
            var actual = new AggregateEnrichedError(default(string)!, default(string)!, EnrichedErrors);

            actual.Errors.ShouldBeEquivalentTo(EnrichedErrors);
        }
    }

    public class Constructor_Type_Code_Description_Errors : AggregateEnrichedErrorFixture
    {
        [Fact]
        public void Should_Throw_When_Errors_Null()
        {
            Invoking(() =>
            {
                _ = new AggregateEnrichedError(default(string)!, default(string)!, default(string)!, (EnrichedError[]) null!);
            })
            .ShouldThrow<ArgumentNullException>()
            .WithNamedMessageWhenNull("errors");
        }

        [Fact]
        public void Should_Throw_When_Errors_Empty()
        {
            Invoking(() =>
            {
                _ = new AggregateEnrichedError(default(string)!, default(string)!, default(string)!);
            })
            .ShouldThrow<ArgumentException>()
            .WithNamedMessageWhenEmpty("errors");

            Invoking(() =>
            {
                _ = new AggregateEnrichedError(default(string)!, default(string)!, default(string)!, EmptyErrors);
            })
            .ShouldThrow<ArgumentException>()
            .WithNamedMessageWhenEmpty("errors");
        }

        [Fact]
        public void Should_Have_Errors()
        {
            var actual = new AggregateEnrichedError(default(string)!, default(string)!, default(string)!, EnrichedErrors);

            actual.Errors.ShouldBeEquivalentTo(EnrichedErrors);
        }
    }
}




