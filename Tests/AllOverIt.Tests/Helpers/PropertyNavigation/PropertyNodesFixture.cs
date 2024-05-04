using AllOverIt.Fixture;
using AllOverIt.Helpers.PropertyNavigation;
using FluentAssertions;

namespace AllOverIt.Tests.Helpers.PropertyNavigation
{
    public class PropertyNodesFixture : FixtureBase
    {
        private class DummyObject
        {
        }

        public class Constructor : PropertyNodesFixture
        {
            [Fact]
            public void Should_Contain_Object_Type()
            {
                var actual = new PropertyNodes<DummyObject>();

                actual.ObjectType.Should().Be(typeof(DummyObject));
            }

            [Fact]
            public void Should_Contain_No_Nodes()
            {
                var actual = new PropertyNodes<DummyObject>();

                actual.Nodes.Should().BeEmpty();
            }
        }

        public class Constructor_Nodes : PropertyNodesFixture
        {
            [Fact]
            public void Should_Return_Nodes()
            {
                var expected = new[] { new PropertyNode(), new PropertyNode(), new PropertyNode() };

                var actual = new PropertyNodes<DummyObject>(expected);

                expected.Should().BeEquivalentTo(actual.Nodes);
            }
        }
    }
}