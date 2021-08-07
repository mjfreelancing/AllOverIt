using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Validation.Extensions;
using FluentAssertions;
using FluentValidation;
using System;
using Xunit;

namespace AllOverIt.Validation.Tests.Extensions
{
    public class ValidationContextExtensionsFixture : FixtureBase
    {
        private class DummyModel
        {
        }

        private class DummyContext
        {
        }

        public class SetContextData : ValidationContextExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Context_Null()
            {
                Invoking(() =>
                {
                    ValidationContextExtensions.SetContextData<DummyModel, string>(null, Create<string>(), Create<string>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("context");
            }

            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                {
                    CreateValidationContext().SetContextData(Create<string>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Throw_When_Key_Empty()
            {
                Invoking(() =>
                {
                    CreateValidationContext().SetContextData(Create<string>(), "  ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("key");
            }

            [Fact]
            public void Should_Set_InstanceToValidate()
            {
                var expected = Create<DummyModel>();
                var context = CreateValidationContext(expected);

                context.SetContextData(Create<string>());

                var actual = context.InstanceToValidate;

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Set_Scalar_Data()
            {
                var expected = Create<string>();
                var context = CreateValidationContext();

                context.SetContextData(expected);

                var actual = context.RootContextData["data"];

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Set_Scalar_Data_With_Custom_Key()
            {
                var key = Create<string>();
                var expected = Create<string>();
                var context = CreateValidationContext();

                context.SetContextData(expected, key);

                var actual = context.RootContextData[key];

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Set_Model_Data()
            {
                var expected = Create<DummyContext>();
                var context = CreateValidationContext();

                context.SetContextData(expected);

                var actual = context.RootContextData["data"];

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Set_Model_Data_With_Custom_Key()
            {
                var key = Create<string>();
                var expected = Create<DummyContext>();
                var context = CreateValidationContext();

                context.SetContextData(expected, key);

                var actual = context.RootContextData[key];

                actual.Should().BeSameAs(expected);
            }
        }

        public class GetContextData : ValidationContextExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Context_Null()
            {
                Invoking(() =>
                {
                    ValidationContextExtensions.GetContextData<DummyModel, string>(null, Create<string>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("context");
            }

            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                {
                    CreateValidationContext().GetContextData<DummyModel, string>(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Throw_When_Key_Empty()
            {
                Invoking(() =>
                {
                    CreateValidationContext().GetContextData<DummyModel, string>("  ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("key");
            }

            [Fact]
            public void Should_Get_Scalar_Data()
            {
                var expected = Create<string>();
                var context = CreateValidationContext();

                context.RootContextData["data"] = expected;

                var actual = context.GetContextData<DummyModel, string>();

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Get_Scalar_Data_With_Custom_Key()
            {
                var key = Create<string>();
                var expected = Create<string>();
                var context = CreateValidationContext();

                context.RootContextData[key] = expected;

                var actual = context.GetContextData<DummyModel, string>(key);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Get_Model_Data()
            {
                var expected = Create<DummyModel>();
                var context = CreateValidationContext();

                context.RootContextData["data"] = expected;

                var actual = context.GetContextData<DummyModel, DummyModel>();

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Set_Model_Data_With_Custom_Key()
            {
                var key = Create<string>();
                var expected = Create<DummyModel>();
                var context = CreateValidationContext();

                context.RootContextData[key] = expected;

                var actual = context.GetContextData<DummyModel, DummyModel>(key);

                actual.Should().BeSameAs(expected);
            }
        }

        private static ValidationContext<DummyModel> CreateValidationContext(DummyModel instance = default)
        {
            return new ValidationContext<DummyModel>(instance);
        }
    }
}
