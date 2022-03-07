using System;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Mapping;
using AllOverIt.Reflection;
using FluentAssertions;
using Xunit;
using ObjectExtensions = AllOverIt.Mapping.Extensions.ObjectExtensions;

namespace AllOverIt.Tests.Mapping.Extensions
{
    public class ObjectExtensionsFixture : FixtureBase
    {
        private class DummySource
        {
            public int Prop1 { get; set; }
            private int Prop2 { get; set; }
            public string Prop3 { get; set; }
            internal int Prop4 { get; set; }
            public int? Prop5 { get; set; }
            public int Prop6 { get; set; }

            public DummySource()
            {
                Prop2 = 10;
            }

            public int GetProp2()
            {
                return Prop2;
            }
        }

        private class DummyTarget
        {
            public int Prop1 { get; set; }
            private int Prop2 { get; set; }
            public string Prop3 { get; set; }
            internal int Prop4 { get; set; }
            public int Prop5 { get; set; }
            public int? Prop6 { get; set; }
        }

        private readonly ObjectMapperOptions _options;
        private readonly DummySource _source;

        public ObjectExtensionsFixture()
        {
            _options = new ObjectMapperOptions();
            _source = Create<DummySource>();
        }

        public class MapTo_Options : ObjectExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Source_Null()
            {
                Invoking(() =>
                    {
                        ObjectExtensions.MapTo<DummyTarget>(null, _options);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("source");
            }

            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() =>
                    {
                        ObjectExtensions.MapTo<DummyTarget>(new { }, null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("options");
            }

            [Fact]
            public void Should_Return_Target_Type()
            {
                var actual = ObjectExtensions.MapTo<DummyTarget>(new { }, _options);

                actual.Should().BeOfType<DummyTarget>();
            }

            [Fact]
            public void Should_Map_Using_Default_Bindings()
            {
                var actual = ObjectExtensions.MapTo<DummyTarget>(_source, _options);

                actual.Should().BeEquivalentTo(new
                {
                    _source.Prop1,
                    Prop2 = default(int),       // is private
                    _source.Prop3,
                    _source.Prop4,
                    _source.Prop5,
                    _source.Prop6
                });
            }

            [Fact]
            public void Should_Map_Using_Filter()
            {
                _options.Filter = propInfo => propInfo.Name != nameof(DummySource.Prop1);

                var actual = ObjectExtensions.MapTo<DummyTarget>(_source, _options);

                actual.Should().BeEquivalentTo(new
                {
                    Prop1 = default(int),       // excluded by the filter
                    Prop2 = default(int),       // is private
                    _source.Prop3,
                    _source.Prop4,
                    _source.Prop5,
                    _source.Prop6
                });
            }

            [Fact]
            public void Should_Map_Private_Properties()
            {
                _options.Binding = BindingOptions.Public | BindingOptions.Private;

                var actual = ObjectExtensions.MapTo<DummyTarget>(_source, _options);

                actual.Should().BeEquivalentTo(new
                {
                    _source.Prop1,
                    Prop2 = _source.GetProp2(),
                    _source.Prop3,
                    Prop4 = default(int),       // is internal,
                    _source.Prop5,
                    _source.Prop6
                });
            }

            [Fact]
            public void Should_Map_Internal_Properties()
            {
                _options.Binding = BindingOptions.Public | BindingOptions.Internal;

                var actual = ObjectExtensions.MapTo<DummyTarget>(_source, _options);

                actual.Should().BeEquivalentTo(new
                {
                    _source.Prop1,
                    Prop2 = default(int),       // is private
                    _source.Prop3,
                    _source.Prop4,
                    _source.Prop5,
                    _source.Prop6
                });
            }
        }
    }
}