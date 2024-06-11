#nullable enable

using AllOverIt.Extensions;
using AllOverIt.Fixture.Assertions;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Reflection;
using AllOverIt.Types;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Text;
using Xunit.Sdk;

namespace AllOverIt.Fixture.Tests
{
    public class PropertyInfoAssertionsFixture : FixtureBase
    {
        private abstract class DummyClassBase
        {
            public abstract string? Prop6 { get; }
            protected DummyClassBase? Prop9 { get; init; }
            public abstract bool Prop15 { get; }
        }

        private class DummyClass1 : DummyClassBase
        {
            public int Prop1 { set { } }
            private bool[]? Prop2 { get; set; }
            public double? Prop3 { private get; set; }
            public string Prop4 { get; internal set; } = string.Empty;
            public DummyClass1? Prop5 { get; protected set; }
            public override string? Prop6 { get; }
            public required virtual int? Prop7 { get; init; }
            public bool Prop10 { set { } }
            public override bool Prop15 { get; }
        }

        private sealed class DummyClass2 : DummyClass1
        {
            public required override int? Prop7 { get; init; }
            public bool Prop8 { get; internal set; }
            public int Prop11 { get; }
            internal required int Prop12 { get; set; }
            public bool this[int index] { get => true; set { } }
            public static int Prop13 { get; set; }
            public static string? Prop14 { get; set; }
        }

