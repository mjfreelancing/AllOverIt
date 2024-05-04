using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Reflection;
using FluentAssertions;
using System.Reflection;

namespace AllOverIt.Mapping.Tests
{
    public class PropertyMatcherOptionsFixture : FixtureBase
    {
        private readonly PropertyMatcherOptions _options;

        protected PropertyMatcherOptionsFixture()
        {
            _options = new PropertyMatcherOptions();
        }

        public class Constructor : PropertyMatcherOptionsFixture
        {
            [Fact]
            public void Should_Have_Default_Settings()
            {
                var expected = new
                {
                    DeepCopy = false,
                    Binding = BindingOptions.Default,
                    Filter = (Func<PropertyInfo, bool>) null
                };

                expected.Should().BeEquivalentTo(_options);
            }
        }

        public class Exclude : PropertyMatcherOptionsFixture
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
            public void Should_Set_Exclude_For_Name()
            {
                var names = CreateMany<string>(3).ToArray();

                _options.Exclude(names);

                _options.IsExcluded(names[1]).Should().BeTrue();
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var names = CreateMany<string>().ToArray();

                var actual = _options.Exclude(names);

                actual.Should().Be(_options);
            }
        }

        public class ExcludeWhen : PropertyMatcherOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_SourceName_Null()
            {
                Invoking(() =>
                {
                    _options.ExcludeWhen(null, _ => Create<bool>());
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
                    _options.ExcludeWhen(string.Empty, _ => Create<bool>());
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
                    _options.ExcludeWhen("  ", _ => Create<bool>());
                })
                   .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("sourceName");
            }

            [Fact]
            public void Should_Set_ExcludeWhen_For_Name()
            {
                var name = Create<string>();
                Func<object, bool> predicate = value => (int) value == 1;

                _options.ExcludeWhen(name, predicate);

                _options.IsExcludedWhen(name, 1).Should().BeTrue();
                _options.IsExcludedWhen(name, 2).Should().BeFalse();
                _options.IsExcludedWhen(Create<string>(), Create<int>()).Should().BeFalse();
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var name = Create<string>();

                var actual = _options.ExcludeWhen(name, _ => Create<bool>());

                actual.Should().Be(_options);
            }
        }

        public class DeepCopy : PropertyMatcherOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_SourceNames_Null()
            {
                Invoking(() =>
                {
                    _options.DeepCopy(null);
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
                    _options.DeepCopy(Array.Empty<string>());
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Set_DeepCopy_For_Name()
            {
                var name = Create<string>();

                _options.DeepCopy(name);

                _options.IsDeepCopy(name).Should().BeTrue();
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var names = Create<string>();

                var actual = _options.DeepCopy(names);

                actual.Should().Be(_options);
            }
        }

        public class WithAlias : PropertyMatcherOptionsFixture
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
            public void Should_Set_Alias_For_Name()
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

        public class UseWhenNull : PropertyMatcherOptionsFixture
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
            public void Should_Throw_When_SourceName_Null()
            {
                Invoking(() =>
                {
                    _options.UseWhenNull(null, new { });
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
                    _options.UseWhenNull(string.Empty, new { });
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
                    _options.UseWhenNull("  ", new { });
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("sourceName");
            }

            [Fact]
            public void Should_Set_Null_Replacement()
            {
                var sourceName = Create<string>();
                var replacement = Create<string>();

                _options.UseWhenNull(sourceName, replacement);

                var actual = _options.GetNullReplacement(sourceName);

                actual.Should().Be(replacement);
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var actual = _options.UseWhenNull(Create<string>(), new { });

                actual.Should().Be(_options);
            }
        }

        public class WithConversion : PropertyMatcherOptionsFixture
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

                IObjectMapper actual = null;

                _options.WithConversion(propName, (mapper, value) =>
                {
                    actual = mapper;
                    return value;
                });

                var mapper = new ObjectMapper();

                _ = _options.GetConvertedValue(mapper, propName, Create<int>());

                actual.Should().BeSameAs(mapper);
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

                _options.WithConversion(sourceName, (mapper, val) => (int) val * factor);

                var mapper = new ObjectMapper();

                var actual = _options.GetConvertedValue(mapper, sourceName, value);

                actual.Should().BeEquivalentTo(value * factor);
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var actual = _options.WithConversion(Create<string>(), (mapper, value) => value);

                actual.Should().Be(_options);
            }
        }

        public class IsExcluded : PropertyMatcherOptionsFixture
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

        public class GetAliasName : PropertyMatcherOptionsFixture
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

        public class GetConvertedValue : PropertyMatcherOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_Mapper_Null()
            {
                Invoking(() =>
                {
                    _options.GetConvertedValue(null, Create<string>(), Create<string>());

                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("objectMapper");
            }

            [Fact]
            public void Should_Throw_When_SourceName_Null()
            {
                Invoking(() =>
                {
                    var mapper = new ObjectMapper();

                    _options.GetConvertedValue(mapper, null, Create<string>());

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
                    var mapper = new ObjectMapper();

                    _options.GetConvertedValue(mapper, string.Empty, Create<string>());
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
                    var mapper = new ObjectMapper();

                    _options.GetConvertedValue(mapper, "  ", Create<string>());
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

                var mapper = new ObjectMapper();

                var actual = _options.GetConvertedValue(mapper, sourceName, value);

                actual.Should().Be(value * factor);
            }

            [Fact]
            public void Should_Return_Same_Value_When_No_Converter()
            {
                var value = Create<int>();

                var mapper = new ObjectMapper();

                var actual = _options.GetConvertedValue(mapper, Create<string>(), value);

                actual.Should().Be(value);
            }
        }
    }
}