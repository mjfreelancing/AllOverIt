using System;
using System.Collections.Generic;
using System.Linq;
using AllOverIt.Extensions;
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
        private enum DummyEnum
        {
            Value1,
            Value2
        }

        private class DummySource
        {
            public int Prop1 { get; set; }
            private int Prop2 { get; set; }
            public string Prop3 { get; set; }
            internal int Prop4 { get; set; }
            public int? Prop5 { get; set; }
            public int Prop6 { get; set; }
            public string Prop7a { get; set; }
            public int Prop8 { get; private set; }
            public IEnumerable<string> Prop9 { get; set; }
            public IReadOnlyCollection<string> Prop10 { get; set; }
            public IEnumerable<string> Prop11 { get; set; }
            public DummyEnum Prop12 { get; set; }
            public int Prop13 { get; set; }

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
            public string Prop7b { get; set; }
            public int Prop8 { get; private set; }
            public IEnumerable<string> Prop9 { get; set; }
            public IEnumerable<string> Prop10 { get; set; }
            public IReadOnlyCollection<string> Prop11 { get; set; }
            public int Prop12 { get; set; }
            public DummyEnum Prop13 { get; set; }
        }

        private readonly ObjectMapperOptions _options;
        private readonly DummySource _source;

        public ObjectExtensionsFixture()
        {
            // Excluding because cannot convert IEnumerable to IReadOnlyCollection without a property conversion
            _options = new ObjectMapperOptions().Exclude(nameof(DummySource.Prop11));
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
                    Prop2 = default(int),
                    _source.Prop3,
                    _source.Prop4,
                    _source.Prop5,
                    _source.Prop6,
                    Prop7b = default(string),
                    Prop8 = default(int),
                    _source.Prop9,
                    _source.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source.Prop12,
                    Prop13 = (DummyEnum) _source.Prop13
                });
            }

            [Fact]
            public void Should_Map_Using_Filter()
            {
                _options.Filter = propInfo => propInfo.Name != nameof(DummySource.Prop1);

                var actual = ObjectExtensions.MapTo<DummyTarget>(_source, _options);

                actual.Should().BeEquivalentTo(new
                {
                    Prop1 = default(int),
                    Prop2 = default(int),
                    _source.Prop3,
                    _source.Prop4,
                    _source.Prop5,
                    _source.Prop6,
                    Prop7b = default(string),
                    Prop8 = default(int),
                    _source.Prop9,
                    _source.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source.Prop12,
                    Prop13 = (DummyEnum) _source.Prop13
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
                    Prop4 = default(int),
                    _source.Prop5,
                    _source.Prop6,
                    Prop7b = default(string),
                    _source.Prop8,
                    _source.Prop9,
                    _source.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source.Prop12,
                    Prop13 = (DummyEnum) _source.Prop13
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
                    Prop2 = default(int),
                    _source.Prop3,
                    _source.Prop4,
                    _source.Prop5,
                    _source.Prop6,
                    Prop7b = default(string),
                    Prop8 = default(int),
                    _source.Prop9,
                    _source.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source.Prop12,
                    Prop13 = (DummyEnum) _source.Prop13
                });
            }

            [Fact]
            public void Should_Map_Alias_Properties_By_Name()
            {
                _options.WithAlias(nameof(DummySource.Prop7a), nameof(DummyTarget.Prop7b));

                var actual = ObjectExtensions.MapTo<DummyTarget>(_source, _options);

                actual.Should().BeEquivalentTo(new
                {
                    _source.Prop1,
                    Prop2 = default(int),
                    _source.Prop3,
                    _source.Prop4,
                    _source.Prop5,
                    _source.Prop6,
                    Prop7b = _source.Prop7a,
                    Prop8 = default(int),
                    _source.Prop9,
                    _source.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source.Prop12,
                    Prop13 = (DummyEnum) _source.Prop13
                });
            }

            [Fact]
            public void Should_Map_Alias_Properties_By_Expression()
            {
                var options = new ObjectMapperOptions()
                    .Exclude(nameof(DummySource.Prop11))
                    .WithAlias(nameof(DummySource.Prop7a), nameof(DummyTarget.Prop7b));

                var actual = ObjectExtensions.MapTo<DummyTarget>(_source, options);

                actual.Should().BeEquivalentTo(new
                {
                    _source.Prop1,
                    Prop2 = default(int),
                    _source.Prop3,
                    _source.Prop4,
                    _source.Prop5,
                    _source.Prop6,
                    Prop7b = _source.Prop7a,
                    Prop8 = default(int),
                    _source.Prop9,
                    _source.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source.Prop12,
                    Prop13 = (DummyEnum) _source.Prop13
                });
            }

            [Fact]
            public void Should_Map_WithConversion()
            {
                var options = new ObjectMapperOptions()
                    .WithConversion(nameof(DummySource.Prop11), value => ((IEnumerable<string>) value).Reverse().AsReadOnlyCollection());

                var actual = ObjectExtensions.MapTo<DummyTarget>(_source, options);

                actual.Should().BeEquivalentTo(new
                {
                    _source.Prop1,
                    Prop2 = default(int),
                    _source.Prop3,
                    _source.Prop4,
                    _source.Prop5,
                    _source.Prop6,
                    Prop7b = default(string),
                    Prop8 = default(int),
                    _source.Prop9,
                    _source.Prop10,
                    Prop11 = _source.Prop11.Reverse(),
                    Prop12 = (int) _source.Prop12,
                    Prop13 = (DummyEnum) _source.Prop13
                });
            }
        }
    }
}