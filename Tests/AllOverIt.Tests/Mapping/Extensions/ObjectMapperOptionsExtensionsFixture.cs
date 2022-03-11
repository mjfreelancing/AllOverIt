using System;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Mapping;
using AllOverIt.Mapping.Extensions;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Mapping.Extensions
{
    public class ObjectMapperOptionsExtensionsFixture : FixtureBase
    {
        private readonly ObjectMapperOptions _options;

        public ObjectMapperOptionsExtensionsFixture()
        {
            _options = new ObjectMapperOptions();
        }

        public class Exclude : ObjectMapperOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() =>
                    {
                        ObjectMapperOptions options = null;

                        ObjectMapperOptionsExtensions.Exclude(options);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("mapperOptions");
            }

            [Fact]
            public void Should_Not_Throw_When_SourceNames_Empty()
            {
                Invoking(() =>
                    {
                        ObjectMapperOptionsExtensions.Exclude(_options);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Exclude_Names()
            {
                var names = Create<string>();

                ObjectMapperOptionsExtensions.Exclude(_options, names);

                _options.SourceTargetOptions.Keys.Should().BeEquivalentTo(names);
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var names = Create<string>();

                var actual = ObjectMapperOptionsExtensions.Exclude(_options, names);

                actual.Should().Be(_options);
            }
        }

        public class WithAlias : ObjectMapperOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() =>
                    {
                        ObjectMapperOptions options = null;

                        ObjectMapperOptionsExtensions.WithAlias(options, Create<string>(), Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("mapperOptions");
            }

            [Fact]
            public void Should_Throw_When_SourceName_Null()
            {
                Invoking(() =>
                    {
                        ObjectMapperOptionsExtensions.WithAlias(_options, null, Create<string>());
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
                        ObjectMapperOptionsExtensions.WithAlias(_options, string.Empty, Create<string>());
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
                        ObjectMapperOptionsExtensions.WithAlias(_options, "  ", Create<string>());
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
                        ObjectMapperOptionsExtensions.WithAlias(_options, Create<string>(), null);
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
                        ObjectMapperOptionsExtensions.WithAlias(_options, Create<string>(), string.Empty);
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
                        ObjectMapperOptionsExtensions.WithAlias(_options, Create<string>(), "  ");
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

                _ = ObjectMapperOptionsExtensions.WithAlias(_options, source, target);

                _options.SourceTargetOptions[source].Alias.Should().Be(target);
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var source = Create<string>();
                var target = Create<string>();

                var actual = ObjectMapperOptionsExtensions.WithAlias(_options, source, target);

                actual.Should().Be(_options);
            }
        }

        public class WithConversion : ObjectMapperOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() =>
                {
                    ObjectMapperOptions options = null;

                    ObjectMapperOptionsExtensions.WithConversion(options, Create<string>(), value => value);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("mapperOptions");
            }

            [Fact]
            public void Should_Throw_When_SourceName_Null()
            {
                Invoking(() =>
                {
                    ObjectMapperOptionsExtensions.WithConversion(_options, null, value => value);
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
                    ObjectMapperOptionsExtensions.WithConversion(_options, string.Empty, value => value);
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
                    ObjectMapperOptionsExtensions.WithConversion(_options, "  ", value => value);
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
                    ObjectMapperOptionsExtensions.WithConversion(_options, Create<string>(), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("converter");
            }

            [Fact]
            public void Should_Set_Converter()
            {
                var sourceName = Create<string>();
                var expected = Create<int>();

                ObjectMapperOptionsExtensions.WithConversion(_options, sourceName, value => expected);

                var converter = _options.SourceTargetOptions[sourceName].Converter;

                var actual = converter.Invoke(Create<int>());

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var actual = ObjectMapperOptionsExtensions.WithConversion(_options, Create<string>(), value => value);

                actual.Should().Be(_options);
            }
        }

        public class IsExcluded : ObjectMapperOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() =>
                    {
                        ObjectMapperOptions options = null;

                        ObjectMapperOptionsExtensions.IsExcluded(options, Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("mapperOptions");
            }

            [Fact]
            public void Should_Throw_When_SourceName_Null()
            {
                Invoking(() =>
                    {
                        ObjectMapperOptionsExtensions.IsExcluded(_options, null);
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
                        ObjectMapperOptionsExtensions.IsExcluded(_options, string.Empty);
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
                        ObjectMapperOptionsExtensions.IsExcluded(_options, "  ");
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

                ObjectMapperOptionsExtensions.IsExcluded(_options, sourceName);

                var actual = _options.IsExcluded(sourceName);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_Is_Not_Expected()
            {
                var sourceName = Create<string>();

                ObjectMapperOptionsExtensions.IsExcluded(_options, sourceName);

                var actual = _options.IsExcluded(sourceName);

                actual.Should().BeFalse();
            }
        }

        public class GetAliasName : ObjectMapperOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() =>
                    {
                        ObjectMapperOptions options = null;

                        ObjectMapperOptionsExtensions.GetAliasName(options, Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("mapperOptions");
            }

            [Fact]
            public void Should_Throw_When_SourceName_Null()
            {
                Invoking(() =>
                    {
                        ObjectMapperOptionsExtensions.GetAliasName(_options, null);
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
                        ObjectMapperOptionsExtensions.GetAliasName(_options, string.Empty);
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
                        ObjectMapperOptionsExtensions.GetAliasName(_options, "  ");
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

        public class GetConvertedValue : ObjectMapperOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() =>
                    {
                        ObjectMapperOptions options = null;

                        ObjectMapperOptionsExtensions.GetConvertedValue(options, Create<string>(), Create<string>());
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("mapperOptions");
            }

            [Fact]
            public void Should_Throw_When_SourceName_Null()
            {
                Invoking(() =>
                    {
                        ObjectMapperOptionsExtensions.GetConvertedValue(_options, null, Create<string>());

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
                        ObjectMapperOptionsExtensions.GetConvertedValue(_options, string.Empty, Create<string>());
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
                        ObjectMapperOptionsExtensions.GetConvertedValue(_options, "  ", Create<string>());
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

                _options.WithConversion(sourceName, value => (int) value * factor);

                var value = Create<int>();

                var actual = ObjectMapperOptionsExtensions.GetConvertedValue(_options, sourceName, value);

                actual.Should().Be(value * factor);
            }

            [Fact]
            public void Should_Return_Same_Value_When_No_Converter()
            {
                var value = Create<int>();

                var actual = ObjectMapperOptionsExtensions.GetConvertedValue(_options, Create<string>(), value);

                actual.Should().Be(value);
            }
        }
    }
}