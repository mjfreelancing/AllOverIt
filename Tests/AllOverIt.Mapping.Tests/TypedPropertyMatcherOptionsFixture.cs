using AllOverIt.Mapping.Exceptions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using static AllOverIt.Mapping.Tests.ObjectMapperTypes;

namespace AllOverIt.Mapping.Tests
{
    public class TypedPropertyMatcherOptionsFixture : FixtureBase
    {
        private class DummyChild
        {
            public int Prop1 { get; set; }
            public int? Prop2 { get; set; }
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

        private TypedPropertyMatcherOptions<DummySource, DummyTarget> _typedMatcherOptions;

        public TypedPropertyMatcherOptionsFixture()
        {
            _typedMatcherOptions = new((source, target, factory) => { });
        }

        public class Constructor : TypedPropertyMatcherOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_Registration_Null()
            {
                Invoking(() =>
                {
                    _ = new TypedPropertyMatcherOptions<DummySource, DummyTarget>(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceTargetFactoryRegistration");
            }
        }

        public class Exclude : TypedPropertyMatcherOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_SourceExpression_Null()
            {
                Invoking(() =>
                    {
                        _typedMatcherOptions.Exclude<int>(null);
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
                        _typedMatcherOptions.Exclude(source => source.Child.Prop1);
                    })
                    .Should()
                    .Throw<ObjectMapperException>()
                    .WithMessage("ObjectMapper do not support nested mappings (source => source.Child.Prop1).");
            }

            [Fact]
            public void Should_Exclude_Name()
            {
                _typedMatcherOptions.Exclude(source => source.Prop2);

                _typedMatcherOptions.IsExcluded(nameof(DummySource.Prop2)).Should().BeTrue();
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var actual = _typedMatcherOptions.Exclude(source => source.Prop2);

                actual.Should().Be(_typedMatcherOptions);
            }
        }

        public class ExcludeWhen : TypedPropertyMatcherOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_SourceExpression_Null()
            {
                Invoking(() =>
                {
                    _typedMatcherOptions.ExcludeWhen<int>(null, _ => Create<bool>());
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
                    _typedMatcherOptions.ExcludeWhen(source => source.Child.Prop1, _ => Create<bool>());
                })
                    .Should()
                    .Throw<ObjectMapperException>()
                    .WithMessage("ObjectMapper do not support nested mappings (source => source.Child.Prop1).");
            }

            [Fact]
            public void Should_Exclude_Name()
            {
                Func<object, bool> predicate = value => (int) value == 1;

                _typedMatcherOptions.ExcludeWhen(source => source.Prop2, predicate);

                _typedMatcherOptions.IsExcludedWhen(nameof(DummySource.Prop2), 1).Should().BeTrue();
                _typedMatcherOptions.IsExcludedWhen(nameof(DummySource.Prop2), 2).Should().BeFalse();
                _typedMatcherOptions.IsExcludedWhen(Create<string>(), Create<int>()).Should().BeFalse();
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var actual = _typedMatcherOptions.ExcludeWhen(source => source.Prop2, _ => Create<bool>());

                actual.Should().Be(_typedMatcherOptions);
            }
        }

        public class DeepCopy : TypedPropertyMatcherOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_SourceExpression_Null()
            {
                Invoking(() =>
                {
                    _typedMatcherOptions.DeepCopy<int>(null);
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
                    _typedMatcherOptions.DeepCopy(source => source.Child.Prop1);
                })
                    .Should()
                    .Throw<ObjectMapperException>()
                    .WithMessage("ObjectMapper do not support nested mappings (source => source.Child.Prop1).");
            }

            [Fact]
            public void Should_DeepCopy_Name()
            {
                _typedMatcherOptions.DeepCopy(source => source.Child);

                _typedMatcherOptions.IsDeepCopy(nameof(DummySource.Child)).Should().BeTrue();
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var actual = _typedMatcherOptions.DeepCopy(source => source.Child);

                actual.Should().Be(_typedMatcherOptions);
            }
        }

        public class WithAlias : TypedPropertyMatcherOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_SourceExpression_Null()
            {
                Invoking(() =>
                    {
                        _typedMatcherOptions.WithAlias<int, int>(null, target => target.Prop1);
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
                        _typedMatcherOptions.WithAlias<int, int>(source => source.Prop2, null);
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
                        _typedMatcherOptions.WithAlias(source => source.Child.Prop1, target => target.Prop2);
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
                        _typedMatcherOptions.WithAlias(source => source.Prop1, target => target.Child.Prop1);
                    })
                    .Should()
                    .Throw<ObjectMapperException>()
                    .WithMessage("ObjectMapper do not support nested mappings (target => target.Child.Prop1).");
            }

            [Fact]
            public void Should_Set_Alias()
            {
                _ = _typedMatcherOptions.WithAlias(source => source.Prop3, target => target.Prop2);

                _typedMatcherOptions.GetAliasName(nameof(DummySource.Prop3)).Should().Be(nameof(DummyTarget.Prop2));
            }

