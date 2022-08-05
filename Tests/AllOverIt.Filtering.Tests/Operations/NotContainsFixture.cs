﻿using AllOverIt.Filtering.Operations;
using AllOverIt.Filtering.Options;
using AllOverIt.Fixture.Extensions;
using FakeItEasy;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Filtering.Tests.Operations
{
    public class NotContainsFixture : OperationsFixtureBase
    {
        [Fact]
        public void Should_Throw_When_PropertyExpression_Null()
        {
            Invoking(() =>
            {
                _ = new NotContainsOperation<DummyClass>(null, Create<string>(), A.Fake<IOperationFilterOptions>());
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
                _ = new NotContainsOperation<DummyClass>(model => model.Name, Create<string>(), null);
            })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("options");
        }

        [Theory]
        [InlineData(false, StringComparisonMode.None)]
        [InlineData(true, StringComparisonMode.None)]
        [InlineData(false, StringComparisonMode.ToUpper)]
        [InlineData(true, StringComparisonMode.ToUpper)]
        [InlineData(false, StringComparisonMode.ToLower)]
        [InlineData(true, StringComparisonMode.ToLower)]
        public void Should_Satisfy_Specification(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
        {
            var options = new OperationFilterOptions
            {
                UseParameterizedQueries = useParameterizedQueries,
                StringComparisonMode = stringComparisonMode
            };

            var operation = new NotContainsOperation<DummyClass>(model => model.Name, Create<string>(), options);

            operation.IsSatisfiedBy(Model).Should().BeTrue();
        }

        [Theory]
        [InlineData(false, StringComparisonMode.None)]
        [InlineData(true, StringComparisonMode.None)]
        [InlineData(false, StringComparisonMode.ToUpper)]
        [InlineData(true, StringComparisonMode.ToUpper)]
        [InlineData(false, StringComparisonMode.ToLower)]
        [InlineData(true, StringComparisonMode.ToLower)]
        public void Should_Not_Satisfy_Specification(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
        {
            var options = new OperationFilterOptions
            {
                UseParameterizedQueries = useParameterizedQueries,
                StringComparisonMode = stringComparisonMode
            };

            var operation = new NotContainsOperation<DummyClass>(model => model.Name, Model.Name, options);

            operation.IsSatisfiedBy(Model).Should().BeFalse();
        }
    }
}
