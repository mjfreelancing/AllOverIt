using System.Linq;
using System.Threading.Tasks;
using AllOverIt.Fixture;
using AllOverIt.Helpers;
using AllOverIt.Reflection;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Helpers
{
    public class ObjectPropertySerializerOptionsFixture : FixtureBase
    {
        private readonly ObjectPropertySerializerOptions _options;

        public ObjectPropertySerializerOptionsFixture()
        {
            _options = new ObjectPropertySerializerOptions();
        }

        public class Constructor : ObjectPropertySerializerOptionsFixture
        {
            [Fact]
            public void Should_Contain_Defaults()
            {
                _options
                    .Should()
                    .BeEquivalentTo(
                        new
                        {
                            IgnoredTypes = new[]
                            {
                                typeof(Task),
                                typeof(Task<>)
                            },
                            IncludeNulls = false,
                            IncludeEmptyCollections = false,
                            BindingOptions = BindingOptions.Default,
                            NullValueOutput= "<null>",
                            EmptyValueOutput = "<empty>",
                            Filter = (ObjectPropertyFilter) null
                        }
                    );
            }
        }

        public class ClearIgnoredTypes : ObjectPropertySerializerOptionsFixture
        {
            [Fact]
            public void Should_Clear_Ignored_Types()
            {
                _options.IgnoredTypes.Count().Should().Be(2);

                _options.ClearIgnoredTypes();

                _options.IgnoredTypes.Count().Should().Be(0);
            }
        }

        public class AddIgnoredTypes : ObjectPropertySerializerOptionsFixture
        {
            [Fact]
            public void Should_Add_To_Ignored_Types()
            {
                _options.IgnoredTypes.Count().Should().Be(2);

                _options.AddIgnoredTypes(typeof(string), typeof(int));

                _options.IgnoredTypes
                    .Should()
                    .BeEquivalentTo(new[]
                    {
                        typeof(Task), typeof(Task<>), typeof(string), typeof(int)
                    });
            }
        }
    }
}