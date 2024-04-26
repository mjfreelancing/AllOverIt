using AllOverIt.Fixture;
using AllOverIt.Formatters.Objects;
using AllOverIt.Reflection;
using FluentAssertions;
using System.Linq;
using System.Threading.Tasks;

namespace AllOverIt.Tests.Formatters.Objects
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
                var expected = new
                {
                    IgnoredTypes = new[]
                    {
                        typeof(Task),
                        typeof(Task<>)
                    },
                    BindingOptions = BindingOptions.Default,
                    EnumerableOptions = new ObjectPropertyEnumerableOptions(),
                    RootValueOptions = new ObjectPropertyRootValueOptions(),
                    Filter = (ObjectPropertyFilter) null,
                    IncludeNulls = false,
                    IncludeEmptyCollections = false,
                    NullValueOutput = "<null>",
                    EmptyValueOutput = "<empty>"
                };

                expected
                    .Should()
                    .BeEquivalentTo(_options);
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

                var expected = new[]
                {
                    typeof(Task), typeof(Task<>), typeof(string), typeof(int)
                };

                expected
                    .Should()
                    .BeEquivalentTo(_options.IgnoredTypes);
            }
        }
    }
}