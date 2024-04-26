﻿using AllOverIt.Expressions.Strings;
using AllOverIt.Filtering.Exceptions;
using AllOverIt.Filtering.Operations;
using AllOverIt.Filtering.Options;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using System;

namespace AllOverIt.Filtering.Tests.Operations
{
    public class EndsWithOperationFixture : OperationsFixtureBase
    {
        [Fact]
        public void Should_Throw_When_PropertyExpression_Null()
        {
            Invoking(() =>
            {
                _ = new EndsWithOperation<DummyClass>(null, Create<string>(), this.CreateStub<IOperationFilterOptions>());
            })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("propertyExpression");
        }

        [Fact]
        public void Should_Throw_When_Value_Null()
        {
            Invoking(() =>
            {
                _ = new EndsWithOperation<DummyClass>(model => model.Name, null, this.CreateStub<IOperationFilterOptions>());
            })
                .Should()
                .Throw<NullNotSupportedException>();
        }

        [Fact]
        public void Should_Throw_When_Options_Null()
        {
            Invoking(() =>
            {
                _ = new EndsWithOperation<DummyClass>(model => model.Name, Create<string>(), null);
            })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("options");
        }

        [Theory]
        [MemberData(nameof(FilterComparisonOptions))]
        public void Should_Satisfy_Specification(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
        {
            var options = new OperationFilterOptions
            {
                UseParameterizedQueries = useParameterizedQueries,
                StringComparisonMode = stringComparisonMode
            };

            var operation = new EndsWithOperation<DummyClass>(model => model.Name, Model.Name[4..], options);

            operation.IsSatisfiedBy(Model).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(FilterComparisonOptions))]
        public void Should_Not_Satisfy_Specification(bool useParameterizedQueries, StringComparisonMode stringComparisonMode)
        {
            var options = new OperationFilterOptions
            {
                UseParameterizedQueries = useParameterizedQueries,
                StringComparisonMode = stringComparisonMode
            };

            var operation = new EndsWithOperation<DummyClass>(model => model.Name, Create<string>(), options);

            operation.IsSatisfiedBy(Model).Should().BeFalse();
        }
    }
}
