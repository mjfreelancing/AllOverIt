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
using AllOverIt.Mapping.Extensions;
using System.Collections.ObjectModel;
using AllOverIt.Fixture.Extensions;

using static AllOverIt.Tests.Mapping.ObjectMapperTypes;

namespace AllOverIt.Tests.Mapping
{
    public class ObjectMapperFixture : FixtureBase
    {
        private readonly DummySource1 _source1;
        private readonly DummySource2 _source2;
        private readonly DummyTarget _target;

        protected ObjectMapperFixture()
        {
            _source1 = Create<DummySource1>();
            _source2 = Create<DummySource2>();
            _target = new DummyTarget();
        }

        public class Constructor_Default : ObjectMapperFixture
        {
            [Fact]
            public void Should_Set_Default_Configuration()
            {
                var mapper = new ObjectMapper();

                mapper._configuration.Should().NotBeNull();
            }
        }

        public class Constructor_Configuration : ObjectMapperFixture
        {
            [Fact]
            public void Should_Throw_When_Configuration_Null()
            {
                Invoking(() =>
                {
                    _ = new ObjectMapper((ObjectMapperConfiguration) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("configuration");
            }

            [Fact]
            public void Should_Set_Configuration()
            {
                var configuration = Create<ObjectMapperConfiguration>();
                var mapper = new ObjectMapper(configuration);

                mapper._configuration.Should().BeSameAs(configuration);
            }
        }

        public class Constructor_Configuration_Action : ObjectMapperFixture
        {
            [Fact]
            public void Should_Throw_When_Configuration_Action_Null()
            {
                Invoking(() =>
                {
                    _ = new ObjectMapper((Action<ObjectMapperConfiguration>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("configuration");
            }

            [Fact]
            public void Should_Set_Configuration()
            {
                PropertyMatcherCache actual = default;

                var objectMapper = new ObjectMapper(config =>
                {
                    config.Configure<DummySource1, DummyTarget>(opt =>
                    {
                        opt.Exclude(src => src.Prop5);
                    });

                    actual = config._propertyMatcherCache;
                });

                actual.Should().NotBeNull();

                actual.TryGetMapper(typeof(DummySource1), typeof(DummyTarget), out var mapper).Should().BeTrue();

                var actualMatches = GetMatchesNameAndType(mapper.Matches);

                var expected = new[]
                {
                    (nameof(DummySource2.Prop1), typeof(int), nameof(DummyTarget.Prop1), typeof(float)),
                    (nameof(DummySource2.Prop3), typeof(string), nameof(DummyTarget.Prop3), typeof(string)),
                    //(nameof(DummySource2.Prop5), typeof(int?), nameof(DummyTarget.Prop5), typeof(int)),
                    (nameof(DummySource2.Prop6), typeof(int), nameof(DummyTarget.Prop6), typeof(int?)),
                    (nameof(DummySource2.Prop8), typeof(int), nameof(DummyTarget.Prop8), typeof(int)),
                    (nameof(DummySource2.Prop9), typeof(IEnumerable<string>), nameof(DummyTarget.Prop9), typeof(IEnumerable<string>)),
                    (nameof(DummySource2.Prop12), typeof(DummyEnum), nameof(DummyTarget.Prop12), typeof(int)),
                    (nameof(DummySource2.Prop13), typeof(int), nameof(DummyTarget.Prop13), typeof(DummyEnum))
                };

                expected
                    .Should()
                    .BeEquivalentTo(actualMatches);
            }
        }

        public class Constructor_Options_Configuration_Action : ObjectMapperFixture
        {
            [Fact]
            public void Should_Throw_When_Options_Action_Null()
            {
                Invoking(() =>
                {
                    _ = new ObjectMapper((Action<ObjectMapperOptions>) null, configuration => { });
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("defaultOptions");
            }

            [Fact]
            public void Should_Throw_When_Configuration_Action_Null()
            {
                Invoking(() =>
                {
                    _ = new ObjectMapper(options => { }, (Action<ObjectMapperConfiguration>) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("configuration");
            }

            [Fact]
            public void Should_Set_Options_And_Configuration()
            {
                var allowNullCollections = Create<bool>();

                PropertyMatcherCache actual = default;

                var objectMapper = new ObjectMapper(
                    options =>
                    {
                        options.AllowNullCollections = allowNullCollections;
                    },
                    config =>
                    {
                        config.Configure<DummySource1, DummyTarget>(options =>
                        {
                            // Testing by name
                            options.Exclude(nameof(DummySource1.Prop1));

                            // Testing by expression
                            options.Exclude(src => src.Prop5);
                        });

                        actual = config._propertyMatcherCache;
                    });

                objectMapper._configuration.Options.AllowNullCollections.Should().Be(allowNullCollections);

                actual.Should().NotBeNull();
               
                actual.TryGetMapper(typeof(DummySource1), typeof(DummyTarget), out var mapper).Should().BeTrue();

                var actualMatches = GetMatchesNameAndType(mapper.Matches);

                var expected = new[]
                {
                    //(nameof(DummySource2.Prop1), typeof(int), nameof(DummyTarget.Prop1), typeof(float)),
                    (nameof(DummySource2.Prop3), typeof(string), nameof(DummyTarget.Prop3), typeof(string)),
                    //(nameof(DummySource2.Prop5), typeof(int?), nameof(DummyTarget.Prop5), typeof(int)),
                    (nameof(DummySource2.Prop6), typeof(int), nameof(DummyTarget.Prop6), typeof(int?)),
                    (nameof(DummySource2.Prop8), typeof(int), nameof(DummyTarget.Prop8), typeof(int)),
                    (nameof(DummySource2.Prop9), typeof(IEnumerable<string>), nameof(DummyTarget.Prop9), typeof(IEnumerable<string>)),
                    (nameof(DummySource2.Prop12), typeof(DummyEnum), nameof(DummyTarget.Prop12), typeof(int)),
                    (nameof(DummySource2.Prop13), typeof(int), nameof(DummyTarget.Prop13), typeof(DummyEnum))
                };

                expected
                    .Should()
                    .BeEquivalentTo(actualMatches);
            }

            [Fact]
            public void Should_Set_Configuration()
            {
                PropertyMatcherCache actual = default;

                var objectMapper = new ObjectMapper(
                    options => { },
                    config =>
                    {
                        config.Configure<DummySource1, DummyTarget>();

                        actual = config._propertyMatcherCache;
                    });

                actual.Should().NotBeNull();

                actual.TryGetMapper(typeof(DummySource1), typeof(DummyTarget), out var mapper).Should().BeTrue();

                var actualMatches = GetMatchesNameAndType(mapper.Matches);

                var expected = new[]
                {
                    (nameof(DummySource2.Prop1), typeof(int), nameof(DummyTarget.Prop1), typeof(float)),
                    (nameof(DummySource2.Prop3), typeof(string), nameof(DummyTarget.Prop3), typeof(string)),
                    (nameof(DummySource2.Prop5), typeof(int?), nameof(DummyTarget.Prop5), typeof(int)),
                    (nameof(DummySource2.Prop6), typeof(int), nameof(DummyTarget.Prop6), typeof(int?)),
                    (nameof(DummySource2.Prop8), typeof(int), nameof(DummyTarget.Prop8), typeof(int)),
                    (nameof(DummySource2.Prop9), typeof(IEnumerable<string>), nameof(DummyTarget.Prop9), typeof(IEnumerable<string>)),
                    (nameof(DummySource2.Prop12), typeof(DummyEnum), nameof(DummyTarget.Prop12), typeof(int)),
                    (nameof(DummySource2.Prop13), typeof(int), nameof(DummyTarget.Prop13), typeof(DummyEnum))
                };

                expected
                    .Should()
                    .BeEquivalentTo(actualMatches);
            }
        }

        public class Map_Target : ObjectMapperFixture
        {
            [Fact]
            public void Should_Return_Null_When_Source_Null()
            {
                var configuration = GetCommonMapperConfiguration();

                var mapper = new ObjectMapper(configuration);

                var actual = mapper.Map<DummyTarget>(null);

                actual.Should().BeNull();
            }

            [Fact]
            public void Should_Return_Target_Type()
            {
                var configuration = GetCommonMapperConfiguration();

                var mapper = new ObjectMapper(configuration);

                var actual = mapper.Map<DummyTarget>(_source1);

                actual.Should().BeOfType<DummyTarget>();
            }

            [Fact]
            public void Should_Not_Throw_When_Not_Configured_For_Compatible_Types()
            {
                var configuration = GetCommonMapperConfiguration();

                var mapper = new ObjectMapper(configuration);

                // _source2 would fail because it needs a conversion from IReadOnlyCollection to IEnumerable on Prop10
                Invoking(() => mapper.Map<DummyTarget>(_source1))
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Default_Map()
            {
                var configuration = GetCommonMapperConfiguration();

                var mapper = new ObjectMapper(configuration);

                var actual = mapper.Map<DummyTarget>(_source1);

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
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Filter = propInfo =>
                        !new[] { nameof(DummySource2.Prop10), nameof(DummySource2.Prop8), nameof(DummySource2.Prop11) }.Contains(propInfo.Name);

                    options.WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value);                    
                });

                var mapper = new ObjectMapper(configuration);

                var actual = mapper.Map<DummyTarget>(_source2);

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
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySource1, DummyTarget>(options =>
                {
                    options.Binding = BindingOptions.Public | BindingOptions.Private;

                    options.WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value);
                });

                configuration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options
                        .WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value)
                        .Exclude(src => src.Prop10)
                        .Exclude(src => src.Prop11);                    
                });

                var mapper = new ObjectMapper(configuration);

                var actual = mapper.Map<DummyTarget>(_source2);

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
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySource1, DummyTarget>(options =>
                {
                    options.Binding = BindingOptions.Public | BindingOptions.Internal;

                    options.WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value);
                });

                configuration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options
                        .WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value)
                        .Exclude(src => src.Prop10)
                        .Exclude(src => src.Prop11);
                });

                var mapper = new ObjectMapper(configuration);

                var actual = mapper.Map<DummyTarget>(_source2);

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
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.WithAlias(nameof(DummySource2.Prop7a), nameof(DummyTarget.Prop7b));

                    options.Binding = BindingOptions.Public | BindingOptions.Internal;

                    options
                        .WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value)
                        .Exclude(src => src.Prop10)
                        .Exclude(src => src.Prop11);
                });