        public class IsReadable : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsReadable();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsReadable();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop1 to be readable, but it isn't.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsReadable(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 to be readable because {reason}, but it isn't.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsReadable(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 to be readable because {expectedReason}, but it isn't.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClass1.Prop3));

                GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop3)).IsReadable();
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop1 to be readable, but it isn't.");
                expected.AppendLine("Expected Prop10 to be readable, but it isn't.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass1>()
                        .Including(nameof(DummyClass1.Prop1), nameof(DummyClass1.Prop10))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsReadable();
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsNotReadable : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsNotReadable();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope("Prop2");

                    GetPropertyAssertions<DummyClass1>("Prop2").IsNotReadable();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop2 not to be readable, but it is.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope("Prop2");

                    GetPropertyAssertions<DummyClass1>("Prop2").IsNotReadable(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop2 not to be readable because {reason}, but it is.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope("Prop2");

                    GetPropertyAssertions<DummyClass1>("Prop2").IsNotReadable(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop2 not to be readable because {expectedReason}, but it is.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsNotReadable();
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop2 not to be readable, but it is.");
                expected.AppendLine("Expected Prop7 not to be readable, but it is.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass1>()
                        .Including("Prop2", nameof(DummyClass1.Prop7))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsNotReadable();
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsWritable : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsWritable();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClassBase.Prop6));

                    GetPropertyAssertions<DummyClassBase>(nameof(DummyClassBase.Prop6)).IsWritable();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop6 to be writable, but it isn't.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClassBase.Prop6));

                    GetPropertyAssertions<DummyClassBase>(nameof(DummyClassBase.Prop6)).IsWritable(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop6 to be writable because {reason}, but it isn't.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClassBase.Prop6));

                    GetPropertyAssertions<DummyClassBase>(nameof(DummyClassBase.Prop6)).IsWritable(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop6 to be writable because {expectedReason}, but it isn't.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope("Prop9");

                GetPropertyAssertions<DummyClassBase>("Prop9").IsWritable();
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop6 to be writable, but it isn't.");
                expected.AppendLine("Expected Prop11 to be writable, but it isn't.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass2>()
                        .Including(nameof(DummyClass2.Prop6), nameof(DummyClass2.Prop11))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsWritable();
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsNotWritable : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsNotWritable();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass2.Prop8));

                    GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop8)).IsNotWritable();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop8 not to be writable, but it is.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass2.Prop8));

                    GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop8)).IsNotWritable(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop8 not to be writable because {reason}, but it is.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass2.Prop8));

                    GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop8)).IsNotWritable(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop8 not to be writable because {expectedReason}, but it is.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClassBase.Prop6));

                GetPropertyAssertions<DummyClassBase>(nameof(DummyClassBase.Prop6)).IsNotWritable();
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop2 not to be writable, but it is.");
                expected.AppendLine("Expected Prop7 not to be writable, but it is.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass1>()
                        .Including("Prop2", nameof(DummyClass1.Prop7))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsNotWritable();
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsPublic : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsPublic(Create<PropertyAccessor>());
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail_When_Different_Access()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop3));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop3)).IsPublic(PropertyAccessor.Get);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop3 to have a public get accessor, but found it has a private accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Get_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsPublic(PropertyAccessor.Get);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop1 to have a public get accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Set_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop6));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop6)).IsPublic(PropertyAccessor.Set);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop6 to have a public set accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Init_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop5));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop5)).IsPublic(PropertyAccessor.Init);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop5 to have a public init accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_When_Set_And_Init_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop5));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop5)).IsPublic(PropertyAccessor.Set | PropertyAccessor.Init);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Property access validation cannot include 'Set' and 'Init'.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop3));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop3)).IsPublic(PropertyAccessor.Get, reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop3 to have a public get accessor because {reason}, but found it has a private accessor.");
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

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop3)).IsPublic(PropertyAccessor.Get, reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop3 to have a public get accessor because {expectedReason}, but found it has a private accessor.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClass1.Prop7));

                GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop7)).IsPublic(PropertyAccessor.Get);
                GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop7)).IsPublic(PropertyAccessor.Set);
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop2 to have a public get accessor, but found it has a private accessor.");
                expected.AppendLine("Expected Prop2 to have a public set accessor, but found it has a private accessor.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass1>()
                        .Including("Prop2")
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsPublic(PropertyAccessor.Get | PropertyAccessor.Set);
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsNotPublic : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsNotPublic(Create<PropertyAccessor>());
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail_When_Different_Access()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsNotPublic(PropertyAccessor.Set);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop1 not to have a public set accessor, but found it has a public accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Get_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsNotPublic(PropertyAccessor.Get);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop1 not to have a public get accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Set_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop6));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop6)).IsNotPublic(PropertyAccessor.Set);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop6 not to have a public set accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Init_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop5));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop5)).IsNotPublic(PropertyAccessor.Init);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop5 not to have a public init accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_When_Set_And_Init_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsNotPublic(PropertyAccessor.Set | PropertyAccessor.Init);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Property access validation cannot include 'Set' and 'Init'.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsNotPublic(PropertyAccessor.Set, reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 not to have a public set accessor because {reason}, but found it has a public accessor.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsNotPublic(PropertyAccessor.Set, reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 not to have a public set accessor because {expectedReason}, but found it has a public accessor.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope("Prop2");

                GetPropertyAssertions<DummyClass1>("Prop2").IsNotPublic(PropertyAccessor.Get);
                GetPropertyAssertions<DummyClass1>("Prop2").IsNotPublic(PropertyAccessor.Set);
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop7 not to have a public get accessor, but found it has a public accessor.");
                expected.AppendLine("Expected Prop7 not to have a public set accessor, but found it has a public accessor.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass1>()
                        .Including(nameof(DummyClass1.Prop7))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsNotPublic(PropertyAccessor.Get | PropertyAccessor.Set);
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsProtected : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsProtected(Create<PropertyAccessor>());
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail_When_Different_Access()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop3));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop3)).IsProtected(PropertyAccessor.Get);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop3 to have a protected get accessor, but found it has a private accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Get_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsProtected(PropertyAccessor.Get);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop1 to have a protected get accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Set_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop6));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop6)).IsProtected(PropertyAccessor.Set);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop6 to have a protected set accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Init_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop5));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop5)).IsProtected(PropertyAccessor.Init);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop5 to have a protected init accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_When_Set_And_Init_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsProtected(PropertyAccessor.Set | PropertyAccessor.Init);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Property access validation cannot include 'Set' and 'Init'.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop3));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop3)).IsProtected(PropertyAccessor.Get, reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop3 to have a protected get accessor because {reason}, but found it has a private accessor.");
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

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop3)).IsProtected(PropertyAccessor.Get, reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop3 to have a protected get accessor because {expectedReason}, but found it has a private accessor.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope("Prop9");

                GetPropertyAssertions<DummyClass1>("Prop9").IsProtected(PropertyAccessor.Get);
                GetPropertyAssertions<DummyClass1>("Prop9").IsProtected(PropertyAccessor.Set);
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop2 to have a protected get accessor, but found it has a private accessor.");
                expected.AppendLine("Expected Prop2 to have a protected set accessor, but found it has a private accessor.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass1>()
                        .Including("Prop2")
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsProtected(PropertyAccessor.Get | PropertyAccessor.Set);
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsNotProtected : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsNotProtected(Create<PropertyAccessor>());
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail_When_Different_Access()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope("Prop5");

                    GetPropertyAssertions<DummyClass1>("Prop5").IsNotProtected(PropertyAccessor.Set);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop5 not to have a protected set accessor, but found it has a protected accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Get_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsNotProtected(PropertyAccessor.Get);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop1 not to have a protected get accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Set_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop6));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop6)).IsNotProtected(PropertyAccessor.Set);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop6 not to have a protected set accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Init_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop5));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop5)).IsNotProtected(PropertyAccessor.Init);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop5 not to have a protected init accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_When_Set_And_Init_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsNotProtected(PropertyAccessor.Set | PropertyAccessor.Init);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Property access validation cannot include 'Set' and 'Init'.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope("Prop9");

                    GetPropertyAssertions<DummyClass1>("Prop9").IsNotProtected(PropertyAccessor.Set, reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop9 not to have a protected set accessor because {reason}, but found it has a protected accessor.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope("Prop9");

                    GetPropertyAssertions<DummyClass1>("Prop9").IsNotProtected(PropertyAccessor.Set, reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop9 not to have a protected set accessor because {expectedReason}, but found it has a protected accessor.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope("Prop2");

                GetPropertyAssertions<DummyClass1>("Prop2").IsNotProtected(PropertyAccessor.Get);
                GetPropertyAssertions<DummyClass1>("Prop2").IsNotProtected(PropertyAccessor.Set);
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop9 not to have a protected get accessor, but found it has a protected accessor.");
                expected.AppendLine("Expected Prop9 not to have a protected set accessor, but found it has a protected accessor.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass1>()
                        .Including("Prop9")
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsNotProtected(PropertyAccessor.Get | PropertyAccessor.Set);
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsPrivate : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsPrivate(Create<PropertyAccessor>());
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail_When_Different_Access()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop4));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop4)).IsPrivate(PropertyAccessor.Get);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop4 to have a private get accessor, but found it has a public accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Get_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsPrivate(PropertyAccessor.Get);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop1 to have a private get accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Set_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop6));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop6)).IsPrivate(PropertyAccessor.Set);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop6 to have a private set accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Init_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop5));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop5)).IsPrivate(PropertyAccessor.Init);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop5 to have a private init accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_When_Set_And_Init_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsPrivate(PropertyAccessor.Set | PropertyAccessor.Init);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Property access validation cannot include 'Set' and 'Init'.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop4));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop4)).IsPrivate(PropertyAccessor.Get, reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop4 to have a private get accessor because {reason}, but found it has a public accessor.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop4));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop4)).IsPrivate(PropertyAccessor.Get, reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop4 to have a private get accessor because {expectedReason}, but found it has a public accessor.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope("Prop2");

                GetPropertyAssertions<DummyClass1>("Prop2").IsPrivate(PropertyAccessor.Get);
                GetPropertyAssertions<DummyClass1>("Prop2").IsPrivate(PropertyAccessor.Set);
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop4 to have a private get accessor, but found it has a public accessor.");
                expected.AppendLine("Expected Prop4 to have a private set accessor, but found it has an internal accessor.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass1>()
                        .Including(nameof(DummyClass1.Prop4))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsPrivate(PropertyAccessor.Get | PropertyAccessor.Set);
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsNotPrivate : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsNotPrivate(Create<PropertyAccessor>());
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail_When_Different_Access()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope("Prop2");

                    GetPropertyAssertions<DummyClass1>("Prop2").IsNotPrivate(PropertyAccessor.Set);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop2 not to have a private set accessor, but found it has a private accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Get_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsNotPrivate(PropertyAccessor.Get);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop1 not to have a private get accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Set_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop6));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop6)).IsNotPrivate(PropertyAccessor.Set);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop6 not to have a private set accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Init_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop5));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop5)).IsNotPrivate(PropertyAccessor.Init);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop5 not to have a private init accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_When_Set_And_Init_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsNotPrivate(PropertyAccessor.Set | PropertyAccessor.Init);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Property access validation cannot include 'Set' and 'Init'.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope("Prop2");

                    GetPropertyAssertions<DummyClass1>("Prop2").IsNotPrivate(PropertyAccessor.Set, reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop2 not to have a private set accessor because {reason}, but found it has a private accessor.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope("Prop2");

                    GetPropertyAssertions<DummyClass1>("Prop2").IsNotPrivate(PropertyAccessor.Set, reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop2 not to have a private set accessor because {expectedReason}, but found it has a private accessor.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClass1.Prop7));

                GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop7)).IsNotPrivate(PropertyAccessor.Get);
                GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop7)).IsNotPrivate(PropertyAccessor.Set);
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop2 not to have a private get accessor, but found it has a private accessor.");
                expected.AppendLine("Expected Prop2 not to have a private set accessor, but found it has a private accessor.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass1>()
                        .Including("Prop2")
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsNotPrivate(PropertyAccessor.Get | PropertyAccessor.Set);
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsInternal : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsInternal(Create<PropertyAccessor>());
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail_When_Different_Access()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop4));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop4)).IsInternal(PropertyAccessor.Get);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop4 to have an internal get accessor, but found it has a public accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsInternal(PropertyAccessor.Get);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop1 to have an internal get accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop4));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop4)).IsInternal(PropertyAccessor.Get, reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop4 to have an internal get accessor because {reason}, but found it has a public accessor.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop4));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop4)).IsInternal(PropertyAccessor.Get, reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop4 to have an internal get accessor because {expectedReason}, but found it has a public accessor.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope("Prop12");

                GetPropertyAssertions<DummyClass2>("Prop12").IsInternal(PropertyAccessor.Get);
                GetPropertyAssertions<DummyClass2>("Prop12").IsInternal(PropertyAccessor.Set);
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop3 to have an internal get accessor, but found it has a private accessor.");
                expected.AppendLine("Expected Prop3 to have an internal set accessor, but found it has a public accessor.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass1>()
                        .Including(nameof(DummyClass1.Prop3))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsInternal(PropertyAccessor.Get | PropertyAccessor.Set);
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsNotInternal : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsNotInternal(Create<PropertyAccessor>());
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail_When_Different_Access()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope("Prop12");

                    GetPropertyAssertions<DummyClass2>("Prop12").IsNotInternal(PropertyAccessor.Set);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop12 not to have an internal set accessor, but found it has an internal accessor.");
            }

            [Fact]
            public void Should_Fail_When_No_Accessor()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsNotInternal(PropertyAccessor.Get);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop1 not to have an internal get accessor, but found no matching accessor.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope("Prop12");

                    GetPropertyAssertions<DummyClass2>("Prop12").IsNotInternal(PropertyAccessor.Set, reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop12 not to have an internal set accessor because {reason}, but found it has an internal accessor.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope("Prop12");

                    GetPropertyAssertions<DummyClass2>("Prop12").IsNotInternal(PropertyAccessor.Set, reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop12 not to have an internal set accessor because {expectedReason}, but found it has an internal accessor.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClass1.Prop7));

                GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop7)).IsNotInternal(PropertyAccessor.Get);
                GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop7)).IsNotInternal(PropertyAccessor.Set);
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop12 not to have an internal get accessor, but found it has an internal accessor.");
                expected.AppendLine("Expected Prop12 not to have an internal init accessor, but found no matching accessor.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass2>()
                        .Including("Prop12")
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsNotInternal(PropertyAccessor.Get | PropertyAccessor.Init);
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsAnIndexer : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsAnIndexer();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClassBase.Prop6));

                    GetPropertyAssertions<DummyClassBase>(nameof(DummyClassBase.Prop6)).IsAnIndexer();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop6 to be an indexer, but it isn't.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClassBase.Prop6));

                    GetPropertyAssertions<DummyClassBase>(nameof(DummyClassBase.Prop6)).IsAnIndexer(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop6 to be an indexer because {reason}, but it isn't.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClassBase.Prop6));

                    GetPropertyAssertions<DummyClassBase>(nameof(DummyClassBase.Prop6)).IsAnIndexer(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop6 to be an indexer because {expectedReason}, but it isn't.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope("Item");

                GetPropertyAssertions<DummyClass2>("Item").IsAnIndexer();
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop6 to be an indexer, but it isn't.");
                expected.AppendLine("Expected Prop11 to be an indexer, but it isn't.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass2>()
                        .Including(nameof(DummyClass2.Prop6), nameof(DummyClass2.Prop11))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsAnIndexer();
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsNotAnIndexer : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsNotAnIndexer();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope("Item");

                    GetPropertyAssertions<DummyClass2>("Item").IsNotAnIndexer();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Item not to be an indexer, but it is.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope("Item");

                    GetPropertyAssertions<DummyClass2>("Item").IsNotAnIndexer(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Item not to be an indexer because {reason}, but it is.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope("Item");

                    GetPropertyAssertions<DummyClass2>("Item").IsNotAnIndexer(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Item not to be an indexer because {expectedReason}, but it is.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsNotAnIndexer();
            }
        }

        public class IsStatic : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsStatic();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClassBase.Prop6));

                    GetPropertyAssertions<DummyClassBase>(nameof(DummyClassBase.Prop6)).IsStatic();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop6 to be static, but it isn't.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClassBase.Prop6));

                    GetPropertyAssertions<DummyClassBase>(nameof(DummyClassBase.Prop6)).IsStatic(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop6 to be static because {reason}, but it isn't.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClassBase.Prop6));

                    GetPropertyAssertions<DummyClassBase>(nameof(DummyClassBase.Prop6)).IsStatic(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop6 to be static because {expectedReason}, but it isn't.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClass2.Prop13));

                GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop13)).IsStatic();
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop6 to be static, but it isn't.");
                expected.AppendLine("Expected Prop11 to be static, but it isn't.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass2>()
                        .Including(nameof(DummyClass2.Prop6), nameof(DummyClass2.Prop11))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsStatic();
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsNotStatic : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsNotStatic();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass2.Prop13));

                    GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop13)).IsNotStatic();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop13 not to be static, but it is.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass2.Prop13));

                    GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop13)).IsNotStatic(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop13 not to be static because {reason}, but it is.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass2.Prop13));

                    GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop13)).IsNotStatic(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop13 not to be static because {expectedReason}, but it is.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClassBase.Prop6));

                GetPropertyAssertions<DummyClassBase>(nameof(DummyClassBase.Prop6)).IsNotStatic();
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop13 not to be static, but it is.");
                expected.AppendLine("Expected Prop14 not to be static, but it is.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass2>()
                        .Including(nameof(DummyClass2.Prop13), nameof(DummyClass2.Prop14))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsNotStatic();
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsAbstract : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsAbstract();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsAbstract();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop1 to be abstract, but it isn't.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsAbstract(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 to be abstract because {reason}, but it isn't.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsAbstract(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 to be abstract because {expectedReason}, but it isn't.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClassBase.Prop6));

                GetPropertyAssertions<DummyClass2>(nameof(DummyClassBase.Prop6)).IsAbstract();
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop1 to be abstract, but it isn't.");
                expected.AppendLine("Expected Prop3 to be abstract, but it isn't.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass2>()
                        .Including(nameof(DummyClass2.Prop1), nameof(DummyClass2.Prop3))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsAbstract();
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsNotAbstract : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsNotAbstract();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass2.Prop15));

                    GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop15)).IsNotAbstract();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop15 not to be abstract, but it is.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass2.Prop15));

                    GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop15)).IsNotAbstract(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop15 not to be abstract because {reason}, but it is.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass2.Prop15));

                    GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop15)).IsNotAbstract(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop15 not to be abstract because {expectedReason}, but it is.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClass2.Prop13));

                GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop13)).IsNotAbstract();
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop6 not to be abstract, but it is.");
                expected.AppendLine("Expected Prop15 not to be abstract, but it is.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass2>() // DummyClassBase
                        .Including(nameof(DummyClass2.Prop6), nameof(DummyClass2.Prop15))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsNotAbstract();
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsVirtual : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsVirtual();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsVirtual();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop1 to be virtual, but it isn't.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsVirtual(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 to be virtual because {reason}, but it isn't.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsVirtual(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 to be virtual because {expectedReason}, but it isn't.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClass1.Prop7));

                GetPropertyAssertions<DummyClass2>(nameof(DummyClass1.Prop7)).IsVirtual();
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop1 to be virtual, but it isn't.");
                expected.AppendLine("Expected Prop3 to be virtual, but it isn't.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass2>()
                        .Including(nameof(DummyClass2.Prop1), nameof(DummyClass2.Prop3))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsVirtual();
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsNotVirtual : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsNotVirtual();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass2.Prop15));

                    GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop15)).IsNotVirtual();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop15 not to be virtual, but it is.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass2.Prop15));

                    GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop15)).IsNotVirtual(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop15 not to be virtual because {reason}, but it is.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass2.Prop15));

                    GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop15)).IsNotVirtual(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop15 not to be virtual because {expectedReason}, but it is.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClass2.Prop13));

                GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop13)).IsNotVirtual();
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop6 not to be virtual, but it is.");
                expected.AppendLine("Expected Prop15 not to be virtual, but it is.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass2>()
                        .Including(nameof(DummyClass2.Prop6), nameof(DummyClass2.Prop15))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsNotVirtual();
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

#if NET8_0_OR_GREATER

        public class HasRequiredModifier : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.HasRequiredModifier();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).HasRequiredModifier();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop1 to have a required modifier, but it doesn't.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).HasRequiredModifier(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 to have a required modifier because {reason}, but it doesn't.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).HasRequiredModifier(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 to have a required modifier because {expectedReason}, but it doesn't.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClass1.Prop7));

                GetPropertyAssertions<DummyClass2>(nameof(DummyClass1.Prop7)).HasRequiredModifier();
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop1 to have a required modifier, but it doesn't.");
                expected.AppendLine("Expected Prop3 to have a required modifier, but it doesn't.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass2>()
                        .Including(nameof(DummyClass2.Prop1), nameof(DummyClass2.Prop3))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.HasRequiredModifier();
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class HasNoRequiredModifier : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.HasNoRequiredModifier();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass2.Prop7));

                    GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop7)).HasNoRequiredModifier();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop7 not to have a required modifier, but it does.");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass2.Prop7));

                    GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop7)).HasNoRequiredModifier(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop7 not to have a required modifier because {reason}, but it does.");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass2.Prop7));

                    GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop7)).HasNoRequiredModifier(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop7 not to have a required modifier because {expectedReason}, but it does.");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClass2.Prop13));

                GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop13)).HasNoRequiredModifier();
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop7 not to have a required modifier, but it does.");
                expected.AppendLine("Expected Prop12 not to have a required modifier, but it does.");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass2>()
                        .Including(nameof(DummyClass2.Prop7), nameof(DummyClass2.Prop12))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.HasNoRequiredModifier();
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

