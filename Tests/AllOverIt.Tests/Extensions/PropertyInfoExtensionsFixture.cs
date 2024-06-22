using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Types;
using FluentAssertions;
using System.Reflection;
using PropertyInfoExtensions = AllOverIt.Extensions.PropertyInfoExtensions;

namespace AllOverIt.Tests.Extensions
{
    public class PropertyInfoExtensionsFixture : FixtureBase
    {
        private abstract class DummyClassBase
        {
            public abstract int Prop1 { get; set; }
        }

        private class DummyClass1 : DummyClassBase
        {
            public override int Prop1 { get; set; }
            internal int Prop2 { get; set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Part of the test")]
            private int Prop3 { get; set; }

            protected int Prop4 { get; set; }
            public string Prop5 { get; set; }
            public static double Prop6 { get; set; }
            public virtual bool Prop7 { get; set; }
        }

        private abstract class DummyClass2
        {
            public abstract int Prop1 { get; }

            internal int Prop2 { get; private set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Part of the test")]
            private int Prop3 { get; }

            protected int Prop4 { get; private set; }
            public string Prop5 { get; protected set; }
            public static double Prop6 { get; internal set; }
            public virtual bool Prop7 { get; init; }
            public bool Prop8 { set { } }
        }

        private sealed record DummyRecord1
        {
        }

        public class IsAbstract : PropertyInfoExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                    {
                        PropertyInfoExtensions.IsAbstract(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyInfo");
            }

            [Fact]
            public void Should_Return_False_When_No_Accessor()
            {
                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop1)).IsAbstract(PropertyAccessor.Set).Should().BeFalse();
                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop8)).IsAbstract(PropertyAccessor.Get).Should().BeFalse();
            }

