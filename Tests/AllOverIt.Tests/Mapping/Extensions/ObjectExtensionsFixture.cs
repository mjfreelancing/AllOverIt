using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Mapping;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using AllOverIt.Tests.Extensions;
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

        private class DummySource1
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
            public DummyEnum Prop12 { get; set; }
            public int Prop13 { get; set; }

            public DummySource1()
            {
                Prop2 = 10;
            }

            public int GetProp2()
            {
                return Prop2;
            }
        }

        private class DummySource2 : DummySource1
        {
            public IEnumerable<string> Prop11 { get; set; }
        }

        private class DummyTarget
        {
            public int Prop1 { get; set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Part of the test")]
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

        private readonly DummySource1 _source1;
        private readonly DummySource2 _source2;

        public ObjectExtensionsFixture()
        {
            _source1 = Create<DummySource1>();
            _source2 = Create<DummySource2>();
        }

        public class MapTo_Target_Options : ObjectExtensionsFixture
        {
            private readonly ObjectMapperOptions _options;

            public MapTo_Target_Options()
            {
                // Excluding because cannot convert IEnumerable to IReadOnlyCollection without a property conversion
                _options = new ObjectMapperOptions().Exclude(nameof(DummySource2.Prop11));
            }

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
                        ObjectExtensions.MapTo<DummyTarget>(_source2, null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("options");
            }

            [Fact]
            public void Should_Return_Target_Type()
            {
                var actual = ObjectExtensions.MapTo<DummyTarget>(_source2, _options);

                actual.Should().BeOfType<DummyTarget>();
            }

            [Fact]
            public void Should_Map_Using_Default_Bindings()
            {
                var actual = ObjectExtensions.MapTo<DummyTarget>(_source2, _options);

                var expected = new
                {
                    _source2.Prop1,
                    Prop2 = default(int),
                    _source2.Prop3,
                    _source2.Prop4,
                    _source2.Prop5,
                    _source2.Prop6,
                    Prop7b = default(string),
                    Prop8 = default(int),
                    _source2.Prop9,
                    _source2.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Using_Filter()
            {
                _options.Filter = propInfo => propInfo.Name != nameof(DummySource2.Prop1);

                var actual = ObjectExtensions.MapTo<DummyTarget>(_source2, _options);

                var expected = new
                {
                    Prop1 = default(int),
                    Prop2 = default(int),
                    _source2.Prop3,
                    _source2.Prop4,
                    _source2.Prop5,
                    _source2.Prop6,
                    Prop7b = default(string),
                    Prop8 = default(int),
                    _source2.Prop9,
                    _source2.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Private_Properties()
            {
                _options.Binding = BindingOptions.Public | BindingOptions.Private;

                var actual = ObjectExtensions.MapTo<DummyTarget>(_source2, _options);

                var expected = new
                {
                    _source2.Prop1,
                    Prop2 = _source2.GetProp2(),
                    _source2.Prop3,
                    Prop4 = default(int),
                    _source2.Prop5,
                    _source2.Prop6,
                    Prop7b = default(string),
                    _source2.Prop8,
                    _source2.Prop9,
                    _source2.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Internal_Properties()
            {
                _options.Binding = BindingOptions.Public | BindingOptions.Internal;

                var actual = ObjectExtensions.MapTo<DummyTarget>(_source2, _options);

                var expected = new
                {
                    _source2.Prop1,
                    Prop2 = default(int),
                    _source2.Prop3,
                    _source2.Prop4,
                    _source2.Prop5,
                    _source2.Prop6,
                    Prop7b = default(string),
                    Prop8 = default(int),
                    _source2.Prop9,
                    _source2.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Alias_Properties_By_Name()
            {
                _options.WithAlias(nameof(DummySource2.Prop7a), nameof(DummyTarget.Prop7b));

                var actual = ObjectExtensions.MapTo<DummyTarget>(_source2, _options);

                var expected = new
                {
                    _source2.Prop1,
                    Prop2 = default(int),
                    _source2.Prop3,
                    _source2.Prop4,
                    _source2.Prop5,
                    _source2.Prop6,
                    Prop7b = _source2.Prop7a,
                    Prop8 = default(int),
                    _source2.Prop9,
                    _source2.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Alias_Properties_By_Expression()
            {
                var options = new TypedObjectMapperOptions<DummySource2, DummyTarget>()
                        .WithAlias(source => source.Prop7a, target => target.Prop7b)
                        .Exclude(nameof(DummySource2.Prop11));

                var actual = ObjectExtensions.MapTo<DummyTarget>(_source2, options);

                var expected = new
                {
                    _source2.Prop1,
                    Prop2 = default(int),
                    _source2.Prop3,
                    _source2.Prop4,
                    _source2.Prop5,
                    _source2.Prop6,
                    Prop7b = _source2.Prop7a,
                    Prop8 = default(int),
                    _source2.Prop9,
                    _source2.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_WithConversion()
            {
                var options = new ObjectMapperOptions()
                    .WithConversion(nameof(DummySource2.Prop11), value => ((IEnumerable<string>) value).Reverse().AsReadOnlyCollection());

                var actual = ObjectExtensions.MapTo<DummyTarget>(_source2, options);

                var expected = new
                {
                    _source2.Prop1,
                    Prop2 = default(int),
                    _source2.Prop3,
                    _source2.Prop4,
                    _source2.Prop5,
                    _source2.Prop6,
                    Prop7b = default(string),
                    Prop8 = default(int),
                    _source2.Prop9,
                    _source2.Prop10,
                    Prop11 = _source2.Prop11.Reverse(),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class MapTo_Target_Binding : ObjectExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Source_Null()
            {
                Invoking(() =>
                {
                    ObjectExtensions.MapTo<DummyTarget>(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("source");
            }

            [Fact]
            public void Should_Return_Target_Type()
            {
                var actual = ObjectExtensions.MapTo<DummyTarget>(_source1);

                actual.Should().BeOfType<DummyTarget>();
            }

            [Fact]
            public void Should_Map_Using_Default_Bindings()
            {
                var actual = ObjectExtensions.MapTo<DummyTarget>(_source1);

                var expected = new
                {
                    _source1.Prop1,
                    Prop2 = default(int),
                    _source1.Prop3,
                    _source1.Prop4,
                    _source1.Prop5,
                    _source1.Prop6,
                    Prop7b = default(string),
                    Prop8 = default(int),
                    _source1.Prop9,
                    _source1.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source1.Prop12,
                    Prop13 = (DummyEnum) _source1.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Private_Properties()
            {
                var binding = BindingOptions.Public | BindingOptions.Private;

                var actual = ObjectExtensions.MapTo<DummyTarget>(_source1, binding);

                var expected = new
                {
                    _source1.Prop1,
                    Prop2 = _source1.GetProp2(),
                    _source1.Prop3,
                    Prop4 = default(int),
                    _source1.Prop5,
                    _source1.Prop6,
                    Prop7b = default(string),
                    _source1.Prop8,
                    _source1.Prop9,
                    _source1.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source1.Prop12,
                    Prop13 = (DummyEnum) _source1.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Internal_Properties()
            {
                var binding = BindingOptions.Public | BindingOptions.Internal;

                var actual = ObjectExtensions.MapTo<DummyTarget>(_source1, binding);

                var expected = new
                {
                    _source1.Prop1,
                    Prop2 = default(int),
                    _source1.Prop3,
                    _source1.Prop4,
                    _source1.Prop5,
                    _source1.Prop6,
                    Prop7b = default(string),
                    Prop8 = default(int),
                    _source1.Prop9,
                    _source1.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source1.Prop12,
                    Prop13 = (DummyEnum) _source1.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class MapTo_Source_Target_Options : ObjectExtensionsFixture
        {
            private readonly TypedObjectMapperOptions<DummySource2, DummyTarget> _options;
            private readonly DummyTarget _target;
            public MapTo_Source_Target_Options()
            {
                // Excluding because cannot convert IEnumerable to IReadOnlyCollection without a property conversion
                _options = new TypedObjectMapperOptions<DummySource2, DummyTarget>().Exclude(source => source.Prop11);
                _target = new DummyTarget();
            }

            [Fact]
            public void Should_Throw_When_Source_Null()
            {
                Invoking(() =>
                {
                    ObjectExtensions.MapTo<DummySource2, DummyTarget>(null, _target, _options);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("source");
            }

            [Fact]
            public void Should_Throw_When_Target_Null()
            {
                Invoking(() =>
                    {
                        ObjectExtensions.MapTo<DummySource2, DummyTarget>(_source2, null, _options);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("target");
            }

            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() =>
                {
                    ObjectExtensions.MapTo<DummySource2, DummyTarget>(_source2, _target, null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("options");
            }

            [Fact]
            public void Should_Return_Target_Type()
            {
                var actual = ObjectExtensions.MapTo<DummySource2, DummyTarget>(_source2, _target, _options);

                actual.Should().BeOfType<DummyTarget>();
            }

            [Fact]
            public void Should_Return_Same_Target()
            {
                var actual = ObjectExtensions.MapTo<DummySource2, DummyTarget>(_source2, _target, _options);

                actual.Should().BeSameAs(_target);
            }

            [Fact]
            public void Should_Map_Using_Default_Bindings()
            {
                var actual = ObjectExtensions.MapTo<DummySource2, DummyTarget>(_source2, _target, _options);

                var expected = new
                {
                    _source2.Prop1,
                    Prop2 = default(int),
                    _source2.Prop3,
                    _source2.Prop4,
                    _source2.Prop5,
                    _source2.Prop6,
                    Prop7b = default(string),
                    Prop8 = default(int),
                    _source2.Prop9,
                    _source2.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Using_Filter()
            {
                _options.Filter = propInfo => propInfo.Name != nameof(DummySource2.Prop1);

                var actual = ObjectExtensions.MapTo<DummySource2, DummyTarget>(_source2, _target, _options);

                var expected = new
                {
                    Prop1 = default(int),
                    Prop2 = default(int),
                    _source2.Prop3,
                    _source2.Prop4,
                    _source2.Prop5,
                    _source2.Prop6,
                    Prop7b = default(string),
                    Prop8 = default(int),
                    _source2.Prop9,
                    _source2.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Private_Properties()
            {
                _options.Binding = BindingOptions.Public | BindingOptions.Private;

                var actual = ObjectExtensions.MapTo<DummySource2, DummyTarget>(_source2, _target, _options);

                var expected = new
                {
                    _source2.Prop1,
                    Prop2 = _source2.GetProp2(),
                    _source2.Prop3,
                    Prop4 = default(int),
                    _source2.Prop5,
                    _source2.Prop6,
                    Prop7b = default(string),
                    _source2.Prop8,
                    _source2.Prop9,
                    _source2.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Internal_Properties()
            {
                _options.Binding = BindingOptions.Public | BindingOptions.Internal;

                var actual = ObjectExtensions.MapTo<DummySource2, DummyTarget>(_source2, _target, _options);

                var expected = new
                {
                    _source2.Prop1,
                    Prop2 = default(int),
                    _source2.Prop3,
                    _source2.Prop4,
                    _source2.Prop5,
                    _source2.Prop6,
                    Prop7b = default(string),
                    Prop8 = default(int),
                    _source2.Prop9,
                    _source2.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Alias_Properties_By_Name()
            {
                _options.WithAlias(nameof(DummySource2.Prop7a), nameof(DummyTarget.Prop7b));

                var actual = ObjectExtensions.MapTo<DummySource2, DummyTarget>(_source2, _target, _options);

                var expected = new
                {
                    _source2.Prop1,
                    Prop2 = default(int),
                    _source2.Prop3,
                    _source2.Prop4,
                    _source2.Prop5,
                    _source2.Prop6,
                    Prop7b = _source2.Prop7a,
                    Prop8 = default(int),
                    _source2.Prop9,
                    _source2.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Alias_Properties_By_Expression()
            {
                var options = new TypedObjectMapperOptions<DummySource2, DummyTarget>()
                    .Exclude(source => source.Prop11)
                    .WithAlias(source => source.Prop7a, target => target.Prop7b);

                var actual = ObjectExtensions.MapTo<DummySource2, DummyTarget>(_source2, _target, options);

                var expected = new
                {
                    _source2.Prop1,
                    Prop2 = default(int),
                    _source2.Prop3,
                    _source2.Prop4,
                    _source2.Prop5,
                    _source2.Prop6,
                    Prop7b = _source2.Prop7a,
                    Prop8 = default(int),
                    _source2.Prop9,
                    _source2.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_WithConversion()
            {
                var options = new TypedObjectMapperOptions<DummySource2, DummyTarget>()
                    .WithConversion(source => source.Prop11, value => value.Reverse().AsReadOnlyCollection());

                var actual = ObjectExtensions.MapTo<DummySource2, DummyTarget>(_source2, _target, options);

                var expected = new
                {
                    _source2.Prop1,
                    Prop2 = default(int),
                    _source2.Prop3,
                    _source2.Prop4,
                    _source2.Prop5,
                    _source2.Prop6,
                    Prop7b = default(string),
                    Prop8 = default(int),
                    _source2.Prop9,
                    _source2.Prop10,
                    Prop11 = _source2.Prop11.Reverse(),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class MapTo_Source_Target_Binding : ObjectExtensionsFixture
        {
            private readonly DummyTarget _target;
            public MapTo_Source_Target_Binding()
            {
                _target = new DummyTarget();
            }

            [Fact]
            public void Should_Throw_When_Source_Null()
            {
                Invoking(() =>
                {
                    ObjectExtensions.MapTo<DummySource1, DummyTarget>(null, _target);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("source");
            }

            [Fact]
            public void Should_Throw_When_Target_Null()
            {
                Invoking(() =>
                {
                    ObjectExtensions.MapTo<DummySource1, DummyTarget>(_source1, null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("target");
            }

            [Fact]
            public void Should_Return_Target_Type()
            {
                var actual = ObjectExtensions.MapTo<DummySource1, DummyTarget>(_source1, _target);

                actual.Should().BeOfType<DummyTarget>();
            }

            [Fact]
            public void Should_Return_Same_Target()
            {
                var actual = ObjectExtensions.MapTo<DummySource1, DummyTarget>(_source1, _target);

                actual.Should().BeSameAs(_target);
            }

            [Fact]
            public void Should_Map_Using_Default_Bindings()
            {
                var actual = ObjectExtensions.MapTo<DummySource1, DummyTarget>(_source1, _target);

                var expected = new
                {
                    _source1.Prop1,
                    Prop2 = default(int),
                    _source1.Prop3,
                    _source1.Prop4,
                    _source1.Prop5,
                    _source1.Prop6,
                    Prop7b = default(string),
                    Prop8 = default(int),
                    _source1.Prop9,
                    _source1.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source1.Prop12,
                    Prop13 = (DummyEnum) _source1.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Private_Properties()
            {
                var binding = BindingOptions.Public | BindingOptions.Private;

                var actual = ObjectExtensions.MapTo<DummySource1, DummyTarget>(_source1, _target, binding);

                var expected = new
                {
                    _source1.Prop1,
                    Prop2 = _source1.GetProp2(),
                    _source1.Prop3,
                    Prop4 = default(int),
                    _source1.Prop5,
                    _source1.Prop6,
                    Prop7b = default(string),
                    _source1.Prop8,
                    _source1.Prop9,
                    _source1.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source1.Prop12,
                    Prop13 = (DummyEnum) _source1.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Internal_Properties()
            {
                var binding = BindingOptions.Public | BindingOptions.Internal;

                var actual = ObjectExtensions.MapTo<DummySource1, DummyTarget>(_source1, _target, binding);

                var expected = new
                {
                    _source1.Prop1,
                    Prop2 = default(int),
                    _source1.Prop3,
                    _source1.Prop4,
                    _source1.Prop5,
                    _source1.Prop6,
                    Prop7b = default(string),
                    Prop8 = default(int),
                    _source1.Prop9,
                    _source1.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source1.Prop12,
                    Prop13 = (DummyEnum) _source1.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }
        }
    }
}