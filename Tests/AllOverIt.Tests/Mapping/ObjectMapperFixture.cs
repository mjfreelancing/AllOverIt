using AllOverIt.Fixture;
using AllOverIt.Mapping;
using AllOverIt.Mapping.Exceptions;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private readonly DummySource2 _source2;
        private readonly DummyTarget _target;

        protected ObjectMapperFixture()
        {
            _mapper = new ObjectMapper();
            _source2 = new DummySource2();
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
                _mapper.Configure<DummySource2, DummyTarget>();

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

                Invoking(()=>_mapper.Configure<DummySource2, DummyTarget>())
                    .Should()
                    .Throw<ObjectMapperException>()
                    .WithMessage($"Mapping already exists between {nameof(DummySource2)} and {nameof(DummyTarget)}");
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
                    options.Filter = propInfo => new[] {"Prop10", "Prop12", "Prop8"}.Contains(propInfo.Name);
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
                        .WithAlias(src => (int)src.Prop12, trg => trg.Prop5);
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
            public void Should_Configure_With_Conversion()
            {

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

        //public class Map_Target : ObjectMapperFixture
        //{
        //    [Fact]
        //    public void Should_Throw_When_Not_Configured()
        //    {
        //        _mapper.Map<DummyTarget>(_source2);
        //    }
        //}

        //public class Map_Source_Target : ObjectMapperFixture
        //{
        //    [Fact]
        //    public void Should_Throw_When_Not_Configured()
        //    {
        //        _mapper.Map(_source2, _target);
        //    }
        //}
    }
}