            [Fact]
            public void Should_Determine_Base_Is_Abstract()
            {
                GetPropertyInfo<DummyClassBase>(nameof(DummyClassBase.Prop1)).IsAbstract(PropertyAccessor.Get).Should().BeTrue();
                GetPropertyInfo<DummyClassBase>(nameof(DummyClassBase.Prop1)).IsAbstract(PropertyAccessor.Set).Should().BeTrue();
                GetPropertyInfo<DummyClassBase>(nameof(DummyClassBase.Prop1)).IsAbstract(PropertyAccessor.Get | PropertyAccessor.Set).Should().BeTrue();

                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop1)).IsAbstract(PropertyAccessor.Get).Should().BeTrue();
            }

            [Fact]
            public void Should_Determine_Derived_Is_Not_Abstract()
            {
                GetPropertyInfo<DummyClass1>(nameof(DummyClass1.Prop1)).IsAbstract(PropertyAccessor.Get).Should().BeFalse();
            }

            [Theory]
            [InlineData("Prop2")]
            [InlineData("Prop3")]
            [InlineData("Prop4")]
            [InlineData("Prop5")]
            [InlineData("Prop6")]
            [InlineData("Prop7")]
            public void Should_Not_Determine_Is_Abstract(string name)
            {
                GetPropertyInfo<DummyClass1>(name).IsAbstract().Should().BeFalse();
                GetPropertyInfo<DummyClass2>(name).IsAbstract().Should().BeFalse();

                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop1)).IsAbstract(PropertyAccessor.Set).Should().BeFalse();    // no setter
            }
        }

        public class IsInternal : PropertyInfoExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                    {
                        PropertyInfoExtensions.IsInternal(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyInfo");
            }

            [Fact]
            public void Should_Return_False_When_No_Accessor()
            {
                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop1)).IsInternal(PropertyAccessor.Set).Should().BeFalse();
                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop8)).IsInternal(PropertyAccessor.Get).Should().BeFalse();
            }

            [Fact]
            public void Should_Determine_Is_Internal()
            {
                GetPropertyInfo<DummyClass1>(nameof(DummyClass1.Prop2)).IsInternal(PropertyAccessor.Get).Should().BeTrue();
                GetPropertyInfo<DummyClass1>(nameof(DummyClass1.Prop2)).IsInternal(PropertyAccessor.Set).Should().BeTrue();
                GetPropertyInfo<DummyClass1>(nameof(DummyClass1.Prop2)).IsInternal(PropertyAccessor.Get | PropertyAccessor.Set).Should().BeTrue();

                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop2)).IsInternal(PropertyAccessor.Get).Should().BeTrue();
            }

            [Theory]
            [InlineData("Prop1")]
            [InlineData("Prop3")]
            [InlineData("Prop4")]
            [InlineData("Prop5")]
            [InlineData("Prop6")]
            [InlineData("Prop7")]
            public void Should_Not_Determine_Is_Internal(string name)
            {
                GetPropertyInfo<DummyClass1>(name).IsInternal().Should().BeFalse();

                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop2)).IsInternal(PropertyAccessor.Set).Should().BeFalse();
                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop2)).IsInternal(PropertyAccessor.Get | PropertyAccessor.Set).Should().BeFalse();
            }
        }

        public class IsPrivate : PropertyInfoExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                    {
                        PropertyInfoExtensions.IsPrivate(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyInfo");
            }

            [Fact]
            public void Should_Return_False_When_No_Accessor()
            {
                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop1)).IsPrivate(PropertyAccessor.Set).Should().BeFalse();
                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop8)).IsPrivate(PropertyAccessor.Get).Should().BeFalse();
            }

            [Theory]
            [InlineData("Prop3")]
            public void Should_Determine_Is_Private(string name)
            {
                var actual = GetPropertyInfo<DummyClass1>(name).IsPrivate();

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData("Prop1")]
            [InlineData("Prop2")]
            [InlineData("Prop4")]
            [InlineData("Prop5")]
            [InlineData("Prop6")]
            [InlineData("Prop7")]
            public void Should_Not_Determine_Is_Private(string name)
            {
                var actual = GetPropertyInfo<DummyClass1>(name).IsPrivate();

                actual.Should().BeFalse();
            }
        }

        public class IsProtected : PropertyInfoExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                    {
                        PropertyInfoExtensions.IsProtected(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyInfo");
            }

            [Fact]
            public void Should_Return_False_When_No_Accessor()
            {
                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop1)).IsProtected(PropertyAccessor.Set).Should().BeFalse();
                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop8)).IsProtected(PropertyAccessor.Get).Should().BeFalse();
            }

            [Theory]
            [InlineData("Prop4")]
            public void Should_Determine_Is_Protected(string name)
            {
                var actual = GetPropertyInfo<DummyClass1>(name).IsProtected();

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData("Prop1")]
            [InlineData("Prop2")]
            [InlineData("Prop3")]
            [InlineData("Prop5")]
            [InlineData("Prop6")]
            [InlineData("Prop7")]
            public void Should_Not_Determine_Is_Protected(string name)
            {
                var actual = GetPropertyInfo<DummyClass1>(name).IsProtected();

                actual.Should().BeFalse();
            }
        }

        public class IsPublic : PropertyInfoExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                    {
                        PropertyInfoExtensions.IsPublic(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyInfo");
            }

            [Fact]
            public void Should_Return_False_When_No_Accessor()
            {
                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop1)).IsPublic(PropertyAccessor.Set).Should().BeFalse();
                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop8)).IsPublic(PropertyAccessor.Get).Should().BeFalse();
            }

            [Theory]
            [InlineData("Prop1")]
            [InlineData("Prop5")]
            [InlineData("Prop6")]
            [InlineData("Prop7")]
            public void Should_Determine_Is_Public(string name)
            {
                var actual = GetPropertyInfo<DummyClass1>(name).IsPublic();

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData("Prop2")]
            [InlineData("Prop3")]
            [InlineData("Prop4")]
            public void Should_Not_Determine_Is_Public(string name)
            {
                var actual = GetPropertyInfo<DummyClass1>(name).IsPublic();

                actual.Should().BeFalse();
            }
        }

        public class IsStatic : PropertyInfoExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                    {
                        PropertyInfoExtensions.IsStatic(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyInfo");
            }

            [Fact]
            public void Should_Return_False_When_No_Accessor()
            {
                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop1)).IsStatic(PropertyAccessor.Set).Should().BeFalse();
                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop8)).IsStatic(PropertyAccessor.Get).Should().BeFalse();
            }

            [Theory]
            [InlineData("Prop6")]
            public void Should_Determine_Is_Static(string name)
            {
                var actual = GetPropertyInfo<DummyClass1>(name).IsStatic();

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData("Prop1")]
            [InlineData("Prop2")]
            [InlineData("Prop3")]
            [InlineData("Prop4")]
            [InlineData("Prop5")]
            [InlineData("Prop7")]
            public void Should_Not_Determine_Is_Static(string name)
            {
                var actual = GetPropertyInfo<DummyClass1>(name).IsStatic();

                actual.Should().BeFalse();
            }
        }

        public class IsVirtual : PropertyInfoExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                    {
                        PropertyInfoExtensions.IsVirtual(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyInfo");
            }

            [Fact]
            public void Should_Return_False_When_No_Accessor()
            {
                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop1)).IsVirtual(PropertyAccessor.Set).Should().BeFalse();
                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop8)).IsVirtual(PropertyAccessor.Get).Should().BeFalse();
            }

            [Theory]
            [InlineData("Prop1")]     // it's abstract, so it's virtual
            [InlineData("Prop7")]
            public void Should_Determine_Is_Virtual(string name)
            {
                var actual = GetPropertyInfo<DummyClass1>(name).IsVirtual();

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData("Prop2")]
            [InlineData("Prop3")]
            [InlineData("Prop4")]
            [InlineData("Prop5")]
            [InlineData("Prop6")]
            public void Should_Not_Determine_Is_Virtual(string name)
            {
                var actual = GetPropertyInfo<DummyClass1>(name).IsVirtual();

                actual.Should().BeFalse();
            }
        }

        public class IsIndexer : PropertyInfoExtensionsFixture
        {
            private sealed class DummyWithIndexer
            {
                public string this[int key] => string.Empty;
                public int That { get; set; }
            }

            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                    {
                        PropertyInfoExtensions.IsIndexer(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyInfo");
            }

            [Fact]
            public void Should_Determine_Is_Indexer()
            {
                var actual = GetPropertyInfo<DummyWithIndexer>("Item").IsIndexer();

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Determine_Is_Not_Indexer()
            {
                var actual = GetPropertyInfo<DummyWithIndexer>(nameof(DummyWithIndexer.That)).IsIndexer();

                actual.Should().BeFalse();
            }
        }

        public class IsInitOnly : PropertyInfoExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    PropertyInfoExtensions.IsInitOnly(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyInfo");
            }

            [Fact]
            public void Should_Determine_Is_Init()
            {
                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop7))
                    .IsInitOnly()
                    .Should()
                    .BeTrue();
            }

            [Fact]
            public void Should_Determine_Is_Not_Init()
            {
                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop6))
                    .IsInitOnly()
                    .Should()
                    .BeFalse();
            }

            [Fact]
            public void Should_Determine_Is_Not_Init_When_Only_Has_Getter()
            {
                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop1))
                    .IsInitOnly()
                    .Should()
                    .BeFalse();
            }
        }

        public class IsCompilerGenerated : PropertyInfoExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                {
                    PropertyInfoExtensions.IsCompilerGenerated(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyInfo");
            }

            [Fact]
            public void Should_Determine_Is_CompilerGenerated()
            {
                GetPropertyInfo<DummyRecord1>("EqualityContract")
                    .IsCompilerGenerated()
                    .Should()
                    .BeTrue();
            }

            [Fact]
            public void Should_Determine_Is_Not_CompilerGenerated()
            {
                GetPropertyInfo<DummyClass2>(nameof(DummyClass2.Prop6))
                    .IsCompilerGenerated()
                    .Should()
                    .BeFalse();
            }
        }

        public class CreateMemberAccessLambda : PropertyInfoExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                    {
                        PropertyInfoExtensions.CreateMemberAccessLambda<DummyClass1, string>(null, Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyInfo");
            }

            [Fact]
            public void Should_Throw_When_ParameterName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        var propInfo = GetPropertyInfo<DummyClass1>(nameof(DummyClass1.Prop5));

                        PropertyInfoExtensions.CreateMemberAccessLambda<DummyClass1, string>(propInfo, stringValue);
                    }, "parameterName");
            }

            [Fact]
            public void Should_Create_Expression()
            {
                var propInfo = GetPropertyInfo<DummyClass1>(nameof(DummyClass1.Prop5));

                var lambda = PropertyInfoExtensions.CreateMemberAccessLambda<DummyClass1, string>(propInfo, "item");

                var actual = lambda.ToString();

                actual.Should().Be("item => item.Prop5");
            }

            [Fact]
            public void Should_Evaluate_Expression()
            {
                var propInfo = GetPropertyInfo<DummyClass1>(nameof(DummyClass1.Prop5));

                var lambda = PropertyInfoExtensions.CreateMemberAccessLambda<DummyClass1, string>(propInfo, "item");

                var dummy = Create<DummyClass1>();
                var expected = dummy.Prop5;

                var compiled = lambda.Compile();
                var actual = compiled.Invoke(dummy);

                actual.Should().Be(expected);
            }
        }

        private static PropertyInfo GetPropertyInfo<TType>(string name)
        {
            // TypeInfoExtensions has its own set of tests so happy to use this to keep these tests simple
            return TypeInfoExtensions.GetPropertyInfo(typeof(TType).GetTypeInfo(), name);
        }
    }
}