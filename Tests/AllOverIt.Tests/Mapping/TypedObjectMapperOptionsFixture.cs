﻿using System;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Mapping;
using AllOverIt.Mapping.Exceptions;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Mapping
{
    public class TypedObjectMapperOptionsFixture : FixtureBase
    {
        private class DummyChild
        {
            public int Prop1 { get; set; }
        }

        private class DummySource
        {
            public int Prop1 { get; set; }
            public int Prop2 { get; set; }
            public int Prop3 { get; set; }
            public DummyChild Child { get; set; }
        }

        private class DummyTarget
        {
            public int Prop1 { get; set; }
            public int Prop2 { get; set; }
            public double Prop3 { get; set; }
            public DummyChild Child { get; set; }
        }

        private readonly ObjectMapper _mapper;
        private TypedObjectMapperOptions<DummySource, DummyTarget> _options;

        public TypedObjectMapperOptionsFixture()
        {
            _mapper = new();
            _options = new(_mapper, (source, target, factory) => { });
        }

        public class Constructor : TypedObjectMapperOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_Mapper_Null()
            {
                Invoking(() =>
                {
                    _ = new TypedObjectMapperOptions<DummySource, DummyTarget>(null, (source, target, factory) => { });
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("mapper");
            }
        }

        public class Exclude : TypedObjectMapperOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_SourceExpression_Null()
            {
                Invoking(() =>
                    {
                        _options.Exclude<int>(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceExpression");
            }

            [Fact]
            public void Should_Not_Allow_Nested_Properties()
            {
                Invoking(() =>
                    {
                        _options.Exclude(source => source.Child.Prop1);
                    })
                    .Should()
                    .Throw<ObjectMapperException>()
                    .WithMessage("ObjectMapper do not support nested mappings (source => source.Child.Prop1).");
            }

            [Fact]
            public void Should_Exclude_Name()
            {
                _options.Exclude(source => source.Prop2);

                _options.IsExcluded(nameof(DummySource.Prop2)).Should().BeTrue();
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var actual = _options.Exclude(source => source.Prop2);

                actual.Should().Be(_options);
            }
        }

        public class DeepClone : TypedObjectMapperOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_SourceExpression_Null()
            {
                Invoking(() =>
                {
                    _options.DeepClone<int>(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceExpression");
            }

            [Fact]
            public void Should_Not_Allow_Nested_Properties()
            {
                Invoking(() =>
                {
                    _options.DeepClone(source => source.Child.Prop1);
                })
                    .Should()
                    .Throw<ObjectMapperException>()
                    .WithMessage("ObjectMapper do not support nested mappings (source => source.Child.Prop1).");
            }

            [Fact]
            public void Should_DeepClone_Name()
            {
                _options.DeepClone(source => source.Child);

                _options.IsDeepClone(nameof(DummySource.Child)).Should().BeTrue();
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var actual = _options.DeepClone(source => source.Child);

                actual.Should().Be(_options);
            }
        }

        public class WithAlias : TypedObjectMapperOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_SourceExpression_Null()
            {
                Invoking(() =>
                    {
                        _options.WithAlias<int, int>(null, target => target.Prop1);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceExpression");
            }

            [Fact]
            public void Should_Throw_When_TargetExpression_Null()
            {
                Invoking(() =>
                    {
                        _options.WithAlias<int, int>(source => source.Prop2, null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("targetExpression");
            }

            [Fact]
            public void Should_Not_Allow_Source_Nested_Properties()
            {
                Invoking(() =>
                    {
                        _options.WithAlias(source => source.Child.Prop1, target => target.Prop2);
                    })
                    .Should()
                    .Throw<ObjectMapperException>()
                    .WithMessage("ObjectMapper do not support nested mappings (source => source.Child.Prop1).");
            }

            [Fact]
            public void Should_Not_Allow_Target_Nested_Properties()
            {
                Invoking(() =>
                    {
                        _options.WithAlias(source => source.Prop1, target => target.Child.Prop1);
                    })
                    .Should()
                    .Throw<ObjectMapperException>()
                    .WithMessage("ObjectMapper do not support nested mappings (target => target.Child.Prop1).");
            }

            [Fact]
            public void Should_Set_Alias()
            {
                _ = _options.WithAlias(source => source.Prop3, target => target.Prop2);

                _options.GetAliasName(nameof(DummySource.Prop3)).Should().Be(nameof(DummyTarget.Prop2));
            }

            [Fact]
            public void Should_Set_Alias_Different_Types()
            {
                _ = _options.WithAlias(source => source.Prop1, target => target.Prop3);

                _options.GetAliasName(nameof(DummySource.Prop1)).Should().Be(nameof(DummyTarget.Prop3));
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var actual = _options.WithAlias(source => source.Prop3, target => target.Prop2);

                actual.Should().Be(_options);
            }
        }

        public class WithConversion : TypedObjectMapperOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_SourceExpression_Null()
            {
                Invoking(() =>
                {
                    _options.WithConversion<int>(null, (mapper, value) => value);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceExpression");
            }

            [Fact]
            public void Should_Throw_When_Converter_Null()
            {
                Invoking(() =>
                {
                    _options.WithConversion<int>(source => source.Prop3, null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("converter");
            }

            [Fact]
            public void Should_Provide_Mapper()
            {
                IObjectMapper actual = null;

                _options.WithConversion<int>(source => source.Prop3, (mapper, value) =>
                {
                    actual = mapper;
                    return value;
                });

                _ = _options.GetConvertedValue(nameof(DummySource.Prop3), Create<int>());

                actual.Should().BeSameAs(_mapper);
            }

            [Fact]
            public void Should_Not_Allow_Source_Nested_Properties()
            {
                Invoking(() =>
                    {
                        _options.WithConversion(source => source.Child.Prop1, (mapper, val) => val);
                    })
                    .Should()
                    .Throw<ObjectMapperException>()
                    .WithMessage("ObjectMapper do not support nested mappings (source => source.Child.Prop1).");
            }

            [Fact]
            public void Should_Set_Converter()
            {
                var value = Create<int>();
                var factor = Create<int>();

                _options.WithConversion(source => source.Prop2, (mapper, val) => val * factor);

                var actual = _options.GetConvertedValue(nameof(DummySource.Prop2), value);

                actual.Should().BeEquivalentTo(value * factor);
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var actual = _options.WithConversion(source => source.Prop3, (mapper, value) => value);

                actual.Should().Be(_options);
            }
        }

        public class ConstructUsing : TypedObjectMapperOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_TargetFactory_Null()
            {
                Invoking(() =>
                {
                    _options.ConstructUsing(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("targetFactory");
            }

            [Fact]
            public void Should_Provide_Mapper_When_Invoke_Factory()
            {
                // the mapper actually stores / invokes the factory

                IObjectMapper actual = null;

                _mapper.Configure<DummyChild, DummyChild>(opt =>
                {
                    opt.ConstructUsing((mapper, value) =>
                    {
                        actual = mapper;
                        return new DummyChild();
                    });
                });

                _mapper.Configure<DummySource, DummyTarget>(opt =>
                {
                    // need to force a deep clone for source / target properties of the same type
                    opt.DeepClone(src => src.Child);
                });

                var source = Create<DummySource>();

                _ = _mapper.Map<DummyTarget>(source);

                actual.Should().BeSameAs(_mapper);
            }

            [Fact]
            public void Should_Use_Factory()
            {
                // the mapper actually stores / invokes the factory

                DummyChild actual = null;
                var expected = Create<DummyChild>();

                _mapper.Configure<DummyChild, DummyChild>(opt =>
                {
                    opt.ConstructUsing((mapper, value) =>
                    {
                        actual = expected;
                        return expected;
                    });
                });

                _mapper.Configure<DummySource, DummyTarget>(opt =>
                {
                    // need to force a deep clone for source / target properties of the same type
                    opt.DeepClone(src => src.Child);
                });

                var source = Create<DummySource>();

                _ = _mapper.Map<DummyTarget>(source);

                expected.Should().BeSameAs(actual);
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var actual = _options.ConstructUsing((mapper, value) => default);

                actual.Should().Be(_options);
            }
        }
    }
}