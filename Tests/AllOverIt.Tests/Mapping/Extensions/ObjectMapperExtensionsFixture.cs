using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Mapping;
using AllOverIt.Mapping.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Mapping.Extensions
{
    public class ObjectMapperExtensionsFixture : FixtureBase
    {
        private class DummySource
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
            public IEnumerable<string> Prop3 { get; set; }
        }

        private class DummyTarget
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
            public IEnumerable<string> Prop3 { get; set; }
        }

        private readonly IObjectMapper _mapper;
        private readonly IEnumerable<DummySource> _sources;

        ObjectMapperExtensionsFixture()
        {
            _mapper = new ObjectMapper();
            _sources = CreateMany<DummySource>();
        }

        public class MapMany_Target : ObjectMapperExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Mapper_Null()
            {
                Invoking(() =>
                {
                    ObjectMapperExtensions.MapMany<DummyTarget>(null, _sources).ToList();
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("mapper");
            }

            [Fact]
            public void Should_Throw_When_Sources_Null()
            {
                Invoking(() =>
                {
                    ObjectMapperExtensions.MapMany<DummyTarget>(_mapper, null).ToList();
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sources");
            }

            [Fact]
            public void Should_Map_Sources()
            {
                var actual = ObjectMapperExtensions.MapMany<DummyTarget>(_mapper, _sources).ToList();

                actual.Should().NotBeEmpty();

                actual.Should().BeEquivalentTo(_sources);
            }
        }

        public class MapMany_Source_Target : ObjectMapperExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Mapper_Null()
            {
                Invoking(() =>
                {
                    ObjectMapperExtensions.MapMany<DummySource, DummyTarget>(null, _sources).ToList();
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("mapper");
            }

            [Fact]
            public void Should_Throw_When_Sources_Null()
            {
                Invoking(() =>
                {
                    ObjectMapperExtensions.MapMany<DummySource, DummyTarget>(_mapper, null).ToList();
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sources");
            }

            [Fact]
            public void Should_Map_Sources()
            {
                var actual = ObjectMapperExtensions.MapMany<DummySource, DummyTarget>(_mapper, _sources).ToList();

                actual.Should().NotBeEmpty();

                actual.Should().BeEquivalentTo(_sources);
            }
        }
    }
}