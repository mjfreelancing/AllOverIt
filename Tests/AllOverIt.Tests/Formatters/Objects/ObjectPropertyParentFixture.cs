using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Formatters.Objects;

namespace AllOverIt.Tests.Formatters.Objects
{
    public class ObjectPropertyParentFixture : FixtureBase
    {
        public class Constructor : ObjectPropertyParentFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Name_Null()
            {
                Invoking(() =>
                    {
                        _ = new ObjectPropertyParent(null, Create<string>(), Create<int>());
                    })
                    .ShouldNotThrow();
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() =>
                    {
                        _ = new ObjectPropertyParent(string.Empty, Create<string>(), Create<int>());
                    })
                    .ShouldThrow<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Theory]
            [InlineData("name", "value", null)]
            [InlineData("otherName", "otherValue", 1)]
            public void Should_Set_Members(string name, object value, int? index)
            {
                var actual = new ObjectPropertyParent(name, value, index);

                actual.Name.ShouldBe(name);
                actual.Value.ShouldBe(value);
                actual.Index.ShouldBe(index);
            }
        }
    }
}



