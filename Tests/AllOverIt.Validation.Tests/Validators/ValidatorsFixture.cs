using AllOverIt.Fixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Validation.Tests.Validators
{
    // These tests implicitly test: RuleBuilderExtensions, ContextComparisonValidator, ValidatorBase
    public partial class ValidatorsFixture : FixtureBase
    {
        private class DummyComparisonModel
        {
            public string Value1 { get; set; }
            public Guid Value2 { get; set; }
            public int? Value3 { get; set; }
            public int Value4 { get; set; }
            public int? Value5 { get; set; }
            public int Value6 { get; set; }
            public IReadOnlyList<int> Value7 { get; set; }
        }

        private class DummyModel
        {
            public bool ANameThatCanBeSplit { get; set; }
        }

        private class DummyValidator1 : ValidatorBase<DummyModel>
        {
            static DummyValidator1()
            {
                DisablePropertyNameSplitting();
            }

            public DummyValidator1()
            {
                RuleFor(model => model.ANameThatCanBeSplit).Equal(true);
            }
        }

        private class DummyValidator2 : ValidatorBase<DummyModel>
        {
            public string PropertyName { get; }
            public string ErrorMessage { get; }

            static DummyValidator2()
            {
                DisablePropertyNameSplitting();
            }

            public DummyValidator2(string propertyName, string errorMessage)
            {
                PropertyName = propertyName;
                ErrorMessage = errorMessage;

                RuleFor(model => model)
                    .Custom((model, context) =>
                    {
                        var failure = new ValidationFailure(PropertyName, ErrorMessage);

                        context.AddFailure(failure);
                    });
            }
        }

        public class PropertyNameSplitting : ValidatorsFixture
        {
            [Fact]
            public void Should_Not_Split_Property_Names()
            {
                var model = new DummyModel();
                var validator = new DummyValidator1();

                var result = validator.Validate(model);

                result.IsValid.Should().BeFalse();

                result.Errors.Single()
                    .ErrorMessage
                    .Should()
                    .Be("'ANameThatCanBeSplit' must be equal to 'True'.");
            }

            [Fact]
            public void Should_Handle_Rule_For_Model()
            {
                var propertyName = Create<string>();
                var errorMessage = Create<string>();

                var model = new DummyModel();
                var validator = new DummyValidator2(propertyName, errorMessage);

                var result = validator.Validate(model);

                var error = result.Errors.Single();

                error
                    .PropertyName
                    .Should()
                    .Be(propertyName);

                error
                    .ErrorMessage
                    .Should()
                    .Be(errorMessage);
            }

            // Not running this test as parallel execution can cause other tests to fail. Not concerned with
            // configuring tests to run sequentially since splitting the property names is the default behaviour
            // and I'm more interested in checking the property names are not split.
            //
            //[Fact]
            //public void Should_Split_Property_Names()
            //{
            //    var currentResolver = ValidatorOptions.Global.DisplayNameResolver;

            //    ValidatorOptions.Global.DisplayNameResolver = null;

            //    try
            //    {
            //        var model = new DummyModel();
            //        var validator = new DummyValidator();

            //        var result = validator.Validate(model);

            //        result.IsValid.Should().BeFalse();

            //        result.Errors.Single()
            //            .ErrorMessage
            //            .Should()
            //            .Be("'A Name That Can Be Split' must be equal to 'True'.");
            //    }
            //    finally
            //    {
            //        ValidatorOptions.Global.DisplayNameResolver = currentResolver;
            //    }
            //}
        }
    }
}
