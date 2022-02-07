using System;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Formatters.Objects;
using FluentAssertions;
using Xunit;

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
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() =>
                    {
                        _ = new ObjectPropertyParent(string.Empty, Create<string>(), Create<int>());
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Theory]
            [InlineData("name", "value", null)]
            [InlineData("otherName", "otherValue", 1)]
            public void Should_Set_Members(string name, object value, int? index)
            {
                var actual = new ObjectPropertyParent(name, value, index);

                actual.Name.Should().Be(name);
                actual.Value.Should().Be(value);
                actual.Index.Should().Be(index);
            }
        }
    }
}