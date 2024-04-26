using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
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

        private class DummyClass : DummyClassBase
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

            [Theory]
            [InlineData("Prop1")]
            public void Should_Determine_Base_Is_Abstract(string name)
            {
                var actual = GetPropertyInfo<DummyClassBase>(name).IsAbstract();

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData("Prop1")]
            public void Should_Determine_Derived_Is_Abstract(string name)
            {
                var actual = GetPropertyInfo<DummyClass>(name).IsAbstract();

                actual.Should().BeFalse();
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
                var actual = GetPropertyInfo<DummyClass>(name).IsAbstract();

                actual.Should().BeFalse();
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

            [Theory]
            [InlineData("Prop2")]
            public void Should_Determine_Is_Internal(string name)
            {
                var actual = GetPropertyInfo<DummyClass>(name).IsInternal();

                actual.Should().BeTrue();
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
                var actual = GetPropertyInfo<DummyClass>(name).IsInternal();

                actual.Should().BeFalse();
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

            [Theory]
            [InlineData("Prop3")]
            public void Should_Determine_Is_Private(string name)
            {
                var actual = GetPropertyInfo<DummyClass>(name).IsPrivate();

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
                var actual = GetPropertyInfo<DummyClass>(name).IsPrivate();

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

            [Theory]
            [InlineData("Prop4")]
            public void Should_Determine_Is_Protected(string name)
            {
                var actual = GetPropertyInfo<DummyClass>(name).IsProtected();

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
                var actual = GetPropertyInfo<DummyClass>(name).IsProtected();

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

            [Theory]
            [InlineData("Prop1")]
            [InlineData("Prop5")]
            [InlineData("Prop6")]
            [InlineData("Prop7")]
            public void Should_Determine_Is_Public(string name)
            {
                var actual = GetPropertyInfo<DummyClass>(name).IsPublic();

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData("Prop2")]
            [InlineData("Prop3")]
            [InlineData("Prop4")]
            public void Should_Not_Determine_Is_Public(string name)
            {
                var actual = GetPropertyInfo<DummyClass>(name).IsPublic();

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

            [Theory]
            [InlineData("Prop6")]
            public void Should_Determine_Is_Static(string name)
            {
                var actual = GetPropertyInfo<DummyClass>(name).IsStatic();

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
                var actual = GetPropertyInfo<DummyClass>(name).IsStatic();

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

            [Theory]
            [InlineData("Prop1")]     // it's abstract, so it's virtual
            [InlineData("Prop7")]
            public void Should_Determine_Is_Virtual(string name)
            {
                var actual = GetPropertyInfo<DummyClass>(name).IsVirtual();

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
                var actual = GetPropertyInfo<DummyClass>(name).IsVirtual();

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

        public class CreateMemberAccessLambda : PropertyInfoExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_PropertyInfo_Null()
            {
                Invoking(() =>
                    {
                        PropertyInfoExtensions.CreateMemberAccessLambda<DummyClass, string>(null, Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyInfo");
            }

            [Fact]
            public void Should_Throw_When_ParameterName_Null()
            {
                Invoking(() =>
                    {
                        var propInfo = GetPropertyInfo<DummyClass>(nameof(DummyClass.Prop5));

                        PropertyInfoExtensions.CreateMemberAccessLambda<DummyClass, string>(propInfo, null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("parameterName");
            }

            [Fact]
            public void Should_Throw_When_ParameterName_Empty()
            {
                Invoking(() =>
                    {
                        var propInfo = GetPropertyInfo<DummyClass>(nameof(DummyClass.Prop5));

                        PropertyInfoExtensions.CreateMemberAccessLambda<DummyClass, string>(propInfo, string.Empty);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("parameterName");
            }

            [Fact]
            public void Should_Throw_When_ParameterName_Whitespace()
            {
                Invoking(() =>
                    {
                        var propInfo = GetPropertyInfo<DummyClass>(nameof(DummyClass.Prop5));

                        PropertyInfoExtensions.CreateMemberAccessLambda<DummyClass, string>(propInfo, "  ");
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("parameterName");
            }

            [Fact]
            public void Should_Create_Expression()
            {
                var propInfo = GetPropertyInfo<DummyClass>(nameof(DummyClass.Prop5));

                var lambda = PropertyInfoExtensions.CreateMemberAccessLambda<DummyClass, string>(propInfo, "item");

                var actual = lambda.ToString();

                actual.Should().Be("item => item.Prop5");
            }

            [Fact]
            public void Should_Evaluate_Expression()
            {
                var propInfo = GetPropertyInfo<DummyClass>(nameof(DummyClass.Prop5));

                var lambda = PropertyInfoExtensions.CreateMemberAccessLambda<DummyClass, string>(propInfo, "item");

                var dummy = Create<DummyClass>();
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