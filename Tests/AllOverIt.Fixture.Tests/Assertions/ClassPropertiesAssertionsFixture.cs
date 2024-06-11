#nullable enable

using AllOverIt.Fixture.Assertions;
using FluentAssertions;
using System.Text;
using Xunit.Sdk;

namespace AllOverIt.Fixture.Tests
{
    // Other tests for ClassPropertiesAssertions are indirectly tested as part of PropertyInfoAssertionsFixture

    public class ClassPropertiesAssertionsFixture : FixtureBase
    {
        private abstract class DummyClass
        {
            public int Prop1 { set { } }
            public int Prop2 { get; private set; }
            public int Prop3 { get; internal set; }
        }

        public class BeDefinedAs : ClassPropertiesAssertionsFixture
        {
            [Fact]
            public void Should_Not_Fail()
            {
                Properties
                    .For<DummyClass>()
                    .Should()
                    .BeDefinedAs(property =>
                    {
                        property.IsOfType<int>();
                    });
            }

            [Fact]
            public void Should_Fail()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop1 to have a private get accessor, but found no matching accessor.");
                expected.AppendLine("Expected Prop2 to have a private get accessor, but found it has a public accessor.");
                expected.AppendLine("Expected Prop3 to have a private get accessor, but found it has a public accessor.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass>()
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsPrivate(Types.PropertyAccessor.Get);
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason1 = Create<string>();
                var reason2 = Create<string>();
                var reason3 = Create<string>();

                var expected = new StringBuilder();

                expected.AppendLine($"Expected Prop1 to have a private get accessor because {reason1}, but found no matching accessor.");
                expected.AppendLine($"Expected Prop1 to be of type \"System.String\" because {reason2}, but found \"System.Int32\".");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass>()
                        .Including("Prop1")
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property
                                .IsPrivate(Types.PropertyAccessor.Get, reason1)
                                .IsOfType<string>(reason2);
                        }, reason3);        // Ignored when the property assertions are executed
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }

            [Fact]
            public void Should_Fail_When_No_Properties()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass>()
                        .Where(_ => false)
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsPrivate(Types.PropertyAccessor.Get, Create<string>());
                        }, reason);
                })
                   .Should()
                   .Throw<XunitException>()
                   .WithMessage($"Expected to validate at least one property on type \"DummyClass\" because {reason}, but found none.");
            }
        }
    }
}