using AllOverIt.Fixture;
using AllOverIt.Helpers;
using AllOverIt.Reflection;
using FluentAssertions;

namespace AllOverIt.Tests.Reflection
{
    public class BindingOptionsFixture : FixtureBase
    {
        public class Values : BindingOptionsFixture
        {

            [Fact]
            public void Should_Have_Available_Values()
            {
                var enumValues = EnumHelper.GetEnumValues<BindingOptions>();

                var expected = new[]
                {
                    BindingOptions.Static, BindingOptions.Instance, BindingOptions.Abstract,
                    BindingOptions.Virtual, BindingOptions.NonVirtual, BindingOptions.Internal, BindingOptions.Private,
                    BindingOptions.Protected, BindingOptions.Public, BindingOptions.DefaultScope,
                    BindingOptions.DefaultAccessor, BindingOptions.DefaultVisibility, BindingOptions.AllScope,
                    BindingOptions.AllAccessor, BindingOptions.AllVisibility, BindingOptions.Default, BindingOptions.All
                };

                expected.Should().BeEquivalentTo(enumValues);
            }

            [Theory]
            [InlineData(BindingOptions.Static, 1)]
            [InlineData(BindingOptions.Instance, 2)]
            [InlineData(BindingOptions.Abstract, 4)]
            [InlineData(BindingOptions.Virtual, 8)]
            [InlineData(BindingOptions.NonVirtual, 16)]
            [InlineData(BindingOptions.Internal, 32)]
            [InlineData(BindingOptions.Private, 64)]
            [InlineData(BindingOptions.Protected, 128)]
            [InlineData(BindingOptions.Public, 256)]
            public void Should_Have_Expected_Base_Value(BindingOptions option, int expected)
            {
                ((int) option).Should().Be(expected);
            }

            [Theory]
            [InlineData(BindingOptions.DefaultScope, (int) (BindingOptions.Static | BindingOptions.Instance))]
            [InlineData(BindingOptions.DefaultAccessor, (int) (BindingOptions.Abstract | BindingOptions.Virtual | BindingOptions.NonVirtual))]
            [InlineData(BindingOptions.DefaultVisibility, (int) (BindingOptions.Public))]
            [InlineData(BindingOptions.AllVisibility, (int) (BindingOptions.DefaultVisibility | BindingOptions.Private | BindingOptions.Protected | BindingOptions.Internal))]
            [InlineData(BindingOptions.Default, (int) (BindingOptions.DefaultScope | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility))]
            [InlineData(BindingOptions.All, (int) (BindingOptions.AllScope | BindingOptions.AllAccessor | BindingOptions.AllVisibility))]
            public void Should_Have_Expected_Composite_Value(BindingOptions option, int expected)
            {
                ((int) option).Should().Be(expected);
            }

            [Theory]
            [InlineData(BindingOptions.AllScope, (int) BindingOptions.DefaultScope)]
            [InlineData(BindingOptions.AllAccessor, (int) BindingOptions.DefaultAccessor)]
            public void Should_Have_Equivalent_Value(BindingOptions option, int expected)
            {
                ((int) option).Should().Be(expected);
            }
        }
    }
}