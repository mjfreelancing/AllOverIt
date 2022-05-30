using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Helpers.PropertyNavigation;
using AllOverIt.Helpers.PropertyNavigation.Extensions;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Helpers.PropertyNavigation.Extensions
{
    public class PropertyNodesExtensionsFixture : FixtureBase
    {
        private class DummyObject
        {
            public class ChildObject
            {
                public int Prop4 { get; }
            }

            public bool Prop1 { get; }
            public IEnumerable<string> Prop2 { get; }
            public IEnumerable<ChildObject> Prop3 { get; }
            public ChildObject Prop5 { get; }
        }

        private readonly IPropertyNodes<DummyObject> _nodes;
        private readonly Expression<Func<DummyObject, bool>> _expression1;
        private readonly Expression<Func<DummyObject, IEnumerable<string>>> _expression2;

        public PropertyNodesExtensionsFixture()
        {
            _nodes = new PropertyNodes<DummyObject>();
            _expression1 = model => model.Prop1;
            _expression2 = model => model.Prop2;
        }

        public class Navigate : PropertyNodesExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Nodes_null()
            {   
                Invoking(() =>
                {
                    _ = PropertyNodesExtensions.Navigate(null, _expression1);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyNodes");
            }

            [Fact]
            public void Should_Throw_When_Expression_null()
            {
                Invoking(() =>
                    {
                        _ = PropertyNodesExtensions.Navigate(_nodes, (Expression<Func<DummyObject, bool>>)null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Append_Single_Node()
            {
                var expected = _expression1.Body as MemberExpression;

                var actual = PropertyNodesExtensions.Navigate(_nodes, _expression1);

                actual.Nodes.Single().Expression.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Append_Multiple_Nodes()
            {
                Expression<Func<DummyObject, int>> expression = model => model.Prop5.Prop4;
                
                var expected1 = expression.Body as MemberExpression;
                var expected0 = expected1.Expression as MemberExpression;

                var actual = PropertyNodesExtensions.Navigate(_nodes, expression);

                actual.Nodes.Should().HaveCount(2);

                expected0.Member.Name.Should().BeEquivalentTo(actual.Nodes.ElementAt(0).Expression.Member.Name);
                expected1.Member.Name.Should().BeSameAs(actual.Nodes.ElementAt(1).Expression.Member.Name);
            }
        }

        public class Navigate_Enumerable : PropertyNodesExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Nodes_null()
            {
                Invoking(() =>
                    {
                        _ = PropertyNodesExtensions.Navigate(null, _expression2);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyNodes");
            }

            [Fact]
            public void Should_Throw_When_Expression_null()
            {
                Invoking(() =>
                    {
                        _ = PropertyNodesExtensions.Navigate(_nodes, (Expression<Func<DummyObject, IEnumerable<string>>>) null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Append_Single_Node()
            {
                var expected = _expression2.Body as MemberExpression;

                var actual = PropertyNodesExtensions.Navigate(_nodes, _expression2);

                actual.Nodes.Single().Expression.Should().BeSameAs(expected);
            }
        }

        public class Navigate_After_Enumerable : PropertyNodesExtensionsFixture
        {
            [Fact]
            public void Should()
            {
                var nodes = PropertyNavigator
                    .For<DummyObject>()
                    .Navigate(model => model.Prop3)
                    .Navigate(model => model.Prop4)
                    .Nodes;

                nodes.Should().HaveCount(2);

                nodes.ElementAt(0).Expression.Member.Name.Should().Be(nameof(DummyObject.Prop3));
                nodes.ElementAt(1).Expression.Member.Name.Should().Be(nameof(DummyObject.ChildObject.Prop4));
            }
        }

        public class GetFullNodePath : PropertyNodesExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Nodes_null()
            {
                Invoking(() =>
                    {
                        _ = PropertyNodesExtensions.GetFullNodePath(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyNodes");
            }

            [Fact]
            public void Should_Return_First_Level_Property()
            {
                var nodes = PropertyNavigator
                    .For<DummyObject>()
                    .Navigate(model => model.Prop3);

                var actual = PropertyNodesExtensions.GetFullNodePath(nodes);

                actual.Should().Be(nameof(DummyObject.Prop3));
            }

            [Fact]
            public void Should_Return_First_And_Second_Level_Properties()
            {
                var nodes = PropertyNavigator
                    .For<DummyObject>()
                    .Navigate(model => model.Prop3)
                    .Navigate(model => model.Prop4);

                var actual = PropertyNodesExtensions.GetFullNodePath(nodes);

                actual.Should().Be($"{nameof(DummyObject.Prop3)}.{nameof(DummyObject.ChildObject.Prop4)}");
            }

            [Fact]
            public void Should_Dot_Separate_Property_Names()
            {
                var nodes = PropertyNavigator
                    .For<DummyObject>()
                    .Navigate(model => model.Prop3)
                    .Navigate(model => model.Prop4);

                var actual = PropertyNodesExtensions.GetFullNodePath(nodes).Split('.');

                var expected = new[] {nameof(DummyObject.Prop3), nameof(DummyObject.ChildObject.Prop4)};

                expected.Should().BeEquivalentTo(actual);
            }
        }
    }
}