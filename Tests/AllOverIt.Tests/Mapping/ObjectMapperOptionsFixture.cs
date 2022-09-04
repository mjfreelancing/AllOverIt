using System;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Mapping;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Mapping
{
    public class ObjectMapperOptionsFixture : FixtureBase
    {
        private readonly ObjectMapper _mapper;
        private readonly ObjectMapperOptions _options;

        protected ObjectMapperOptionsFixture()
        {
            _mapper = new();
            _options = new ObjectMapperOptions(_mapper);
        }

        public class Constructor : ObjectMapperOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_Mapper_Null()
            {
                Invoking(() =>
                {
                    // The object extensions canmap simple objects without a mapper
                    _ = new ObjectMapperOptions(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("mapper");
            }
        }

        public class Exclude : ObjectMapperOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_SourceNames_Null()
            {
                Invoking(() =>
                    {
                        _options.Exclude(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceNames");
            }

            [Fact]
            public void Should_Not_Throw_When_SourceNames_Empty()
            {
                Invoking(() =>
                    {
                        _options.Exclude(Array.Empty<string>());
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Exclude_Names()
            {
                var name = Create<string>();

                _options.Exclude(name);

                _options.IsExcluded(name).Should().BeTrue();
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var names = Create<string>();

                var actual = _options.Exclude(names);

                actual.Should().Be(_options);
            }
        }

        public class DeepClone : ObjectMapperOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_SourceNames_Null()
            {
                Invoking(() =>
                {
                    _options.DeepClone(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceNames");
            }

            [Fact]
            public void Should_Not_Throw_When_SourceNames_Empty()
            {
                Invoking(() =>
                {
                    _options.DeepClone(Array.Empty<string>());
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_DeepClone_Names()
            {
                var name = Create<string>();

                _options.DeepClone(name);

                _options.IsDeepClone(name).Should().BeTrue();
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var names = Create<string>();

                var actual = _options.DeepClone(names);

                actual.Should().Be(_options);
            }
        }

        public class WithAlias : ObjectMapperOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_SourceName_Null()
            {
                Invoking(() =>
                {
                    _options.WithAlias(null, Create<string>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceName");
            }

            [Fact]
            public void Should_Throw_When_SourceName_Empty()
            {
                Invoking(() =>
                {
                    _options.WithAlias(string.Empty, Create<string>());
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("sourceName");
            }

            [Fact]
            public void Should_Throw_When_SourceName_Whitespace()
            {
                Invoking(() =>
                {
                    _options.WithAlias("  ", Create<string>());
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("sourceName");
            }

            [Fact]
            public void Should_Throw_When_TargetName_Null()
            {
                Invoking(() =>
                {
                    _options.WithAlias(Create<string>(), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("targetName");
            }

            [Fact]
            public void Should_Throw_When_TargetName_Empty()
            {
                Invoking(() =>
                {
                    _options.WithAlias(Create<string>(), string.Empty);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("targetName");
            }

            [Fact]
            public void Should_Throw_When_TargetName_Whitespace()
            {
                Invoking(() =>
                {
                    _options.WithAlias(Create<string>(), "  ");
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("targetName");
            }

            [Fact]
            public void Should_Set_Alias()
            {
                var source = Create<string>();
                var target = Create<string>();

                _ = _options.WithAlias(source, target);

                _options.GetAliasName(source).Should().Be(target);
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var source = Create<string>();
                var target = Create<string>();

                var actual = _options.WithAlias(source, target);

                actual.Should().Be(_options);
            }
        }

        public class WithConversion : ObjectMapperOptionsFixture
        {
            private class DummySource
            {
                public int Prop1 { get; set; }
            }

            private class DummyTarget
            {
                public string Prop1 { get; set; }
            }

            [Fact]
            public void Should_Provide_Mapper()
            {
                var propName = Create<string>();

                // ObjectMapperOptions allows a null mapper, this test ensures it is provided if constructed with one
                IObjectMapper actual = null;

                _options.WithConversion(propName, (mapper, value) =>
                {
                    actual = mapper;
                    return value;
                });

                _ = _options.GetConvertedValue(propName, Create<int>());

                actual.Should().BeSameAs(_mapper);
            }

            [Fact]
            public void Should_Throw_When_SourceName_Null()
            {
                Invoking(() =>
                {
                    _options.WithConversion(null, (mapper, value) => value);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceName");
            }

            [Fact]
            public void Should_Throw_When_SourceName_Empty()
            {
                Invoking(() =>
                {
                    _options.WithConversion(string.Empty, (mapper, value) => value);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("sourceName");
            }

            [Fact]
            public void Should_Throw_When_SourceName_Whitespace()
            {
                Invoking(() =>
                {
                    _options.WithConversion("  ", (mapper, value) => value);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("sourceName");
            }

            [Fact]
            public void Should_Throw_When_Converter_Null()
            {
                Invoking(() =>
                {
                    _options.WithConversion(Create<string>(), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("converter");
            }

            [Fact]
            public void Should_Set_Converter()
            {
                var sourceName = Create<string>();
                var value = Create<int>();
                var factor = Create<int>();

                _options.WithConversion(sourceName, (mapper, val) => (int)val * factor);

                var actual = _options.GetConvertedValue(sourceName, value);

                actual.Should().BeEquivalentTo(value * factor);
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var actual = _options.WithConversion(Create<string>(), (mapper, value) => value);

                actual.Should().Be(_options);
            }
        }

        public class IsExcluded : ObjectMapperOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_SourceName_Null()
            {
                Invoking(() =>
                {
                    _options.IsExcluded(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceName");
            }

            [Fact]
            public void Should_Throw_When_SourceName_Empty()
            {
                Invoking(() =>
                {
                    _options.IsExcluded(string.Empty);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("sourceName");
            }

            [Fact]
            public void Should_Throw_When_SourceName_Whitespace()
            {
                Invoking(() =>
                {
                    _options.IsExcluded("  ");
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("sourceName");
            }

            [Fact]
            public void Should_Return_Is_Expected()
            {
                var sourceName = Create<string>();

                _options.Exclude(sourceName);

                _options.IsExcluded(sourceName);

                var actual = _options.IsExcluded(sourceName);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_Is_Not_Expected()
            {
                var sourceName = Create<string>();

                _options.IsExcluded(sourceName);

                var actual = _options.IsExcluded(sourceName);

                actual.Should().BeFalse();
            }
        }

        public class GetAliasName : ObjectMapperOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_SourceName_Null()
            {
                Invoking(() =>
                {
                    _options.GetAliasName(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceName");
            }

            [Fact]
            public void Should_Throw_When_SourceName_Empty()
            {
                Invoking(() =>
                {
                    _options.GetAliasName(string.Empty);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("sourceName");
            }

            [Fact]
            public void Should_Throw_When_SourceName_Whitespace()
            {
                Invoking(() =>
                {
                    _options.GetAliasName("  ");
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("sourceName");
            }

            [Fact]
            public void Should_Get_Alias()
            {
                var sourceName = Create<string>();
                var expected = Create<string>();

                _options.WithAlias(sourceName, expected);

                var actual = _options.GetAliasName(sourceName);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Return_SourceName_When_No_Alias()
            {
                var sourceName = Create<string>();

                var actual = _options.GetAliasName(sourceName);

                actual.Should().Be(sourceName);
            }
        }

        public class GetConvertedValue : ObjectMapperOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_SourceName_Null()
            {
                Invoking(() =>
                {
                    _options.GetConvertedValue(null, Create<string>());

                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceName");
            }

            [Fact]
            public void Should_Throw_When_SourceName_Empty()
            {
                Invoking(() =>
                {
                    _options.GetConvertedValue(string.Empty, Create<string>());
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("sourceName");
            }

            [Fact]
            public void Should_Throw_When_SourceName_Whitespace()
            {
                Invoking(() =>
                {
                    _options.GetConvertedValue("  ", Create<string>());
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("sourceName");
            }

            [Fact]
            public void Should_Get_Converted_Value()
            {
                var sourceName = Create<string>();
                var factor = GetWithinRange(2, 5);

                _options.WithConversion(sourceName, (mapper, value) => (int) value * factor);

                var value = Create<int>();

                var actual = _options.GetConvertedValue(sourceName, value);

                actual.Should().Be(value * factor);
            }

            [Fact]
            public void Should_Return_Same_Value_When_No_Converter()
            {
                var value = Create<int>();

                var actual = _options.GetConvertedValue(Create<string>(), value);

                actual.Should().Be(value);
            }
        }
    }
}