#endif

        public class IsOfType : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsOfType<bool>();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsOfType<decimal>();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop1 to be of type \"System.Decimal\", but found \"System.Int32\".");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsOfType<decimal>(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 to be of type \"System.Decimal\" because {reason}, but found \"System.Int32\".");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsOfType<decimal>(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 to be of type \"System.Decimal\" because {expectedReason}, but found \"System.Int32\".");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClass1.Prop7));

                GetPropertyAssertions<DummyClass2>(nameof(DummyClass1.Prop7)).IsOfType<int?>();
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop4 to be of type \"System.Boolean\", but found \"System.String\".");
                expected.AppendLine("Expected Prop14 to be of type \"System.Boolean\", but found \"System.String\".");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass2>()
                        .Including(nameof(DummyClass2.Prop4), nameof(DummyClass2.Prop14))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsOfType<bool>();
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsOfType_NullabilityInfo : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Throw_When_NullabilityInfoAssertions_Null()
            {
                Invoking(() =>
                {
                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsOfType<decimal>((Action<NullabilityInfoAssertions>) null!);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("nullabilityAssertions");
            }

            [Fact]
            public void Should_Invoke_Action()
            {
                var actual = false;

                using var _ = new AssertionScope(nameof(DummyClass1.Prop7));

                GetPropertyAssertions<DummyClass2>(nameof(DummyClass1.Prop7)).IsOfType<int?>(_ => actual = true);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsOfType<bool>(_ => { });
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsOfType<decimal>(_ => { });
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop1 to be of type \"System.Decimal\", but found \"System.Int32\".");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsOfType<decimal>(_ => { }, reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 to be of type \"System.Decimal\" because {reason}, but found \"System.Int32\".");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsOfType<decimal>(_ => { }, reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 to be of type \"System.Decimal\" because {expectedReason}, but found \"System.Int32\".");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClass1.Prop7));

                GetPropertyAssertions<DummyClass2>(nameof(DummyClass1.Prop7)).IsOfType<int?>(_ => { });
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop4 to be of type \"System.Boolean\", but found \"System.String\".");
                expected.AppendLine("Expected Prop14 to be of type \"System.Boolean\", but found \"System.String\".");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass2>()
                        .Including(nameof(DummyClass2.Prop4), nameof(DummyClass2.Prop14))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsOfType<bool>(_ => { });
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsAssignableTo : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsAssignableTo<bool>();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsAssignableTo<DummyClassBase>();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 to be assignable to type \"{typeof(DummyClassBase).GetFriendlyName(true)}\", but found \"System.Int32\".");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsAssignableTo<DummyClassBase>(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 to be assignable to type \"{typeof(DummyClassBase).GetFriendlyName(true)}\" because {reason}, but found \"System.Int32\".");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsAssignableTo<DummyClassBase>(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 to be assignable to type \"{typeof(DummyClassBase).GetFriendlyName(true)}\" because {expectedReason}, but found \"System.Int32\".");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClass1.Prop5));

                GetPropertyAssertions<DummyClass2>(nameof(DummyClass1.Prop5)).IsAssignableTo<DummyClassBase>();
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine($"Expected Prop4 to be assignable to type \"{typeof(DummyClassBase).GetFriendlyName(true)}\", but found \"System.String\".");
                expected.AppendLine($"Expected Prop14 to be assignable to type \"{typeof(DummyClassBase).GetFriendlyName(true)}\", but found \"System.String\".");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass2>()
                        .Including(nameof(DummyClass2.Prop4), nameof(DummyClass2.Prop14))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsAssignableTo<DummyClassBase>();
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsAssignableFrom : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsAssignableFrom<bool>();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsAssignableFrom<DummyClassBase>();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 to be assignable from type \"{typeof(DummyClassBase).GetFriendlyName(true)}\", but found \"System.Int32\".");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsAssignableFrom<DummyClassBase>(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 to be assignable from type \"{typeof(DummyClassBase).GetFriendlyName(true)}\" because {reason}, but found \"System.Int32\".");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop1));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop1)).IsAssignableFrom<DummyClassBase>(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop1 to be assignable from type \"{typeof(DummyClassBase).GetFriendlyName(true)}\" because {expectedReason}, but found \"System.Int32\".");
            }

            [Fact]
            public void Should_Not_Fail()
            {
                // Not required for this test
                //using var _ = new AssertionScope("Prop9");

                GetPropertyAssertions<DummyClass2>("Prop9").IsAssignableFrom<DummyClass1>();
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine($"Expected Prop4 to be assignable from type \"{typeof(DummyClassBase).GetFriendlyName(true)}\", but found \"System.String\".");
                expected.AppendLine($"Expected Prop14 to be assignable from type \"{typeof(DummyClassBase).GetFriendlyName(true)}\", but found \"System.String\".");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass2>()
                        .Including(nameof(DummyClass2.Prop4), nameof(DummyClass2.Prop14))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsAssignableFrom<DummyClassBase>();
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsNullable : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsNullable();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail_Reference()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop4));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop4)).IsNullable();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop4 to be \"Nullable\", but it is \"NotNull\".");
            }

            [Fact]
            public void Should_Fail_Struct()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop15));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop15)).IsNullable();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop15 to be \"Nullable\", but it is \"NotNull\".");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop15));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop15)).IsNullable(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop15 to be \"Nullable\" because {reason}, but it is \"NotNull\".");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop15));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop15)).IsNullable(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop15 to be \"Nullable\" because {expectedReason}, but it is \"NotNull\".");
            }

            [Fact]
            public void Should_Not_Fail_Nullable_Reference()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClass2.Prop6));

                GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop6)).IsNullable();
            }

            [Fact]
            public void Should_Not_Fail_Nullable_Struct()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClass2.Prop3));

                GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop3)).IsNullable();
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop1 to be \"Nullable\", but it is \"NotNull\".");
                expected.AppendLine("Expected Prop4 to be \"Nullable\", but it is \"NotNull\".");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass2>()
                        .Including(nameof(DummyClass1.Prop1), nameof(DummyClass1.Prop4))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsNullable();
                        });
                })
                    .Should()
                    .Throw<XunitException>()
                    .WithMessage(expected.ToString());
            }
        }

        public class IsNotNullable : PropertyInfoAssertionsFixture
        {
            [Fact]
            public void Should_Fail_When_Subject_Null()
            {
                Invoking(() =>
                {
                    var assertions = new PropertyInfoAssertions(null!);

                    assertions.IsNotNullable();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Cannot validate property when its <PropertyInfo> is <null>.");
            }

            [Fact]
            public void Should_Fail_Reference()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop6));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop6)).IsNotNullable();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop6 to be \"NotNull\", but it is \"Nullable\".");
            }

            [Fact]
            public void Should_Fail_Struct()
            {
                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop3));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop3)).IsNotNullable();
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected Prop3 to be \"NotNull\", but it is \"Nullable\".");
            }

            [Fact]
            public void Should_Fail_With_Reason()
            {
                var reason = Create<string>();

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop6));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop6)).IsNotNullable(reason);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop6 to be \"NotNull\" because {reason}, but it is \"Nullable\".");
            }

            [Fact]
            public void Should_Fail_With_Reason_Args()
            {
                var reason = $"{Create<string>()} {0}";
                var reasonArgs = Create<string>();
                var expectedReason = string.Format(reason, reasonArgs);

                Invoking(() =>
                {
                    using var _ = new AssertionScope(nameof(DummyClass1.Prop6));

                    GetPropertyAssertions<DummyClass1>(nameof(DummyClass1.Prop6)).IsNotNullable(reason, reasonArgs);
                })
                .Should()
                .Throw<XunitException>()
                .WithMessage($"Expected Prop6 to be \"NotNull\" because {expectedReason}, but it is \"Nullable\".");
            }

            [Fact]
            public void Should_Not_Fail_Reference()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClass2.Prop4));

                GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop4)).IsNotNullable();
            }

            [Fact]
            public void Should_Not_Fail_Struct()
            {
                // Not required for this test
                //using var _ = new AssertionScope(nameof(DummyClass2.Prop10));

                GetPropertyAssertions<DummyClass2>(nameof(DummyClass2.Prop10)).IsNotNullable();
            }

            [Fact]
            public void Should_Fail_Multiple()
            {
                var expected = new StringBuilder();

                expected.AppendLine("Expected Prop6 to be \"NotNull\", but it is \"Nullable\".");
                expected.AppendLine("Expected Prop3 to be \"NotNull\", but it is \"Nullable\".");

                Invoking(() =>
                {
                    Properties
                        .For<DummyClass2>()
                        .Including(nameof(DummyClass1.Prop3), nameof(DummyClass1.Prop6))
                        .Should()
                        .BeDefinedAs(property =>
                        {
                            property.IsNotNullable();
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
    }
}