                var mapper = new ObjectMapper(configuration);

                var actual = mapper.Map<DummyTarget>(_source2);

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
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Binding = BindingOptions.Public | BindingOptions.Internal;

                    options
                        .WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value)
                        .Exclude(src => src.Prop11)
                        .WithAlias(source => source.Prop7a, target => target.Prop7b);
                });

                var mapper = new ObjectMapper(configuration);

                var actual = mapper.Map<DummyTarget>(_source2);

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
            public void Should_Map_Exclude_Using_Predicate()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options
                        .Exclude(src => src.Prop13)
                        .ExcludeWhen(src => src.Prop11, value => value.NotAny())
                        .WithAlias(source => source.Prop7a, target => target.Prop7b);
                });

                var mapper = new ObjectMapper(configuration);

                _source2.Prop11 = Array.Empty<string>();    // test ExcludeWhen()

                var actual = mapper.Map<DummyTarget>(_source2);

                var expected = new
                {
                    _source2.Prop1,
                    _source2.Prop3,
                    _source2.Prop5,
                    _source2.Prop6,
                    Prop7b = _source2.Prop7a,
                    Prop8 = default(int),
                    _source2.Prop9,
                    _source2.Prop10,
                    Prop11 = default(IReadOnlyCollection<string>),
                    Prop12 = (int) _source2.Prop12,
                    Prop13 = default (DummyEnum)
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_WithConversion()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.WithConversion(nameof(DummySource2.Prop11), (mapper, value) => ((IEnumerable<string>) value).Reverse().AsReadOnlyCollection());

                    options.WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value);
                });

                var mapper = new ObjectMapper(configuration);

                var actual = mapper.Map<DummyTarget>(_source2);

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

            [Fact]
            public void Should_Shallow_Map_Nested_Properties()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummyRootParentSource, DummyRootParentTarget>(opt =>
                {
                    opt.Exclude(src => src.RootB);
                });

                var mapper = new ObjectMapper(configuration);

                var source = new DummyRootParentSource();

                var actual = mapper.Map<DummyRootParentTarget>(source);

                actual.RootA.Should().BeSameAs(source.RootA);
            }

            [Fact]
            public void Should_Auto_Convert_Nested_Value_Type_Properties()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummyRootParentSource, DummyRootParentTarget>(opt =>
                {
                    opt.Exclude(src => src.RootA);
                });

                var mapper = new ObjectMapper(configuration);

                var source = new DummyRootParentSource();

                var actual = mapper.Map<DummyRootParentTarget>(source);

                actual.RootB.Prop1.Should().Be((double) source.RootB.Prop1);
            }

            [Fact]
            public void Should_Deep_Clone_Nested_Properties()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummyRootParentSource, DummyRootParentTarget>(opt =>
                {
                    opt.DeepCopy(src => src.RootA);
                });

                var mapper = new ObjectMapper(configuration);

                var source = new DummyRootParentSource();

                var actual = mapper.Map<DummyRootParentTarget>(source);

                actual.RootA.Should().NotBeSameAs(source.RootA);                // deep cloned
                actual.RootA.Prop2a.Should().NotBeSameAs(source.RootA.Prop2a);  // deep cloned
                actual.RootB.Should().NotBeSameAs(source.RootB);                // source and target types are different
                actual.RootC.Should().BeSameAs(source.RootC);                   // not deep cloned

                var expected = new
                {
                    RootA = new
                    {
                        Prop1 = source.RootA.Prop1,
                        Prop2a = new
                        {
                            Prop2 = source.RootA.Prop2a.Prop2,
                            Prop3 = source.RootA.Prop2a.Prop3
                        },
                        Prop2b = new
                        {
                            Prop2 = source.RootA.Prop2b.Prop2,
                            Prop3 = source.RootA.Prop2b.Prop3
                        }
                    },
                    RootB = new
                    {
                        Prop1 = source.RootB.Prop1,
                        Prop2a = new
                        {
                            Prop2 = source.RootB.Prop2a.Prop2,
                            Prop3 = source.RootB.Prop2a.Prop3
                        },
                        Prop2b = new
                        {
                            Prop2 = source.RootB.Prop2b.Prop2,
                            Prop3 = source.RootB.Prop2b.Prop3
                        }
                    },
                    RootC = new
                    {
                        Prop1 = source.RootC.Prop1,
                        Prop2a = new
                        {
                            Prop2 = source.RootC.Prop2a.Prop2,
                            Prop3 = source.RootC.Prop2a.Prop3
                        },
                        Prop2b = new
                        {
                            Prop2 = source.RootC.Prop2b.Prop2,
                            Prop3 = source.RootC.Prop2b.Prop3
                        }
                    }
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Enumerables_Different_Types()
            {
                var mapper = new ObjectMapper();

                // Configuration is only required for upfront performance gains.
                // mapper.Configure<DummyEnumerableRootSource, DummyEnumerableRootTarget>();
                // mapper.Configure<DummyRootParentSource, DummyRootParentTarget>();

                var source = Create<DummyEnumerableRootSource>();

                var actual = mapper.Map<DummyEnumerableRootTarget>(source);

                actual.Prop1.Should().NotBeEmpty();

                actual.Prop1.ForEach((prop, index) =>
                {
                    var sourceItem = source.Prop1.ElementAt(index);

                    var expected = new
                    {
                        RootA = new
                        {
                            Prop1 = sourceItem.RootA.Prop1,
                            Prop2a = new
                            {
                                Prop2 = sourceItem.RootA.Prop2a.Prop2,
                                Prop3 = sourceItem.RootA.Prop2a.Prop3       // this is an IEnumerable<int>
                            },
                            Prop2b = new
                            {
                                Prop2 = sourceItem.RootA.Prop2b.Prop2,
                                Prop3 = sourceItem.RootA.Prop2b.Prop3       // this is an IEnumerable<int>
                            }
                        },
                        RootB = new
                        {
                            Prop1 = sourceItem.RootB.Prop1,
                            Prop2a = new
                            {
                                Prop2 = sourceItem.RootB.Prop2a.Prop2,
                                Prop3 = sourceItem.RootB.Prop2a.Prop3       // this is an IEnumerable<int>
                            },
                            Prop2b = new
                            {
                                Prop2 = sourceItem.RootB.Prop2b.Prop2,
                                Prop3 = sourceItem.RootB.Prop2b.Prop3       // this is an IEnumerable<int>
                            }
                        },
                        RootC = new
                        {
                            Prop1 = sourceItem.RootC.Prop1,
                            Prop2a = new
                            {
                                Prop2 = sourceItem.RootC.Prop2a.Prop2,
                                Prop3 = sourceItem.RootC.Prop2a.Prop3       // this is an IEnumerable<int>
                            },
                            Prop2b = new
                            {
                                Prop2 = sourceItem.RootC.Prop2b.Prop2,
                                Prop3 = sourceItem.RootC.Prop2b.Prop3       // this is an IEnumerable<int>
                            }
                        }
                    };

                    expected.Should().BeEquivalentTo(prop);
                });
            }

            [Fact]
            public void Should_Map_Abstract_Target()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySourceHost, DummyAbstractTarget>(opt =>
                {
                    opt.WithConversion(src => src.Prop1, (mapper2, value) =>
                    {
                        // value is a 'DummySource1' and Prop1 on 'DummyAbstractTarget' is abstract (DummyAbstractBase)
                        // so this will map it to the concrete type 'DummyConcrete2'
                        return mapper2.Map<DummyConcrete2>(value);
                    });
                });

                var mapper = new ObjectMapper(configuration);

                var source = Create<DummySourceHost>();

                var actual = mapper.Map<DummyAbstractTarget>(source);

                actual.Prop1.Prop1.Should().Be(source.Prop1.Prop1);
            }

            [Fact]
            public void Should_Map_Enumerable_Using_MapMany()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummyEnumerableRootSource, DummyEnumerableRootTarget>(opt =>
                {
                    // This approach is not required for mapping enumerables of different types - it's only written this way for the test
                    opt.WithConversion(src => src.Prop1, (mapper2, value) => mapper2.MapMany<DummyRootParentTarget>(value));
                });

                var mapper = new ObjectMapper(configuration);

                var source = Create<DummyEnumerableRootSource>();

                var actual = mapper.Map<DummyEnumerableRootTarget>(source);

                actual.Prop1.ForEach((prop, index) =>
                {
                    var sourceItem = source.Prop1.ElementAt(index);

                    var expected = new
                    {
                        RootA = new
                        {
                            Prop1 = sourceItem.RootA.Prop1,
                            Prop2a = new
                            {
                                Prop2 = sourceItem.RootA.Prop2a.Prop2,
                                Prop3 = sourceItem.RootA.Prop2a.Prop3       // this is an IEnumerable<int>
                            },
                            Prop2b = new
                            {
                                Prop2 = sourceItem.RootA.Prop2b.Prop2,
                                Prop3 = sourceItem.RootA.Prop2b.Prop3       // this is an IEnumerable<int>
                            }
                        },
                        RootB = new
                        {
                            Prop1 = sourceItem.RootB.Prop1,
                            Prop2a = new
                            {
                                Prop2 = sourceItem.RootB.Prop2a.Prop2,
                                Prop3 = sourceItem.RootB.Prop2a.Prop3       // this is an IEnumerable<int>
                            },
                            Prop2b = new
                            {
                                Prop2 = sourceItem.RootB.Prop2b.Prop2,
                                Prop3 = sourceItem.RootB.Prop2b.Prop3       // this is an IEnumerable<int>
                            }
                        },
                        RootC = new
                        {
                            Prop1 = sourceItem.RootC.Prop1,
                            Prop2a = new
                            {
                                Prop2 = sourceItem.RootC.Prop2a.Prop2,
                                Prop3 = sourceItem.RootC.Prop2a.Prop3       // this is an IEnumerable<int>
                            },
                            Prop2b = new
                            {
                                Prop2 = sourceItem.RootC.Prop2b.Prop2,
                                Prop3 = sourceItem.RootC.Prop2b.Prop3       // this is an IEnumerable<int>
                            }
                        }
                    };

                    expected.Should().BeEquivalentTo(prop);
                });
            }

            [Fact]
            public void Should_Map_To_ObservableCollection()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySource1, DummyObservableCollectionHost>(opt =>
                {
                    opt.Exclude(src => src.Prop1);                                  // Conflicts with Prop1 on the target
                    opt.WithAlias(src => src.Prop9, target => target.Prop1);        // Alias Prop9 to Prop1 on the target

                    // This is a typical example of how to map to something other than a list
                    opt.WithConversion(src => src.Prop9, (mapper2, value) =>
                    {
                        return new ObservableCollection<string>(value.ToList());
                    });
                });

                var mapper = new ObjectMapper(configuration);

                var source = Create<DummySource1>();

                var actual = mapper.Map<DummyObservableCollectionHost>(source);

                actual.Prop1.Should().BeEquivalentTo(source.Prop9);
            }

            [Fact]
            public void Should_Shallow_Copy_Dictionary_Values()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummyDictionarySource, DummyDictionaryTarget>(opt =>
                {
                    // Excluding as the source destination are different types - would never be shallow copied
                    opt.Exclude(src => src.Prop2);
                });

                var mapper = new ObjectMapper(configuration);

                var source = Create<DummyDictionarySource>();

                var actual = mapper.Map<DummyDictionaryTarget>(source);

                actual.Prop1.Should().BeSameAs(source.Prop1);
            }

            [Fact]
            public void Should_Map_Enumerable_To_Array()
            {
                var mapper = new ObjectMapper();

                var source = Create<DummyEnumerableSource>();

                var actual = mapper.Map<DummyArrayTarget>(source);

                actual.Prop1.Should().BeEquivalentTo(source.Prop1);
            }

            [Fact]
            public void Should_Throw_When_Factory_Not_Defined()
            {
                var mapper = new ObjectMapper();

                var source = Create<DummyDictionarySource>();

                Invoking(() =>
                {
                    _ = mapper.Map<DummyDictionaryTarget>(source);
                })
                    .Should()
                    .Throw<ObjectMapperException>()
                    .WithMessage($"The type '{typeof(KeyValuePair<string, int>).GetFriendlyName()}' cannot be assigned to type '{typeof(KeyValuePair<string, double>).GetFriendlyName()}'.");
            }

            [Fact]
            public void Should_Deep_Copy_Dictionary_Values()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<KeyValuePair<string, int>, KeyValuePair<string, double>>(opt =>
                {
                    opt.ConstructUsing((mapper, value) => new KeyValuePair<string, double>(value.Key, (double) value.Value));
                });

                configuration.Configure<DummyDictionarySource, DummyDictionaryTarget>(opt =>
                {
                    opt.DeepCopy(src => src.Prop1);
                });

                var objectMapper = new ObjectMapper(configuration);

                var source = Create<DummyDictionarySource>();

                var actual = objectMapper.Map<DummyDictionaryTarget>(source);

                actual.Prop1.Should().NotBeSameAs(source.Prop1);
                source.Prop1.Should().BeEquivalentTo(actual.Prop1);

                var expected = source.Prop2
                    .Select(kvp => new KeyValuePair<string, double>(kvp.Key, (double) kvp.Value))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                expected.Should().BeEquivalentTo(actual.Prop2);
            }
        }

        public class Map_Source_Target : ObjectMapperFixture
        {
            [Fact]
            public void Should_Throw_When_Source_Null()
            {
                Invoking(() =>
                {
                    var mapper = new ObjectMapper();

                    _ = mapper.Map<DummySource1, DummyTarget>(null, _target);
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
                    var mapper = new ObjectMapper();

                    _ = mapper.Map<DummySource1, DummyTarget>(_source1, null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithParameterName("target");
            }

            [Fact]
            public void Should_Return_Same_Target()
            {
                var configuration = GetCommonMapperConfiguration();

                var mapper = new ObjectMapper(configuration);

                var actual = mapper.Map<DummySource1, DummyTarget>(_source1, _target);

                actual.Should().BeSameAs(_target);
            }

            [Fact]
            public void Should_Not_Throw_When_Not_Configured_For_Compatible_Types()
            {
                var configuration = GetCommonMapperConfiguration();

                var mapper = new ObjectMapper(configuration);

                // _source2 would fail because it needs a conversion from IReadOnlyCollection to IEnumerable on Prop10
                Invoking(() => mapper.Map<DummySource1, DummyTarget>(_source1, _target))
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Default_Map()
            {
                var configuration = GetCommonMapperConfiguration();

                var mapper = new ObjectMapper(configuration);

                var actual = mapper.Map<DummySource1, DummyTarget>(_source1, _target);

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
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Filter = propInfo =>
                        !new[] { nameof(DummySource2.Prop10), nameof(DummySource2.Prop8), nameof(DummySource2.Prop11) }.Contains(propInfo.Name);

                    options.WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value);
                });

                var mapper = new ObjectMapper(configuration);

                var actual = mapper.Map<DummySource2, DummyTarget>(_source2, _target);

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
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Binding = BindingOptions.Public | BindingOptions.Private;

                    options
                        .WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value)
                        .Exclude(src => src.Prop10)
                        .Exclude(src => src.Prop11);
                });

                var mapper = new ObjectMapper(configuration);

                var actual = mapper.Map<DummySource2, DummyTarget>(_source2, _target);

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
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Binding = BindingOptions.Public | BindingOptions.Internal;

                    options
                        .WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value)
                        .Exclude(src => src.Prop10)
                        .Exclude(src => src.Prop11);
                });

                var mapper = new ObjectMapper(configuration);

                var actual = mapper.Map<DummySource2, DummyTarget>(_source2, _target);

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
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Binding = BindingOptions.Public | BindingOptions.Internal;

                    options.WithAlias(nameof(DummySource2.Prop7a), nameof(DummyTarget.Prop7b));

                    options
                        .WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value)
                        .Exclude(src => src.Prop10)
                        .Exclude(src => src.Prop11);
                });

                var mapper = new ObjectMapper(configuration);

                var actual = mapper.Map<DummySource2, DummyTarget>(_source2, _target);

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
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.Binding = BindingOptions.Public | BindingOptions.Internal;

                    options
                        .WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value)
                        .Exclude(src => src.Prop11)
                        .WithAlias(source => source.Prop7a, target => target.Prop7b);
                });

                var mapper = new ObjectMapper(configuration);

                var actual = mapper.Map<DummySource2, DummyTarget>(_source2, _target);

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
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySource2, DummyTarget>(options =>
                {
                    options.WithConversion(nameof(DummySource2.Prop11), (mapper, value) => ((IEnumerable<string>) value).Reverse().AsReadOnlyCollection());

                    options.WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value);
                });

                var mapper = new ObjectMapper(configuration);

                var actual = mapper.Map<DummySource2, DummyTarget>(_source2, _target);

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

            [Fact]
            public void Should_Shallow_Map_Nested_Properties()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummyRootParentSource, DummyRootParentTarget>(opt =>
                {
                    opt.Exclude(src => src.RootB);
                });

                var mapper = new ObjectMapper(configuration);

                var source = new DummyRootParentSource();
                var actual = new DummyRootParentTarget();

                _ = mapper.Map<DummyRootParentSource, DummyRootParentTarget>(source, actual);

                actual.RootA.Should().BeSameAs(source.RootA);
            }

            [Fact]
            public void Should_Auto_Convert_Nested_Value_Type_Properties()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummyRootParentSource, DummyRootParentTarget>(opt =>
                {
                    opt.Exclude(src => src.RootA);
                });

                var mapper = new ObjectMapper(configuration);

                var source = new DummyRootParentSource();
                var actual = new DummyRootParentTarget();

                _ = mapper.Map<DummyRootParentSource, DummyRootParentTarget>(source, actual);

                actual.RootB.Prop1.Should().Be((double) source.RootB.Prop1);
            }

            [Fact]
            public void Should_Deep_Clone_Nested_Properties()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummyRootParentSource, DummyRootParentTarget>(opt =>
                {
                    opt.DeepCopy(src => src.RootA);
                });

                var mapper = new ObjectMapper(configuration);

                var source = new DummyRootParentSource();
                var actual = new DummyRootParentTarget();

                _ = mapper.Map<DummyRootParentSource, DummyRootParentTarget>(source, actual);

                actual.RootA.Should().NotBeSameAs(source.RootA);                // deep cloned
                actual.RootA.Prop2a.Should().NotBeSameAs(source.RootA.Prop2a);  // deep cloned
                actual.RootB.Should().NotBeSameAs(source.RootB);                // source and target types are different
                actual.RootC.Should().BeSameAs(source.RootC);                   // not deep cloned

                var expected = new
                {
                    RootA = new
                    {
                        Prop1 = source.RootA.Prop1,
                        Prop2a = new
                        {
                            Prop2 = source.RootA.Prop2a.Prop2,
                            Prop3 = source.RootA.Prop2a.Prop3
                        },
                        Prop2b = new
                        {
                            Prop2 = source.RootA.Prop2b.Prop2,
                            Prop3 = source.RootA.Prop2b.Prop3
                        }
                    },
                    RootB = new
                    {
                        Prop1 = source.RootB.Prop1,
                        Prop2a = new
                        {
                            Prop2 = source.RootB.Prop2a.Prop2,
                            Prop3 = source.RootB.Prop2a.Prop3
                        },
                        Prop2b = new
                        {
                            Prop2 = source.RootB.Prop2b.Prop2,
                            Prop3 = source.RootB.Prop2b.Prop3
                        }
                    },
                    RootC = new
                    {
                        Prop1 = source.RootC.Prop1,
                        Prop2a = new
                        {
                            Prop2 = source.RootC.Prop2a.Prop2,
                            Prop3 = source.RootC.Prop2a.Prop3
                        },
                        Prop2b = new
                        {
                            Prop2 = source.RootC.Prop2b.Prop2,
                            Prop3 = source.RootC.Prop2b.Prop3
                        }
                    }
                };

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Map_Enumerables_Different_Types()
            {
                var mapper = new ObjectMapper();

                // Configuration is only required for upfront performance gains.
                // mapper.Configure<DummyEnumerableRootSource, DummyEnumerableRootTarget>();
                // mapper.Configure<DummyRootParentSource, DummyRootParentTarget>();

                var source = Create<DummyEnumerableRootSource>();
                var actual = new DummyEnumerableRootTarget();

                _ = mapper.Map<DummyEnumerableRootSource, DummyEnumerableRootTarget>(source, actual);

                actual.Prop1.Should().NotBeEmpty();

                actual.Prop1.ForEach((prop, index) =>
                {
                    var sourceItem = source.Prop1.ElementAt(index);

                    var expected = new
                    {
                        RootA = new
                        {
                            Prop1 = sourceItem.RootA.Prop1,
                            Prop2a = new
                            {
                                Prop2 = sourceItem.RootA.Prop2a.Prop2,
                                Prop3 = sourceItem.RootA.Prop2a.Prop3       // this is an IEnumerable<int>
                            },
                            Prop2b = new
                            {
                                Prop2 = sourceItem.RootA.Prop2b.Prop2,
                                Prop3 = sourceItem.RootA.Prop2b.Prop3       // this is an IEnumerable<int>
                            }
                        },
                        RootB = new
                        {
                            Prop1 = sourceItem.RootB.Prop1,
                            Prop2a = new
                            {
                                Prop2 = sourceItem.RootB.Prop2a.Prop2,
                                Prop3 = sourceItem.RootB.Prop2a.Prop3       // this is an IEnumerable<int>
                            },
                            Prop2b = new
                            {
                                Prop2 = sourceItem.RootB.Prop2b.Prop2,
                                Prop3 = sourceItem.RootB.Prop2b.Prop3       // this is an IEnumerable<int>
                            }
                        },
                        RootC = new
                        {
                            Prop1 = sourceItem.RootC.Prop1,
                            Prop2a = new
                            {
                                Prop2 = sourceItem.RootC.Prop2a.Prop2,
                                Prop3 = sourceItem.RootC.Prop2a.Prop3       // this is an IEnumerable<int>
                            },
                            Prop2b = new
                            {
                                Prop2 = sourceItem.RootC.Prop2b.Prop2,
                                Prop3 = sourceItem.RootC.Prop2b.Prop3       // this is an IEnumerable<int>
                            }
                        }
                    };

                    expected.Should().BeEquivalentTo(prop);
                });
            }

            [Fact]
            public void Should_Map_Abstract_Target()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySourceHost, DummyAbstractTarget>(opt =>
                {
                    opt.WithConversion(src => src.Prop1, (mapper2, value) =>
                    {
                        // value is a 'DummySource1' and Prop1 on 'DummyAbstractTarget' is abstract (DummyAbstractBase)
                        // so this will map it to the concrete type 'DummyConcrete2'
                        return mapper2.Map<DummyConcrete2>(value);
                    });
                });

                var mapper = new ObjectMapper(configuration);

                var source = Create<DummySourceHost>();
                var actual = new DummyAbstractTarget();

                _ = mapper.Map<DummySourceHost, DummyAbstractTarget>(source, actual);

                actual.Prop1.Prop1.Should().Be(source.Prop1.Prop1);
            }

            [Fact]
            public void Should_Map_Enumerable_Using_MapMany()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummyEnumerableRootSource, DummyEnumerableRootTarget>(opt =>
                {
                    // This approach is not required for mapping enumerables of different types - it's only written this way for the test
                    opt.WithConversion(src => src.Prop1, (mapper2, value) => mapper2.MapMany<DummyRootParentTarget>(value));
                });

                var mapper = new ObjectMapper(configuration);

                var source = Create<DummyEnumerableRootSource>();
                var actual = new DummyEnumerableRootTarget();

                _ = mapper.Map<DummyEnumerableRootSource, DummyEnumerableRootTarget>(source, actual);

                actual.Prop1.ForEach((prop, index) =>
                {
                    var sourceItem = source.Prop1.ElementAt(index);

                    var expected = new
                    {
                        RootA = new
                        {
                            Prop1 = sourceItem.RootA.Prop1,
                            Prop2a = new
                            {
                                Prop2 = sourceItem.RootA.Prop2a.Prop2,
                                Prop3 = sourceItem.RootA.Prop2a.Prop3       // this is an IEnumerable<int>
                            },
                            Prop2b = new
                            {
                                Prop2 = sourceItem.RootA.Prop2b.Prop2,
                                Prop3 = sourceItem.RootA.Prop2b.Prop3       // this is an IEnumerable<int>
                            }
                        },
                        RootB = new
                        {
                            Prop1 = sourceItem.RootB.Prop1,
                            Prop2a = new
                            {
                                Prop2 = sourceItem.RootB.Prop2a.Prop2,
                                Prop3 = sourceItem.RootB.Prop2a.Prop3       // this is an IEnumerable<int>
                            },
                            Prop2b = new
                            {
                                Prop2 = sourceItem.RootB.Prop2b.Prop2,
                                Prop3 = sourceItem.RootB.Prop2b.Prop3       // this is an IEnumerable<int>
                            }
                        },
                        RootC = new
                        {
                            Prop1 = sourceItem.RootC.Prop1,
                            Prop2a = new
                            {
                                Prop2 = sourceItem.RootC.Prop2a.Prop2,
                                Prop3 = sourceItem.RootC.Prop2a.Prop3       // this is an IEnumerable<int>
                            },
                            Prop2b = new
                            {
                                Prop2 = sourceItem.RootC.Prop2b.Prop2,
                                Prop3 = sourceItem.RootC.Prop2b.Prop3       // this is an IEnumerable<int>
                            }
                        }
                    };

                    expected.Should().BeEquivalentTo(prop);
                });
            }

            [Fact]
            public void Should_Map_To_ObservableCollection()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummySource1, DummyObservableCollectionHost>(opt =>
                {
                    opt.Exclude(src => src.Prop1);                                  // Conflicts with Prop1 on the target
                    opt.WithAlias(src => src.Prop9, target => target.Prop1);        // Alias Prop9 to Prop1 on the target

                    // This is a typical example of how to map to something other than a list
                    opt.WithConversion(src => src.Prop9, (mapper2, value) =>
                    {
                        return new ObservableCollection<string>(value.ToList());
                    });
                });

                var mapper = new ObjectMapper(configuration);

                var source = Create<DummySource1>();
                var actual = new DummyObservableCollectionHost();

                _ = mapper.Map<DummySource1, DummyObservableCollectionHost>(source, actual);

                actual.Prop1.Should().BeEquivalentTo(source.Prop9);
            }

            [Fact]
            public void Should_Shallow_Copy_Dictionary_Values()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummyDictionarySource, DummyDictionaryTarget>(opt =>
                {
                    // Excluding as the source destination are different types - would never be shallow copied
                    opt.Exclude(src => src.Prop2);
                });

                var mapper = new ObjectMapper(configuration);

                var source = Create<DummyDictionarySource>();
                var actual = new DummyDictionaryTarget();

                _ = mapper.Map<DummyDictionarySource, DummyDictionaryTarget>(source, actual);

                actual.Prop1.Should().BeSameAs(source.Prop1);
            }

            [Fact]
            public void Should_Map_Enumerable_To_Array()
            {
                var mapper = new ObjectMapper();

                var source = Create<DummyEnumerableSource>();
                var actual = new DummyArrayTarget();

                _ = mapper.Map<DummyEnumerableSource, DummyArrayTarget>(source, actual);

                actual.Prop1.Should().BeEquivalentTo(source.Prop1);
            }

            [Fact]
            public void Should_Map_To_Null_Array()
            {
                var configuration = new ObjectMapperConfiguration(options =>
                {
                    options.AllowNullCollections = true;
                });

                var mapper = new ObjectMapper(configuration);

                var source = new DummyEnumerableSource();

                var actual = new DummyArrayTarget();

                _ = mapper.Map<DummyEnumerableSource, DummyArrayTarget>(source, actual);

                actual.Prop1.Should().BeNull();
            }

            [Fact]
            public void Should_Default_Map_To_Empty_Array()
            {
                var mapper = new ObjectMapper();

                var source = new DummyEnumerableSource();

                var actual = new DummyArrayTarget();

                _ = mapper.Map<DummyEnumerableSource, DummyArrayTarget>(source, actual);

                actual.Prop1.Should().BeEmpty();
            }

            [Fact]
            public void Should_Default_Map_To_Empty_Dictionary()
            {
                var objectMapper = new ObjectMapper();

                var source = new DummyDictionarySource();
                var actual = new DummyDictionaryTarget();

                _ = objectMapper.Map<DummyDictionarySource, DummyDictionaryTarget>(source, actual);

                actual.Prop1.Should().BeEmpty();
                actual.Prop2.Should().BeEmpty();
            }

            [Fact]
            public void Should_Throw_When_Factory_Not_Defined()
            {
                var mapper = new ObjectMapper();

                var source = Create<DummyDictionarySource>();
                var actual = new DummyDictionaryTarget();

                Invoking(() =>
                {
                    _ = mapper.Map<DummyDictionarySource, DummyDictionaryTarget>(source, actual);
                })
                    .Should()
                    .Throw<ObjectMapperException>()
                    .WithMessage($"The type '{typeof(KeyValuePair<string, int>).GetFriendlyName()}' cannot be assigned to type '{typeof(KeyValuePair<string, double>).GetFriendlyName()}'.");
            }

            [Fact]
            public void Should_Deep_Copy_Dictionary_Values()
            {
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<KeyValuePair<string, int>, KeyValuePair<string, double>>(opt =>
                {
                    opt.ConstructUsing((mapper, value) => new KeyValuePair<string, double>(value.Key, (double) value.Value));
                });

                configuration.Configure<DummyDictionarySource, DummyDictionaryTarget>(opt =>
                {
                    opt.DeepCopy(src => src.Prop1);
                });

                var objectMapper = new ObjectMapper(configuration);

                var source = Create<DummyDictionarySource>();
                var actual = new DummyDictionaryTarget();

                _ = objectMapper.Map<DummyDictionarySource, DummyDictionaryTarget>(source, actual);

                actual.Prop1.Should().NotBeSameAs(source.Prop1);
                source.Prop1.Should().BeEquivalentTo(actual.Prop1);

                var expected = source.Prop2
                    .Select(kvp => new KeyValuePair<string, double>(kvp.Key, (double) kvp.Value))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                expected.Should().BeEquivalentTo(actual.Prop2);
            }
        }

        private ObjectMapperConfiguration GetCommonMapperConfiguration()
        {
            var configuration = new ObjectMapperConfiguration();

            configuration.Configure<DummySource1, DummyTarget>(opt =>
            {
                opt.WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value);
            });

            configuration.Configure<DummySource2, DummyTarget>(opt =>
            {
                opt.WithConversion(src => src.Prop13, (mapper, value) => (DummyEnum) value);
            });

            return configuration;
        }

        private static IEnumerable<(string SourceName, Type SourceType, string TargetName, Type TargetType)>
            GetMatchesNameAndType(IEnumerable<ObjectPropertyMatcher.PropertyMatchInfo> matches)
        {
            return matches.Select(
                match => (match.SourceInfo.Name, match.SourceInfo.PropertyType,
                          match.TargetInfo.Name, match.TargetInfo.PropertyType));
        }
    }
}