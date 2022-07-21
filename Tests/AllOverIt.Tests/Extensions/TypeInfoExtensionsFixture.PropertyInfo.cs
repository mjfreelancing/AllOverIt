using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Reflection;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
    public partial class TypeInfoExtensionsFixture : FixtureBase
    {
        private class PropertyBaseClass
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
            public virtual double Prop3 { get; set; }
        }

        private class PropertySuperClass : PropertyBaseClass
        {
            public override double Prop3 { get; set; }
            public long Prop4 { get; set; }
        }

        public class GetPropertyInfo_All : TypeInfoExtensionsFixture
        {
            [Fact]
            public void Should_Get_All_Properties_Of_Super()
            {
                var typeInfo = typeof(PropertySuperClass).GetTypeInfo();

                var actual = TypeInfoExtensions.GetPropertyInfo(typeInfo, false);

                var expected = new[]
                {
                    new {Name = "Prop1", PropertyType = typeof(int)},
                    new {Name = "Prop2", PropertyType = typeof(string)},
                    new {Name = "Prop3", PropertyType = typeof(double)},
                    new {Name = "Prop4", PropertyType = typeof(long)}
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_All_Properties_Of_Base()
            {
                var typeInfo = typeof(PropertyBaseClass).GetTypeInfo();

                var actual = TypeInfoExtensions.GetPropertyInfo(typeInfo, false);

                var expected = new[]
                {
                    new {Name = "Prop1", PropertyType = typeof(int)},
                    new {Name = "Prop2", PropertyType = typeof(string)},
                    new {Name = "Prop3", PropertyType = typeof(double)}
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_Declared_Properties_Only()
            {
                var typeInfo = typeof(PropertySuperClass).GetTypeInfo();

                var actual = TypeInfoExtensions.GetPropertyInfo(typeInfo, true);

                var expected = new[]
                {
                    new {Name = "Prop3", PropertyType = typeof(double)},
                    new {Name = "Prop4", PropertyType = typeof(long)}
                };

                actual.Should().BeEquivalentTo(expected);
            }
        }

        public class GetPropertyInfo_Property : TypeInfoExtensionsFixture
        {
            [Fact]
            public void Should_Get_Property_Of_Super()
            {
                var typeInfo = typeof(PropertySuperClass).GetTypeInfo();

                // cast is required for BeEquivalentTo()
                var actual = (object)TypeInfoExtensions.GetPropertyInfo(typeInfo, "Prop4");

                var expected = new {Name = "Prop4", PropertyType = typeof(long)};

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_Property_Of_Base()
            {
                var typeInfo = typeof(PropertyBaseClass).GetTypeInfo();

                var actual = (object)TypeInfoExtensions.GetPropertyInfo(typeInfo, "Prop1");

                var expected = new {Name = "Prop1", PropertyType = typeof(int)};

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Not_Get_Property()
            {
                var typeInfo = typeof(PropertySuperClass).GetTypeInfo();

                var actual = (object)TypeInfoExtensions.GetPropertyInfo(typeInfo, "PropXYZ");

                actual.Should().BeNull();
            }
        }
    }
}