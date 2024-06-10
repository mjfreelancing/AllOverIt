#nullable enable

using AllOverIt.Extensions;
using AllOverIt.Fixture.Assertions;
using AllOverIt.Reflection;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Reflection;
using System.Text;
using Xunit.Sdk;

namespace AllOverIt.Fixture.Tests
{
    public class NullabilityInfoAssertionsFixture : FixtureBase
    {
        private abstract class DummyClass1
        {
            public string? Prop1 { get; }
            public DummyClass1? Prop2 { get; init; }
            public bool Prop3 { get; }
            public Dictionary<int, string?[]?> Prop4 { get; } = [];
            public IDictionary<int, IDictionary<int?, string?[]?>>? Prop5 { get; }
            public bool? Prop6 { get; }
            public string Prop7 { get; } = string.Empty;
            public bool[] Prop8 { get; } = [];
            public bool[]? Prop9 { get; }
            public bool?[]? Prop10 { get; }
            public string?[] Prop11 { get; } = [];
        }

        public class IsNullable : NullabilityInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new NullabilityInfoAssertions(null!);

                    assertions.IsNullable();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <NullabilityInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop3));

                    GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop3)).IsNullable();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop3 to be \"Nullable\", but it is \"NotNull\".");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop3));

                    GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop3)).IsNullable(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop3 to be \"Nullable\" because {reason}, but it is \"NotNull\".");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop3));

                    GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop3)).IsNullable(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop3 to be \"Nullable\" because {expectedReason}, but it is \"NotNull\".");
            }

            [Fact]
            public void Should_Not_Fail_Reference()
            {
                using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsNullable();
            }

            [Fact]
            public void Should_Not_Fail_Struct()
            {
                using var _ = new AssertionScope(nameof(DummyClass1.Prop6));

                GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop6)).IsNullable();
            }
        }

        public class IsNotNull : NullabilityInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new NullabilityInfoAssertions(null!);

                    assertions.IsNotNull();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <NullabilityInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop2));

                    GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop2)).IsNotNull();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop2 to be \"NotNull\", but it is \"Nullable\".");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop2));

                    GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop2)).IsNotNull(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop2 to be \"NotNull\" because {reason}, but it is \"Nullable\".");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop2));

                    GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop2)).IsNotNull(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop2 to be \"NotNull\" because {expectedReason}, but it is \"Nullable\".");
            }

            [Fact]
            public void Should_Not_Fail_Reference()
            {
                using var _ = new AssertionScope(nameof(DummyClass1.Prop7));

                GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop7)).IsNotNull();
            }

            [Fact]
            public void Should_Not_Fail_Struct()
            {
                using var _ = new AssertionScope(nameof(DummyClass1.Prop3));

                GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop3)).IsNotNull();
            }
        }

        public class IsCollectionOf : NullabilityInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new NullabilityInfoAssertions(null!);

                    assertions.IsCollectionOf<bool>(_ => { });
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <NullabilityInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop7));

                    GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop7)).IsCollectionOf<bool>(_ => { });
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop7 to be a collection of type Boolean, but it isn't.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop7));

                    GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop7)).IsCollectionOf<bool>(_ => { }, reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop7 to be a collection of type Boolean because {reason}, but it isn't.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop7));

                    GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop7)).IsCollectionOf<bool>(_ => { }, reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop7 to be a collection of type Boolean because {expectedReason}, but it isn't.");
            }

            [Fact]
            public void Should_Not_Fail_Reference()
            {
                using var _ = new AssertionScope(nameof(DummyClass1.Prop11));

                GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop11)).IsCollectionOf<string>(_ => { });
            }

            [Fact]
            public void Should_Not_Fail_Struct()
            {
                using var _ = new AssertionScope(nameof(DummyClass1.Prop8));

                GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop8)).IsCollectionOf<bool>(_ => { });
            }

            [Fact]
            public void Should_Fail_Inner()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop11));

                    GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop11)).IsCollectionOf<string>(nullability =>
                    {
                        nullability.IsNotNull();
                    });
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop11 to be \"NotNull\", but it is \"Nullable\".");
            }

            [Fact]
            public void Should_Fail_Inner_Reason()
            {
                var outer = Create<string>();
                var inner = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop11));

                    GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop11)).IsCollectionOf<string>(nullability =>
                    {
                        nullability.IsNotNull(inner);
                    }, outer);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop11 to be \"NotNull\" because {inner}, but it is \"Nullable\".");
            }

            [Fact]
            public void Should_Fail_Outer_Reason_Only()
            {
                var outer = Create<string>();
                var inner = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop11));

                    GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop11)).IsCollectionOf<bool>(nullability =>
                    {
                        nullability.IsNotNull(inner);
                    }, outer);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop11 to be a collection of type Boolean because {outer}, but it isn't.");
            }

            [Fact]
            public void Should_Not_Fail_Inner()
            {
                using var _ = new AssertionScope(nameof(DummyClass1.Prop11));

                GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop11)).IsCollectionOf<string>(nullability =>
                {
                    nullability.IsNullable();
                });
            }
        }

        public class IsOfType : NullabilityInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new NullabilityInfoAssertions(null!);

                    assertions.IsOfType<bool>();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <NullabilityInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop7));

                    GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop7)).IsOfType<bool>();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop7 to have type \"System.Boolean\", but found \"System.String\".");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop7));

                    GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop7)).IsOfType<bool>(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop7 to have type \"System.Boolean\" because {reason}, but found \"System.String\".");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop7));

                    GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop7)).IsOfType<bool>(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop7 to have type \"System.Boolean\" because {expectedReason}, but found \"System.String\".");
            }

            [Fact]
            public void Should_Fail_With_Generic_Name_When_No_Current_Scope()
            {
                Invoking(() =>
                {
                    // There will be a scope when used via ClassPropertiesAssertions.BeDefinedAs() and the associated PropertyInfo Assertions.
                    // This test ensures there's no null reference if no scope has been defined.

                    // using var _ = new AssertionScope(nameof(DummyClass1.Prop4));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop4))
                        .IsOfType<Dictionary<int, string?[]?>>(propertyAssertions =>
                        {
                            propertyAssertions.ForGenericArg(1, nullability =>
                            {
                                nullability.IsOfType<string>();   // is string?[]?
                            });
                        });
                })
               .Should()
               .Throw<XunitException>()
               .WithMessage("Expected generic arg<1> to have type \"System.String\", but found \"System.String[]\".");
            }

            [Fact]
            public void Should_Fail_With_Scope_Name()
            {
                Invoking(() =>
                {
                    // There will be a scope when used via ClassPropertiesAssertions.BeDefinedAs() and the associated PropertyInfo Assertions.
                    // This test ensures there's no null reference if no scope has been defined.

                    using var _ = new AssertionScope(nameof(DummyClass1.Prop4));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop4))
                        .IsOfType<Dictionary<int, string?[]?>>(propertyAssertions =>
                        {
                            propertyAssertions.ForGenericArg(1, nullability =>
                            {
                                nullability.IsOfType<string>();   // is string?[]?
                            });
                        });
                })
               .Should()
               .Throw<XunitException>()
               .WithMessage("Expected Prop4<1> to have type \"System.String\", but found \"System.String[]\".");
            }

            [Fact]
            public void Should_Not_Fail_As_Inner_Assertion()
            {
                // There will be a scope when used via ClassPropertiesAssertions.BeDefinedAs() and the associated PropertyInfo Assertions.
                // This test also ensures there's no null reference if no scope has been defined.

                // using var _ = new AssertionScope(nameof(DummyClass1.Prop4));

                GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop4))
                    .IsOfType<Dictionary<int, string?[]?>>(propertyAssertions =>
                    {
                        propertyAssertions.ForGenericArg(1, nullability =>
                        {
                            // This is the actual type - the latter ? has no bearing on the end result (related to nullable references and generics)
                            nullability.IsOfType<string?[]?>();

                            // This will also succeed
                            nullability.IsOfType<string?[]>();
                        });
                    });
            }
        }

        public class ForGenericArg : NullabilityInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new NullabilityInfoAssertions(null!);

                    assertions.ForGenericArg(0, _ => { });
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <NullabilityInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop7));

                    GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop7)).ForGenericArg(0, _ => { });
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop7 to have a generic type argument at index 0, but the index is out of range.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop7));

                    GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop7)).ForGenericArg(0, _ => { }, reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop7 to have a generic type argument at index 0 because {reason}, but the index is out of range.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop7));

                    GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop7)).ForGenericArg(0, _ => { }, reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop7 to have a generic type argument at index 0 because {expectedReason}, but the index is out of range.");
            }

            [Fact]
            public void Should_Not_Fail_When_No_Scope()
            {
                GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop5))
                    .ForGenericArg(0, nullabilityInfo =>
                    {
                        nullabilityInfo.IsOfType<int>();
                    })
                    .ForGenericArg(1, nullabilityInfo =>
                    {
                        nullabilityInfo
                            .IsOfType<IDictionary<int?, string?[]?>?>()
                            .ForGenericArg(0, arg0 =>
                            {
                                arg0.IsOfType<int?>();
                            })
                            .ForGenericArg(1, arg1 =>
                            {
                                arg1.IsOfType<string?[]?>();
                            });
                    });
            }

            [Fact]
            public void Should_Not_Fail_When_Has_Scope()
            {
                using var _ = new AssertionScope(nameof(DummyClass1.Prop5));

                GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop5))
                    .ForGenericArg(0, nullabilityInfo =>
                    {
                        nullabilityInfo.IsOfType<int>();
                    })
                    .ForGenericArg(1, nullabilityInfo =>
                    {
                        nullabilityInfo
                            .IsOfType<IDictionary<int?, string?[]?>?>()
                            .ForGenericArg(0, arg0 =>
                            {
                                arg0.IsOfType<int?>();
                            })
                            .ForGenericArg(1, arg1 =>
                            {
                                arg1.IsOfType<string?[]?>();
                            });
                    });
            }

            [Fact]
            public void Should_Fail_With_Inner_Reasons()
            {
                var reason1 = Create<string>();
                var reason2 = Create<string>();
                var reason3 = Create<string>();
                var reason4 = Create<string>();

                var expected = new StringBuilder();

                expected.AppendLine($"Expected Prop5<0> to have type \"System.Boolean\" because {reason1}, but found \"System.Int32\".");
                expected.AppendLine($"Expected Prop5<1> to have type \"System.String\" because {reason2}, but found \"System.Collections.Generic.IDictionary<Int32?, String[]>\".");
                expected.AppendLine($"Expected Prop5<1><0> to have type \"System.Boolean\" because {reason3}, but found \"System.Int32?\".");
                expected.AppendLine($"Expected Prop5<1><1> to be \"NotNull\" because {reason4}, but it is \"Nullable\".");

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop5));

                    GetNullabilityAssertions<DummyClass1>(nameof(DummyClass1.Prop5))
                        .ForGenericArg(0, nullabilityInfo =>
                        {
                            nullabilityInfo.IsOfType<bool>(reason1);        // int
                        })
                        .ForGenericArg(1, nullabilityInfo =>
                        {
                            nullabilityInfo
                                .IsOfType<string>(reason2)                  // IDictionary<int?, string?[]?>?
                                .ForGenericArg(0, arg0 =>
                                {
                                    arg0.IsOfType<bool>(reason3);           // int?
                                })
                                .ForGenericArg(1, arg1 =>
                                {
                                    arg1
                                        .IsOfType<string[]>()               // Is same as string?[]?
                                        .IsNotNull(reason4);
                                });
                        });
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage(expected.ToString());
            }
        }

        private PropertyInfoAssertions GetPropertyAssertions<TType>(string name)
        {
            var propInfo = typeof(TType)
                .GetPropertyInfo(BindingOptions.All)
                .Single(propInfo => propInfo.Name == name);

            return new PropertyInfoAssertions(propInfo);
        }

        private NullabilityInfoAssertions GetNullabilityAssertions<TType>(string name)
        {
            var propInfo = typeof(TType)
                .GetPropertyInfo(BindingOptions.All)
                .Single(propInfo => propInfo.Name == name);

            var nullabilityInfo = new NullabilityInfoContext().Create(propInfo);

            return new NullabilityInfoAssertions(nullabilityInfo);
        }
    }
}