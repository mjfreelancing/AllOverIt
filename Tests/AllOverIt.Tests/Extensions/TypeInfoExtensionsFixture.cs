using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Reflection;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
    public class TypeInfoExtensionsFixture : FixtureBase
    {
        private class DummyBaseClass
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
            public virtual double Prop3 { get; set; }
        }

        private class DummySuperClass : DummyBaseClass
        {
            public override double Prop3 { get; set; }
            public long Prop4 { get; set; }
        }

        public class GetPropertyInfo_All : TypeInfoExtensionsFixture
        {
            [Fact]
            public void Should_Get_All_Properties_Of_Super()
            {
                var typeInfo = typeof(DummySuperClass).GetTypeInfo();

                var actual = TypeInfoExtensions.GetPropertyInfo(typeInfo, false);

                actual.Should().BeEquivalentTo(
                    new {Name = "Prop1", PropertyType = typeof(int)},
                    new {Name = "Prop2", PropertyType = typeof(string)},
                    new {Name = "Prop3", PropertyType = typeof(double)},
                    new {Name = "Prop4", PropertyType = typeof(long)}
                );
            }

            [Fact]
            public void Should_Get_All_Properties_Of_Base()
            {
                var typeInfo = typeof(DummyBaseClass).GetTypeInfo();

                var actual = TypeInfoExtensions.GetPropertyInfo(typeInfo, false);

                actual.Should().BeEquivalentTo(
                    new {Name = "Prop1", PropertyType = typeof(int)},
                    new {Name = "Prop2", PropertyType = typeof(string)},
                    new {Name = "Prop3", PropertyType = typeof(double)}
                );
            }

            [Fact]
            public void Should_Get_Declared_Properties_Only()
            {
                var typeInfo = typeof(DummySuperClass).GetTypeInfo();

                var actual = TypeInfoExtensions.GetPropertyInfo(typeInfo, true);

                actual.Should().BeEquivalentTo(
                    new {Name = "Prop3", PropertyType = typeof(double)},
                    new {Name = "Prop4", PropertyType = typeof(long)}
                );
            }
        }

        public class GetPropertyInfo_Property : TypeInfoExtensionsFixture
        {
            [Fact]
            public void Should_Get_Property_Of_Super()
            {
                var typeInfo = typeof(DummySuperClass).GetTypeInfo();

                // cast is required for BeEquivalentTo()
                var actual = (object)TypeInfoExtensions.GetPropertyInfo(typeInfo, "Prop4");

                actual.Should().BeEquivalentTo(
                    new {Name = "Prop4", PropertyType = typeof(long)});
            }

            [Fact]
            public void Should_Get_Property_Of_Base()
            {
                var typeInfo = typeof(DummyBaseClass).GetTypeInfo();

                var actual = (object)TypeInfoExtensions.GetPropertyInfo(typeInfo, "Prop1");

                actual.Should().BeEquivalentTo(
                    new {Name = "Prop1", PropertyType = typeof(int)});
            }

            [Fact]
            public void Should_Not_Get_Property()
            {
                var typeInfo = typeof(DummySuperClass).GetTypeInfo();

                var actual = (object)TypeInfoExtensions.GetPropertyInfo(typeInfo, "PropXYZ");

                actual.Should().BeNull();
            }
        }
    }
}