using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Helpers.PropertyNavigation;
using AllOverIt.Helpers.PropertyNavigation.Extensions;
using FluentAssertions;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Tests.Helpers.PropertyNavigation.Extensions
{
    public class PropertyNodeExtensionsFixture : FixtureBase
    {
        private class DummyObject
        {
            public bool Prop1 { get; }
        }

        public class Name : PropertyNodeExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                    {
                        PropertyNode node = null;

                        _ = PropertyNodeExtensions.Name(node);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("node");
            }

            [Fact]
            public void Should_Return_Member_Name()
            {
                Expression<Func<DummyObject, bool>> expression = model => model.Prop1;

                var node = new PropertyNode
                {
                    Expression = expression.Body as MemberExpression
                };

                var actual = PropertyNodeExtensions.Name(node);

                actual.Should().Be(nameof(DummyObject.Prop1));
            }
        }
    }
}