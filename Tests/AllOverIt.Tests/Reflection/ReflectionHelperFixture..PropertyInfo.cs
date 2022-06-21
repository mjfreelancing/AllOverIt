using AllOverIt.Fixture;
using AllOverIt.Reflection;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Reflection
{
    public partial class ReflectionHelperFixture : FixtureBase
    {
        public class GetPropertyInfo_Property : ReflectionHelperFixture
        {
            [Fact]
            public void Should_Get_Property_In_Super()
            {
                var actual = (object)ReflectionHelper.GetPropertyInfo<DummySuperClass>("Prop3");

                var expected = new
                {
                    Name = "Prop3",
                    PropertyType = typeof(double)
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Get_Property_In_Base()
            {
                var actual = (object)ReflectionHelper.GetPropertyInfo<DummySuperClass>("Prop1");

                var expected = new
                {
                    Name = "Prop1",
                    PropertyType = typeof(int)
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Not_Find_Property()
            {
                var actual = (object)ReflectionHelper.GetPropertyInfo<DummySuperClass>("PropXYZ");

                actual.Should().BeNull();
            }
        }

        public class GetPropertyInfo_Bindings : ReflectionHelperFixture
        {
            [Fact]
            public void Should_Use_Default_Binding_Not_Declared_Only()
            {
                var actual = ReflectionHelper.GetPropertyInfo<DummySuperClass>();

                var expected = new[]
                {
                    new
                    {
                        Name = "Prop1",
                        PropertyType = typeof(int)
                    },
                    new
                    {
                        Name = "Prop2",
                        PropertyType = typeof(string)
                    },
                    new
                    {
                        Name = "Prop3",
                        PropertyType = typeof(double)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Use_Default_Binding_Declared_Only()
            {
                var actual = ReflectionHelper.GetPropertyInfo<DummySuperClass>(BindingOptions.Default, true);

                var expected = new[]
                {
                    new
                    {
                        Name = "Prop3",
                        PropertyType = typeof(double)
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Include_Private_Property()
            {
                var binding = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility;

                var actual = ReflectionHelper.GetPropertyInfo<DummySuperClass>(binding, false);

                actual.Single(item => item.Name == "Prop4").Should().NotBeNull();
            }
        }
    }
}
