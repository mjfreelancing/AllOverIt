﻿using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Mapping;
using FluentAssertions;
using System;
using Xunit;
using static AllOverIt.Tests.Mapping.ObjectMapperTypes;

namespace AllOverIt.Tests.Mapping
{
    public class ObjectMapperTypeFactoryFixture : FixtureBase
    {
        private readonly ObjectMapperTypeFactory _factory = new();

        public class Add_Type : ObjectMapperTypeFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Type_Null()
            {
                Invoking(() =>
                {
                    _factory.Add(null, () => new { });
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("type");
            }

            [Fact]
            public void Should_Throw_When_Factory_Null()
            {
                Invoking(() =>
                {
                    _factory.Add(typeof(DummyTarget), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("factory");
            }

            [Fact]
            public void Should_Add_Factory()
            {
                Func<object> factory = () => new { };

                _factory.Add(typeof(DummyTarget), factory);

                _ = _factory.TryGet(typeof(DummyTarget), out var actual);

                factory.Should().BeSameAs(actual);
            }
        }

        public class TryGet_Type : ObjectMapperTypeFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Type_Null()
            {
                Invoking(() =>
                {
                    _factory.TryGet(null, out _);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("type");
            }

            [Fact]
            public void Should_Get_Factory()
            {
                Func<object> factory = () => new { };

                _factory.Add(typeof(DummyTarget), factory);

                var success = _factory.TryGet(typeof(DummyTarget), out var actual);

                success.Should().BeTrue();
                factory.Should().BeSameAs(actual);
            }

            [Fact]
            public void Should_Not_Get_Factory()
            {
                var success = _factory.TryGet(typeof(DummyTarget), out var actual);

                success.Should().BeFalse();
                actual.Should().BeNull();
            }
        }

        public class Add_Source_Target_Type : ObjectMapperTypeFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Source_Type_Null()
            {
                Invoking(() =>
                {
                    _factory.Add((Type)null, typeof(DummyTarget), (mapper, value) => new { });
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceType");
            }

            [Fact]
            public void Should_Throw_When_Target_Type_Null()
            {
                Invoking(() =>
                {
                    _factory.Add(typeof(DummySource2), (Type) null, (mapper, value) => new { });
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("targetType");
            }

            [Fact]
            public void Should_Throw_When_Factory_Null()
            {
                Invoking(() =>
                {
                    _factory.Add(typeof(DummySource2), typeof(DummyTarget), null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("factory");
            }

            [Fact]
            public void Should_Add_Factory()
            {
                Func<IObjectMapper, object, object> factory = (mapper, value) => new { };

                _factory.Add(typeof(DummySource2), typeof(DummyTarget), factory);

                var success = _factory.TryGet(typeof(DummySource2), typeof(DummyTarget), out var actual);

                success.Should().BeTrue();
                factory.Should().BeSameAs(actual);
            }
        }

        public class TryGet_Source_Target_Type : ObjectMapperTypeFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Source_Type_Null()
            {
                Invoking(() =>
                {
                    _ = _factory.TryGet((Type) null, typeof(DummyTarget), out _);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("sourceType");
            }

            [Fact]
            public void Should_Throw_When_Target_Type_Null()
            {
                Invoking(() =>
                {
                    _ = _factory.TryGet(typeof(DummySource2), (Type) null, out _);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("targetType");
            }

            [Fact]
            public void Should_Get_Factory()
            {
                Func<IObjectMapper, object, object> factory = (mapper, value) => new { };

                _factory.Add(typeof(DummySource2), typeof(DummyTarget), factory);

                var success = _factory.TryGet(typeof(DummySource2), typeof(DummyTarget), out var actual);

                success.Should().BeTrue();
                factory.Should().BeSameAs(actual);
            }

            [Fact]
            public void Should_Not_Get_Factory()
            {
                var success = _factory.TryGet(typeof(DummySource2), typeof(DummyTarget), out var actual);

                success.Should().BeFalse();
                actual.Should().BeNull();
            }
        }
    }
}