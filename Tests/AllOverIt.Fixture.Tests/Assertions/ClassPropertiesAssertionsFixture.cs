#nullable enable

using AllOverIt;
using AllOverIt.Fixture.Assertions;
using FluentAssertions;
using System.Text;
using Xunit.Sdk;

namespace AllOverIt.Fixture.Tests.Assertions
{
    // Other tests for ClassPropertiesAssertions are indirectly tested as part of PropertyInfoAssertionsFixture

    public class ClassPropertiesAssertions_Generic_Fixture : FixtureBase
    {
        private abstract class DummyClass
        {
            public int Prop1 { set { } }
            public int Prop2 { get; private set; }
            public int Prop3 { get; internal set; }
        }

        public class MatchNames : ClassPropertiesAssertions_Generic_Fixture
        {
            [Fact]
            public void Should_Not_Fail_When_Property_Names_Out_Of_Order()
            {
                Properties
                    .For<DummyClass>()
                    .Should()
                    .MatchNames([nameof(DummyClass.Prop1), nameof(DummyClass.Prop3), nameof(DummyClass.Prop2)]);
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    Properties
                        .For<DummyClass>()
                        .Should()
                        .MatchNames([nameof(DummyClass.Prop1), nameof(DummyClass.Prop3)]);
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage("Expected properties on type DummyClass to be named {\"Prop1\",\"Prop3\"}, but found {\"Prop1\",\"Prop2\",\"Prop3\"}.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass>()
                        .Should()
                        .MatchNames([nameof(DummyClass.Prop1), nameof(DummyClass.Prop3)], reason);
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage($"Expected properties on type DummyClass to be named {{\"Prop1\",\"Prop3\"}} because {reason}, but found {{\"Prop1\",\"Prop2\",\"Prop3\"}}.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass>()
                        .Should()
                        .MatchNames([nameof(DummyClass.Prop1), nameof(DummyClass.Prop3)], reason, reasonArgs);
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage($"Expected properties on type DummyClass to be named {{\"Prop1\",\"Prop3\"}} because {expectedReason}, but found {{\"Prop1\",\"Prop2\",\"Prop3\"}}.");
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
                        .MatchNames([nameof(DummyClass.Prop1), nameof(DummyClass.Prop2), nameof(DummyClass.Prop3)], reason);
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage($"Expected to validate at least one property on type \"DummyClass\" because {reason}, but found none.");
            }
        }

        public class BeDefinedAs : ClassPropertiesAssertions_Generic_Fixture
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

                expected.AppendLine("Expected Prop1 to have a private get accessor, but found it has no matching accessor.");
                expected.AppendLine("Expected Prop2 to have a private get accessor, but found it has a public get accessor.");
                expected.AppendLine("Expected Prop3 to have a private get accessor, but found it has a public get accessor.");

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

                expected.AppendLine($"Expected Prop1 to have a private get accessor because {reason1}, but found it has no matching accessor.");
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

    public class ClassPropertiesAssertions_Type_Fixture : FixtureBase
    {
        private abstract class DummyClass<T>
        {
            public int Prop1 { set { } }
            public int Prop2 { get; private set; }
            public T? Prop3 { get; internal set; }
        }

        public class MatchNames : ClassPropertiesAssertions_Type_Fixture
        {
            [Fact]
            public void Should_Not_Fail_When_Property_Names_Out_Of_Order()
            {
                Properties
                    .For(typeof(DummyClass<>))
                    .Should()
                    .MatchNames(["Prop1", "Prop3", "Prop2"]);
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    Properties
                        .For(typeof(DummyClass<>))
                        .Should()
                        .MatchNames(["Prop1", "Prop3"]);
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage("Expected properties on type DummyClass<T> to be named {\"Prop1\",\"Prop3\"}, but found {\"Prop1\",\"Prop2\",\"Prop3\"}.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    Properties
                        .For(typeof(DummyClass<>))
                        .Should()
                        .MatchNames(["Prop1", "Prop3"], reason);
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage($"Expected properties on type DummyClass<T> to be named {{\"Prop1\",\"Prop3\"}} because {reason}, but found {{\"Prop1\",\"Prop2\",\"Prop3\"}}.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    Properties
                        .For(typeof(DummyClass<>))
                        .Should()
                        .MatchNames(["Prop1", "Prop3"], reason, reasonArgs);
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage($"Expected properties on type DummyClass<T> to be named {{\"Prop1\",\"Prop3\"}} because {expectedReason}, but found {{\"Prop1\",\"Prop2\",\"Prop3\"}}.");
            }

            [Fact]
            public void Should_Fail_When_No_Properties()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    Properties
                        .For(typeof(DummyClass<>))
                        .Where(_ => false)
                        .Should()
                        .MatchNames(["Prop1", "Prop2", "Prop3"], reason);
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage($"Expected to validate at least one property on type \"DummyClass<T>\" because {reason}, but found none.");
            }
        }

        public class BeDefinedAs : ClassPropertiesAssertions_Type_Fixture
        {
            [Fact]
            public void Should_Not_Fail()
            {
                Properties
                    .For(typeof(DummyClass<>))
                    .Excluding("Prop3")
                    .Should()
                    .BeDefinedAs(property =>
                    {
                        property.IsOfType<int>();
                    });

                Properties
                    .For(typeof(DummyClass<>))
                    .Including("Prop3")
                    .Should()
                    .BeDefinedAs(property =>
                    {
                        property.MeetsCriteria(propInfo => propInfo.PropertyType.IsGenericTypeParameter);
                    });
            }

            [Fact]
            public void Should_Fail()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop1 to have a private get accessor, but found it has no matching accessor.");
                expected.AppendLine("Expected Prop2 to have a private get accessor, but found it has a public get accessor.");
                expected.AppendLine("Expected Prop3 to have a private get accessor, but found it has a public get accessor.");

                Invoking(() =>
                {
                    Properties
                        .For(typeof(DummyClass<>))
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

                expected.AppendLine($"Expected Prop1 to have a private get accessor because {reason1}, but found it has no matching accessor.");
                expected.AppendLine($"Expected Prop1 to be of type \"System.String\" because {reason2}, but found \"System.Int32\".");

                Invoking(() =>
                {
                    Properties
                        .For(typeof(DummyClass<>))
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
                        .For(typeof(DummyClass<>))
                        .Where(_ => false)
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsPrivate(Types.PropertyAccessor.Get, Create<string>());
                        }, reason);
                })
                   .Should()
                   .Throw<XunitException>()
                   .WithMessage($"Expected to validate at least one property on type \"DummyClass<T>\" because {reason}, but found none.");
            }
        }
    }
}