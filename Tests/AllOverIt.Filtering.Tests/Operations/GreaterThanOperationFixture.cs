﻿using AllOverIt.Expressions.Strings;
using AllOverIt.Filtering.Operations;
using AllOverIt.Filtering.Options;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;

namespace AllOverIt.Filtering.Tests.Operations
{
    public class GreaterThanOperationFixture : OperationsFixtureBase
    {
        [Fact]
        public void Should_Throw_When_PropertyExpression_Null()
        {
            Invoking(() =>
            {
                _ = new GreaterThanOperation<DummyClass, string>(null, Create<string>(), this.CreateStub<IOperationFilterOptions>());
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
                _ = new GreaterThanOperation<DummyClass, string>(model => model.Name, Create<string>(), null);
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

            var operation = new GreaterThanOperation<DummyClass, string>(model => model.Name, Model.Name[..4], options);

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

            var operation = new GreaterThanOperation<DummyClass, int>(model => model.Id, Model.Id - 1, options);

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

            var operation = new GreaterThanOperation<DummyClass, string>(model => model.Name, Model.Name, options);

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

            var operation = new GreaterThanOperation<DummyClass, int>(model => model.Id, Model.Id + 1, options);

            operation.IsSatisfiedBy(Model).Should().BeFalse();
        }
    }
}
