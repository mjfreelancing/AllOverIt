using AllOverIt.Fixture;
using AllOverIt.Helpers;
using AllOverIt.Reflection;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Helpers
{
    public class ObjectPropertySerializationHelperFixture : FixtureBase
    {
        public class IgnoredTypes : ObjectPropertySerializationHelperFixture
        {
            [Fact]
            public void Should_Have_Default_Bindings()
            {
                var expected = BindingOptions.DefaultScope | BindingOptions.Virtual | BindingOptions.NonVirtual | BindingOptions.Public;

                ObjectPropertySerializationHelper.DefaultBindingOptions
                    .Should()
                    .BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Have_Known_Ignored_Types()
            {
                object[] expected =
                {
                    typeof(Task),
                    typeof(Task<>),
                };

                ObjectPropertySerializationHelper.IgnoredTypes
                    .Should()
                    .BeEquivalentTo(expected);
            }
        }
    }
}