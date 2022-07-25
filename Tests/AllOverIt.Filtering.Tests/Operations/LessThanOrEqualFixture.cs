using AllOverIt.Filtering.Operations;
using AllOverIt.Filtering.Options;
using AllOverIt.Fixture.Extensions;
using FakeItEasy;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Filtering.Tests.Operations
{
    public class LessThanOrEqualFixture : OperationsFixtureBase
    {
        [Fact]
        public void Should_Throw_When_PropertyExpression_Null()
        {
            Invoking(() =>
            {
                _ = new LessThanOrEqualOperation<DummyClass, string>(null, Create<string>(), A.Fake<IOperationFilterOptions>());
            })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("propertyExpression");
        }

        [Fact]
        public void Should_Throw_When_Options_Null()
        {
            Invoking(() =>
            {
                _ = new LessThanOrEqualOperation<DummyClass, string>(model => model.Name, Create<string>(), null);
            })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("options");
        }

        [Theory]
        [InlineData(false, default)]
        [InlineData(true, default)]
        [InlineData(false, StringComparison.InvariantCultureIgnoreCase)]
        [InlineData(true, StringComparison.InvariantCultureIgnoreCase)]
        public void Should_Satisfy_Specification_String_When_Equal(bool useParameterizedQueries, StringComparison? stringComparison)
        {
            var options = new OperationFilterOptions
            {
                UseParameterizedQueries = useParameterizedQueries,
                StringComparison = stringComparison
            };

            var operation = new LessThanOrEqualOperation<DummyClass, string>(model => model.Name, Model.Name, options);

            operation.IsSatisfiedBy(Model).Should().BeTrue();
        }

        [Theory]
        [InlineData(false, default)]
        [InlineData(true, default)]
        [InlineData(false, StringComparison.InvariantCultureIgnoreCase)]
        [InlineData(true, StringComparison.InvariantCultureIgnoreCase)]
        public void Should_Satisfy_Specification_String_When_LessThan(bool useParameterizedQueries, StringComparison? stringComparison)
        {
            var options = new OperationFilterOptions
            {
                UseParameterizedQueries = useParameterizedQueries,
                StringComparison = stringComparison
            };

            var operation = new LessThanOrEqualOperation<DummyClass, string>(model => model.Name, $"{Model.Name}ZZZ", options);

            operation.IsSatisfiedBy(Model).Should().BeTrue();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Should_Satisfy_Specification_Value_When_Equal(bool useParameterizedQueries)
        {
            var options = new OperationFilterOptions
            {
                UseParameterizedQueries = useParameterizedQueries
            };

            var operation = new LessThanOrEqualOperation<DummyClass, int>(model => model.Id, Model.Id, options);

            operation.IsSatisfiedBy(Model).Should().BeTrue();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Should_Satisfy_Specification_Value_When_LessThan(bool useParameterizedQueries)
        {
            var options = new OperationFilterOptions
            {
                UseParameterizedQueries = useParameterizedQueries
            };

            var operation = new LessThanOrEqualOperation<DummyClass, int>(model => model.Id, Model.Id + 1, options);

            operation.IsSatisfiedBy(Model).Should().BeTrue();
        }

        [Theory]
        [InlineData(false, default)]
        [InlineData(true, default)]
        [InlineData(false, StringComparison.InvariantCultureIgnoreCase)]
        [InlineData(true, StringComparison.InvariantCultureIgnoreCase)]
        public void Should_Not_Satisfy_Specification_String(bool useParameterizedQueries, StringComparison? stringComparison)
        {
            var options = new OperationFilterOptions
            {
                UseParameterizedQueries = useParameterizedQueries,
                StringComparison = stringComparison
            };

            var operation = new LessThanOrEqualOperation<DummyClass, string>(model => model.Name, Model.Name[..4], options);

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

            var operation = new LessThanOrEqualOperation<DummyClass, int>(model => model.Id, Model.Id - 1, options);

            operation.IsSatisfiedBy(Model).Should().BeFalse();
        }
    }
}
