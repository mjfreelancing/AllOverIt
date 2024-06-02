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
            // Not testing the full list of names since there are duplicate values, such as DefaultMethod = MethodGet
            [Fact]
            public void Should_Have_Expected_Count()
            {
                EnumHelper.GetEnumValues<BindingOptions>().Should().HaveCount(21);
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
            [InlineData(BindingOptions.GetMethod, 512)]
            [InlineData(BindingOptions.SetMethod, 1024)]
            public void Should_Have_Expected_Base_Value(BindingOptions option, int expected)
            {
                ((int) option).Should().Be(expected);
            }

            [Theory]
            [InlineData(BindingOptions.DefaultScope, (int) (BindingOptions.Static | BindingOptions.Instance))]
            [InlineData(BindingOptions.DefaultAccessor, (int) (BindingOptions.Abstract | BindingOptions.Virtual | BindingOptions.NonVirtual))]
            [InlineData(BindingOptions.DefaultVisibility, (int) (BindingOptions.Public))]
            [InlineData(BindingOptions.DefaultMethod, (int) (BindingOptions.GetMethod))]
            [InlineData(BindingOptions.AllScope, (int) (BindingOptions.Static | BindingOptions.Instance))]
            [InlineData(BindingOptions.AllAccessor, (int) (BindingOptions.Abstract | BindingOptions.Virtual | BindingOptions.NonVirtual))]
            [InlineData(BindingOptions.AllVisibility, (int) (BindingOptions.DefaultVisibility | BindingOptions.Private | BindingOptions.Protected | BindingOptions.Internal))]
            [InlineData(BindingOptions.AllMethod, (int) (BindingOptions.GetMethod | BindingOptions.SetMethod))]
            [InlineData(BindingOptions.Default, (int) (BindingOptions.DefaultScope | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility | BindingOptions.GetMethod))]
            [InlineData(BindingOptions.All, (int) (BindingOptions.AllScope | BindingOptions.AllAccessor | BindingOptions.AllVisibility | BindingOptions.AllMethod))]
            public void Should_Have_Expected_Composite_Value(BindingOptions option, int expected)
            {
                ((int) option).Should().Be(expected);
            }
        }
    }
}