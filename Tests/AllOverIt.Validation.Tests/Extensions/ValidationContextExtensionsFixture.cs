using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Validation.Extensions;
using FluentValidation;

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
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    ValidationContextExtensions.SetContextData<DummyModel, string>(null, Create<string>(), Create<string>());
                });
                exception.WithNamedMessageWhenNull("context");
            }

            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    CreateValidationContext().SetContextData(Create<string>(), null);
                });
                exception.WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Throw_When_Key_Empty()
            {
                var exception = Should.Throw<ArgumentException>(() =>
                {
                    CreateValidationContext().SetContextData(Create<string>(), "  ");
                });
                exception.WithNamedMessageWhenEmpty("key");
            }

            [Fact]
            public void Should_Set_InstanceToValidate()
            {
                var expected = Create<DummyModel>();
                var context = CreateValidationContext(expected);

                context.SetContextData(Create<string>());

                var actual = context.InstanceToValidate;

                actual.ShouldBeSameAs(expected);
            }

            [Fact]
            public void Should_Set_Scalar_Data()
            {
                var expected = Create<string>();
                var context = CreateValidationContext();

                context.SetContextData(expected);

                var actual = context.RootContextData["data"];

                actual.ShouldBe(expected);
            }

            [Fact]
            public void Should_Set_Scalar_Data_With_Custom_Key()
            {
                var key = Create<string>();
                var expected = Create<string>();
                var context = CreateValidationContext();

                context.SetContextData(expected, key);

                var actual = context.RootContextData[key];

                actual.ShouldBe(expected);
            }

            [Fact]
            public void Should_Set_Model_Data()
            {
                var expected = Create<DummyContext>();
                var context = CreateValidationContext();

                context.SetContextData(expected);

                var actual = context.RootContextData["data"];

                actual.ShouldBeSameAs(expected);
            }

            [Fact]
            public void Should_Set_Model_Data_With_Custom_Key()
            {
                var key = Create<string>();
                var expected = Create<DummyContext>();
                var context = CreateValidationContext();

                context.SetContextData(expected, key);

                var actual = context.RootContextData[key];

                actual.ShouldBeSameAs(expected);
            }
        }

        public class GetContextData : ValidationContextExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Context_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    ValidationContextExtensions.GetContextData<DummyModel, string>(null, Create<string>());
                });
                exception.WithNamedMessageWhenNull("context");
            }

            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    CreateValidationContext().GetContextData<DummyModel, string>(null);
                });
                exception.WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Throw_When_Key_Empty()
            {
                var exception = Should.Throw<ArgumentException>(() =>
                {
                    CreateValidationContext().GetContextData<DummyModel, string>("  ");
                });
                exception.WithNamedMessageWhenEmpty("key");
            }

            [Fact]
            public void Should_Get_Scalar_Data()
            {
                var expected = Create<string>();
                var context = CreateValidationContext();

                context.RootContextData["data"] = expected;

                var actual = context.GetContextData<DummyModel, string>();

                actual.ShouldBe(expected);
            }

            [Fact]
            public void Should_Get_Scalar_Data_With_Custom_Key()
            {
                var key = Create<string>();
                var expected = Create<string>();
                var context = CreateValidationContext();

                context.RootContextData[key] = expected;

                var actual = context.GetContextData<DummyModel, string>(key);

                actual.ShouldBe(expected);
            }

            [Fact]
            public void Should_Get_Model_Data()
            {
                var expected = Create<DummyModel>();
                var context = CreateValidationContext();

                context.RootContextData["data"] = expected;

                var actual = context.GetContextData<DummyModel, DummyModel>();

                actual.ShouldBeSameAs(expected);
            }

            [Fact]
            public void Should_Set_Model_Data_With_Custom_Key()
            {
                var key = Create<string>();
                var expected = Create<DummyModel>();
                var context = CreateValidationContext();

                context.RootContextData[key] = expected;

                var actual = context.GetContextData<DummyModel, DummyModel>(key);

                actual.ShouldBeSameAs(expected);
            }
        }

        private static ValidationContext<DummyModel> CreateValidationContext(DummyModel instance = default)
        {
            return new ValidationContext<DummyModel>(instance);
        }
    }
}






