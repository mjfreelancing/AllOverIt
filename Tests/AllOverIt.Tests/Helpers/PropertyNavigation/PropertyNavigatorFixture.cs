using AllOverIt.Fixture;
using AllOverIt.Helpers.PropertyNavigation;

namespace AllOverIt.Tests.Helpers.PropertyNavigation
{
    public class PropertyNavigatorFixture : FixtureBase
    {
        private class DummyObject
        {
        }

        public class For : PropertyNavigatorFixture
        {
            [Fact]
            public void Should_Return_PropertyNodes()
            {
                var actual = PropertyNavigator.For<DummyObject>();

                actual.ShouldBeAssignableTo<IPropertyNodes<DummyObject>>();
            }

            [Fact]
            public void Should_Contain_Object_Type()
            {
                var actual = PropertyNavigator.For<DummyObject>();

                actual.ObjectType.ShouldBe(typeof(DummyObject));
            }

            [Fact]
            public void Should_Contain_No_Nodes()
            {
                var actual = PropertyNavigator.For<DummyObject>();

                actual.Nodes.ShouldBeEmpty();
            }
        }
    }
}


