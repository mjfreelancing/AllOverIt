using System;
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
            public DummyChild Child { get; set; }
        }

        private readonly TypedObjectMapperOptions<DummySource, DummyTarget> _options = new();

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
                    .WithMessage("ObjectMapper do not support nested mappings (source => source.Child.Prop1)");
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

        public class WithAlias : TypedObjectMapperOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_SourceExpression_Null()
            {
                Invoking(() =>
                    {
                        _options.WithAlias(null, target => target.Prop1);
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
                        _options.WithAlias<int>(source => source.Prop2, null);
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
                    .WithMessage("ObjectMapper do not support nested mappings (source => source.Child.Prop1)");
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
                    .WithMessage("ObjectMapper do not support nested mappings (target => target.Child.Prop1)");
            }

            [Fact]
            public void Should_Set_Alias()
            {
                _ = _options.WithAlias(source => source.Prop3, target => target.Prop2);

                _options.GetAliasName(nameof(DummySource.Prop3)).Should().Be(nameof(DummyTarget.Prop2));
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
                        _options.WithConversion<int>(null, value => value);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceExpression");
            }

            [Fact]
            public void Should_Not_Allow_Source_Nested_Properties()
            {
                Invoking(() =>
                    {
                        _options.WithConversion(source => source.Child.Prop1, val => val);
                    })
                    .Should()
                    .Throw<ObjectMapperException>()
                    .WithMessage("ObjectMapper do not support nested mappings (source => source.Child.Prop1)");
            }

            [Fact]
            public void Should_Throw_When_Converter_Null()
            {
                Invoking(() =>
                    {
                        _options.WithConversion(source => source.Prop3, null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("converter");
            }

            [Fact]
            public void Should_Set_Converter()
            {
                var value = Create<int>();
                var factor = Create<int>();

                _options.WithConversion(source => source.Prop2, val => val * factor);

                var actual = _options.GetConvertedValue(nameof(DummySource.Prop2), value);

                actual.Should().BeEquivalentTo(value * factor);
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var actual = _options.WithConversion(source => source.Prop3, value => value);

                actual.Should().Be(_options);
            }
        }
    }
}