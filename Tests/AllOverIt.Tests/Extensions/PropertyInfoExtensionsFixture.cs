using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Reflection;
using Xunit;

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
            private int Prop3 { get; set; }
            protected int Prop4 { get; set; }
            public string Prop5 { get; set; }
            public static double Prop6 { get; set; }
            public virtual bool Prop7 { get; set; }
        }

        public class IsAbstract : PropertyInfoExtensionsFixture
        {
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

        private static PropertyInfo GetPropertyInfo<TType>(string name)
        {
            // TypeInfoExtensions has its own set of tests so happy to use this to keep these tests simple
            return TypeInfoExtensions.GetPropertyInfo(typeof(TType).GetTypeInfo(), name);
        }
    }
}