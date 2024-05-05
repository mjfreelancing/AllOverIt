using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Helpers.PropertyNavigation;
using FluentAssertions;

namespace AllOverIt.Tests.Helpers.PropertyNavigation
{
    public class PropertyNodesFixture : FixtureBase
    {
        private class DummyObject1
        {
        }

        private class DummyObject2
        {
            public int Id { get; set; }
        }

        public class Constructor : PropertyNodesFixture
        {
            [Fact]
            public void Should_Contain_Object_Type()
            {
                var actual = new PropertyNodes<DummyObject1>();

                actual.ObjectType.Should().Be(typeof(DummyObject1));
            }

            [Fact]
            public void Should_Contain_No_Nodes()
            {
                var actual = new PropertyNodes<DummyObject1>();

                actual.Nodes.Should().BeEmpty();
            }
        }

        public class Constructor_Nodes : PropertyNodesFixture
        {
            [Fact]
            public void Should_Return_Nodes()
            {
                var memberExpression = ExpressionExtensions.GetParameterPropertyOrFieldExpression<DummyObject2, int>(subject => subject.Id);

                PropertyNode[] expected =
                [
                    new PropertyNode { Expression = memberExpression },
                    new PropertyNode { Expression = memberExpression },
                    new PropertyNode { Expression = memberExpression }
                ];

                var actual = new PropertyNodes<DummyObject1>(expected);

                expected.Should().BeEquivalentTo(actual.Nodes);
            }
        }
    }
}