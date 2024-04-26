using AllOverIt.Expressions.Strings;
using AllOverIt.Filtering.Exceptions;
using AllOverIt.Filtering.Operations;
using AllOverIt.Filtering.Options;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;

namespace AllOverIt.Filtering.Tests.Operations
{
    public class InOperationFixture : OperationsFixtureBase
    {
        [Fact]
        public void Should_Throw_When_PropertyExpression_Null()
        {
            Invoking(() =>
            {
                _ = new InOperation<DummyClass, string>(null, CreateMany<string>().ToList(), this.CreateStub<IOperationFilterOptions>());
            })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("propertyExpression");
        }

        [Fact]
        public void Should_Throw_When_Values_Null()
        {
            Invoking(() =>
            {
                _ = new InOperation<DummyClass, string>(model => model.Name, null, this.CreateStub<IOperationFilterOptions>());
            })
                .Should()
                .Throw<NullNotSupportedException>();
        }

        [Fact]
        public void Should_Throw_When_Options_Null()
        {
            Invoking(() =>
            {
                _ = new InOperation<DummyClass, string>(model => model.Name, CreateMany<string>().ToList(), null);
            })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("options");
        }

        [Theory]
        [MemberData(nameof(FilterComparisonOptions))]
        public void Should_Satisfy_Specification_String(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
        {
            var options = new OperationFilterOptions
            {
                UseParameterizedQueries = useParameterizedQueries,
                StringComparisonMode = stringComparisonMode
            };

            var operation = new InOperation<DummyClass, string>(model => model.Name, new[] { Model.Name, Create<string>() }, options);

            operation.IsSatisfiedBy(Model).Should().BeTrue();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Should_Satisfy_Specification_Value(bool useParameterizedQueries)
        {
            var options = new OperationFilterOptions
            {
                UseParameterizedQueries = useParameterizedQueries
            };

            var operation = new InOperation<DummyClass, int>(model => model.Id, new[] { Model.Id, Create<int>() }, options);

            operation.IsSatisfiedBy(Model).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(FilterComparisonOptions))]
        public void Should_Not_Satisfy_Specification_String(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
        {
            var options = new OperationFilterOptions
            {
                UseParameterizedQueries = useParameterizedQueries,
                StringComparisonMode = stringComparisonMode
            };

            var operation = new InOperation<DummyClass, string>(model => model.Name, CreateMany<string>().ToList(), options);

            operation.IsSatisfiedBy(Model).Should().BeFalse();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Should_Not_Satisfy_Specification_Value(bool useParameterizedQueries)
        {
            var options = new OperationFilterOptions
            {
                UseParameterizedQueries = useParameterizedQueries
            };

            var operation = new InOperation<DummyClass, int>(model => model.Id, new[] { Model.Id - 1, Model.Id + 1 }, options);

            operation.IsSatisfiedBy(Model).Should().BeFalse();
        }
    }
}
