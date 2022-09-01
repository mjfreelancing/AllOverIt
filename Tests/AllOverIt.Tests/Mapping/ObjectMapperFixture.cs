using AllOverIt.Fixture;
using AllOverIt.Mapping;
using AllOverIt.Mapping.Exceptions;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AllOverIt.Extensions;
using Xunit;

namespace AllOverIt.Tests.Mapping
{
    public class ObjectMapperFixture : FixtureBase
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
            public IReadOnlyCollection<string> Prop10 { get; set; }
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

        private readonly ObjectMapper _mapper;
        private readonly DummySource1 _source1;
        private readonly DummySource2 _source2;
        private readonly DummyTarget _target;

        protected ObjectMapperFixture()
        {
            _mapper = new ObjectMapper();
            _source1 = Create<DummySource1>();
            _source2 = Create<DummySource2>();
            _target = new DummyTarget();
        }

        public class DefaultOptions : ObjectMapperFixture
        {
            [Fact]
            public void Should_Have_Default_Options()
            {
                var expected = new
                {
                    Binding = BindingOptions.Default,
                    Filter = (Func<PropertyInfo, bool>) null
                };

                expected
                    .Should()
                    .BeEquivalentTo(_mapper.DefaultOptions, opt => opt.IncludingInternalProperties());
            }
        }

        public class Configure : ObjectMapperFixture
        {
            [Fact]
            public void Should_Default_Configure()
            {
                var propertyMapper = _mapper.GetMapper(_source2.GetType(), _target.GetType());

                propertyMapper.MapperOptions.Should().Be(_mapper.DefaultOptions);

                var actualMatches = GetMatchesNameAndType(propertyMapper.Matches);

                var expected = new[]
                {
                    (nameof(DummySource2.Prop1), typeof(int), nameof(DummyTarget.Prop1), typeof(int)),
                    (nameof(DummySource2.Prop3), typeof(string), nameof(DummyTarget.Prop3), typeof(string)),
                    (nameof(DummySource2.Prop5), typeof(int?), nameof(DummyTarget.Prop5), typeof(int)),
                    (nameof(DummySource2.Prop6), typeof(int), nameof(DummyTarget.Prop6), typeof(int?)),
                    (nameof(DummySource2.Prop8), typeof(int), nameof(DummyTarget.Prop8), typeof(int)),
                    (nameof(DummySource2.Prop9), typeof(IEnumerable<string>), nameof(DummyTarget.Prop9), typeof(IEnumerable<string>)),
                    (nameof(DummySource2.Prop10), typeof(IReadOnlyCollection<string>), nameof(DummyTarget.Prop10), typeof(IEnumerable<string>)),
                    (nameof(DummySource2.Prop11), typeof(IEnumerable<string>), nameof(DummyTarget.Prop11), typeof(IReadOnlyCollection<string>)),
                    (nameof(DummySource2.Prop12), typeof(DummyEnum), nameof(DummyTarget.Prop12), typeof(int)),
                    (nameof(DummySource2.Prop13), typeof(int), nameof(DummyTarget.Prop13), typeof(DummyEnum))
                };

                expected
                    .Should()
                    .BeEquivalentTo(actualMatches);
            }

            [Fact]
            public void Should_Throw_When_Configured_More_Than_Once()
            {
                _mapper.Configure<DummySource2, DummyTarget>();

                Invoking(() => _mapper.Configure<DummySource2, DummyTarget>())
                    .Should()
                    .Throw<ObjectMapperException>()
                    .WithMessage($"Mapping already exists between {nameof(DummySource2)} and {nameof(DummyTarget)}.");
            }

            [Fact]
            public void Should_Configure_With_Custom_Bindings()
            {
                var binding = BindingOptions.Instance | BindingOptions.Internal;

                _mapper.Configure<DummySource2, DummyTarget>(options => options.Binding = binding);

                var propertyMapper = _mapper.GetMapper(_source2.GetType(), _target.GetType());

                propertyMapper.MapperOptions.Binding.Should().Be(binding);

                var actualMatches = GetMatchesNameAndType(propertyMapper.Matches);

                var expected = new[]
                {
                    (nameof(DummySource2.Prop4), typeof(int), nameof(DummyTarget.Prop4), typeof(int))
                };

                expected
                    .Should()
                    .BeEquivalentTo(actualMatches);
            }