            [Fact]
            public void Should_Set_Alias_Different_Types()
            {
                _ = _typedMatcherOptions.WithAlias(source => source.Prop1, target => target.Prop3);

                _typedMatcherOptions.GetAliasName(nameof(DummySource.Prop1)).Should().Be(nameof(DummyTarget.Prop3));
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var actual = _typedMatcherOptions.WithAlias(source => source.Prop3, target => target.Prop2);

                actual.Should().Be(_typedMatcherOptions);
            }
        }

        public class UseWhenNull : TypedPropertyMatcherOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_SourceExpression_Null()
            {
                Invoking(() =>
                {
                    _typedMatcherOptions.UseWhenNull<DummyChild, DummyChild>(null, Create<DummyChild>());
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
                    _typedMatcherOptions.UseWhenNull<int?, int?>(source => source.Child.Prop2, Create<int>());
                })
                    .Should()
                    .Throw<ObjectMapperException>()
                    .WithMessage("ObjectMapper do not support nested mappings (source => source.Child.Prop2).");
            }

            [Fact]
            public void Should_Set_Null_Replacement()
            {
                var replacement = new DummyTarget();

                _ = _typedMatcherOptions.UseWhenNull(source => source.Child, replacement);

                _typedMatcherOptions.GetNullReplacement(nameof(DummySource.Child)).Should().BeSameAs(replacement);
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var actual = _typedMatcherOptions.UseWhenNull(source => source.Child, Create<DummyTarget>());

                actual.Should().Be(_typedMatcherOptions);
            }
        }

        public class WithConversion : TypedPropertyMatcherOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_SourceExpression_Null()
            {
                Invoking(() =>
                {
                    _typedMatcherOptions.WithConversion<int>(null, (mapper, value) => value);
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
                    _typedMatcherOptions.WithConversion(source => source.Prop3, null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("converter");
            }

            [Fact]
            public void Should_Provide_Mapper()
            {
                IObjectMapper actual = null;

                _typedMatcherOptions.WithConversion(source => source.Prop3, (mapper, value) =>
                {
                    actual = mapper;
                    return value;
                });

                var mapper = this.CreateStub<IObjectMapper>();

                _ = _typedMatcherOptions.GetConvertedValue(mapper, nameof(DummySource.Prop3), Create<int>());

                actual.Should().BeSameAs(mapper);
            }

            [Fact]
            public void Should_Not_Allow_Source_Nested_Properties()
            {
                Invoking(() =>
                    {
                        _typedMatcherOptions.WithConversion(source => source.Child.Prop1, (mapper, val) => val);
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

                _typedMatcherOptions.WithConversion(source => source.Prop2, (mapper, val) => val * factor);

                var actual = _typedMatcherOptions.GetConvertedValue(this.CreateStub<IObjectMapper>(), nameof(DummySource.Prop2), value);

                actual.Should().BeEquivalentTo(value * factor);
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var actual = _typedMatcherOptions.WithConversion(source => source.Prop3, (mapper, value) => value);

                actual.Should().Be(_typedMatcherOptions);
            }
        }

        public class ConstructUsing : TypedPropertyMatcherOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_TargetFactory_Null()
            {
                Invoking(() =>
                {
                    _typedMatcherOptions.ConstructUsing(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("targetFactory");
            }

            [Fact]
            public void Should_Provide_Mapper_When_Invoke_ConstructUsing()
            {
                // the mapper actually stores / invokes the factory

                IObjectMapper actual = null;

                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummyChild, DummyChild>(opt =>
                {
                    opt.ConstructUsing((mapper, value) =>
                    {
                        actual = mapper;
                        return new DummyChild();
                    });
                });

                var mapper = new ObjectMapper(configuration);

                var source = Create<DummySource>();

                _ = mapper.Map<DummyTarget>(source);

                actual.Should().BeSameAs(mapper);
            }

            [Fact]
            public void Should_Map_Property_Using_ConstructUsing()
            {
                var source = Create<DummyEnumerableSource>();
                var expected = source.Prop1.Select(item => $"{item}").AsReadOnlyCollection();

                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<IEnumerable<int>, ObservableCollection<string>>(opt =>
                {
                    opt.ConstructUsing((mapper, value) =>
                    {
                        return new ObservableCollection<string>(expected);
                    });
                });

                var mapper = new ObjectMapper(configuration);

                var actual = mapper.Map<DummyObservableCollectionHost>(source);

                actual.Prop1.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Use_Factory()
            {
                var expected = Create<DummyChild>();
                DummyChild actual = null;

                // the mapper actually stores / invokes the factory
                var configuration = new ObjectMapperConfiguration();

                configuration.Configure<DummyChild, DummyChild>(opt =>
                {
                    opt.ConstructUsing((mapper, value) =>
                    {
                        actual = expected;
                        return expected;
                    });
                });

                var mapper = new ObjectMapper(configuration);

                var source = Create<DummySource>();

                _ = mapper.Map<DummyTarget>(source);

                expected.Should().BeSameAs(actual);
            }

            [Fact]
            public void Should_Return_Same_Options()
            {
                var actual = _typedMatcherOptions.ConstructUsing((mapper, value) => default);

                actual.Should().Be(_typedMatcherOptions);
            }
        }
    }
}