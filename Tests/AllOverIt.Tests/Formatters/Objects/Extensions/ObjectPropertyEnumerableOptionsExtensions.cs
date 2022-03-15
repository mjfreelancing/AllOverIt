using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Formatters.Objects;
using AllOverIt.Helpers.PropertyNavigation;
using FluentAssertions;
using System;
using System.Collections.Generic;
using AllOverIt.Exceptions;
using AllOverIt.Formatters.Objects.Extensions;
using AllOverIt.Helpers.PropertyNavigation.Extensions;
using Xunit;

namespace AllOverIt.Tests.Formatters.Objects.Extensions
{
    public class ObjectPropertyEnumerableOptionsExtensionsFixture : FixtureBase
    {
        private class DummyObject
        {
            public class ChildObject
            {
                public int Prop4 { get; }
            }

            public bool Prop1 { get; }
            public IEnumerable<ChildObject> Prop3 { get; }
            public ChildObject Prop5 { get; }
        }

        public class SetAutoCollatedPaths : ObjectPropertyEnumerableOptionsExtensionsFixture
        {
            private readonly ObjectPropertyEnumerableOptions _options;

            public SetAutoCollatedPaths()
            {
                _options = new ObjectPropertyEnumerableOptions();
            }

            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() =>
                    {
                        ObjectPropertyEnumerableOptionsExtensions.SetAutoCollatedPaths((ObjectPropertyEnumerableOptions) null, new PropertyNodes<DummyObject>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("options");
            }

            [Fact]
            public void Should_Throw_When_Nodes_Null()
            {
                Invoking(() =>
                    {
                        ObjectPropertyEnumerableOptionsExtensions.SetAutoCollatedPaths(new ObjectPropertyEnumerableOptions(), null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("propertyNodes");
            }

            [Fact]
            public void Should_Throw_When_Nodes_Empty()
            {
                Invoking(() =>
                    {
                        ObjectPropertyEnumerableOptionsExtensions.SetAutoCollatedPaths(_options);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("propertyNodes");
            }

            [Fact]
            public void Should_Throw_When_Leaf_Is_Class()
            {
                Invoking(() =>
                    {
                        var nodes = PropertyNavigator
                            .For<DummyObject>()
                            .Navigate(model => model.Prop3);

                        ObjectPropertyEnumerableOptionsExtensions.SetAutoCollatedPaths(_options, nodes);
                    })
                    .Should()
                    .Throw<ObjectPropertyFilterException>()
                    .WithMessage("The leaf property on path 'Prop3' cannot be a class type (ChildObject).");
            }

            [Fact]
            public void Should_Set_Expected_FullPath()
            {
                var nodes = PropertyNavigator
                    .For<DummyObject>()
                    .Navigate(model => model.Prop3)
                    .Navigate(model => model.Prop4);

                ObjectPropertyEnumerableOptionsExtensions.SetAutoCollatedPaths(_options, nodes);

                var expected = new[] {"Prop3.Prop4"};

                expected.Should().BeEquivalentTo(_options.AutoCollatedPaths);
            }

            [Fact]
            public void Should_Set_Multiple_FullPath()
            {
                var nodes1 = PropertyNavigator
                    .For<DummyObject>()
                    .Navigate(model => model.Prop3)
                    .Navigate(model => model.Prop4);

                var nodes2 = PropertyNavigator
                    .For<DummyObject>()
                    .Navigate(model => model.Prop5.Prop4);

                var nodes3 = PropertyNavigator
                    .For<DummyObject>()
                    .Navigate(model => model.Prop1);

                ObjectPropertyEnumerableOptionsExtensions.SetAutoCollatedPaths(_options, nodes1, nodes2, nodes3);

                var expected = new[] {"Prop3.Prop4", "Prop5.Prop4", "Prop1"};

                expected.Should().BeEquivalentTo(_options.AutoCollatedPaths);
            }
        }
    }
}