            [Fact]
            public void Should_Configure_With_Filter()
            {
                _mapper.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Filter = propInfo => new[] { "Prop10", "Prop12", "Prop8" }.Contains(propInfo.Name);
                });

                var propertyMapper = _mapper.GetMapper(_source2.GetType(), _target.GetType());

                var actualMatches = GetMatchesNameAndType(propertyMapper.Matches);

                var expected = new[]
                {
                    (nameof(DummySource2.Prop8), typeof(int), nameof(DummyTarget.Prop8), typeof(int)),
                    (nameof(DummySource2.Prop10), typeof(IReadOnlyCollection<string>), nameof(DummyTarget.Prop10), typeof(IEnumerable<string>)),
                    (nameof(DummySource2.Prop12), typeof(DummyEnum), nameof(DummyTarget.Prop12), typeof(int))
                };

                expected
                    .Should()
                    .BeEquivalentTo(actualMatches);
            }

            [Fact]
            public void Should_Configure_With_Exclude()
            {
                _mapper.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Filter = propInfo => new[] { "Prop10", "Prop12", "Prop8" }.Contains(propInfo.Name);
                    options.Exclude(src => src.Prop10);
                });

                var propertyMapper = _mapper.GetMapper(_source2.GetType(), _target.GetType());

                var actualMatches = GetMatchesNameAndType(propertyMapper.Matches);

                var expected = new[]
                {
                    (nameof(DummySource2.Prop8), typeof(int), nameof(DummyTarget.Prop8), typeof(int)),
                    (nameof(DummySource2.Prop12), typeof(DummyEnum), nameof(DummyTarget.Prop12), typeof(int))
                };

                expected
                    .Should()
                    .BeEquivalentTo(actualMatches);
            }

            [Fact]
            public void Should_Configure_With_Filter_And_Alias()
            {
                _mapper.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Filter = propInfo => new[] { "Prop10", "Prop12", "Prop8" }.Contains(propInfo.Name);

                    options
                        .WithAlias(src => src.Prop8, trg => trg.Prop1)
                        .WithAlias(src => (int) src.Prop12, trg => trg.Prop5);
                });

                var propertyMapper = _mapper.GetMapper(_source2.GetType(), _target.GetType());

                var actualMatches = GetMatchesNameAndType(propertyMapper.Matches);

                var expected = new[]
                {
                    (nameof(DummySource2.Prop8), typeof(int), nameof(DummyTarget.Prop1), typeof(int)),
                    (nameof(DummySource2.Prop10), typeof(IReadOnlyCollection<string>), nameof(DummyTarget.Prop10), typeof(IEnumerable<string>)),
                    (nameof(DummySource2.Prop12), typeof(DummyEnum), nameof(DummyTarget.Prop5), typeof(int))
                };

                expected
                    .Should()
                    .BeEquivalentTo(actualMatches);
            }

            [Fact]
            public void Should_Configure_With_Filter_And_Alias_And_Conversion()
            {
                var factor = GetWithinRange(2, 5);

                _mapper.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Filter = propInfo => new[] { "Prop10", "Prop12", "Prop8" }.Contains(propInfo.Name);

                    options
                        .WithAlias(src => src.Prop8, trg => trg.Prop1)
                        .WithAlias(src => (int) src.Prop12, trg => trg.Prop5);

                    options.WithConversion(src => src.Prop8, value => value * factor);
                });

                var propertyMapper = _mapper.GetMapper(_source2.GetType(), _target.GetType());

                var actualMatches = GetMatchesNameAndType(propertyMapper.Matches);

                var expectedAliases = new[]
                {
                    (nameof(DummySource2.Prop8), typeof(int), nameof(DummyTarget.Prop1), typeof(int)),
                    (nameof(DummySource2.Prop10), typeof(IReadOnlyCollection<string>), nameof(DummyTarget.Prop10), typeof(IEnumerable<string>)),
                    (nameof(DummySource2.Prop12), typeof(DummyEnum), nameof(DummyTarget.Prop5), typeof(int))
                };

                expectedAliases
                    .Should()
                    .BeEquivalentTo(actualMatches);

                var convertedValue = _mapper.Map<DummyTarget>(_source2).Prop1;

                convertedValue.Should().Be(_source2.Prop8 * factor);
            }

            private static IEnumerable<(string SourceName, Type SourceType, string TargetName, Type TargetType)>
                GetMatchesNameAndType(IEnumerable<ObjectMapper.MatchingPropertyMapper.PropertyMatchInfo> matches)
            {
                return matches.Select(
                    match => (match.SourceInfo.Name, match.SourceInfo.PropertyType,
                              match.TargetInfo.Name, match.TargetInfo.PropertyType)
                );
            }
        }

        public class Map_Target : ObjectMapperFixture
        {
            [Fact]
            public void Should_Throw_When_Source_Null()
            {
                Invoking(() =>
                    {
                        _ = _mapper.Map<DummyTarget>(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithParameterName("source");
            }

            [Fact]
            public void Should_Return_Target_Type()
            {
                var actual = _mapper.Map<DummyTarget>(_source1);

                actual.Should().BeOfType<DummyTarget>();
            }

            [Fact]
            public void Should_Not_Throw_When_Not_Configured_For_Compatible_Types()
            {
                // _source2 would fail because it needs a conversion from IReadOnlyCollection to IEnumerable on Prop10
                Invoking(() => _mapper.Map<DummyTarget>(_source1))
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Default_Map()
            {
                var actual = _mapper.Map<DummyTarget>(_source1);

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
                    Prop10 = default(IEnumerable<string>),
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source1.Prop12,
                    Prop13 = (DummyEnum) _source1.Prop13
                };

                expected
                    .Should()
                    .BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Using_Filter()
            {
                _mapper.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Filter = propInfo =>
                        !new[] { nameof(DummySource2.Prop10), nameof(DummySource2.Prop8), nameof(DummySource2.Prop11) }.Contains(propInfo.Name);
                });

                var actual = _mapper.Map<DummyTarget>(_source2);

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
                    Prop10 = default(IEnumerable<string>),
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Exclude_And_Private_Properties()
            {
                _mapper.Configure<DummySource2, DummyTarget>(options =>
                {
                    options
                        .Exclude(src => src.Prop10)
                        .Exclude(src => src.Prop11);

                    options.Binding = BindingOptions.Public | BindingOptions.Private;
                });

                var actual = _mapper.Map<DummyTarget>(_source2);

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
                    Prop10 = default(IEnumerable<string>),
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Exclude_And_Internal_Properties()
            {
                _mapper.Configure<DummySource2, DummyTarget>(options =>
                {
                    options
                        .Exclude(src => src.Prop10)
                        .Exclude(src => src.Prop11);

                    options.Binding = BindingOptions.Public | BindingOptions.Internal;
                });

                var actual = _mapper.Map<DummyTarget>(_source2);

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
                    Prop10 = default(IEnumerable<string>),
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Exclude_Bind_And_Alias_Properties_By_Name()
            {
                _mapper.Configure<DummySource2, DummyTarget>(options =>
                {
                    options
                        .Exclude(src => src.Prop10)
                        .Exclude(src => src.Prop11);

                    options.WithAlias(nameof(DummySource2.Prop7a), nameof(DummyTarget.Prop7b));

                    options.Binding = BindingOptions.Public | BindingOptions.Internal;
                });

                var actual = _mapper.Map<DummyTarget>(_source2);

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
                    Prop10 = default(IEnumerable<string>),
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Exclude_Bind_And_Alias_Properties_By_Expression()
            {
                _mapper.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Exclude(src => src.Prop11);
                    options.WithAlias(source => source.Prop7a, target => target.Prop7b);
                    options.Binding = BindingOptions.Public | BindingOptions.Internal;
                });

                var actual = _mapper.Map<DummyTarget>(_source2);

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
                _mapper.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.WithConversion(nameof(DummySource2.Prop11), value => ((IEnumerable<string>) value).Reverse().AsReadOnlyCollection());
                });

                var actual = _mapper.Map<DummyTarget>(_source2);

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

        public class Map_Source_Target : ObjectMapperFixture
        {
            [Fact]
            public void Should_Throw_When_Source_Null()
            {
                Invoking(() =>
                {
                    _ = _mapper.Map<DummySource1, DummyTarget>(null, _target);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithParameterName("source");
            }

            [Fact]
            public void Should_Throw_When_Target_Null()
            {
                Invoking(() =>
                    {
                        _ = _mapper.Map<DummySource1, DummyTarget>(_source1, null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithParameterName("target");
            }

            [Fact]
            public void Should_Return_Same_Target()
            {
                var actual = _mapper.Map<DummySource1, DummyTarget>(_source1, _target);

                actual.Should().BeSameAs(_target);
            }

            [Fact]
            public void Should_Not_Throw_When_Not_Configured_For_Compatible_Types()
            {
                // _source2 would fail because it needs a conversion from IReadOnlyCollection to IEnumerable on Prop10
                Invoking(() => _mapper.Map<DummySource1, DummyTarget>(_source1, _target))
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Default_Map()
            {
                var actual = _mapper.Map<DummySource1, DummyTarget>(_source1, _target);

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
                    Prop10 = default(IEnumerable<string>),
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source1.Prop12,
                    Prop13 = (DummyEnum) _source1.Prop13
                };

                expected
                    .Should()
                    .BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Using_Filter()
            {
                _mapper.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Filter = propInfo =>
                        !new[] { nameof(DummySource2.Prop10), nameof(DummySource2.Prop8), nameof(DummySource2.Prop11) }.Contains(propInfo.Name);
                });

                var actual = _mapper.Map<DummySource2, DummyTarget>(_source2, _target);

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
                    Prop10 = default(IEnumerable<string>),
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Exclude_And_Private_Properties()
            {
                _mapper.Configure<DummySource2, DummyTarget>(options =>
                {
                    options
                        .Exclude(src => src.Prop10)
                        .Exclude(src => src.Prop11);

                    options.Binding = BindingOptions.Public | BindingOptions.Private;
                });

                var actual = _mapper.Map<DummySource2, DummyTarget>(_source2, _target);

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
                    Prop10 = default(IEnumerable<string>),
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Exclude_And_Internal_Properties()
            {
                _mapper.Configure<DummySource2, DummyTarget>(options =>
                {
                    options
                        .Exclude(src => src.Prop10)
                        .Exclude(src => src.Prop11);

                    options.Binding = BindingOptions.Public | BindingOptions.Internal;
                });

                var actual = _mapper.Map<DummySource2, DummyTarget>(_source2, _target);

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
                    Prop10 = default(IEnumerable<string>),
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Exclude_Bind_And_Alias_Properties_By_Name()
            {
                _mapper.Configure<DummySource2, DummyTarget>(options =>
                {
                    options
                        .Exclude(src => src.Prop10)
                        .Exclude(src => src.Prop11);

                    options.WithAlias(nameof(DummySource2.Prop7a), nameof(DummyTarget.Prop7b));

                    options.Binding = BindingOptions.Public | BindingOptions.Internal;
                });

                var actual = _mapper.Map<DummySource2, DummyTarget>(_source2, _target);

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
                    Prop10 = default(IEnumerable<string>),
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = (DummyEnum) _source2.Prop13
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Exclude_Bind_And_Alias_Properties_By_Expression()
            {
                _mapper.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Exclude(src => src.Prop11);
                    options.WithAlias(source => source.Prop7a, target => target.Prop7b);
                    options.Binding = BindingOptions.Public | BindingOptions.Internal;
                });

                var actual = _mapper.Map<DummySource2, DummyTarget>(_source2, _target);

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
                _mapper.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.WithConversion(nameof(DummySource2.Prop11), value => ((IEnumerable<string>) value).Reverse().AsReadOnlyCollection());
                });

                var actual = _mapper.Map<DummySource2, DummyTarget>(_source2, _target);

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
    }
}